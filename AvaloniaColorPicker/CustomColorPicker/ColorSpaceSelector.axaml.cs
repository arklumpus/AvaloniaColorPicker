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
    /// Represents a control used to select the current colour space.
    /// </summary>
    public interface IColorSpaceSelector
    {
        /// <summary>
        /// The current colour space.
        /// </summary>
        ColorPicker.ColorSpaces ColorSpace { get; set; }

        /// <summary>
        /// Invoked when the selected colour space changes.
        /// </summary>
        event EventHandler<ColorSpaceChangedEventArgs> ColorSpaceChanged;
    }

    /// <summary>
    /// A control used to select the current colour space.
    /// </summary>
    public partial class ColorSpaceSelector : UserControl, IColorSpaceSelector
    {
        /// <summary>
        /// Defines the <see cref="ColorSpace"/> property.
        /// </summary>
        public static readonly StyledProperty<ColorPicker.ColorSpaces> ColorSpaceProperty = AvaloniaProperty.Register<ColorSpaceSelector, ColorPicker.ColorSpaces>(nameof(ColorSpace), ColorPicker.ColorSpaces.HSB);

        /// <summary>
        /// The currently selected colour space.
        /// </summary>
        public ColorPicker.ColorSpaces ColorSpace
        {
            get { return GetValue(ColorSpaceProperty); }
            set { SetValue(ColorSpaceProperty, value); }
        }

        /// <inheritdoc/>
        public event EventHandler<ColorSpaceChangedEventArgs> ColorSpaceChanged;

        /// <inheritdoc/>
        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == ColorSpaceProperty)
            {
                this.ColorSpaceChanged?.Invoke(this, new ColorSpaceChangedEventArgs(ColorSpace));
            }
        }

        /// <summary>
        /// Create a new <see cref="ColorSpaceSelector"/>.
        /// </summary>
        public ColorSpaceSelector()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }

    /// <summary>
    /// <see cref="EventArgs"/> for the <see cref="IColorSpaceSelector.ColorSpaceChanged"/> event.
    /// </summary>
    public class ColorSpaceChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The new colour space.
        /// </summary>
        public ColorPicker.ColorSpaces ColorSpace { get; }

        /// <summary>
        /// Create a new <see cref="ColorSpaceChangedEventArgs"/> object.
        /// </summary>
        /// <param name="colorSpace">The new colour space.</param>
        public ColorSpaceChangedEventArgs(ColorPicker.ColorSpaces colorSpace)
        {
            this.ColorSpace = colorSpace;
        }
    }
}
