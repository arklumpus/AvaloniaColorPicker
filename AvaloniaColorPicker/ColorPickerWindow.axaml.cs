/*
    AvaloniaColorPicker - A color picker for Avalonia.
    Copyright (C) 2021  Giorgio Bianchini
 
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

using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System.Threading.Tasks;

namespace AvaloniaColorPicker
{
    /// <summary>
    /// Interface describing a contract for a colour picker window. Note that, even though this is not enforced by the interface, a class implementing this interface MUST have a constructor which takes a single <see cref="Color"/>? parameter representing the previous colour in the colour picker.
    /// </summary>
    public interface IColorPickerWindow
    {
        /// <summary>
        /// The color that is currently selected in the <see cref="ColorPicker"/>.
        /// </summary>
        Color Color { get; set; }

        /// <summary>
        /// Represents the previously selected <see cref="Avalonia.Media.Color"/> (e.g. if the <see cref="ColorPicker"/> is being used to change the colour of an object, it would represent the previous colour of the object. Set to <see langword="null" /> to hide the previous colour display.
        /// </summary>
        Color? PreviousColor { get; set; }

        /// <summary>
        /// Shows the <see cref="ColorPickerWindow"/> as a dialog.
        /// </summary>
        /// <param name="parent">The <see cref="ColorPickerWindow"/>'s owner window.</param>
        /// <returns>The selected <see cref="Avalonia.Media.Color"/> if the user clicks on the "OK" button; <see langword="null"/> otherwise.</returns>
        Task<Color?> ShowDialog(Window parent);
    }


    /// <summary>
    /// A <see cref="Window"/> containing a <see cref="ColorPicker"/> control.
    /// </summary>
    public partial class ColorPickerWindow : Window, IColorPickerWindow
    {
        /// <summary>
        /// The color that is currently selected in the <see cref="ColorPicker"/>.
        /// </summary>
        public Color Color
        {
            get => this.FindControl<ColorPicker>("ColorPicker").Color;
            set
            {
                this.FindControl<ColorPicker>("ColorPicker").Color = value;
            }
        }

        /// <summary>
        /// Represents the previously selected <see cref="Avalonia.Media.Color"/> (e.g. if the <see cref="ColorPicker"/> is being used to change the colour of an object, it would represent the previous colour of the object. Set to <see langword="null" /> to hide the previous colour display.
        /// </summary>
        public Color? PreviousColor
        {
            get => this.FindControl<ColorPicker>("ColorPicker").PreviousColor;
            set
            {
                this.FindControl<ColorPicker>("ColorPicker").PreviousColor = value;
            }
        }

        /// <summary>
        /// Determines whether the palette selector is visible or not.
        /// </summary>
        public bool IsPaletteVisible
        {
            get => this.FindControl<ColorPicker>("ColorPicker").IsPaletteVisible;
            set { this.FindControl<ColorPicker>("ColorPicker").IsPaletteVisible = value; }
        }

        /// <summary>
        /// Determines whether the colour blindness selector is visible or not.
        /// </summary>
        public bool IsColourBlindnessSelectorVisible
        {
            get => this.FindControl<ColorPicker>("ColorPicker").IsColourBlindnessSelectorVisible;
            set { this.FindControl<ColorPicker>("ColorPicker").IsColourBlindnessSelectorVisible = value; }
        }

        /// <summary>
        /// Determines whether the hex value text box is visible or not.
        /// </summary>
        public bool IsHexVisible
        {
            get => this.FindControl<ColorPicker>("ColorPicker").IsHexVisible;
            set { this.FindControl<ColorPicker>("ColorPicker").IsHexVisible = value; }
        }

        /// <summary>
        /// Determines whether the alpha value text box and slider are visible or not.
        /// </summary>
        public bool IsAlphaVisible
        {
            get => this.FindControl<ColorPicker>("ColorPicker").IsAlphaVisible;
            set { this.FindControl<ColorPicker>("ColorPicker").IsAlphaVisible = value; }
        }

        /// <summary>
        /// Determines whether the CIELAB component text boxes are visible or not.
        /// </summary>
        public bool IsCIELABVisible
        {
            get => this.FindControl<ColorPicker>("ColorPicker").IsCIELABVisible;
            set { this.FindControl<ColorPicker>("ColorPicker").IsCIELABVisible = value; }
        }

        /// <summary>
        /// Determines whether the HSB component text boxes are visible or not.
        /// </summary>
        public bool IsHSBVisible
        {
            get => this.FindControl<ColorPicker>("ColorPicker").IsHSBVisible;
            set { this.FindControl<ColorPicker>("ColorPicker").IsHSBVisible = value; }
        }

        /// <summary>
        /// Determines whether RGB component text boxes are visible or not.
        /// </summary>
        public bool IsRGBVisible
        {
            get => this.FindControl<ColorPicker>("ColorPicker").IsRGBVisible;
            set { this.FindControl<ColorPicker>("ColorPicker").IsRGBVisible = value; }
        }

        /// <summary>
        /// Determines whether the colour space preview is visible or not.
        /// </summary>
        public bool IsColourSpacePreviewVisible
        {
            get => this.FindControl<ColorPicker>("ColorPicker").IsColourSpacePreviewVisible;
            set { this.FindControl<ColorPicker>("ColorPicker").IsColourSpacePreviewVisible = value; }
        }

        /// <summary>
        /// Determines whether the colour space selector is visible or not.
        /// </summary>
        public bool IsColourSpaceSelectorVisible
        {
            get => this.FindControl<ColorPicker>("ColorPicker").IsColourSpaceSelectorVisible;
            set { this.FindControl<ColorPicker>("ColorPicker").IsColourSpaceSelectorVisible = value; }
        }

        /// <summary>
        /// Determines whether the CIELAB colour space can be selected or not.
        /// </summary>
        public bool IsCIELABSelectable
        {
            get => this.FindControl<ColorPicker>("ColorPicker").IsCIELABSelectable;
            set { this.FindControl<ColorPicker>("ColorPicker").IsCIELABSelectable = value; }
        }

        /// <summary>
        /// Determines whether the HSB colour space can be selected or not.
        /// </summary>
        public bool IsHSBSelectable
        {
            get => this.FindControl<ColorPicker>("ColorPicker").IsHSBSelectable;
            set { this.FindControl<ColorPicker>("ColorPicker").IsHSBSelectable = value; }
        }

        /// <summary>
        /// Determines whether the RGB colour space can be selected or not.
        /// </summary>
        public bool IsRGBSelectable
        {
            get => this.FindControl<ColorPicker>("ColorPicker").IsRGBSelectable;
            set { this.FindControl<ColorPicker>("ColorPicker").IsRGBSelectable = value; }
        }

        /// <summary>
        /// The currently selected colour space.
        /// </summary>
        public ColorPicker.ColorSpaces ColorSpace
        {
            get => this.FindControl<ColorPicker>("ColorPicker").ColorSpace;
            set { this.FindControl<ColorPicker>("ColorPicker").ColorSpace = value; }
        }

        private bool Result = false;

        /// <summary>
        /// Creates a new <see cref="ColorPickerWindow"/> instance.
        /// </summary>
        public ColorPickerWindow()
        {
            this.InitializeComponent();

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            this.FindControl<Button>("OKButton").Click += (s, e) =>
            {
                this.Result = true;
                this.Close();
            };

            this.FindControl<Button>("CancelButton").Click += (s, e) =>
            {
                this.Result = false;
                this.Close();
            };
        }

        /// <summary>
        /// Creates a new <see cref="ColorPickerWindow"/> instance, setting the <see cref="Color"/> and <see cref="PreviousColor"/> to the specified value.
        /// </summary>
        /// <param name="previousColor"></param>
        public ColorPickerWindow(Color? previousColor) : this()
        {
            this.PreviousColor = previousColor;

            if (previousColor != null)
            {
                this.Color = previousColor.Value;
            }
        }

        /// <summary>
        /// Shows the <see cref="ColorPickerWindow"/> as a dialog.
        /// </summary>
        /// <param name="parent">The <see cref="ColorPickerWindow"/>'s owner window.</param>
        /// <returns>The selected <see cref="Avalonia.Media.Color"/> if the user clicks on the "OK" button; <see langword="null"/> otherwise.</returns>
        public new async Task<Color?> ShowDialog(Window parent)
        {
            await base.ShowDialog(parent);

            if (this.Result)
            {
                return this.Color;
            }
            else
            {
                return null;
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
