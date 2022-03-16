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
    /// Represents a control used to set the colour's alpha value.
    /// </summary>
    public interface IAControl
    {
        /// <summary>
        /// The colour's alpha value.
        /// </summary>
        byte A { get; set; }

        /// <summary>
        /// Invoked when the colour's alpha value changes.
        /// </summary>
        event EventHandler<AChangedEventArgs> AChanged;
    }

    /// <summary>
    /// A control used to set the colour's alpha value.
    /// </summary>
    public partial class AControl : UserControl, IAControl
    {
        /// <summary>
        /// Defines the <see cref="A"/> property.
        /// </summary>
        public static readonly StyledProperty<byte> AProperty = AvaloniaProperty.Register<AControl, byte>(nameof(A), 255);

        /// <summary>
        /// Determines the alpha component of the colour.
        /// </summary>
        public byte A
        {
            get { return GetValue(AProperty); }
            set
            {
                SetValue(AProperty, value);
                this.FindControl<NumericUpDown>("ABox").Value = value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<AChangedEventArgs> AChanged;

        /// <inheritdoc/>
        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == AProperty)
            {
                this.AChanged?.Invoke(this, new AChangedEventArgs(A));
            }
        }

        /// <summary>
        /// Create a new <see cref="AControl"/>.
        /// </summary>
        public AControl()
        {
            InitializeComponent();

            this.FindControl<NumericUpDown>("ABox").ValueChanged += (s, e) =>
            {
                this.A = (byte)e.NewValue;
            };
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }

    /// <summary>
    /// <see cref="EventArgs"/> for the <see cref="IAControl.AChanged"/> event.
    /// </summary>
    public class AChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The new alplha value.
        /// </summary>
        public byte A { get; }

        /// <summary>
        /// Create a new <see cref="AChangedEventArgs"/> object.
        /// </summary>
        /// <param name="a">The new alpha value.</param>
        public AChangedEventArgs(byte a)
        {
            this.A = a;
        }
    }
}
