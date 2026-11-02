// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: AGPL-3.0-or-later

using CommunityToolkit.Localized.Mvvm.ValidateAttributes;
using CommunityToolkit.Mvvm.ComponentModel;
using Maiko.Contexts;
using Maiko.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Maiko.ViewModels
{
    public partial class ItemViewModel : ObservableValidator
    {
        private readonly IDbContextFactory<MaikoContext> factory;
        private Item item;

        public ItemViewModel(IDbContextFactory<MaikoContext> factory)
        {
            this.factory = factory;
            item = new();
        }

        [LocalRequired(ErrorMessageFileName = "Maiko/Resources", ErrorMessageResourceName = "RequiredMessage")]
        public string Name
        {
            get => item.Name;
            set => SetProperty(item.Name, value, item, (m, v) => m.Name = v, true);
        }

        public string Description
        {
            get => item.Description;
            set => SetProperty(item.Description, value, item, (m, v) => m.Description = v, false);
        }

        [Required]
        public DateTimeOffset? Date
        {
            get => item.DateTime.Date;
            set
            {
                ValidateProperty(value);

                if (value is not DateTimeOffset dateTimeOffset)
                {
                    OnPropertyChanged();
                    return;
                }

                DateTime localValue = dateTimeOffset.LocalDateTime.Date;
                if (localValue != item.DateTime.Date)
                {
                    TimeSpan time = item.DateTime.ToLocalTime().TimeOfDay;
                    item.DateTime = localValue;
                    item.DateTime.Add(time);
                    OnPropertyChanged();
                }
            }
        }

        [Required]
        public TimeSpan? Time
        {
            get => item.DateTime.TimeOfDay;
            set
            {
                ValidateProperty(value);

                if (value is not TimeSpan time)
                {
                    OnPropertyChanged();
                    return;
                }

                if (time != item.DateTime.TimeOfDay)
                {
                    item.DateTime = item.DateTime.Date;
                    item.DateTime.Add(time);
                    OnPropertyChanged();
                }
            }
        }

        [LocalRange(0d, double.MaxValue, ErrorMessageFileName = "Maiko/Resources", ErrorMessageResourceName = "RangeErrorMessage")]
        public double Amount
        {
            get => item.Amount;
            set => SetProperty(item.Amount, value, item, (m, v) => m.Amount = v, true);
        }
    }
}
