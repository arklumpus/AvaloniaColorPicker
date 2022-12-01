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
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using System;

namespace AvaloniaColorPicker
{
    /// <summary>
    /// Represents a control used to select a colour component.
    /// </summary>
    public interface IColorComponentSelector
    {
        /// <summary>
        /// The colour space whose components can be selected.
        /// </summary>
        ColorPicker.ColorSpaces ColorSpace { get; set; }

        /// <summary>
        /// The selected colour component.
        /// </summary>
        ColorComponents ColorComponent { get; set; }

        /// <summary>
        /// Invoked when the selected colour component is changed.
        /// </summary>
        event EventHandler<ColorComponentChangedEventArgs> ColorComponentChanged;
    }

    /// <summary>
    /// The colour components.
    /// </summary>
    public enum ColorComponents
    {
        /// <summary>
        /// The first component (R, H, or L*).
        /// </summary>
        Component1,

        /// <summary>
        /// The second component (G, S, or a*).
        /// </summary>
        Component2,

        /// <summary>
        /// The third component (B, V or b*).
        /// </summary>
        Component3
    }

    /// <summary>
    /// A control to select a colour component.
    /// </summary>
    public partial class ColorComponentSelector : UserControl, IColorComponentSelector
    {
        /// <summary>
        /// Defines the <see cref="ColorSpace"/> property.
        /// </summary>
        public static readonly StyledProperty<ColorPicker.ColorSpaces> ColorSpaceProperty = AvaloniaProperty.Register<ColorComponentSelector, ColorPicker.ColorSpaces>(nameof(ColorSpace), ColorPicker.ColorSpaces.HSB);

        /// <summary>
        /// The currently selected colour space.
        /// </summary>
        public ColorPicker.ColorSpaces ColorSpace
        {
            get { return GetValue(ColorSpaceProperty); }
            set { SetValue(ColorSpaceProperty, value); }
        }

        /// <summary>
        /// Defines the <see cref="ColorComponent"/> property.
        /// </summary>
        public static readonly StyledProperty<ColorComponents> ColorComponentProperty = AvaloniaProperty.Register<ColorComponentSelector, ColorComponents>(nameof(ColorComponent), ColorComponents.Component1);

        /// <summary>
        /// The currently selected colour component.
        /// </summary>
        public ColorComponents ColorComponent
        {
            get { return GetValue(ColorComponentProperty); }
            set
            {
                if (!programmaticChange)
                {
                    programmaticChange = true;
                    switch (value)
                    {
                        case ColorComponents.Component1:
                            this.FindControl<ToggleButton>("Dim1Toggle").IsChecked = true;
                            this.FindControl<ToggleButton>("Dim1Toggle").IsEnabled = false;
                            this.FindControl<ToggleButton>("Dim2Toggle").IsChecked = false;
                            this.FindControl<ToggleButton>("Dim2Toggle").IsEnabled = true;
                            this.FindControl<ToggleButton>("Dim3Toggle").IsChecked = false;
                            this.FindControl<ToggleButton>("Dim3Toggle").IsEnabled = true;
                            break;
                        case ColorComponents.Component2:
                            this.FindControl<ToggleButton>("Dim1Toggle").IsChecked = false;
                            this.FindControl<ToggleButton>("Dim1Toggle").IsEnabled = true;
                            this.FindControl<ToggleButton>("Dim2Toggle").IsChecked = true;
                            this.FindControl<ToggleButton>("Dim2Toggle").IsEnabled = false;
                            this.FindControl<ToggleButton>("Dim3Toggle").IsChecked = false;
                            this.FindControl<ToggleButton>("Dim3Toggle").IsEnabled = true;
                            break;
                        case ColorComponents.Component3:
                            this.FindControl<ToggleButton>("Dim1Toggle").IsChecked = false;
                            this.FindControl<ToggleButton>("Dim1Toggle").IsEnabled = true;
                            this.FindControl<ToggleButton>("Dim2Toggle").IsChecked = false;
                            this.FindControl<ToggleButton>("Dim2Toggle").IsEnabled = true;
                            this.FindControl<ToggleButton>("Dim3Toggle").IsChecked = true;
                            this.FindControl<ToggleButton>("Dim3Toggle").IsEnabled = false;
                            break;
                    }
                    programmaticChange = false;
                }

                SetValue(ColorComponentProperty, value);
            }
        }

        private bool programmaticChange = false;

        /// <inheritdoc/>
        public event EventHandler<ColorComponentChangedEventArgs> ColorComponentChanged;

        /// <inheritdoc/>
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == ColorComponentProperty)
            {
                this.ColorComponentChanged?.Invoke(this, new ColorComponentChangedEventArgs(ColorComponent));
            }
            else if (change.Property == ColorSpaceProperty)
            {
                switch (ColorSpace)
                {
                    case ColorPicker.ColorSpaces.RGB:
                        this.FindControl<ToggleButton>("Dim1Toggle").Content = "R";
                        this.FindControl<ToggleButton>("Dim2Toggle").Content = "G";
                        this.FindControl<ToggleButton>("Dim3Toggle").Content = "B";
                        break;
                    case ColorPicker.ColorSpaces.HSB:
                        this.FindControl<ToggleButton>("Dim1Toggle").Content = "H";
                        this.FindControl<ToggleButton>("Dim2Toggle").Content = "S";
                        this.FindControl<ToggleButton>("Dim3Toggle").Content = "B";
                        break;
                    case ColorPicker.ColorSpaces.LAB:
                        this.FindControl<ToggleButton>("Dim1Toggle").Content = "L*";
                        this.FindControl<ToggleButton>("Dim2Toggle").Content = "a*";
                        this.FindControl<ToggleButton>("Dim3Toggle").Content = "b*";
                        break;
                }

                ColorComponent = ColorComponents.Component1;
            }
        }
        
        /// <summary>
        /// Create a new <see cref="ColorComponentSelector"/>.
        /// </summary>
        public ColorComponentSelector()
        {
            InitializeComponent();

            this.FindControl<ToggleButton>("Dim1Toggle").Checked += (s, e) => { this.ColorComponent = ColorComponents.Component1; };
            this.FindControl<ToggleButton>("Dim2Toggle").Checked += (s, e) => { this.ColorComponent = ColorComponents.Component2; };
            this.FindControl<ToggleButton>("Dim3Toggle").Checked += (s, e) => { this.ColorComponent = ColorComponents.Component3; };
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }

    /// <summary>
    /// <see cref="EventArgs"/> for the <see cref="IColorComponentSelector.ColorComponentChanged"/> event.
    /// </summary>
    public class ColorComponentChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The new colour component.
        /// </summary>
        public ColorComponents ColorComponent { get; }

        /// <summary>
        /// Create a new <see cref="ColorComponentChangedEventArgs"/> object.
        /// </summary>
        /// <param name="colorComponent">The new colour component.</param>
        public ColorComponentChangedEventArgs(ColorComponents colorComponent)
        {
            this.ColorComponent = colorComponent;
        }
    }
}
