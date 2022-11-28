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
using Avalonia.Media.Imaging;
using System;

namespace AvaloniaColorPicker
{
    /// <summary>
    /// Represents a control providing a preview of the current colour space.
    /// </summary>
    public interface IColorSpacePreview
    {
        /// <summary>
        /// The current colour space.
        /// </summary>
        ColorPicker.ColorSpaces ColorSpace { get; set; }

        /// <summary>
        /// The currently selected colour component.
        /// </summary>
        ColorComponents ColorComponent { get; set; }

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
        /// The hue of the colour.
        /// </summary>
        byte H { get; set; }

        /// <summary>
        /// The saturation of the colour.
        /// </summary>
        byte S { get; set; }

        /// <summary>
        /// The value (brightness) of the colour.
        /// </summary>
        byte V { get; set; }

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
        /// Update the preview of the colour space after the selected colour has changed.
        /// </summary>
        /// <param name="instantTransition">Whether the update should happen instantly or not.</param>
        /// <param name="warningTooltip">An <see cref="IOutsideRGBWarning"/> control representing the warning that appears if the user selects a colour outside of the sRGB colour space. This can be <see langword="null"/>.</param>
        void UpdatePreview(bool instantTransition, IOutsideRGBWarning warningTooltip);
    }

    /// <summary>
    /// A control providing a preview of the currently selected colour space.
    /// </summary>
    public partial class ColorSpacePreview : UserControl, IColorSpacePreview
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
            set { SetValue(RProperty, value); }
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
            set { SetValue(GProperty, value); }
        }


        /// <summary>
        /// Defines the <see cref="B"/> property.
        /// </summary>
        public static readonly StyledProperty<byte> BProperty = AvaloniaProperty.Register<RGBControls, byte>(nameof(B), 232);

        /// <summary>
        /// Determines the B component (blue) of the colour.
        /// </summary>
        public byte B
        {
            get { return GetValue(BProperty); }
            set { SetValue(BProperty, value); }
        }



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
            set { SetValue(HProperty, value); }
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
            set { SetValue(SProperty, value); }
        }


        /// <summary>
        /// Defines the <see cref="V"/> property.
        /// </summary>
        public static readonly StyledProperty<byte> VProperty = AvaloniaProperty.Register<HSBControls, byte>(nameof(V), 232);

        /// <summary>
        /// Determines the V component of the colour.
        /// </summary>
        public byte V
        {
            get { return GetValue(VProperty); }
            set { SetValue(VProperty, value); }
        }

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
            set { SetValue(LProperty, Math.Min(value, (byte)100)); }
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
            set { SetValue(aProperty, Math.Max(-100, Math.Min(100, value))); }
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
            set { SetValue(bProperty, Math.Max(-100, Math.Min(100, value))); }
        }

        /// <summary>
        /// Defines the <see cref="ColorSpace"/> property.
        /// </summary>
        public static readonly StyledProperty<ColorPicker.ColorSpaces> ColorSpaceProperty = AvaloniaProperty.Register<ColorSpacePreview, ColorPicker.ColorSpaces>(nameof(ColorSpace), ColorPicker.ColorSpaces.HSB);

        /// <summary>
        /// The currently selected colour space.
        /// </summary>
        public ColorPicker.ColorSpaces ColorSpace
        {
            get { return GetValue(ColorSpaceProperty); }
            set
            {
                SetValue(ColorSpaceProperty, value);
            }
        }

        /// <summary>
        /// Defines the <see cref="ColorComponent"/> property.
        /// </summary>
        public static readonly StyledProperty<ColorComponents> ColorComponentProperty = AvaloniaProperty.Register<ColorSpacePreview, ColorComponents>(nameof(ColorComponent), ColorComponents.Component1);

        /// <summary>
        /// The currently selected colour component.
        /// </summary>
        public ColorComponents ColorComponent
        {
            get { return GetValue(ColorComponentProperty); }
            set
            {
                SetValue(ColorComponentProperty, value);
            }
        }

        /// <inheritdoc/>
        public void UpdatePreview(bool instantTransition, IOutsideRGBWarning warningTooltip)
        {
            Canvas colorSpaceCanvas = null;

            Canvas previousCanvas = null;

            if (this.FindControl<Canvas>("ColorSpaceCanvas").Children.Count > 0)
            {
                previousCanvas = (Canvas)this.FindControl<Canvas>("ColorSpaceCanvas").Children[0];
            }

            WriteableBitmap canvas2Dimage;

            switch (ColorSpace)
            {
                case ColorPicker.ColorSpaces.RGB:
                    switch (ColorComponent)
                    {
                        case ColorComponents.Component1:
                            canvas2Dimage = RGB.GetGB(R);
                            colorSpaceCanvas = RGB.GetRCanvas(R, G, B, canvas2Dimage, previousCanvas, instantTransition);
                            break;
                        case ColorComponents.Component2:
                            canvas2Dimage = RGB.GetRB(G);
                            colorSpaceCanvas = RGB.GetGCanvas(R, G, B, canvas2Dimage, previousCanvas, instantTransition);
                            break;
                        case ColorComponents.Component3:
                            canvas2Dimage = RGB.GetRG(B);
                            colorSpaceCanvas = RGB.GetBCanvas(R, G, B, canvas2Dimage, previousCanvas, instantTransition);
                            break;
                    }
                    break;
                case ColorPicker.ColorSpaces.HSB:
                    switch (ColorComponent)
                    {
                        case ColorComponents.Component1:
                            canvas2Dimage = HSB.GetSB(H / 255.0);
                            colorSpaceCanvas = HSB.GetHCanvas(H / 255.0, S / 255.0, V / 255.0, canvas2Dimage, previousCanvas, instantTransition);
                            break;
                        case ColorComponents.Component2:
                            canvas2Dimage = HSB.GetHB(S / 255.0);
                            colorSpaceCanvas = HSB.GetSCanvas(H / 255.0, S / 255.0, V / 255.0, canvas2Dimage, previousCanvas, instantTransition);
                            break;
                        case ColorComponents.Component3:
                            canvas2Dimage = HSB.GetHS(V / 255.0);
                            colorSpaceCanvas = HSB.GetBCanvas(H / 255.0, S / 255.0, V / 255.0, canvas2Dimage, previousCanvas, instantTransition);
                            break;
                    }
                    break;
                case ColorPicker.ColorSpaces.LAB:
                    switch (ColorComponent)
                    {
                        case ColorComponents.Component1:
                            colorSpaceCanvas = Lab.GetLCanvas(L / 100.0, a / 100.0, b / 100.0, previousCanvas, instantTransition, warningTooltip);
                            break;
                        case ColorComponents.Component2:
                            colorSpaceCanvas = Lab.GetACanvas(L / 100.0, a / 100.0, b / 100.0, previousCanvas, instantTransition, warningTooltip);
                            break;
                        case ColorComponents.Component3:
                            colorSpaceCanvas = Lab.GetBCanvas(L / 100.0, a / 100.0, b / 100.0, previousCanvas, instantTransition, warningTooltip);
                            break;
                    }
                    break;
            }

            if (colorSpaceCanvas != null && colorSpaceCanvas != previousCanvas)
            {
                this.FindControl<Canvas>("ColorSpaceCanvas").Children.Clear();
                colorSpaceCanvas.ClipToBounds = true;
                this.FindControl<Canvas>("ColorSpaceCanvas").Children.Add(colorSpaceCanvas);
            }
        }

        /// <summary>
        /// Create a new <see cref="ColorSpacePreview"/>.
        /// </summary>
        public ColorSpacePreview()
        {
            InitializeComponent();
            UpdatePreview(true, null);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
