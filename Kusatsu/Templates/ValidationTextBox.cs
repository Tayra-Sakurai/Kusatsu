// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: AGPL-3.0-or-later

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
    [TemplatePart(Name = "TextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "Caption", Type = typeof(TextBlock))]
    public sealed partial class ValidationTextBox : Control
    {
        /// <summary>
        /// The text box associated with this control.
        /// </summary>
        /// <remarks>This text box has no customization.</remarks>
        private TextBox? textBox;

        /// <summary>
        /// Caption block to show error message.
        /// </summary>
        /// <remarks>This text block has a colored text of error message.</remarks>
        private TextBlock? caption;

        public ValidationTextBox()
        {
            DefaultStyleKey = typeof(ValidationTextBox);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            textBox = (TextBox)GetTemplateChild("TextBox");
            caption = (TextBlock)GetTemplateChild("Caption");

            textBox.TextChanged += TextBox_TextChanged;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Text = ((TextBox)sender).Text;
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static DependencyProperty TextProperty { get; } = DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(ValidationTextBox),
            new("", OnTextChanged));

        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public static DependencyProperty HeaderProperty { get; } = DependencyProperty.Register(
            nameof(Header),
            typeof(string),
            typeof(ValidationTextBox),
            null);

        public string PlaceholderText
        {
            get => (string)GetValue(PlaceholderTextProperty);
            set => SetValue(PlaceholderTextProperty, value);
        }

        public static DependencyProperty PlaceholderTextProperty { get; } = DependencyProperty.Register(
            nameof(PlaceholderText),
            typeof(string),
            typeof(ValidationTextBox),
            new(""));

        public string Property
        {
            get => (string)GetValue(PropertyProperty);
            set => SetValue(PropertyProperty, value);
        }

        public static DependencyProperty PropertyProperty { get; } = DependencyProperty.Register(
            nameof(Property),
            typeof(string),
            typeof(ValidationTextBox),
            new(null, OnPropertyPropertyChanged));

        public INotifyDataErrorInfo NotifyDataErrorInfo
        {
            get => (INotifyDataErrorInfo)GetValue(NotifyDataErrorInfoProperty);
            set => SetValue(NotifyDataErrorInfoProperty, value);
        }

        public static DependencyProperty NotifyDataErrorInfoProperty { get; } = DependencyProperty.Register(
            nameof(NotifyDataErrorInfo),
            typeof(INotifyDataErrorInfo),
            typeof(ValidationTextBox),
            new(null, OnNotifyDataErrorInfoChanged));

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ValidationTextBox source = (ValidationTextBox)d;
            source.ValidateErrors();
        }

        private static void OnNotifyDataErrorInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ValidationTextBox source = (ValidationTextBox)d;

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
            ValidationTextBox source = (ValidationTextBox)d;
            source.ValidateErrors();
        }

        private void NotifyDataErrorInfo_DataErrorsChanged(object? sender, DataErrorsChangedEventArgs args)
        {
            ValidateErrors();
        }

        private void ValidateErrors()
        {
            if (textBox is null ||
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
