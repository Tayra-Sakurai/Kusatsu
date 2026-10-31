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
using System.Collections.ObjectModel;
using System.Text;

namespace Maiko.ViewModels
{
    public partial class CategoryViewModel : ObservableValidator
    {
        private readonly IDbContextFactory<MaikoContext> factory;
        private Category category;

        public CategoryViewModel(IDbContextFactory<MaikoContext> factory)
        {
            this.factory = factory;
            LargeCategories = [];
            MediumCategories = [];
            PropertyChanged += CategoryViewModel_PropertyChanged;
            category = new LargeCategory();
            ErrorsChanged += CategoryViewModel_ErrorsChanged;
        }

        private void CategoryViewModel_ErrorsChanged(object? sender, System.ComponentModel.DataErrorsChangedEventArgs e)
        {
            SaveCommand.NotifyCanExecuteChanged();
        }

        private void CategoryViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LargeCategory))
            {
                if (LargeCategory is not null)
                {
                    MediumCategories.Clear();
                    foreach (
                        MediumCategory category in
                        LargeCategory.MediumCategories)
                        MediumCategories.Add(category);
                }
            }
        }

        [ObservableProperty]
        public partial ObservableCollection<LargeCategory> LargeCategories { get; set; }

        [ObservableProperty]
        public partial ObservableCollection<MediumCategory> MediumCategories { get; set; }

        public LargeCategory? LargeCategory
        {
            get
            {
                if (category == null ||
                    category is LargeCategory)
                    return null;

                if (category is MediumCategory mediumCategory)
                    return LargeCategories.SingleOrDefault(lc => lc.Id == mediumCategory.LargeCategoryId);

                if (category is SmallCategory)
                    return LargeCategories.SingleOrDefault(lc => lc.Id == MediumCategory?.Id);

                return null;
            }

            set
            {
                if (value == null)
                    return;

                if (category is MediumCategory mediumCategory &&
                    mediumCategory.LargeCategoryId != value.Id)
                {
                    OnPropertyChanged();
                    SetParent(value);
                }

                if (category is SmallCategory &&
                    MediumCategory?.LargeCategoryId != value.Id)
                {
                    OnPropertyChanged();
                    SetParent(value);
                }
            }
        }

        public MediumCategory? MediumCategory
        {
            get
            {
                if (category is not SmallCategory smallCategory)
                    return null;

                return MediumCategories
                    .SingleOrDefault(mc => mc.Id == smallCategory.MediumCategoryId) ??
                    LargeCategories
                    .FirstOrDefault(lc => lc.MediumCategories.Any(mc => mc.Id == smallCategory.MediumCategoryId))
                    ?.MediumCategories
                    ?.FirstOrDefault(mc => mc.Id == smallCategory.MediumCategoryId);
            }

            set
            {
                if (value is null ||
                    category is not SmallCategory smallCategory)
                    return;

                if (smallCategory.MediumCategoryId != value.Id)
                {
                    bool mustChangeLarge = value.LargeCategoryId == LargeCategory?.Id;
                    SetParent(value);
                    OnPropertyChanged();
                    if (mustChangeLarge)
                        OnPropertyChanged(nameof(LargeCategory));
                }
            }
        }

        private void SetParent(LargeCategory largeCategory)
        {
            if (category is LargeCategory)
                return;

            if (category is MediumCategory mc)
            {
                mc.LargeCategoryId = largeCategory.Id;
                return;
            }

            if (category is SmallCategory sc)
            {
                sc.MediumCategoryId = largeCategory.MediumCategories.FirstOrDefault()?.Id ?? 0;
                return;
            }
        }

        private void SetParent(MediumCategory mediumCategory)
        {
            if (category is not SmallCategory smallCategory)
                return;

            smallCategory.MediumCategoryId = mediumCategory.Id;
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task LoadAsync()
        {
            using MaikoContext context = await factory.CreateDbContextAsync();

            LargeCategories.Clear();

            await foreach (
                LargeCategory largeCategory in
                context.LargeCategories
                .Include(lc => lc.MediumCategories)
                .AsNoTracking()
                .AsAsyncEnumerable())
                LargeCategories.Add(largeCategory);

            LargeCategory = LargeCategories.FirstOrDefault();
        }

        public void InitializeForExistingValue(Category category)
        {
            this.category = category;

            if (category is SmallCategory ||
                category is MediumCategory)
            {
                if (category is SmallCategory)
                    OnPropertyChanged(nameof(MediumCategory));
                OnPropertyChanged(nameof(LargeCategory));
            }
        }

        [LocalRequired(ErrorMessageFileName = "Maiko/Resources", ErrorMessageResourceName = "RequiredMessage")]
        public string Name
        {
            get => category.Name;
            set => SetProperty(category.Name, value, category, (m, v) => m.Name = v, true);
        }

        [LocalRange(0d, double.MaxValue, ErrorMessageFileName = "Maiko/Resources", ErrorMessageResourceName = "RangeErrorMessage")]
        public double Budget
        {
            get => category.Budget;
            set => SetProperty(category.Budget, value, category, (m, v) => m.Budget = v, true);
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanSave))]
        private async Task SaveAsync()
        {
            if (HasErrors)
                return;

            using MaikoContext context = await factory.CreateDbContextAsync();

            if (category is LargeCategory largeCategory)
            {
                largeCategory.MediumCategories.Clear();
                context.Update(largeCategory);
            }

            if (category is MediumCategory mediumCategory)
            {
                mediumCategory.LargeCategory = null;
                mediumCategory.SmallCategories.Clear();
                context.Update(mediumCategory);
            }

            if (category is SmallCategory smallCategory)
            {
                smallCategory.MediumCategory = null;
                smallCategory.Items.Clear();
                context.Update(smallCategory);
            }

            await context.SaveChangesAsync();
            WeakReferenceMessenger.Default.Send(new CategoryUpdatedMessage(category));
        }

        private bool CanSave()
        {
            return !HasErrors;
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task RemoveAsync()
        {
            using MaikoContext context = await factory.CreateDbContextAsync();

            if (category is LargeCategory largeCategory)
            {
                largeCategory.MediumCategories.Clear();
                context.Remove(largeCategory);
            }
            else if (category is MediumCategory mediumCategory)
            {
                mediumCategory.LargeCategory = null;
                mediumCategory.SmallCategories.Clear();
                context.Remove(mediumCategory);
            }
            else if (category is SmallCategory smallCategory)
            {
                smallCategory.MediumCategory = null;
                smallCategory.Items.Clear();
                context.Remove(smallCategory);
            }

            await context.SaveChangesAsync();
            WeakReferenceMessenger.Default.Send(new CategoryUpdatedMessage(category));
        }
    }
}
