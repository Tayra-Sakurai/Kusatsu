// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: AGPL-3.0-or-later

using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Imazu.Models
{
    public class ChatTextData
    {
        public ChatRole ChatRole { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
