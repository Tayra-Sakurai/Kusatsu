// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: AGPL-3.0-or-later

using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using Maiko.Messages;
using Maiko.Models;
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

namespace Kusatsu.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CategoriesPage : Page, IRecipient<CategoryInvokedMessage>
    {
        private CategoriesViewModel? viewModel;

        public CategoriesPage()
        {
            InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            viewModel = Ioc.Default.GetRequiredService<CategoriesViewModel>();
            await viewModel.LoadAsync();

            WeakReferenceMessenger.Default.Register(this);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            WeakReferenceMessenger.Default.UnregisterAll(this);
        }

        public void Receive(CategoryInvokedMessage message)
        {
            Frame.Navigate(typeof(CategoryPage), message.Value);
        }
    }

    public partial class CategoryTemplateSelector : DataTemplateSelector
    {
        public DataTemplate LargeTemplate { get; set; }
        public DataTemplate MediumTemplate { get; set; }
        public DataTemplate SmallTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is not Category category)
                throw new NotImplementedException();

            if (category is LargeCategory)
                return LargeTemplate;
            else if (category is MediumCategory)
                return MediumTemplate;
            else if (category is SmallCategory)
                return SmallTemplate;

            throw new NotImplementedException();
        }
    }
}
