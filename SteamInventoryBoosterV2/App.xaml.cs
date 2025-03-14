using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SteamInventoryBoosterV2.Data;
using SteamInventoryBoosterV2.ViewModels;
using SteamInventoryBoosterV2.Views;
using System;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Windows;

namespace SteamInventoryBoosterV2;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private IServiceProvider _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        _serviceProvider = serviceCollection.BuildServiceProvider();

        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Database.Migrate();
        }

        // Execut Start chrome Script
        ExecuteOpenChromePagesScript("start_chrome.bat");

        // Inicializa o singleton do webScraper manualmente, o singleton é iniciado quando o mesmo é utilizado 
        //pela primeira vez no projeto, como eu quero que as abras abra logo no inicio vou já forçar
        //essa inicialização
        //var webScraper = _serviceProvider.GetRequiredService<IWebScraper>();

        // Show Main Window
        var newMainWindow = _serviceProvider.GetRequiredService<MainAppWindow>();
        newMainWindow.Show();

        base.OnStartup(e);
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Regist Services
        services.AddDbContext<AppDbContext>();
        //services.AddSingleton<IWebScraper, WebScraper>();
        //.AddSingleton<ISteamApiService, SteamApiService>();
        //services.AddSingleton<ISteamItemDao, SteamItemDao>();
        //services.AddSingleton<ISteamItemService, SteamItemService>();

        // Regist Main Window
        services.AddTransient<MainAppWindow>();
        services.AddTransient<MainAppWindowViewModel>();

        // Regist pages
        services.AddSingleton<SteamItemsPage>();
        services.AddSingleton<InventoryManagementPage>();
        services.AddSingleton<BotManagerPage>();
        services.AddSingleton<SettingsPage>();

    }
    private static void ExecuteOpenChromePagesScript(string scriptName)
    {
        string scriptPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts", scriptName);

        ProcessStartInfo processStartInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/c \"{scriptPath}\"", // /c para executar e fechar
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        Process process = new Process
        {
            StartInfo = processStartInfo
        };
        process.Start();
    }
}

