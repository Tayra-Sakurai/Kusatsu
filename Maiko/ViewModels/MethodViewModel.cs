// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: AGPL-3.0-or-later

using CommunityToolkit.Localized.Mvvm.ValidateAttributes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Maiko.Contexts;
using Maiko.Messages;
using Maiko.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maiko.ViewModels
{
    [ObservableRecipient]
    public partial class MethodViewModel : ObservableValidator, IRecipient<MethodInvokedMessage>
    {
        private readonly IDbContextFactory<MaikoContext> factory;
        private Method method;

        public MethodViewModel(IDbContextFactory<MaikoContext> factory)
        {
            Messenger = WeakReferenceMessenger.Default;
            this.factory = factory;
            method = new();
            ErrorsChanged += MethodViewModel_ErrorsChanged;
        }

        private void MethodViewModel_ErrorsChanged(object? sender, System.ComponentModel.DataErrorsChangedEventArgs e)
        {
            SaveCommand.NotifyCanExecuteChanged();
        }

        protected virtual partial void OnActivated()
        {
            Messenger.RegisterAll(this);
        }

        protected virtual partial void OnDeactivated()
        {
            Messenger.UnregisterAll(this);
        }

        private void InitializeForExistingValue(Method method)
        {
            this.method = method;
            OnPropertyChanged(nameof(Name));
        }

        public void Receive(MethodInvokedMessage message)
        {
            InitializeForExistingValue(message.Value);
        }

        [LocalRequired(ErrorMessageFileName = "Maiko/Resources", ErrorMessageResourceName = "RequiredMessage")]
        public string Name
        {
            get => method.Name;
            set => SetProperty(method.Name, value, method, (m, v) => m.Name = v, true);
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanSave))]
        private async Task SaveAsync()
        {
            if (HasErrors)
                return;

            using MaikoContext context = await factory.CreateDbContextAsync();

            method.Items.Clear();
            context.Update(method);

            await context.SaveChangesAsync();
            WeakReferenceMessenger.Default.Send(new MethodUpdatedMessage(method));

            method = new();
            OnPropertyChanged(nameof(Name));
        }

        private bool CanSave()
        {
            return !HasErrors;
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task RemoveAsync()
        {
            using MaikoContext context = await factory.CreateDbContextAsync();

            method.Items.Clear();

            context.Remove(method);
            await context.SaveChangesAsync();
        }
    }
}
