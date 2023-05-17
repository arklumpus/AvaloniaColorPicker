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
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using System;

namespace AvaloniaColorPicker
{
    /// <summary>
    /// Represents controls used to select a colour from a colour canvas.
    /// </summary>
    public interface IColorCanvasControls
    {
        /// <summary>
        /// The colour space of the canvas.
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
        /// The alpha component of the colour.
        /// </summary>
        byte A { get; set; }

        /// <summary>
        /// Invoked when the user selects a new colour from the TwoD canvas.
        /// </summary>
        event EventHandler<ColorUpdatedEventArgs> ColorUpdatedFromCanvasTwoD;

        /// <summary>
        /// Invoked when the user selects a new colour from the OneD canvas.
        /// </summary>
        event EventHandler<ColorUpdatedEventArgs> ColorUpdatedFromCanvasOneD;

        /// <summary>
        /// Invoked when the user changes the alpha value of the colour.
        /// </summary>
        event EventHandler<ColorUpdatedEventArgs> ColorUpdatedFromAlphaCanvas;

        /// <summary>
        /// Update the canvases after the value of the colour components has been changed.
        /// </summary>
        /// <param name="instantTransition">If this is <see langword="true" />, the update happens instantly. Otherwise, a smooth transition is applied.</param>
        void UpdatePreview(bool instantTransition);
    }

    /// <summary>
    /// Controls used to select a colour from a colour canvas.
    /// </summary>
    public partial class ColorCanvasControls : UserControl, IColorCanvasControls
    {
        /// <summary>
        /// Defines the <see cref="IsAlphaVisible"/> property.
        /// </summary>
        public static readonly StyledProperty<bool> IsAlphaVisibleProperty = AvaloniaProperty.Register<ColorPicker, bool>(nameof(IsAlphaVisible), true);

        /// <summary>
        /// Determines whether the alpha slider is visible or not.
        /// </summary>
        public bool IsAlphaVisible
        {
            get { return GetValue(IsAlphaVisibleProperty); }
            set { SetValue(IsAlphaVisibleProperty, value); }
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
                UpdatePreview(true);
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
                UpdatePreview(true);
            }
        }


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
        /// Defines the <see cref="A"/> property.
        /// </summary>
        public static readonly StyledProperty<byte> AProperty = AvaloniaProperty.Register<AControl, byte>(nameof(A), 255);

        /// <summary>
        /// Determines the alpha component of the colour.
        /// </summary>
        public byte A
        {
            get { return GetValue(AProperty); }
            set { SetValue(AProperty, value); }
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

        /// <inheritdoc/>
        public event EventHandler<ColorUpdatedEventArgs> ColorUpdatedFromCanvasTwoD;

        /// <inheritdoc/>
        public event EventHandler<ColorUpdatedEventArgs> ColorUpdatedFromCanvasOneD;

        /// <inheritdoc/>
        public event EventHandler<ColorUpdatedEventArgs> ColorUpdatedFromAlphaCanvas;

        private AnimatableColourCanvas AnimatableColourCanvas { get; }

        /// <inheritdoc/>
        public void UpdatePreview(bool instantTransition)
        {
            switch (ColorSpace)
            {
                case ColorPicker.ColorSpaces.RGB:
                    switch (ColorComponent)
                    {
                        case ColorComponents.Component1:
                            AnimatableColourCanvas.UpdateR(R, G, B, instantTransition);
                            ColorPicker.SetTranslateRenderTransform(this.FindControl<Ellipse>("TwoDPositionCircle"), G, 255 - B, instantTransition);
                            ColorPicker.SetTranslateRenderTransform(this.FindControl<Canvas>("OneDPositionCanvas"), 0, 255 - R, instantTransition);
                            this.FindControl<Rectangle>("OneDPositionRectangle").Fill = new SolidColorBrush(Color.FromRgb(R, G, B));
                            break;
                        case ColorComponents.Component2:
                            AnimatableColourCanvas.UpdateG(R, G, B, instantTransition);
                            ColorPicker.SetTranslateRenderTransform(this.FindControl<Ellipse>("TwoDPositionCircle"), 255 - R, 255 - B, instantTransition);
                            ColorPicker.SetTranslateRenderTransform(this.FindControl<Canvas>("OneDPositionCanvas"), 0, 255 - G, instantTransition);
                            this.FindControl<Rectangle>("OneDPositionRectangle").Fill = new SolidColorBrush(Color.FromRgb(R, G, B));
                            break;
                        case ColorComponents.Component3:
                            AnimatableColourCanvas.UpdateB(R, G, B, instantTransition);
                            ColorPicker.SetTranslateRenderTransform(this.FindControl<Ellipse>("TwoDPositionCircle"), G, R, instantTransition);
                            ColorPicker.SetTranslateRenderTransform(this.FindControl<Canvas>("OneDPositionCanvas"), 0, 255 - B, instantTransition);
                            this.FindControl<Rectangle>("OneDPositionRectangle").Fill = new SolidColorBrush(Color.FromRgb(R, G, B));
                            break;
                    }
                    if (S < 128 && V > 128)
                    {
                        this.FindControl<Ellipse>("TwoDPositionCircle").Stroke = Brushes.Black;
                    }
                    else
                    {
                        this.FindControl<Ellipse>("TwoDPositionCircle").Stroke = Brushes.White;
                    }
                    break;
                case ColorPicker.ColorSpaces.HSB:
                    switch (ColorComponent)
                    {
                        case ColorComponents.Component1:
                            AnimatableColourCanvas.UpdateH(H / 255.0, S / 255.0, V / 255.0, instantTransition);
                            ColorPicker.SetTranslateRenderTransform(this.FindControl<Ellipse>("TwoDPositionCircle"), S, 255 - V, instantTransition);
                            ColorPicker.SetTranslateRenderTransform(this.FindControl<Canvas>("OneDPositionCanvas"), 0, 255 - H, instantTransition);
                            this.FindControl<Rectangle>("OneDPositionRectangle").Fill = new SolidColorBrush(HSB.ColorFromHSV(H / 255.0, 1, 1));
                            break;
                        case ColorComponents.Component2:
                            AnimatableColourCanvas.UpdateS(H / 255.0, S / 255.0, V / 255.0, instantTransition);
                            ColorPicker.SetTranslateRenderTransform(this.FindControl<Ellipse>("TwoDPositionCircle"), H, 255 - V, instantTransition);
                            ColorPicker.SetTranslateRenderTransform(this.FindControl<Canvas>("OneDPositionCanvas"), 0, 255 - S, instantTransition);
                            this.FindControl<Rectangle>("OneDPositionRectangle").Fill = new SolidColorBrush(Color.FromRgb(R, G, B));
                            break;
                        case ColorComponents.Component3:
                            AnimatableColourCanvas.UpdateV(H / 255.0, S / 255.0, V / 255.0, instantTransition);
                            ColorPicker.SetTranslateRenderTransform(this.FindControl<Ellipse>("TwoDPositionCircle"), 128 + S * 0.5 * Math.Cos(H / 255.0 * 2 * Math.PI), 128 + S * 0.5 * Math.Sin(H / 255.0 * 2 * Math.PI), instantTransition);
                            ColorPicker.SetTranslateRenderTransform(this.FindControl<Canvas>("OneDPositionCanvas"), 0, 255 - V, instantTransition);
                            this.FindControl<Rectangle>("OneDPositionRectangle").Fill = new SolidColorBrush(Color.FromRgb(R, G, B));
                            break;
                    }
                    if (S < 128 && V > 128)
                    {
                        this.FindControl<Ellipse>("TwoDPositionCircle").Stroke = Brushes.Black;
                    }
                    else
                    {
                        this.FindControl<Ellipse>("TwoDPositionCircle").Stroke = Brushes.White;
                    }
                    break;
                case ColorPicker.ColorSpaces.LAB:
                    switch (ColorComponent)
                    {
                        case ColorComponents.Component1:
                            AnimatableColourCanvas.UpdateL(L / 100.0, a / 100.0, b / 100.0, instantTransition);
                            ColorPicker.SetTranslateRenderTransform(this.FindControl<Ellipse>("TwoDPositionCircle"), (255 + 255 * b / 100.0) / 2, (255 - 255 * a / 100.0) / 2, instantTransition);
                            ColorPicker.SetTranslateRenderTransform(this.FindControl<Canvas>("OneDPositionCanvas"), 0, 255 - L / 100.0 * 255, instantTransition);
                            this.FindControl<Rectangle>("OneDPositionRectangle").Fill = new SolidColorBrush(Color.FromRgb(R, G, B));
                            break;
                        case ColorComponents.Component2:
                            AnimatableColourCanvas.Updatea(L / 100.0, a / 100.0, b / 100.0, instantTransition);
                            ColorPicker.SetTranslateRenderTransform(this.FindControl<Ellipse>("TwoDPositionCircle"), (255 + 255 * b / 100.0) / 2, 255 - 255 * L / 100.0, instantTransition);
                            ColorPicker.SetTranslateRenderTransform(this.FindControl<Canvas>("OneDPositionCanvas"), 0, (255 - a / 100.0 * 255) * 0.5, instantTransition);
                            this.FindControl<Rectangle>("OneDPositionRectangle").Fill = new SolidColorBrush(Color.FromRgb(R, G, B));
                            break;
                        case ColorComponents.Component3:
                            AnimatableColourCanvas.Updateb(L / 100.0, a / 100.0, b / 100.0, instantTransition);
                            ColorPicker.SetTranslateRenderTransform(this.FindControl<Ellipse>("TwoDPositionCircle"), 255 - 255 * L / 100.0, (255 - 255 * a / 100.0) / 2, instantTransition);
                            ColorPicker.SetTranslateRenderTransform(this.FindControl<Canvas>("OneDPositionCanvas"), 0, (255 - b / 100.0 * 255) * 0.5, instantTransition);
                            this.FindControl<Rectangle>("OneDPositionRectangle").Fill = new SolidColorBrush(Color.FromRgb(R, G, B));
                            break;
                    }

                    (double L1, double a1, double b1) = Lab.ToLab(R, G, B);

                    double C = Math.Sqrt(a1 * a1 + b1 * b1);
                    double Sl = C / Math.Sqrt(C * C + L1 * L1);

                    if (Sl < 0.5 && L1 > 0.5)
                    {
                        this.FindControl<Ellipse>("TwoDPositionCircle").Stroke = Brushes.Black;
                    }
                    else
                    {
                        this.FindControl<Ellipse>("TwoDPositionCircle").Stroke = Brushes.White;
                    }
                    break;
            }

            ColorPicker.SetTranslateRenderTransform(this.FindControl<Canvas>("AlphaPositionCanvas"), 0, 255 - this.A, instantTransition);
            this.FindControl<Rectangle>("AlphaPositionRectangle").Fill = new SolidColorBrush(Color.FromArgb(A, R, G, B));
        }

        /// <inheritdoc/>
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == IsAlphaVisibleProperty)
            {
                if (change.GetNewValue<bool>())
                {
                    this.FindControl<Grid>("MainGrid").ColumnDefinitions[3].Width = new GridLength(15, GridUnitType.Pixel);
                    this.FindControl<Grid>("MainGrid").ColumnDefinitions[4].Width = new GridLength(24, GridUnitType.Pixel);
                    this.FindControl<Grid>("MainGrid").Width = 354;
                }
                else
                {
                    this.FindControl<Grid>("MainGrid").ColumnDefinitions[3].Width = new GridLength(0, GridUnitType.Pixel);
                    this.FindControl<Grid>("MainGrid").ColumnDefinitions[4].Width = new GridLength(0, GridUnitType.Pixel);
                    this.FindControl<Grid>("MainGrid").Width = 315;
                }
            }

        }

        /// <summary>
        /// Create a new <see cref="ColorCanvasControls"/>.
        /// </summary>
        public ColorCanvasControls()
        {
            InitializeComponent();

            this.FindControl<Canvas>("CanvasTwoD").PointerPressed += CanvasTwoDPressed;
            this.FindControl<Canvas>("CanvasTwoD").PointerReleased += CanvasTwoDReleased;
            this.FindControl<Canvas>("CanvasTwoD").PointerMoved += CanvasTwoDMove;

            this.FindControl<Canvas>("CanvasOneD").PointerPressed += CanvasOneDPressed;
            this.FindControl<Canvas>("CanvasOneD").PointerReleased += CanvasOneDReleased;
            this.FindControl<Canvas>("CanvasOneD").PointerMoved += CanvasOneDMove;

            this.FindControl<Canvas>("AlphaCanvas").PointerPressed += AlphaCanvasPressed;
            this.FindControl<Canvas>("AlphaCanvas").PointerReleased += AlphaCanvasReleased;
            this.FindControl<Canvas>("AlphaCanvas").PointerMoved += AlphaCanvasMove;

            AnimatableColourCanvas = new AnimatableColourCanvas();
            this.FindControl<Canvas>("CanvasTwoDImageContainer").Children.Add(AnimatableColourCanvas.CanvasTwoD);
            this.FindControl<Canvas>("CanvasOneDImageContainer").Children.Add(AnimatableColourCanvas.CanvasOneD);
            this.FindControl<Canvas>("AlphaCanvasImageContainer").Children.Add(AnimatableColourCanvas.AlphaCanvas);

            IBrush alphaBGBrush = Brush.Parse("#DCDCDC");

            for (int i = 0; i < 256 / 8; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if ((i + j) % 2 == 0)
                    {
                        Rectangle rect = new Rectangle() { Width = 8, Height = 8, Fill = alphaBGBrush, Margin = new Thickness(j * 8, i * 8, 0, 0) };
                        this.FindControl<Canvas>("AlphaCanvasBackground").Children.Add(rect);
                    }
                }
            }

            UpdatePreview(true);
        }

        private bool IsCanvasTwoDPressed = false;

        private DispatcherTimer CanvasTwoDTimer { get; } = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 100), DispatcherPriority.Input, StopTimer);
        private DispatcherTimer CanvasOneDTimer { get; } = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 100), DispatcherPriority.Input, StopTimer);
        private DispatcherTimer AlphaCanvasTimer { get; } = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 100), DispatcherPriority.Input, StopTimer);

        private static void StopTimer(object sender, EventArgs e)
        {
            ((DispatcherTimer)sender).Stop();
        }

        private void UpdateColorFromCanvasTwoD(Point pos, bool instantTransition)
        {
            double x = Math.Max(0, Math.Min(pos.X, 255));
            double y = Math.Max(0, Math.Min(pos.Y, 255));

            byte R = this.R;
            byte G = this.G;
            byte B = this.B;

            switch (ColorSpace)
            {
                case ColorPicker.ColorSpaces.RGB:
                    switch (ColorComponent)
                    {
                        case ColorComponents.Component1:
                            G = (byte)Math.Round(x);
                            B = (byte)Math.Round(255 - y);
                            break;
                        case ColorComponents.Component2:
                            R = (byte)Math.Round(255 - x);
                            B = (byte)Math.Round(255 - y);
                            break;
                        case ColorComponents.Component3:
                            R = (byte)Math.Round(y);
                            G = (byte)Math.Round(x);
                            break;
                    }
                    break;

                case ColorPicker.ColorSpaces.HSB:
                    switch (ColorComponent)
                    {
                        case ColorComponents.Component1:
                            S = (byte)x;
                            V = (byte)(255 - y);
                            HSB.HSVToRGB(H / 255.0, S / 255.0, V / 255.0, out R, out G, out B);
                            break;
                        case ColorComponents.Component2:
                            H = (byte)x;
                            V = (byte)(255 - y);
                            HSB.HSVToRGB(H / 255.0, S / 255.0, V / 255.0, out R, out G, out B);
                            break;
                        case ColorComponents.Component3:
                            double h = Math.Atan2(y - 128, x - 128);
                            if (h < 0)
                            {
                                h += 2 * Math.PI;
                            }
                            h /= 2 * Math.PI;
                            H = (byte)(h * 255);
                            S = (byte)(Math.Min(1, Math.Sqrt((x - 128) * (x - 128) + (y - 128) * (y - 128)) / 128) * 255);
                            HSB.HSVToRGB(H / 255.0, S / 255.0, V / 255.0, out R, out G, out B);
                            break;
                    }

                    break;
                case ColorPicker.ColorSpaces.LAB:
                    switch (ColorComponent)
                    {
                        case ColorComponents.Component1:
                            a = (int)(100 - 2 * y / 255.0 * 100);
                            b = (int)(2 * x / 255.0 * 100 - 100);
                            Lab.FromLab(L / 100.0, a / 100.0, b / 100.0, out R, out G, out B);
                            break;
                        case ColorComponents.Component2:
                            L = (byte)(100 - y / 255.0 * 100);
                            b = (int)(2 * x / 255.0 * 100 - 100);
                            Lab.FromLab(L / 100.0, a / 100.0, b / 100.0, out R, out G, out B);
                            break;
                        case ColorComponents.Component3:
                            L = (byte)(100 - x / 255.0 * 100);
                            a = (int)(100 - 2 * y / 255.0 * 100);
                            Lab.FromLab(L / 100.0, a / 100.0, b / 100.0, out R, out G, out B);
                            break;
                    }
                    break;
            }

            this.R = R;
            this.G = G;
            this.B = B;

            if (ColorSpace != ColorPicker.ColorSpaces.LAB)
            {
                (double _L, double _a, double _b) = Lab.ToLab(R, G, B);

                this.L = (byte)Math.Min(100, Math.Max(0, (_L * 100)));
                this.a = (int)Math.Min(100, Math.Max(-100, (_a * 100)));
                this.b = (int)Math.Min(100, Math.Max(-100, (_b * 100)));
            }

            if (ColorSpace != ColorPicker.ColorSpaces.HSB)
            {
                (double _H, double _S, double _V) = HSB.RGBToHSB(R / 255.0, G / 255.0, B / 255.0);

                this.H = (byte)Math.Min(255, Math.Max(0, (_H * 255)));
                this.S = (byte)Math.Min(255, Math.Max(0, (_S * 255)));
                this.V = (byte)Math.Min(255, Math.Max(0, (_V * 255)));
            }

            UpdatePreview(instantTransition);

            ColorUpdatedFromCanvasTwoD?.Invoke(this, new ColorUpdatedEventArgs(R, G, B, H, S, V, L, a, b, A, instantTransition));
        }

        private void CanvasTwoDPressed(object sender, PointerPressedEventArgs e)
        {
            Point pos = e.GetPosition(this.FindControl<Canvas>("CanvasTwoD"));

            UpdateColorFromCanvasTwoD(pos, false);

            IsCanvasTwoDPressed = true;

            CanvasTwoDTimer.Start();
        }

        private void CanvasTwoDReleased(object sender, PointerReleasedEventArgs e)
        {
            IsCanvasTwoDPressed = false;
        }

        private void CanvasTwoDMove(object sender, PointerEventArgs e)
        {
            if (IsCanvasTwoDPressed && !CanvasTwoDTimer.IsEnabled)
            {
                Point pos = e.GetPosition(this.FindControl<Canvas>("CanvasTwoD"));

                UpdateColorFromCanvasTwoD(pos, true);
            }
        }



        bool IsCanvasOneDPressed = false;

        private void UpdateColorFromCanvasOneD(Point pos, bool instantTransition)
        {
            double y = Math.Max(0, Math.Min(pos.Y, 255));

            byte R = this.R;
            byte G = this.G;
            byte B = this.B;

            switch (ColorSpace)
            {
                case ColorPicker.ColorSpaces.RGB:
                    switch (ColorComponent)
                    {
                        case ColorComponents.Component1:
                            R = (byte)Math.Round(255 - y);
                            break;
                        case ColorComponents.Component2:
                            G = (byte)Math.Round(255 - y);
                            break;
                        case ColorComponents.Component3:
                            B = (byte)Math.Round(255 - y);
                            break;
                    }
                    break;

                case ColorPicker.ColorSpaces.HSB:
                    switch (ColorComponent)
                    {
                        case ColorComponents.Component1:
                            H = (byte)(255 - y);
                            HSB.HSVToRGB(H / 255.0, S / 255.0, V / 255.0, out R, out G, out B);
                            break;
                        case ColorComponents.Component2:
                            S = (byte)(255 - y);
                            HSB.HSVToRGB(H / 255.0, S / 255.0, V / 255.0, out R, out G, out B);
                            break;
                        case ColorComponents.Component3:
                            V = (byte)(255 - y);
                            HSB.HSVToRGB(H / 255.0, S / 255.0, V / 255.0, out R, out G, out B);
                            break;
                    }
                    break;

                case ColorPicker.ColorSpaces.LAB:
                    switch (ColorComponent)
                    {
                        case ColorComponents.Component1:
                            L = (byte)(100 - y / 255.0 * 100.0);
                            Lab.FromLab(L / 100.0, a / 100.0, b / 100.0, out R, out G, out B);
                            break;
                        case ColorComponents.Component2:
                            a = (int)(100 - 2 * y / 255.0 * 100.0);
                            Lab.FromLab(L / 100.0, a / 100.0, b / 100.0, out R, out G, out B);
                            break;
                        case ColorComponents.Component3:
                            b = (int)(100 - 2 * y / 255.0 * 100.0);
                            Lab.FromLab(L / 100.0, a / 100.0, b / 100.0, out R, out G, out B);
                            break;
                    }
                    break;
            }

            this.R = R;
            this.G = G;
            this.B = B;

            if (ColorSpace != ColorPicker.ColorSpaces.LAB)
            {
                (double _L, double _a, double _b) = Lab.ToLab(R, G, B);

                this.L = (byte)Math.Min(100, Math.Max(0, (_L * 100)));
                this.a = (int)Math.Min(100, Math.Max(-100, (_a * 100)));
                this.b = (int)Math.Min(100, Math.Max(-100, (_b * 100)));
            }

            if (ColorSpace != ColorPicker.ColorSpaces.HSB)
            {
                (double _H, double _S, double _V) = HSB.RGBToHSB(R / 255.0, G / 255.0, B / 255.0);

                this.H = (byte)Math.Min(255, Math.Max(0, (_H * 255)));
                this.S = (byte)Math.Min(255, Math.Max(0, (_S * 255)));
                this.V = (byte)Math.Min(255, Math.Max(0, (_V * 255)));
            }

            UpdatePreview(instantTransition);

            ColorUpdatedFromCanvasOneD?.Invoke(this, new ColorUpdatedEventArgs(R, G, B, H, S, V, L, a, b, A, instantTransition));
        }

        private void CanvasOneDPressed(object sender, PointerPressedEventArgs e)
        {
            Point pos = e.GetPosition(this.FindControl<Canvas>("CanvasOneD"));

            UpdateColorFromCanvasOneD(pos, false);

            IsCanvasOneDPressed = true;
            CanvasOneDTimer.Start();
        }

        private void CanvasOneDReleased(object sender, PointerReleasedEventArgs e)
        {
            IsCanvasOneDPressed = false;
        }

        private void CanvasOneDMove(object sender, PointerEventArgs e)
        {
            if (IsCanvasOneDPressed && !CanvasOneDTimer.IsEnabled)
            {
                Point pos = e.GetPosition(this.FindControl<Canvas>("CanvasOneD"));

                UpdateColorFromCanvasOneD(pos, true);
            }
        }

        bool IsAlphaCanvasPressed = false;

        private void UpdateColorFromAlphaCanvas(Point pos, bool instantTransition)
        {
            double y = Math.Max(0, Math.Min(pos.Y, 255));

            this.A = (byte)Math.Round(255 - y);

            UpdatePreview(instantTransition);

            ColorUpdatedFromAlphaCanvas?.Invoke(this, new ColorUpdatedEventArgs(R, G, B, H, S, V, L, a, b, A, instantTransition));
        }

        private void AlphaCanvasPressed(object sender, PointerPressedEventArgs e)
        {
            Point pos = e.GetPosition(this.FindControl<Canvas>("AlphaCanvas"));

            UpdateColorFromAlphaCanvas(pos, false);

            IsAlphaCanvasPressed = true;

            AlphaCanvasTimer.Start();
        }

        private void AlphaCanvasReleased(object sender, PointerReleasedEventArgs e)
        {
            IsAlphaCanvasPressed = false;
        }

        private void AlphaCanvasMove(object sender, PointerEventArgs e)
        {
            if (IsAlphaCanvasPressed && !AlphaCanvasTimer.IsEnabled)
            {
                Point pos = e.GetPosition(this.FindControl<Canvas>("AlphaCanvas"));

                UpdateColorFromAlphaCanvas(pos, true);
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }

    /// <summary>
    /// <see cref="EventArgs"/> for the <see cref="IColorCanvasControls.ColorUpdatedFromAlphaCanvas"/>, <see cref="IColorCanvasControls.ColorUpdatedFromCanvasOneD"/>
    /// and <see cref="IColorCanvasControls.ColorUpdatedFromCanvasTwoD"/> events.
    /// </summary>
    public class ColorUpdatedEventArgs : EventArgs
    {
        /// <summary>
        /// The R component of the new colour.
        /// </summary>
        public byte R { get; }

        /// <summary>
        /// The G component of the new colour.
        /// </summary>
        public byte G { get; }

        /// <summary>
        /// The B component of the new colour.
        /// </summary>
        public byte B { get; }

        /// <summary>
        /// The H component of the new colour.
        /// </summary>
        public byte H { get; }

        /// <summary>
        /// The S component of the new colour.
        /// </summary>
        public byte S { get; }

        /// <summary>
        /// The V component of the new colour.
        /// </summary>
        public byte V { get; }

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
        /// The alpha component of the new colour.
        /// </summary>
        public byte A { get; }

        /// <summary>
        /// Whether the event should trigger an instantaneous update or not.
        /// </summary>
        public bool InstantTransition { get; }

        /// <summary>
        /// Create a new <see cref="ColorUpdatedEventArgs"/> object.
        /// </summary>
        /// <param name="r">The R component of the new colour.</param>
        /// <param name="g">The G component of the new colour.</param>
        /// <param name="b">The B component of the new colour.</param>
        /// <param name="h">The H component of the new colour.</param>
        /// <param name="s">The S component of the new colour.</param>
        /// <param name="v">The V component of the new colour.</param>
        /// <param name="l">The L* component of the new colour.</param>
        /// <param name="aStar">The a* component of the new colour.</param>
        /// <param name="bStar">The b* component of the new colour.</param>
        /// <param name="a">The alpha component of the new colour.</param>
        /// <param name="instantTransition">Whether the event should trigger an instantaneous update or not.</param>
        public ColorUpdatedEventArgs(byte r, byte g, byte b, byte h, byte s, byte v, byte l, int aStar, int bStar, byte a, bool instantTransition)
        {
            this.R = r;
            this.G = g;
            this.B = b;
            this.H = h;
            this.S = s;
            this.V = v;
            this.L = l;
            this.a = aStar;
            this.b = bStar;
            this.A = a;
            this.InstantTransition = instantTransition;
        }
    }
}
