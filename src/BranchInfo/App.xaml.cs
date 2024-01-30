using System.Windows;
using BranchInfo.Business;
using Serilog;

namespace BranchInfo;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Occurs when the app was started
    /// </summary>
    /// <param name="sender">The <see cref="App"/></param>
    /// <param name="e">The event arguments</param>
    private void App_OnStartup(object sender, StartupEventArgs e)
    {
        // Init the logger
        const string template = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}";
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day, outputTemplate: template)
            .CreateLogger();

        ThemeHelper.SetColorTheme();
    }
}