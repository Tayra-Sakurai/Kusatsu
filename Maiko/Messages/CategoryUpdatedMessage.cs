// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: AGPL-3.0-or-later

using CommunityToolkit.Mvvm.Messaging.Messages;
using Maiko.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maiko.Messages
{
    public class CategoryUpdatedMessage : ValueChangedMessage<Category>
    {
        public CategoryUpdatedMessage(Category value)
            : base(value) { }
    }
}
