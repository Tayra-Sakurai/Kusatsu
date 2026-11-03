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
    public partial class ItemsViewModel : ObservableObject
    {
        private readonly IDbContextFactory<MaikoContext> factory;

        public ItemsViewModel(IDbContextFactory<MaikoContext> factory)
        {
            this.factory = factory;
            Items = [];
            Incomes = [];
            Expenses = [];
            Candidates = [];
            DisplayedCandidates = [];
            Phrase = string.Empty;
        }

        public ObservableCollection<Item> Items { get; }

        public ObservableCollection<Item> Incomes { get; }

        public ObservableCollection<Item> Expenses { get; }

        public ObservableCollection<string> Candidates { get; }

        public ObservableCollection<string> DisplayedCandidates { get; }

        [ObservableProperty]
        public partial Item? Item { get; set; }

        [ObservableProperty]
        public partial string Phrase { get; set; }

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task LoadAsync()
        {
            using MaikoContext context = await factory.CreateDbContextAsync();

            Items.Clear();
            Expenses.Clear();
            Incomes.Clear();

            await foreach (
                Item item in
                context.Items
                .Include(i => i.Category)
                .ThenInclude(sc => sc!.MediumCategory)
                .ThenInclude(mc => mc!.LargeCategory)
                .Include(i => i.Method)
                .Include(i => i.Tags)
                .AsSplitQuery()
                .AsNoTracking()
                .AsAsyncEnumerable())
                Items.Add(item);

            foreach (
                Item expense in
                Items.Where(i => i.IsExpense))
                Expenses.Add(expense);

            foreach (
                Item income in
                Items.Where(i => !i.IsExpense))
                Incomes.Add(income);

            Candidates.Clear();

            foreach (
                string tagName in
                Items.SelectMany(i => i.Tags)
                .Select(t => t.Name))
                if (!Candidates.Contains(tagName))
                    Candidates.Add(tagName);

            foreach (
                string name in
                Items.Select(i => i.Name))
                if (!Candidates.Contains(name))
                    Candidates.Add(name);

            Item = null;
        }
        
        public async virtual Task FilterAsync()
        {
            if (string.IsNullOrWhiteSpace(Phrase))
                return;

            string[] splitQuery = Phrase.Split(' ');

            await LoadAsync();

            Item[] filteredItems = Items
                .Where(i => splitQuery.All(query => i.Name.Contains(query) || i.Tags.Any(tag => tag.Name.Contains(query))))
                .ToArray();

            Items.Clear();
            Expenses.Clear();
            Incomes.Clear();

            foreach (Item item in filteredItems)
                Items.Add(item);

            foreach (Item income in filteredItems.Where(i => !i.IsExpense))
                Incomes.Add(income);

            foreach (Item expense in filteredItems.Where(i => i.IsExpense))
                Expenses.Add(expense);
        }

        public virtual void FilterCandidates()
        {
            string[] phrases = Phrase.Split(" ");
            string prefix = string.Join(" ", phrases.SkipLast(1));
            prefix += " ";

            DisplayedCandidates.Clear();

            if (string.IsNullOrWhiteSpace(phrases.Last()))
                foreach (
                    string candidate in
                    Candidates
                    .Order()
                    .AsEnumerable())
                    DisplayedCandidates.Add(prefix + candidate);

            string lastQuery = phrases.Last();

            IEnumerable<string> queries = Candidates.Where(s => s.Contains(lastQuery, StringComparison.CurrentCultureIgnoreCase))
                .Order()
                .OrderByDescending(s => s.StartsWith(lastQuery, StringComparison.CurrentCultureIgnoreCase))
                .AsEnumerable();

            foreach (
                string query in queries)
                DisplayedCandidates.Add(prefix + query);
        }

        [RelayCommand(CanExecute = nameof(CanInvoke))]
        private void Detail()
        {
            if (Item is null)
                return;

            WeakReferenceMessenger.Default.Send(new ItemInvokedMessage(Item));
        }

        private bool CanInvoke()
        {
            return Item is not null;
        }

        [RelayCommand]
        private void AddIncome()
        {
            Item item = new()
            {
                IsExpense = false,
            };

            WeakReferenceMessenger.Default.Send(new ItemInvokedMessage(item));
        }

        [RelayCommand]
        private void AddExpense()
        {
            Item item = new()
            {
                IsExpense = true,
            };

            WeakReferenceMessenger.Default.Send(new ItemInvokedMessage(item));
        }
    }
}
