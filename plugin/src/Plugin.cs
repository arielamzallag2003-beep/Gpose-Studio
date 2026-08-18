using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using GPoseStudio.Ui;

namespace GPoseStudio;

public sealed class Plugin : IDalamudPlugin
{
    private const string CmdName = "/gposestudio";

    private readonly WindowSystem _windows = new("GPoseStudio");
    private readonly MainWindow _main;
    private readonly GposeGate _gate;
    private readonly LiveOverlay _live;

    internal static PluginConfig Config { get; private set; } = null!;

    public Plugin(IDalamudPluginInterface pi)
    {
        Services.Init(pi);
        Config = PluginConfig.Load();
        try { LookStore.SeedBuiltins(); } catch {  }
        Config.LivePreview = false;

        _gate = new GposeGate();
        _live = new LiveOverlay(_gate);
        _main = new MainWindow(_gate, _live);
        _windows.AddWindow(_main);

        var ui = Services.PluginInterface.UiBuilder;
        ui.DisableGposeUiHide = true;
        ui.DisableUserUiHide = true;
        ui.DisableCutsceneUiHide = true;

        Services.PluginInterface.UiBuilder.Draw += DrawUi;
        Services.PluginInterface.UiBuilder.OpenMainUi += OpenMain;
        Services.PluginInterface.UiBuilder.OpenConfigUi += OpenMain;

        Services.Commands.AddHandler(CmdName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Toggle the GPoseStudio window.",
        });

        Services.Log.Info($"GPoseStudio plugin loaded — build {typeof(Plugin).Assembly.GetName().Version}.");
    }

    private void DrawUi()
    {
        if (!Services.GameGui.GameUiHidden)
            _windows.Draw();
    }

    private void OpenMain() => _main.IsOpen = true;
    private void OnCommand(string _, string __) => _main.Toggle();

    public void Dispose()
    {
        Services.Commands.RemoveHandler(CmdName);
        Services.PluginInterface.UiBuilder.Draw -= DrawUi;
        Services.PluginInterface.UiBuilder.OpenMainUi -= OpenMain;
        Services.PluginInterface.UiBuilder.OpenConfigUi -= OpenMain;
        _windows.RemoveAllWindows();
        _live.Dispose();
        _gate.Dispose();
        _main.Dispose();
    }
}
