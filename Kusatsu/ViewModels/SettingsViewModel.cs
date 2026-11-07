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
            Models = [EmbeddingModel, OllamaModel];
        }

        public async Task LoadModelsAsync()
        {
            IEnumerable<Model> models = await ollama.ListLocalModelsAsync();
            string embedding = EmbeddingModel;
            string chatModel = OllamaModel;
            Models.Clear();

            foreach (Model model in models.OrderBy(x => x.Name))
                Models.Add(model.Name);

            EmbeddingModel = embedding;
            OllamaModel = chatModel;
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

        public string OllamaModel
        {
            get => (settingValues["OllamaModel"] as string) ?? string.Empty;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (settingValues["OllamaModel"] is not string ollamaModel ||
                        ollamaModel != value)
                    {
                        settingValues["OllamaModel"] = value;
                        OnPropertyChanged();
                    }
                }
            }
        }

        public string EmbeddingModel
        {
            get => (settingValues["EmbeddingModel"] as string) ?? string.Empty;
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                if (settingValues["EmbeddingModel"] is not string embeddingModel ||
                    embeddingModel != value)
                {
                    settingValues["EmbeddingModel"] = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
