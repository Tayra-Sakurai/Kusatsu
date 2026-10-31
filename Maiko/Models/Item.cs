// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Maiko.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsExpense { get; set; } = false;
        public int MethodId { get; set; }
        public Method? Method { get; set; }
        public double Amount { get; set; } = 0d;
        public int CategoryId { get; set; }
        public SmallCategory? Category { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now;
        public ObservableCollection<Tag> Tags { get; } = [];
    }
}
