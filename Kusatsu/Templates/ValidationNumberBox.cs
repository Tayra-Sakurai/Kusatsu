// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: AGPL-3.0-or-later

using Google.Protobuf.WellKnownTypes;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Input.Preview.Text;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

// To add another templated control to this project (which already has a
// Themes\Generic.xaml), run with `--IncludeDefaultStyle false` or `-I false`:
//
//     dotnet new winui-templatedcontrol -n NewName --IncludeDefaultStyle false
//
// Then open Themes\Generic.xaml and add a <Style TargetType="local:NewName">
// block. Without it, the new control will render blank at runtime.

namespace Kusatsu.Templates
{
    [TemplatePart(Name = "TextBox", Type = typeof(NumberBox))]
    [TemplatePart(Name = "Caption", Type = typeof(TextBlock))]
    public sealed partial class ValidationNumberBox : Control
    {
        private NumberBox? numberBox;

        private TextBlock? caption;

        public ValidationNumberBox()
        {
            DefaultStyleKey = typeof(ValidationNumberBox);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            numberBox = (NumberBox)GetTemplateChild("TextBox");
            caption = (TextBlock)GetTemplateChild("Caption");

            numberBox.ValueChanged += TextBox_TextChanged;
        }

        private void TextBox_TextChanged(NumberBox sender, NumberBoxValueChangedEventArgs e)
        {
            Value = sender.Value;
        }

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static DependencyProperty ValueProperty { get; } = DependencyProperty.Register(
            nameof(Value),
            typeof(double),
            typeof(ValidationNumberBox),
            new(0d, OnTextChanged));

        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public static DependencyProperty HeaderProperty { get; } = DependencyProperty.Register(
            nameof(Header),
            typeof(string),
            typeof(ValidationNumberBox),
            null);

        public string PlaceholderText
        {
            get => (string)GetValue(PlaceholderTextProperty);
            set => SetValue(PlaceholderTextProperty, value);
        }

        public static DependencyProperty PlaceholderTextProperty { get; } = DependencyProperty.Register(
            nameof(PlaceholderText),
            typeof(string),
            typeof(ValidationNumberBox),
            new(""));

        public string Property
        {
            get => (string)GetValue(PropertyProperty);
            set => SetValue(PropertyProperty, value);
        }

        public static DependencyProperty PropertyProperty { get; } = DependencyProperty.Register(
            nameof(Property),
            typeof(string),
            typeof(ValidationNumberBox),
            new(null, OnPropertyPropertyChanged));

        public INotifyDataErrorInfo NotifyDataErrorInfo
        {
            get => (INotifyDataErrorInfo)GetValue(NotifyDataErrorInfoProperty);
            set => SetValue(NotifyDataErrorInfoProperty, value);
        }

        public static DependencyProperty NotifyDataErrorInfoProperty { get; } = DependencyProperty.Register(
            nameof(NotifyDataErrorInfo),
            typeof(INotifyDataErrorInfo),
            typeof(ValidationNumberBox),
            new(null, OnNotifyDataErrorInfoChanged));

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ValidationNumberBox source = (ValidationNumberBox)d;
            source.ValidateErrors();
        }

        private static void OnNotifyDataErrorInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ValidationNumberBox source = (ValidationNumberBox)d;

            if (e.OldValue is INotifyDataErrorInfo notifyDataErrorInfo)
            {
                notifyDataErrorInfo.ErrorsChanged -= source.NotifyDataErrorInfo_DataErrorsChanged;
            }

            if (e.NewValue is INotifyDataErrorInfo notifyDataErrorInfo1)
                notifyDataErrorInfo1.ErrorsChanged += source.NotifyDataErrorInfo_DataErrorsChanged;

            source.ValidateErrors();
        }

        private static void OnPropertyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ValidationNumberBox source = (ValidationNumberBox)d;
            source.ValidateErrors();
        }

        private void NotifyDataErrorInfo_DataErrorsChanged(object? sender, DataErrorsChangedEventArgs args)
        {
            ValidateErrors();
        }

        private void ValidateErrors()
        {
            if (numberBox is null ||
                caption is null ||
                NotifyDataErrorInfo is null ||
                string.IsNullOrEmpty(Property))
                return;

            ValidationResult? result = NotifyDataErrorInfo.GetErrors(Property).OfType<ValidationResult>().FirstOrDefault();

            if (result != null)
                caption.Text = result.ErrorMessage;
            else
                caption.Text = string.Empty;
        }
    }
}
