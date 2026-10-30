// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: AGPL-3.0-or-later

using Microsoft.Windows.ApplicationModel.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CommunityToolkit.Localized.Mvvm.ValidateAttributes
{
    public class LocalMaxLengthAttribute : MaxLengthAttribute
    {
        /// <summary>
        /// Gets or sets the file name of the resource file used to resolve the error message.
        /// </summary>
        /// <remarks>Setting this property updates the error message. A null, empty, or whitespace value
        /// uses the default resource loader.</remarks>
        public string? ErrorMessageFileName
        {
            get;

            set
            {
                field = value;
                ResourceLoader resourceLoader = string.IsNullOrWhiteSpace(value) ? new() : new(ResourceLoader.GetDefaultResourceFilePath(), value);
                ErrorMessage = resourceLoader.GetString(ErrorMessageResourceName);
            }
        }

        /// <summary>
        /// Gets or sets the name of the resource used to retrieve the localized error message.
        /// </summary>
        /// <remarks>Setting this property also updates the error message using the specified resource
        /// name.</remarks>
        public new required string ErrorMessageResourceName
        {
            get;

            set
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(value);
                field = value;

                ResourceLoader resourceLoader = string.IsNullOrWhiteSpace(ErrorMessageFileName) ? new() : new(ResourceLoader.GetDefaultResourceFilePath(), ErrorMessageFileName);
                ErrorMessage = resourceLoader.GetString(value);
            }
        }

        public LocalMaxLengthAttribute(int length)
            : base(length)
        { }
    }
}
