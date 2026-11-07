// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: AGPL-3.0-or-later

using CommunityToolkit.Mvvm.DependencyInjection;
using Imazu.ViewModels;
using Kusatsu.Models;
using Kusatsu.ViewModels;
using Maiko.Contexts;
using Maiko.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OllamaSharp;
using System.Text.Json;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Kusatsu
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private Window? _window;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            SetupServices();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            IDbContextFactory<MaikoContext> dbContextFactory = Ioc.Default.GetRequiredService<IDbContextFactory<MaikoContext>>();
            using (MaikoContext context = await dbContextFactory.CreateDbContextAsync())
                await context.Database.MigrateAsync();

            _window = new MainWindow();
            OverlappedPresenter overlappedPresenter = OverlappedPresenter.Create();
            overlappedPresenter.IsMaximizable = true;
            overlappedPresenter.IsMinimizable = true;
            overlappedPresenter.IsResizable = true;

            Microsoft.Windows.Storage.ApplicationData applicationData = Microsoft.Windows.Storage.ApplicationData.GetDefault();
            string cachePath = applicationData.LocalCacheFolder.Path;
            string cacheDataPath = System.IO.Path.Combine(cachePath, "chache.json");

            if (System.IO.File.Exists(cacheDataPath))
            {
                string jsonStr = await System.IO.File.ReadAllTextAsync(cacheDataPath);

                CacheData? cacheData = JsonSerializer.Deserialize<CacheData>(jsonStr);
                if (cacheData != null)
                {
                    switch (cacheData.State)
                    {
                        case OverlappedPresenterState.Restored:
                        case OverlappedPresenterState.Minimized:
                            overlappedPresenter.Restore();
                            break;
                        case OverlappedPresenterState.Maximized:
                            overlappedPresenter.Maximize();
                            break;
                    }
                }
            }

            _window.AppWindow.SetPresenter(overlappedPresenter);
            _window.Activate();
        }

        private static void SetupServices()
        {
            Microsoft.Windows.Storage.ApplicationDataContainer settings = Microsoft.Windows.Storage.ApplicationData.GetDefault().LocalSettings;
            if (settings.Values["IsAIEnabled"] is not true)
            {
                settings.Values["IsAIEnabled"] = false;
                Ioc.Default.ConfigureServices(GetService(false));
                return;
            }
            Ioc.Default.ConfigureServices(GetService(true));
        }

        private static IServiceProvider GetService(bool isAiEnabled)
        {
            ServiceCollection services = new();

            services.AddDbContextFactory<MaikoContext>(
                optionsBuilder =>
                optionsBuilder.UseSqlite($"Data Source={System.IO.Path.Join(Microsoft.Windows.Storage.ApplicationData.GetDefault().LocalFolder.Path, "Maiko.db")}"));
            services.AddTransient<CategoriesViewModel>();
            services.AddTransient<CategoryViewModel>();
            services.AddTransient<MethodsViewModel>();
            services.AddTransient<MethodViewModel>();
            services.AddTransient<ItemsViewModel>();
            services.AddTransient<ItemViewModel>();
            services.AddTransient<SettingsViewModel>();

            if (isAiEnabled)
            {
                IPropertySet keyValuePairs = Microsoft.Windows.Storage.ApplicationData.GetDefault().LocalSettings.Values;
                services.AddChatClient(new OllamaApiClient(
                    new Uri("http://localhost:11434/api"),
                    (keyValuePairs["OllamaModel"] as string) ?? "gemma4:e2b"));
                services.AddEmbeddingGenerator(new OllamaApiClient(
                    new Uri("http://localhost:11434/api"),
                    (keyValuePairs["EmbeddingModel"] as string) ?? "embeddinggemma:latest"));
                services.AddSqliteVectorStore(
                    provider => $"Data Source={System.IO.Path.Join(Microsoft.Windows.Storage.ApplicationData.GetDefault().LocalFolder.Path, "Context.sqlite3")}",
                    provider => new() { EmbeddingGenerator = provider.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>() });
                services.AddTransient<AgentViewModel>();
            }

            return services.BuildServiceProvider();
        }
    }
}
