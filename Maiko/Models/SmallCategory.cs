// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Text;

namespace Maiko.Models
{
    public class SmallCategory : Category
    {
        public int MediumCategoryId { get; set; }
        public MediumCategory? MediumCategory { get; set; }
        public ICollection<Item> Items { get; } = new HashSet<Item>();
        public double TotalIncome => Items.Sum(i => i.IsExpense ? 0d : i.Amount);
        public double TotalExpense => Items.Sum(i => i.IsExpense ? i.Amount : 0d);
        public double MonthlyTotal => 
            Items
            .Where(item => item.DateTime.Month == DateTime.Now.Month)
            .Sum(item => item.IsExpense ? item.Amount : 0d);
    }
}
