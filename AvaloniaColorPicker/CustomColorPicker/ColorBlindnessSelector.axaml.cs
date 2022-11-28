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
    /// Colour blindness simulation modes.
    /// </summary>
    public enum ColorBlindnessModes
    {
        /// <summary>
        /// Normal vision.
        /// </summary>
        Normal,

        /// <summary>
        /// Protanopia (red-green colour blindness).
        /// </summary>
        Protanopia,

        /// <summary>
        /// Deuteranopia (red-green colour blindness).
        /// </summary>
        Deuteranopia,

        /// <summary>
        /// Tritanopia (blue-yellow colour blindness).
        /// </summary>
        Tritanopia,

        /// <summary>
        /// Cone achromatopsia (only one kind of cone cells).
        /// </summary>
        ConeAchromatopsia,

        /// <summary>
        /// Rod achromatopsia (lack of all cone cells).
        /// </summary>
        RodAchromatopsia
    }

    /// <summary>
    /// Represents a control used to select the colour blindness simulation mode.
    /// </summary>
    public interface IColorBlindnessSelector
    {
        /// <summary>
        /// The colour blindness simulation mode.
        /// </summary>
        ColorBlindnessModes ColorBlindnessMode { get; set; }

        /// <summary>
        /// Invoked when the selected colour blindness simulation mode changes.
        /// </summary>
        event EventHandler<ColorBlindnessModeChangedEventArgs> ColorBlindnessModeChanged;
    }

    /// <summary>
    /// A control used to select the colour blindness simulation mode.
    /// </summary>
    public partial class ColorBlindnessSelector : UserControl, IColorBlindnessSelector
    {
        /// <summary>
        /// Defines the <see cref="ColorBlindnessMode"/> property.
        /// </summary>
        public static readonly StyledProperty<ColorBlindnessModes> ColorBlindnessModeProperty = AvaloniaProperty.Register<ColorBlindnessSelector, ColorBlindnessModes>(nameof(ColorBlindnessMode), ColorBlindnessModes.Normal);

        /// <summary>
        /// Determines the colour-blindness preview mode.
        /// </summary>
        public ColorBlindnessModes ColorBlindnessMode
        {
            get { return GetValue(ColorBlindnessModeProperty); }
            set { SetValue(ColorBlindnessModeProperty, value); }
        }

        /// <inheritdoc/>
        public event EventHandler<ColorBlindnessModeChangedEventArgs> ColorBlindnessModeChanged;

        /// <inheritdoc/>
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == ColorBlindnessModeProperty)
            {
                this.ColorBlindnessModeChanged?.Invoke(this, new ColorBlindnessModeChangedEventArgs(ColorBlindnessMode));
            }
        }

        /// <summary>
        /// Create a new <see cref="ColorBlindnessSelector"/>.
        /// </summary>
        public ColorBlindnessSelector()
        {
            InitializeComponent();

            this.FindControl<ComboBox>("ColorBlindnessComboBox").SelectionChanged += (s, e) =>
            {
                this.ColorBlindnessMode = (ColorBlindnessModes)this.FindControl<ComboBox>("ColorBlindnessComboBox").SelectedIndex;
            };
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
    
    /// <summary>
    /// <see cref="EventArgs"/> for the <see cref="IColorBlindnessSelector.ColorBlindnessModeChanged"/> event.
    /// </summary>
    public class ColorBlindnessModeChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The new colour blindness simulation mode.
        /// </summary>
        public ColorBlindnessModes ColorBlindnessMode { get; }

        /// <summary>
        /// Create a new <see cref="ColorBlindnessModeChangedEventArgs"/> object.
        /// </summary>
        /// <param name="colorBlindnessMode">The new colour blindness simulation mode.</param>
        public ColorBlindnessModeChangedEventArgs(ColorBlindnessModes colorBlindnessMode)
        {
            this.ColorBlindnessMode = colorBlindnessMode;
        }
    }
}
