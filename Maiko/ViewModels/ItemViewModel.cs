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

        private LargeCategory? largeCategory;
        private MediumCategory? mediumCategory;

        [Required]
        public LargeCategory? LargeCategory
        {
            get => largeCategory;

            set
            {
                ValidateProperty(value);
                if (value is null || largeCategory?.Id == value.Id)
                {
                    return;
                }

                largeCategory = value;
                OnPropertyChanged();

                MediumCategories.Clear();

                foreach (
                    MediumCategory mc in value.MediumCategories)
                    MediumCategories.Add(mc);

                MediumCategory = MediumCategories.FirstOrDefault();
            }
        }

        [Required]
        public MediumCategory? MediumCategory
        {
            get => mediumCategory;

            set
            {
                ValidateProperty(value);

                if (value == null || mediumCategory?.Id == value.Id)
                {
                    return;
                }

                mediumCategory = value;
                OnPropertyChanged();

                SmallCategories.Clear();

                foreach (
                    SmallCategory sc in value.SmallCategories)
                    SmallCategories.Add(sc);

                SmallCategory = SmallCategories.FirstOrDefault();
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
                if (value == null || item.CategoryId == value.Id)
                {
                    return;
                }

                item.CategoryId = value.Id;
                item.Category = value;
                OnPropertyChanged();
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

                largeCategory = lc;
                mediumCategory = mc;

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
