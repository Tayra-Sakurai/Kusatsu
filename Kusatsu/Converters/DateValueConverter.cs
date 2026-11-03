// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: AGPL-3.0-or-later

using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Kusatsu.Converters
{
    public partial class DateValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            CultureInfo culture = CultureInfo.CurrentCulture;

            DateTime dateTime = (DateTime)value;
            return dateTime.ToString("g", culture.DateTimeFormat);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
