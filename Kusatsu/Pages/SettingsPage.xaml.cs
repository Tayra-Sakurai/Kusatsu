// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: AGPL-3.0-or-later

using CommunityToolkit.Mvvm.DependencyInjection;
using Kusatsu.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Kusatsu.Pages
{
    public sealed partial class SettingsPage : Page
    {
        private SettingsViewModel? viewModel;

        public SettingsPage()
        {
            InitializeComponent();
            CategoryJumper.Click += CategoryJumper_Click;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            viewModel = Ioc.Default.GetRequiredService<SettingsViewModel>();
            await viewModel.LoadModelsAsync();
        }

        private void CategoryJumper_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CategoriesPage));
        }
    }
}
