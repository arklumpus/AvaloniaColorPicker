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

namespace AvaloniaColorPicker
{
    /// <summary>
    /// Represents a control containing a warning that is displayed when the user selects a colour that falls outside of the sRGB spectrum.
    /// </summary>
    public interface IOutsideRGBWarning
    {
        /// <summary>
        /// The opacity of the warning.
        /// </summary>
        double Opacity { get; set; }
    }

    /// <summary>
    /// A control containing a warning that is displayed when the user selects a colour that falls outside of the sRGB spectrum.
    /// </summary>
    public partial class OutsideRGBWarning : UserControl, IOutsideRGBWarning
    {
        /// <summary>
        /// Create a new <see cref="OutsideRGBWarning"/> object.
        /// </summary>
        public OutsideRGBWarning()
        {
            InitializeComponent();

            this.FindControl<Border>("WarningTooltipBorder").BorderBrush = Colours.BorderLowColour;
            this.FindControl<Border>("WarningTooltipBorder").Background = Colours.BackgroundColour;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
