// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Maiko.Models
{
    public class Method
    {
        public string Name { get; set; } = string.Empty;
        public int Id { get; set; }
        public ObservableCollection<Item> Items { get; } = [];
        public double Remainder => Items.Sum(item => item.IsExpense ? -item.Amount : item.Amount);
    }
}
