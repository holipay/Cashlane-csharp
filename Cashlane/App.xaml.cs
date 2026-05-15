using System.Windows;
using Cashlane.Services;
using Cashlane.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Cashlane;

public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();

        // Register services
        var db = new DatabaseService();
        db.Initialize();
        services.AddSingleton(db);
        services.AddSingleton<ExcelService>();
        services.AddSingleton<MainViewModel>();

        _serviceProvider = services.BuildServiceProvider();

        var mainWindow = new Views.MainWindow
        {
            DataContext = _serviceProvider.GetRequiredService<MainViewModel>()
        };
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}
