using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace SpellforceDataEditor.Avalonia;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        Name = "Spellforce Data Editor";
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
            SFEngine.SFLua.SFLuaEnvironment.MessageBoxService = new AvaloniaMessageBoxService();
        }

        base.OnFrameworkInitializationCompleted();
    }
}