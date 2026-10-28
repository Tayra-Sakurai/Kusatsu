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
    }
}
