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
    public sealed partial class ItemPage : Page, IRecipient<ItemUpdatedMessage>
    {
        private ItemViewModel? viewModel;

        public ItemPage()
        {
            InitializeComponent();

            TagsBox.TokenItemAdding += TagsBox_TokenItemAdding;
        }

        private void TagsBox_TokenItemAdding(CommunityToolkit.WinUI.Controls.TokenizingTextBox sender, CommunityToolkit.WinUI.Controls.TokenItemAddingEventArgs args)
        {
            args.Item = viewModel?.GetTag(args.TokenText);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            viewModel = Ioc.Default.GetRequiredService<ItemViewModel>();
            await viewModel.LoadAsync();

            if (e.Parameter is Item item)
                viewModel.InitializeForExistingValue(item);

            WeakReferenceMessenger.Default.Register(this);
        }

        public void Receive(ItemUpdatedMessage message)
        {
            Frame.Navigate(typeof(ItemsPage));
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            WeakReferenceMessenger.Default.UnregisterAll(this);
        }
    }
}
