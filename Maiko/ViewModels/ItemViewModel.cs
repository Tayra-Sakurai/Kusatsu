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
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Maiko.ViewModels
{
    public partial class ItemViewModel : ObservableValidator
    {
        private readonly IDbContextFactory<MaikoContext> factory;
        private Item item;

        public ItemViewModel(IDbContextFactory<MaikoContext> factory)
        {
            this.factory = factory;
            item = new();
            Tags = [];
            AllTags = [];
            Tags.CollectionChanged += Tags_CollectionChanged;
            LargeCategories = [];
            MediumCategories = [];
            SmallCategories = [];
            Methods = [];
            ErrorsChanged += ItemViewModel_ErrorsChanged;
        }

        private void ItemViewModel_ErrorsChanged(object? sender, System.ComponentModel.DataErrorsChangedEventArgs e)
        {
            SaveCommand.NotifyCanExecuteChanged();
        }

        private void Tags_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            item.Tags.Clear();

            foreach (Tag tag in Tags)
                item.Tags.Add(tag);
        }

        [LocalRequired(ErrorMessageFileName = "Maiko/Resources", ErrorMessageResourceName = "RequiredMessage")]
        public string Name
        {
            get => item.Name;
            set => SetProperty(item.Name, value, item, (m, v) => m.Name = v, true);
        }

        public string Description
        {
            get => item.Description;
            set => SetProperty(item.Description, value, item, (m, v) => m.Description = v, false);
        }

        [Required]
        public DateTimeOffset? Date
        {
            get => item.DateTime.Date;
            set
            {
                ValidateProperty(value);

                if (value is not DateTimeOffset dateTimeOffset)
                {
                    OnPropertyChanged();
                    return;
                }

                DateTime localValue = dateTimeOffset.LocalDateTime.Date;
                if (localValue != item.DateTime.Date)
                {
                    TimeSpan time = item.DateTime.ToLocalTime().TimeOfDay;
                    item.DateTime = localValue;
                    item.DateTime.Add(time);
                    OnPropertyChanged();
                }
            }
        }

        [Required]
        public TimeSpan? Time
        {
            get => item.DateTime.TimeOfDay;
            set
            {
                ValidateProperty(value);

                if (value is not TimeSpan time)
                {
                    OnPropertyChanged();
                    return;
                }

                if (time != item.DateTime.TimeOfDay)
                {
                    item.DateTime = item.DateTime.Date;
                    item.DateTime.Add(time);
                    OnPropertyChanged();
                }
            }
        }

        [LocalRange(0d, double.MaxValue, ErrorMessageFileName = "Maiko/Resources", ErrorMessageResourceName = "RangeErrorMessage")]
        public double Amount
        {
            get => item.Amount;
            set => SetProperty(item.Amount, value, item, (m, v) => m.Amount = v, true);
        }

        public ObservableCollection<Tag> Tags
        {
            get;

            set
            {
                if (field == value) return;

                field?.CollectionChanged -= Tags_CollectionChanged;
                field = value;
                field.CollectionChanged += Tags_CollectionChanged;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Tag> AllTags { get; }
        public ObservableCollection<LargeCategory> LargeCategories { get; }
        public ObservableCollection<MediumCategory> MediumCategories { get; }
        public ObservableCollection<SmallCategory> SmallCategories { get; }
        public ObservableCollection<Method> Methods { get; }

        [Required]
        public Method? Method
        {
            get
            {
                return Methods.FirstOrDefault(mtd => item.MethodId == mtd.Id);
            }

            set
            {
                ValidateProperty(value);
                if (value == null)
                {
                    OnPropertyChanged();
                    return;
                }

                if (value.Id != item.MethodId)
                {
                    item.MethodId = value.Id;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The previously selected large category.
        /// </summary>
        /// <remarks>
        /// This is used to compare with the new value.
        /// </remarks>
        private LargeCategory? prevLargeCategory;

        /// <summary>
        /// The previously selected medium category.
        /// </summary>
        private MediumCategory? prevMediumCategory;

        [Required]
        public LargeCategory? LargeCategory
        {
            get
            {
                if (LargeCategories.Any(lc => MediumCategories.All(mc => mc.LargeCategoryId == lc.Id)))
                    LargeCategories.FirstOrDefault(lc => MediumCategories.All(mc => mc.LargeCategoryId == lc.Id));

                return LargeCategories.FirstOrDefault();
            }

            set
            {
                ValidateProperty(value);
                if (value is null)
                {
                    OnPropertyChanged();
                    return;
                }

                if (value.Id != MediumCategory?.LargeCategoryId)
                {
                    MediumCategories.Clear();

                    foreach (
                        MediumCategory mc in value.MediumCategories)
                        MediumCategories.Add(mc);

                    MediumCategory = MediumCategories.FirstOrDefault();
                }

                if (prevLargeCategory?.Id != value.Id)
                {
                    OnPropertyChanged();
                    prevLargeCategory = value;
                }
            }
        }

        [Required]
        public MediumCategory? MediumCategory
        {
            get
            {
                if (MediumCategories.Any(mc => mc.Id == SmallCategory?.MediumCategoryId))
                    return MediumCategories.FirstOrDefault(mc => mc.Id == SmallCategory?.MediumCategoryId);

                return MediumCategories.FirstOrDefault();
            }

            set
            {
                ValidateProperty(value);

                if (value == null)
                {
                    OnPropertyChanged();
                    return;
                }

                if (value.Id != SmallCategory?.MediumCategoryId)
                {
                    SmallCategories.Clear();

                    foreach (
                        SmallCategory sc in value.SmallCategories)
                        SmallCategories.Add(sc);

                    SmallCategory = SmallCategories.FirstOrDefault();
                    OnPropertyChanged(nameof(SmallCategory));
                }

                if (prevMediumCategory?.Id != value.Id)
                {
                    OnPropertyChanged();
                    prevMediumCategory = value;
                }
            }
        }

        [Required]
        public SmallCategory? SmallCategory
        {
            get
            {
                if (SmallCategories.Any(sc => sc.Id == item.CategoryId))
                    return SmallCategories.First(sc => sc.Id == item.CategoryId);

                return null;
            }

            set
            {
                ValidateProperty(value);
                if (value == null)
                {
                    OnPropertyChanged();
                    return;
                }

                if (value.Id != item.CategoryId)
                {
                    item.CategoryId = value.Id;
                    item.Category = value;
                    OnPropertyChanged();
                }
            }
        }

        public async Task LoadAsync()
        {
            using MaikoContext context = await factory.CreateDbContextAsync();

            LargeCategories.Clear();

            await foreach (
                LargeCategory largeCategory in
                context.LargeCategories
                .Include(lc => lc.MediumCategories)
                .ThenInclude(mc => mc.SmallCategories)
                .AsNoTracking()
                .AsAsyncEnumerable())
                LargeCategories.Add(largeCategory);

            AllTags.Clear();

            await foreach (
                Tag tag in
                context.Tags
                .OrderBy(t => t.Name)
                .AsNoTracking()
                .AsAsyncEnumerable())
                AllTags.Add(tag);

            Methods.Clear();

            await foreach (
                Method method in
                context.Methods
                .AsNoTracking()
                .AsAsyncEnumerable())
                Methods.Add(method);
        }

        public void InitializeForExistingValue(Item item)
        {
            this.item = item;

            if (LargeCategories.Any(
                lc => lc.MediumCategories.Any(
                    mc => mc.SmallCategories.Any(
                        sc => sc.Id == item.CategoryId))))
            {
                LargeCategory lc = LargeCategories.First(
                    large => large.MediumCategories.Any(
                        medium => medium.SmallCategories.Any(
                            small => small.Id == item.CategoryId)));
                MediumCategories.Clear();

                foreach (
                    MediumCategory medium in
                    lc.MediumCategories)
                    MediumCategories.Add(medium);

                MediumCategory mc = MediumCategories.First(
                    medium => medium.SmallCategories.Any(
                        small => small.Id == item.CategoryId));

                SmallCategories.Clear();

                foreach (SmallCategory smallCategory in mc.SmallCategories)
                    SmallCategories.Add(smallCategory);

                OnPropertyChanged(nameof(SmallCategory));
                OnPropertyChanged(nameof(MediumCategory));
                OnPropertyChanged(nameof(LargeCategory));
            }

            Tags.CollectionChanged -= Tags_CollectionChanged;

            Tags.Clear();

            foreach (Tag tag in item.Tags)
                Tags.Add(tag);

            Tags.CollectionChanged += Tags_CollectionChanged;

            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(Date));
            OnPropertyChanged(nameof(Time));
            OnPropertyChanged(nameof(Amount));
            OnPropertyChanged(nameof(Method));

            ValidateAllProperties();
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanSave))]
        private async Task SaveAsync()
        {
            using MaikoContext context = await factory.CreateDbContextAsync();

            // Remove all related entity data to avoid concurrency.

            foreach (Tag tag in item.Tags)
            {
                tag.Items.Clear();
            }

            item.Category = null;

            item.Method = null;

            // Then, update the database.

            context.Update(item);
            await context.SaveChangesAsync();

            // Finally send the message to leave the page.

            WeakReferenceMessenger.Default.Send(new ItemUpdatedMessage(item));
        }

        private bool CanSave()
        {
            return !HasErrors;
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task RemoveAsync()
        {
            using MaikoContext context = await factory.CreateDbContextAsync();

            // Clear all relation loaders.

            item.Category = null;
            item.Tags.Clear();
            item.Method = null;

            context.Remove(item);
            await context.SaveChangesAsync();

            WeakReferenceMessenger.Default.Send(new ItemUpdatedMessage(item));
        }

        public virtual Tag GetTag(string key)
        {
            Tag? result = AllTags
                .ExceptBy(Tags.Select(t => t.Id), t => t.Id)
                .FirstOrDefault(t => t.Name.Contains(key));

            if (result == null)
                return new()
                {
                    Name = key,
                };

            return result;
        }
    }
}
