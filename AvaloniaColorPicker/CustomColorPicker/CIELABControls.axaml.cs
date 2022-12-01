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
    /// Represents controls used to set the colour based on CIELAB coordinates.
    /// </summary>
    public interface ICIELABControls
    {
        /// <summary>
        /// The L* component of the colour.
        /// </summary>
        byte L { get; set; }

        /// <summary>
        /// The a* component of the colour.
        /// </summary>
        int a { get; set; }
        
        /// <summary>
        /// The b* component of the colour.
        /// </summary>
        int b { get; set; }

        /// <summary>
        /// Invoked when the selected colour changes.
        /// </summary>
        event EventHandler<CIELABChangedEventArgs> ColorChanged;
    }

    /// <summary>
    /// Controls used to set the colour based on CIELAB coordinates.
    /// </summary>
    public partial class CIELABControls : UserControl, ICIELABControls
    {
        /// <summary>
        /// Defines the <see cref="L"/> property.
        /// </summary>
        public static readonly StyledProperty<byte> LProperty = AvaloniaProperty.Register<CIELABControls, byte>(nameof(L), 63);

        /// <summary>
        /// Determines the L* component of the colour.
        /// </summary>
        public byte L
        {
            get { return GetValue(LProperty); }
            set { SetValue(LProperty, Math.Min(value, (byte)100));
                this.FindControl<NumericUpDown>("LBox").Value = L;
            }
        }


        /// <summary>
        /// Defines the <see cref="a"/> property.
        /// </summary>
        public static readonly StyledProperty<int> aProperty = AvaloniaProperty.Register<CIELABControls, int>(nameof(a), -10);

        /// <summary>
        /// Determines the a* component of the colour.
        /// </summary>
        public int a
        {
            get { return GetValue(aProperty); }
            set { SetValue(aProperty, Math.Max(-100, Math.Min(100, value)));
                this.FindControl<NumericUpDown>("aBox").Value = a;
            }
        }


        /// <summary>
        /// Defines the <see cref="b"/> property.
        /// </summary>
        public static readonly StyledProperty<int> bProperty = AvaloniaProperty.Register<CIELABControls, int>(nameof(b), -44);

        /// <summary>
        /// Determines the b* component of the colour.
        /// </summary>
        public int b
        {
            get { return GetValue(bProperty); }
            set { SetValue(bProperty, Math.Max(-100, Math.Min(100, value)));
                this.FindControl<NumericUpDown>("bBox").Value = b;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<CIELABChangedEventArgs> ColorChanged;

        /// <inheritdoc/>
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == LProperty || change.Property == aProperty || change.Property == bProperty)
            {
                this.ColorChanged?.Invoke(this, new CIELABChangedEventArgs(L, a, b));
            }
        }

        /// <summary>
        /// Create a new <see cref="CIELABControls"/> object.
        /// </summary>
        public CIELABControls()
        {
            InitializeComponent();

            this.FindControl<NumericUpDown>("LBox").ValueChanged += (s, e) =>
            {
                this.L = (byte)e.NewValue;
            };

            this.FindControl<NumericUpDown>("aBox").ValueChanged += (s, e) =>
            {
                this.a = (int)e.NewValue;
            };

            this.FindControl<NumericUpDown>("bBox").ValueChanged += (s, e) =>
            {
                this.b = (int)e.NewValue;
            };
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }

    /// <summary>
    /// <see cref="EventArgs"/> for the <see cref="ICIELABControls.ColorChanged"/> event.
    /// </summary>
    public class CIELABChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The L* component of the new colour.
        /// </summary>
        public byte L { get; }
        
        /// <summary>
        /// The a* component of the new colour.
        /// </summary>
        public int a { get; }

        /// <summary>
        /// The b* component of the new colour.
        /// </summary>
        public int b { get; }

        /// <summary>
        /// Create a new <see cref="CIELABChangedEventArgs"/> object.
        /// </summary>
        /// <param name="l">The L* component of the new colour.</param>
        /// <param name="a">The a* component of the new colour.</param>
        /// <param name="b">The b* component of the new colour.</param>
        public CIELABChangedEventArgs(byte l, int a, int b)
        {
            this.L = l;
            this.a = a;
            this.b = b;
        }
    }
}
