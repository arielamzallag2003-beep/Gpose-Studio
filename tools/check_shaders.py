# check_shaders.py — compiles the plugin's HLSL without the game.
# The plugin compiles its shaders at runtime via D3DCompiler, so a syntax error
# only surfaces when you enter GPose. This extracts the HLSL const strings from
# plugin/src/GpuRenderer.cs and runs them through fxc with the same targets,
# turning that into a desk check.
#
#   python tools/check_shaders.py                    # compile, report errors
#   python tools/check_shaders.py --asm out/         # also emit disassembly
#   python tools/check_shaders.py --asm a/ --base b/ # prove a refactor is a no-op
#   python tools/check_shaders.py --out dir/         # build step: emit .cso only
#
# --out is what the csproj calls. The PS entry takes ~90s to compile, so doing it
# at build time instead of on the first GPose frame is the difference between a
# minute-plus freeze and none.
#
# The --base compare answers "did I change what the shader computes?" by diffing
# the optimized opcode sequence and the set of constant-buffer slots read. A pure
# refactor keeps both identical; only register allocation may move.
import glob, hashlib, os, re, subprocess, sys

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
SRC = os.path.join(ROOT, "plugin", "src", "GpuRenderer.cs")

# const string name -> entry points to build from it
SHADERS = {
    "Hlsl": [("VS", "vs_5_0"), ("PS", "ps_5_0")],
    "BloomHlsl": [("BrightPS", "ps_5_0"), ("BlurPS", "ps_5_0"),
                  ("GodrayPS", "ps_5_0"), ("HaloMaskPS", "ps_5_0")],
}

def find_fxc():
    hits = glob.glob(r"C:\Program Files (x86)\Windows Kits\10\bin\*\x64\fxc.exe")
    if not hits:
        sys.exit("fxc.exe not found — install the Windows 10/11 SDK")
    return sorted(hits)[-1]

def extract(src, name):
    """Pull one `const string NAME = @"..."` verbatim string out of the C#."""
    start = re.search(r'const\s+string\s+%s\s*=\s*@"' % name, src)
    if not start:
        sys.exit("could not find const string " + name)
    i, out = start.end(), []
    while i < len(src):
        if src[i] == '"':
            if src[i + 1:i + 2] == '"':      # "" is an escaped quote
                out.append('"')
                i += 2
                continue
            return "".join(out)              # unescaped quote ends the literal
        out.append(src[i])
        i += 1
    sys.exit("unterminated string " + name)

def check_quotes(src):
    """Warn about a bare " inside the HLSL verbatim strings.

    A single unescaped double-quote silently ENDS the C# @"..." literal, so the
    shader is truncated and fxc reports only 'unexpected end of file' hundreds of
    lines later. Catching it here names the real problem. (Inside a verbatim string
    a literal quote must be doubled: "".)
    """
    bad = 0
    for name in SHADERS:
        start = re.search(r'const\s+string\s+%s\s*=\s*@"' % name, src)
        if not start:
            continue
        i, line = start.end(), 1
        while i < len(src):
            ch = src[i]
            if ch == '"':
                if src[i + 1:i + 2] == '"':
                    i += 2
                    continue
                break                      # legitimate terminator
            if ch == '\n':
                line += 1
            i += 1
        # If the literal ends well before the C# closing `";`, a stray quote cut it.
        tail = src[i:i + 3]
        if not tail.startswith('";'):
            print("  WARNING: %s literal ends at line %d without a closing \";  "
                  "— likely a stray double-quote in a comment" % (name, line))
            bad += 1
    return bad

def check_layout(asm, cs_src):
    """Verify the C# Params struct still matches the HLSL cbuffer field-for-field.

    The two are mapped purely by position with no compile-time check, so inserting
    a field on one side silently shifts every parameter after it -- the shader
    reads garbage and nothing errors. fxc's reflection lists the authoritative
    HLSL offset of every field, so we can compare it against the C# layout.
    """
    blk = asm[asm.index("cbuffer P"):asm.index("// }")]
    hlsl = [(m.group(2), int(m.group(3)), m.group(1)) for m in
            re.finditer(r"//\s+(int|float|float4)\s+(\w+)(?:\[\d+\])?;\s+// Offset:\s+(\d+)", blk)]

    # Anchored on declarations, not on comments. The public export strips comments,
    # and a checker that located the struct by its prose stopped finding it there --
    # so the one tree that most needs verifying was the one it could not verify.
    body = cs_src[cs_src.index("public unsafe struct Params"):
                  cs_src.index("struct BloomParams")]
    cs, off = [], 0
    for m in re.finditer(r"public\s+(int|float)\s+(\w+)\s*;|public fixed float (\w+)\[(\d+)\]", body):
        if m.group(3):                      # fixed buffer, e.g. Elem[160]
            cs.append((m.group(3), off, "float4"))
            off += int(m.group(4)) * 4
        else:
            cs.append((m.group(2), off, m.group(1)))
            off += 4

    if len(hlsl) != len(cs):
        print("  LAYOUT: field count differs — HLSL %d, C# %d" % (len(hlsl), len(cs)))
        return 1
    bad = [(h, c) for h, c in zip(hlsl, cs) if h[1] != c[1] or h[2] != c[2]]
    for h, c in bad[:8]:
        print("  LAYOUT MISMATCH: HLSL %s @%d %s vs C# %s @%d %s"
              % (h[0], h[1], h[2], c[0], c[1], c[2]))
    if bad:
        return 1
    # A D3D11 constant buffer's total byte size must be a multiple of 16, or
    # CreateBuffer fails at runtime with E_INVALIDARG. The struct<->cbuffer offsets
    # can agree perfectly and still trip this, so check it explicitly.
    if off % 16 != 0:
        print("  LAYOUT: cbuffer is %d bytes, NOT a multiple of 16 — CreateBuffer will "
              "fail (E_INVALIDARG). Pad by %d more bytes." % (off, 16 - off % 16))
        return 1
    print("  layout OK: %d fields, %d bytes (16-aligned), offsets match C# exactly"
          % (len(cs), off))
    return 0

def opcodes(asm):
    """Instruction mnemonics only — the shape of the program, minus register names."""
    return [l.strip().split(" ")[0] for l in asm.splitlines()
            if l.strip() and not l.startswith("//")]

def cbuf_reads(asm):
    """Every constant-buffer slot+component read, with how often. This is what
    catches a parameter accidentally rewired to a different slot."""
    hits = {}
    for m in re.findall(r"cb0\[\d+\]\.[xyzw]+", asm):
        hits[m] = hits.get(m, 0) + 1
    return hits

argv = sys.argv[1:]
outdir = argv[argv.index("--asm") + 1] if "--asm" in argv else None
basedir = argv[argv.index("--base") + 1] if "--base" in argv else None
csodir = argv[argv.index("--out") + 1] if "--out" in argv else None

fxc = find_fxc()
src = open(SRC, encoding="utf-8").read()
check_quotes(src)
work = csodir or outdir or os.path.join(ROOT, "tools", ".shadercheck")
os.makedirs(work, exist_ok=True)
# Always emit disassembly for the main PS -- the layout check reads it, and we
# want a bad C#<->HLSL mapping to fail the build, not ship.
want_asm = True

failures, compared = 0, 0
for const_name, entries in SHADERS.items():
    hlsl = extract(src, const_name)
    path = os.path.join(work, const_name + ".hlsl")
    open(path, "w", encoding="utf-8").write(hlsl)
    print("%s: %d lines" % (const_name, len(hlsl.splitlines())))

    # The HLSL lives inside GpuRenderer.cs, and MSBuild keys this step to that whole
    # FILE -- so editing any C# in it triggered a full recompile of a shader whose text
    # had not changed. The main PS takes many minutes, so that was the single biggest
    # cost of routine work. Hash the extracted HLSL instead: if it is byte-identical to
    # what produced the existing bytecode, reuse it.
    #
    # The layout check below still runs against the cached disassembly, which is the
    # part that matters -- a C# struct edit with unchanged HLSL is exactly the case that
    # breaks the positional cbuffer mapping, and it is still caught.
    digest = hashlib.sha256(hlsl.encode("utf-8")).hexdigest()
    stamp = os.path.join(work, const_name + ".hash")
    try:
        cached = open(stamp).read().strip() == digest
    except OSError:
        cached = False
    const_ok = True

    for entry, target in entries:
        cso_path = os.path.join(work, "%s_%s.cso" % (const_name, entry))
        asm_path = os.path.join(work, "%s_%s.asm" % (const_name, entry))
        if cached and os.path.exists(cso_path) and os.path.exists(asm_path):
            ok = True
            print("  [skip] %-12s %s  (HLSL unchanged)" % (entry, target))
        else:
            cmd = [fxc, "/nologo", "/T", target, "/E", entry, path, "/Fo", cso_path]
            if want_asm:
                cmd += ["/Fc", asm_path]
            proc = subprocess.run(cmd, capture_output=True, text=True)
            ok = proc.returncode == 0
            failures += 0 if ok else 1
            print("  [%s] %-12s %s" % ("OK  " if ok else "FAIL", entry, target))
            for line in (proc.stdout + proc.stderr).splitlines():
                if "error" in line.lower() or "warning" in line.lower():
                    print("         " + line.strip())
        if not ok:
            const_ok = False

        # The main PS carries the full cbuffer; use it to police the C# mapping.
        if ok and const_name == "Hlsl" and entry == "PS" and os.path.exists(asm_path):
            failures += check_layout(open(asm_path).read(), src)

        # Compare against a previous run to prove a refactor changed nothing.
        base_asm = os.path.join(basedir, "%s_%s.asm" % (const_name, entry)) if basedir else None
        if ok and base_asm and os.path.exists(base_asm):
            compared += 1
            new, old = open(asm_path).read(), open(base_asm).read()
            same_ops = opcodes(new) == opcodes(old)
            same_cb = cbuf_reads(new) == cbuf_reads(old)
            print("         opcodes %s, cbuffer reads %s"
                  % ("SAME" if same_ops else "CHANGED",
                     "SAME" if same_cb else "CHANGED"))
            if not (same_ops and same_cb):
                failures += 1

    # Only stamp after every entry succeeded, so a failed build never marks itself cached.
    if const_ok:
        try:
            open(stamp, "w").write(digest)
        except OSError:
            pass

print()
if failures:
    print("%d problem(s)" % failures)
elif compared:
    print("all shaders compiled; %d compared, no behaviour change" % compared)
else:
    print("all shaders compiled")
sys.exit(1 if failures else 0)
