// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: AGPL-3.0-or-later

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Windows.Storage;
using OllamaSharp;
using OllamaSharp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Windows.Foundation.Collections;

namespace Kusatsu.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly IPropertySet settingValues;
        private readonly OllamaApiClient ollama;

        public SettingsViewModel()
        {
            settingValues = ApplicationData.GetDefault().LocalSettings.Values;
            ollama = new(new Uri("http://localhost:11434/api"));
            Models = [];
        }

        public async Task LoadModelsAsync()
        {
            IEnumerable<Model> models = await ollama.ListLocalModelsAsync();
            Models.Clear();

            foreach (Model model in models.OrderBy(x => x.Name))
                Models.Add(model.Name);

            OnPropertyChanged(nameof(OllamaModelIndex));
            OnPropertyChanged(nameof(EmbeddingModelIndex));
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

        public ObservableCollection<string> Models { get; }

        public int OllamaModelIndex
        {
            get => Models.IndexOf(settingValues["OllamaModel"] as string ?? string.Empty);
            set
            {
                if (value >= 0)
                {
                    if (settingValues["OllamaModel"] is not string ollamaModel ||
                        ollamaModel != Models[value])
                    {
                        settingValues["OllamaModel"] = Models[value];
                        OnPropertyChanged();
                    }
                }
            }
        }

        public int EmbeddingModelIndex
        {
            get => Models.IndexOf((settingValues["EmbeddingModel"] as string) ?? string.Empty);
            set
            {
                if (value == -1)
                    return;

                if (settingValues["EmbeddingModel"] is not string embeddingModel ||
                    embeddingModel != Models[value])
                {
                    settingValues["EmbeddingModel"] = Models[value];
                    OnPropertyChanged();
                }
            }
        }
    }
}
