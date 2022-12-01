/*
    AvaloniaColorPicker - A color picker for Avalonia.
    Copyright (C) 2022  Giorgio Bianchini
 
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, version 3.
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.
    You should have received a copy of the GNU Lesser General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;

namespace AvaloniaColorPicker
{
    /// <summary>
    /// Represents a control used to display the hex representation of a colour.
    /// </summary>
    public interface IHexControl
    {
        /// <summary>
        /// The hex representation of the colour.
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// Invoked when the user changes the hex representation of the colour.
        /// </summary>
        event EventHandler<HexTextChangedEventArgs> TextChanged;
    }

    /// <summary>
    /// A control used to display the hex representation of a colour.
    /// </summary>
    public partial class HexControl : UserControl, IHexControl
    {
        /// <summary>
        /// Defines the <see cref="Text"/> property.
        /// </summary>
        public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<HexControl, string>(nameof(Text), "00A2E8");

        /// <summary>
        /// Determines the text of the Hex control.
        /// </summary>
        public string Text
        {
            get { return GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <inheritdoc/>
        public event EventHandler<HexTextChangedEventArgs> TextChanged;

        /// <inheritdoc/>
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == TextProperty)
            {
                this.TextChanged?.Invoke(this, new HexTextChangedEventArgs(Text));
            }
        }

        /// <summary>
        /// Create a new <see cref="HexControl"/>.
        /// </summary>
        public HexControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }

    /// <summary>
    /// <see cref="EventArgs"/> for the <see cref="IHexControl.TextChanged"/> event.
    /// </summary>
    public class HexTextChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The hex representation of the colour.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Create a new <see cref="HexTextChangedEventArgs"/> object.
        /// </summary>
        /// <param name="text">The hex representation of the colour.</param>
        public HexTextChangedEventArgs(string text)
        {
            this.Text = text;
        }
    }
}
