// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: AGPL-3.0-or-later

using Kusatsu.Models;
using Kusatsu.Pages;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Text.Json;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Kusatsu
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);
            AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
            AppWindow.SetIcon("Assets/AppIcon.ico");
            AppWindow.Closing += AppWindow_Closing;
        }

        private async void AppWindow_Closing(AppWindow sender, AppWindowClosingEventArgs args)
        {
            OverlappedPresenter? overlappedPresenter = sender.Presenter as OverlappedPresenter;

            if (overlappedPresenter != null)
            {
                string cacheFile = Path.Combine(Microsoft.Windows.Storage.ApplicationData.GetDefault().LocalCacheFolder.Path, "cache.json");
                if (!File.Exists(cacheFile))
                {
                    await Microsoft.Windows.Storage.ApplicationData.GetDefault().LocalCacheFolder.CreateFileAsync("cache.json");
                }
                CacheData cacheData = new()
                {
                    State = overlappedPresenter.State,
                };
                string jsonContent = JsonSerializer.Serialize(cacheData);
                await File.WriteAllTextAsync(cacheFile, jsonContent);
            }
        }

        private void TitleBar_PaneToggleRequested(TitleBar sender, object args)
        {
            NavView.IsPaneOpen = !NavView.IsPaneOpen;
        }

        private void TitleBar_BackRequested(TitleBar sender, object args)
        {
            NavFrame.GoBack();
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                NavFrame.Navigate(typeof(SettingsPage));
            }
            else if (args.SelectedItem is NavigationViewItem item)
            {
                switch (item.Tag)
                {
                    case "home":
                        NavFrame.Navigate(typeof(HomePage));
                        break;
                    case "about":
                        NavFrame.Navigate(typeof(AboutPage));
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown navigation item tag: {item.Tag}");
                }
            }
        }
    }
}
