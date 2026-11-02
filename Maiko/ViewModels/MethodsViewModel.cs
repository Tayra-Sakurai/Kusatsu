// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: AGPL-3.0-or-later

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Maiko.Contexts;
using Maiko.Messages;
using Maiko.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Maiko.ViewModels
{
    public partial class MethodsViewModel : ObservableRecipient, IRecipient<MethodUpdatedMessage>
    {
        private readonly IDbContextFactory<MaikoContext> factory;

        public MethodsViewModel(IDbContextFactory<MaikoContext> factory)
            : base()
        {
            this.factory = factory;
            Methods = [];
        }

        [ObservableProperty]
        public partial ObservableCollection<Method> Methods { get; set; }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(DetailCommand))]
        public partial Method? Method { get; set; }

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task LoadAsync()
        {
            using MaikoContext context = await factory.CreateDbContextAsync();

            Methods.Clear();

            await foreach (
                Method method in
                context
                .Methods
                .Include(method => method.Items)
                .AsNoTracking()
                .AsAsyncEnumerable())
                Methods.Add(method);

            Method = null;
        }

        public async void Receive(MethodUpdatedMessage message)
        {
            await LoadAsync();
        }

        [RelayCommand(CanExecute = nameof(CanInvoke))]
        private void Detail()
        {
            if (Method is null)
                return;

            WeakReferenceMessenger.Default.Send(new MethodInvokedMessage(Method));
        }

        private bool CanInvoke()
        {
            return Method is not null;
        }
    }
}
