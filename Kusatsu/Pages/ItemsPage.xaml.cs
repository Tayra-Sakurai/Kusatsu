// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: AGPL-3.0-or-later

using CommunityToolkit.Mvvm.Messaging;
using Maiko.Messages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
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
    public sealed partial class ItemsPage : Page, IRecipient<ItemInvokedMessage>
    {
        private int previousSelectedIndex = 0;

        public ItemsPage()
        {
            InitializeComponent();

            TableSelector.SelectionChanged += TableSelector_SelectionChanged;
            PageFrame.Navigate(typeof(ExpensePage));
        }

        private void TableSelector_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
        {
            SelectorBarItem selectorBarItem = sender.SelectedItem;
            int selectedIndex = sender.Items.IndexOf(selectorBarItem);

            Type pageType = selectedIndex switch
            {
                0 => typeof(ExpensePage),
                _ => typeof(IncomePage),
            };

            SlideNavigationTransitionEffect slideNavigationTransitionEffect = selectedIndex > previousSelectedIndex ? SlideNavigationTransitionEffect.FromRight : SlideNavigationTransitionEffect.FromLeft;

            PageFrame.Navigate(pageType, null, new SlideNavigationTransitionInfo() { Effect = slideNavigationTransitionEffect });
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            WeakReferenceMessenger.Default.UnregisterAll(this);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            WeakReferenceMessenger.Default.Register(this);
        }

        public void Receive(ItemInvokedMessage message)
        {
            Frame.Navigate(typeof(ItemPage), message.Value);
        }
    }
}
