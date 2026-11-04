// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: AGPL-3.0-or-later

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Windows.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Foundation.Collections;

namespace Kusatsu.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly IPropertySet settingValues;

        public SettingsViewModel()
        {
            settingValues = ApplicationData.GetDefault().LocalSettings.Values;
        }

        public bool IsAIEnabled
        {
            get => settingValues["IsAIEnabled"] is true;
            set
            {
                if (settingValues["IsAIEnabled"] is bool isAIEnabled)
                {
                    if (value != isAIEnabled)
                    {
                        settingValues["IsAIEnabled"] = value;
                        OnPropertyChanged();
                    }
                }
            }
        }
    }
}
