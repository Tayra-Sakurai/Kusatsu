// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Maiko.Models
{
    public class LargeCategory : Category
    {
        public ObservableCollection<MediumCategory> MediumCategories { get; } = [];
        public double TotalExpense => MediumCategories.Sum(mc => mc.TotalExpense);
        public double TotalIncome => MediumCategories.Sum(mc => mc.TotalIncome);
        public double MonthlyTotal => MediumCategories.Sum(mc => mc.MonthlyTotal);
    }
}
