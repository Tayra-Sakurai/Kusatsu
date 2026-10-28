// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: AGPL-3.0-or-later

using Microsoft.Windows.ApplicationModel.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CommunityToolkit.Localized.Mvvm.ValidateAttributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class LocalRequiredAttribute : RequiredAttribute
    {
        /// <summary>
        /// Gets the resource name.
        /// </summary>
        public required string ResourceName { get; init; }
        public string? ResourceFile { get; init; }

        public LocalRequiredAttribute()
            : base()
        {
            ResourceLoader resourceLoader = string.IsNullOrWhiteSpace(ResourceFile) ? new() : new(ResourceFile);
            ErrorMessage = resourceLoader.GetString(ResourceName);
        }
    }
}
