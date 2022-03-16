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
    /// Represents controls used to specify a colour using hue, saturation and brightness (value).
    /// </summary>
    public interface IHSBControls
    {
        /// <summary>
        /// The hue of the colour.
        /// </summary>
        byte H { get; set; }

        /// <summary>
        /// The saturation of the colour.
        /// </summary>
        byte S { get; set; }

        /// <summary>
        /// The brightness (value) of the colour.
        /// </summary>
        byte B { get; set; }

        /// <summary>
        /// Invoked when the selected colour changes.
        /// </summary>
        event EventHandler<HSBChangedEventArgs> ColorChanged;
    }

    /// <summary>
    /// Controls used to specify a colour using hue, saturation and brightness (value).
    /// </summary>
    public partial class HSBControls : UserControl, IHSBControls
    {
        /// <summary>
        /// Defines the <see cref="H"/> property.
        /// </summary>
        public static readonly StyledProperty<byte> HProperty = AvaloniaProperty.Register<HSBControls, byte>(nameof(H), 140);

        /// <summary>
        /// Determines the H component of the colour.
        /// </summary>
        public byte H
        {
            get { return GetValue(HProperty); }
            set
            {
                SetValue(HProperty, value);
                this.FindControl<NumericUpDown>("HBox").Value = value;
            }
        }


        /// <summary>
        /// Defines the <see cref="S"/> property.
        /// </summary>
        public static readonly StyledProperty<byte> SProperty = AvaloniaProperty.Register<HSBControls, byte>(nameof(S), 255);

        /// <summary>
        /// Determines the S component of the colour.
        /// </summary>
        public byte S
        {
            get { return GetValue(SProperty); }
            set
            {
                SetValue(SProperty, value);
                this.FindControl<NumericUpDown>("SBox").Value = value;
            }
        }


        /// <summary>
        /// Defines the <see cref="B"/> property.
        /// </summary>
        public static readonly StyledProperty<byte> BProperty = AvaloniaProperty.Register<HSBControls, byte>(nameof(B), 232);

        /// <summary>
        /// Determines the B/V component of the colour.
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
        public event EventHandler<HSBChangedEventArgs> ColorChanged;

        /// <inheritdoc/>
        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == HProperty || change.Property == SProperty || change.Property == BProperty)
            {
                this.ColorChanged?.Invoke(this, new HSBChangedEventArgs(H, S, B));
            }
        }

        /// <summary>
        /// Create a new <see cref="HSBControls"/>.
        /// </summary>
        public HSBControls()
        {
            InitializeComponent();

            this.FindControl<NumericUpDown>("HBox").ValueChanged += (s, e) =>
            {
                this.H = (byte)e.NewValue;
            };

            this.FindControl<NumericUpDown>("SBox").ValueChanged += (s, e) =>
            {
                this.S = (byte)e.NewValue;
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
    /// <see cref="EventArgs"/> for the <see cref="IHSBControls.ColorChanged"/> event.
    /// </summary>
    public class HSBChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The hue of the new colour.
        /// </summary>
        public byte H { get; }

        /// <summary>
        /// The saturation of the new colour.
        /// </summary>
        public byte S { get; }

        /// <summary>
        /// The brightness (value) of the new colour.
        /// </summary>
        public byte B { get; }

        /// <summary>
        /// Create a new <see cref="HSBChangedEventArgs"/> object.
        /// </summary>
        /// <param name="h">The hue of the new colour.</param>
        /// <param name="s">The saturation of the new colour.</param>
        /// <param name="b">The brightness (value) of the new colour.</param>
        public HSBChangedEventArgs(byte h, byte s, byte b)
        {
            this.H = h;
            this.S = s;
            this.B = b;
        }
    }
}
