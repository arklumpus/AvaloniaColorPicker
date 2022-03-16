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
    /// Represents controls used to specify a colour based on its RGB components.
    /// </summary>
    public interface IRGBControls
    {
        /// <summary>
        /// The red component of the colour.
        /// </summary>
        byte R { get; set; }

        /// <summary>
        /// The green component of the colour.
        /// </summary>
        byte G { get; set; }

        /// <summary>
        /// The blue component of the colour.
        /// </summary>
        byte B { get; set; }

        /// <summary>
        /// Invoked when the selected colour changes.
        /// </summary>
        event EventHandler<RGBChangedEventArgs> ColorChanged;
    }

    /// <summary>
    /// Controls used to specify a colour based on its RGB components.
    /// </summary>
    public partial class RGBControls : UserControl, IRGBControls
    {
        /// <summary>
        /// Defines the <see cref="R"/> property.
        /// </summary>
        public static readonly StyledProperty<byte> RProperty = AvaloniaProperty.Register<RGBControls, byte>(nameof(R), 0);

        /// <summary>
        /// Determines the R component of the colour.
        /// </summary>
        public byte R
        {
            get { return GetValue(RProperty); }
            set
            {
                SetValue(RProperty, value);
                this.FindControl<NumericUpDown>("RBox").Value = value;
            }
        }


        /// <summary>
        /// Defines the <see cref="G"/> property.
        /// </summary>
        public static readonly StyledProperty<byte> GProperty = AvaloniaProperty.Register<RGBControls, byte>(nameof(G), 162);

        /// <summary>
        /// Determines the G component of the colour.
        /// </summary>
        public byte G
        {
            get { return GetValue(GProperty); }
            set
            {
                SetValue(GProperty, value);
                this.FindControl<NumericUpDown>("GBox").Value = value;
            }
        }


        /// <summary>
        /// Defines the <see cref="B"/> property.
        /// </summary>
        public static readonly StyledProperty<byte> BProperty = AvaloniaProperty.Register<RGBControls, byte>(nameof(B), 232);

        /// <summary>
        /// Determines the B component of the colour.
        /// </summary>
        public byte B
        {
            get { return GetValue(BProperty); }
            set
            {
                SetValue(BProperty, value);
                this.FindControl<NumericUpDown>("BBox").Value = value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<RGBChangedEventArgs> ColorChanged;

        /// <inheritdoc/>
        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == RProperty || change.Property == GProperty || change.Property == BProperty)
            {
                this.ColorChanged?.Invoke(this, new RGBChangedEventArgs(R, G, B));
            }
        }

        /// <summary>
        /// Create a new <see cref="RGBControls"/>.
        /// </summary>
        public RGBControls()
        {
            InitializeComponent();

            this.FindControl<NumericUpDown>("RBox").ValueChanged += (s, e) =>
            {
                this.R = (byte)e.NewValue;
            };

            this.FindControl<NumericUpDown>("GBox").ValueChanged += (s, e) =>
            {
                this.G = (byte)e.NewValue;
            };

            this.FindControl<NumericUpDown>("BBox").ValueChanged += (s, e) =>
            {
                this.B = (byte)e.NewValue;
            };
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }

    /// <summary>
    /// <see cref="EventArgs"/> for the <see cref="IRGBControls.ColorChanged"/> event.
    /// </summary>
    public class RGBChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The red component of the new colour.
        /// </summary>
        public byte R { get; }

        /// <summary>
        /// The green component of the new colour.
        /// </summary>
        public byte G { get; }

        /// <summary>
        /// The blue component of the new colour.
        /// </summary>
        public byte B { get; }

        /// <summary>
        /// Create a new <see cref="RGBChangedEventArgs"/> object.
        /// </summary>
        /// <param name="r">The red component of the new colour.</param>
        /// <param name="g">The green component of the new colour.</param>
        /// <param name="b">The blue component of the new colour.</param>
        public RGBChangedEventArgs(byte r, byte g, byte b)
        {
            this.R = r;
            this.G = g;
            this.B = b;
        }
    }
}
