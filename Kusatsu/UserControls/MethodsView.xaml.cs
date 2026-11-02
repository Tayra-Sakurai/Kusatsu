// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: AGPL-3.0-or-later

using CommunityToolkit.Mvvm.DependencyInjection;
using Maiko.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Kusatsu.UserControls
{
    public sealed partial class MethodsView : UserControl
    {
        private MethodsViewModel viewModel;

        public MethodsView()
        {
            viewModel = Ioc.Default.GetRequiredService<MethodsViewModel>();
            InitializeComponent();

            Unloaded += MethodsView_Unloaded;
            Loading += MethodsView_Loading;
        }

        private async void MethodsView_Loading(FrameworkElement sender, object e)
        {
            viewModel.IsActive = true;
            await viewModel.LoadAsync();
        }

        private void MethodsView_Unloaded(object sender, RoutedEventArgs e)
        {
            viewModel?.IsActive = false;
        }
    }
}
