# GPoseStudio

Live cinematic looks for FFXIV's GPose. A Dalamud plugin that grades, lights and
re-backgrounds the GPose frame in real time, and exports the result at up to 4x.

Cosmetic and client-side only. It reads the rendered frame and its depth buffer,
draws over them, and saves a PNG. Nothing is sent anywhere, no game memory is
modified, and it sits alongside Brio and Ktisis rather than competing with them.

## Install

Add the plugin repository once, then install from Dalamud's plugin installer.

1. In game: `/xlsettings` → **Experimental** → **Custom Plugin Repositories**
2. Add this URL and press the **+**, then **Save and Close**:

   ```
   https://raw.githubusercontent.com/arielamzallag2003-beep/Gpose-Studio/repo/pluginmaster.json
   ```

3. `/xlplugins` → search **GPoseStudio** → **Install**

Updates then arrive through the normal plugin installer. If you previously copied
a build into `devPlugins` by hand, delete that copy first or you will run two.

Open the window with `/gposestudio` (or the button in the GPose panel).

## Using it

Start in the **Looks** tab. Load one of the built-in looks, then work rightward
through the tabs to adjust what it set. Every effect can be routed to the
foreground, the character, the background, or any combination.

The one control worth knowing about first is **Start (depth)** in the Background
tab. It decides how far from the camera counts as "background". Set too far, it
selects nothing and the backdrop never appears — which looks like the plugin is
broken. Tick **Show what this covers** next to it: the frame repaints magenta
where the backdrop reaches and cyan where the solid fill reaches. If nothing is
tinted, lower the value until the wall behind your subject lights up.

**Bypass** at the top of the window is an A/B compare against the original frame.

## Building

Requires the .NET 10 SDK, a Dalamud install, Python 3, and the Windows SDK's
`fxc.exe`.

```
cd plugin
dotnet build -c Release
```

The build precompiles the HLSL and embeds the bytecode. The main pixel shader
takes around fifteen minutes through `fxc`, so the step is cached against the
shader text and only re-runs when that text changes; a build that touches only C#
takes a few seconds. Without Python or `fxc` the build still succeeds and the
plugin compiles its shaders at runtime instead.

`python tools/check_shaders.py` compiles every shader and checks that the C#
`Params` struct still matches the HLSL `cbuffer` — that mapping is positional and
has no compile-time guard, so run it after touching either.

## License

MIT. See [LICENSE](LICENSE).
