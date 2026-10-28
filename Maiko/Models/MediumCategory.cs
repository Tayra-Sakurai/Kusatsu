// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Maiko.Models
{
    public class MediumCategory : Category
    {
        public int LargeCategoryId { get; set; }
        public LargeCategory? LargeCategory { get; set; }
        public ObservableCollection<SmallCategory> SmallCategories { get; } = [];
    }
}
