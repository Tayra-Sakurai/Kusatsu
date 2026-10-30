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
    public partial class CategoriesViewModel : ObservableObject
    {
        private readonly IDbContextFactory<MaikoContext> factory;

        public CategoriesViewModel(IDbContextFactory<MaikoContext> factory)
        {
            this.factory = factory;
            LargeCategories = [];
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(InvokeCommand))]
        public partial Category? Category { get; set; }

        [ObservableProperty]
        public partial ObservableCollection<LargeCategory> LargeCategories { get; set; }

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task LoadAsync()
        {
            using MaikoContext context = await factory.CreateDbContextAsync();

            LargeCategories.Clear();

            await foreach (
                LargeCategory largeCategory in
                context.LargeCategories
                .Include(lc => lc.MediumCategories)
                .ThenInclude(mc => mc.SmallCategories)
                .ThenInclude(sc => sc.Items)
                .AsNoTracking()
                .AsAsyncEnumerable())
                LargeCategories.Add(largeCategory);

            Category = null;
        }

        [RelayCommand(CanExecute = nameof(CanInvoke))]
        private void Invoke()
        {
            if (Category is null)
                return;

            WeakReferenceMessenger.Default.Send(new CategoryInvokedMessage(Category));
        }

        private bool CanInvoke()
        {
            return Category is not null;
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanAdd))]
        private async Task AddAsync()
        {
            using MaikoContext context = await factory.CreateDbContextAsync();

            if (Category is null)
            {
                LargeCategory largeCategory = new();
                WeakReferenceMessenger.Default.Send(new CategoryInvokedMessage(largeCategory));
            }
            else if (Category is LargeCategory largeCategory1)
            {
                MediumCategory mediumCategory = new()
                {
                    LargeCategoryId = largeCategory1.Id,
                };
                WeakReferenceMessenger.Default.Send(new CategoryInvokedMessage(mediumCategory));
            }
            else if (Category is MediumCategory mediumCategory2)
            {
                SmallCategory smallCategory = new()
                {
                    MediumCategoryId = mediumCategory2.Id,
                };

                WeakReferenceMessenger.Default.Send(new CategoryInvokedMessage(smallCategory));
            }
            else if (Category is SmallCategory smallCategory2)
            {
                SmallCategory smallCategory3 = new()
                {
                    MediumCategoryId = smallCategory2.MediumCategoryId,
                };

                WeakReferenceMessenger.Default.Send(new CategoryInvokedMessage(smallCategory3));
            }
        }

        private bool CanAdd()
        {
            return
                Category is LargeCategory ||
                Category is MediumCategory ||
                Category is SmallCategory ||
                Category is null;
        }
    }
}
