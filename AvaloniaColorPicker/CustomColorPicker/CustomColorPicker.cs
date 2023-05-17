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
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.LogicalTree;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AvaloniaColorPicker
{
    /// <summary>
    /// Represents a colour picker control.
    /// </summary>
    public interface IColorPicker
    {
        /// <summary>
        /// The <see cref="Avalonia.Media.Color"/> that is currently selected.
        /// </summary>
        Color Color { get; set; }

        /// <summary>
        /// Represents the previously selected <see cref="Avalonia.Media.Color"/> (e.g. if the <see cref="IColorPicker"/> is being used to change the colour of an object, it would represent the previous colour of the object). Set to <see langword="null" /> to hide the previous colour display.
        /// </summary>
        Color? PreviousColor { get; set; }
    }

    /// <summary>
    /// A custom colour picker control.
    /// </summary>
    public class CustomColorPicker : ContentControl, IStyleable, IColorPicker
    {
        Type IStyleable.StyleKey => typeof(ContentControl);

        /// <summary>
        /// Defines the <see cref="ColorSpace"/> property.
        /// </summary>
        public static readonly StyledProperty<ColorPicker.ColorSpaces> ColorSpaceProperty = AvaloniaProperty.Register<CustomColorPicker, ColorPicker.ColorSpaces>(nameof(ColorSpace), ColorPicker.ColorSpaces.HSB);

        /// <summary>
        /// The currently selected colour space.
        /// </summary>
        public ColorPicker.ColorSpaces ColorSpace
        {
            get { return GetValue(ColorSpaceProperty); }
            set { SetValue(ColorSpaceProperty, value); }
        }

        private byte R { get; set; }

        private byte G { get; set; }

        private byte B { get; set; }

        private byte A { get; set; }

        private byte H { get; set; }

        private byte S { get; set; }

        private byte V { get; set; }

        private byte L { get; set; }

        private int a { get; set; }

        private int b { get; set; }


        /// <summary>
        /// Defines the <see cref="Color"/> property.
        /// </summary>
        public static readonly StyledProperty<Color> ColorProperty = AvaloniaProperty.Register<ColorPicker, Color>(nameof(Color), Color.FromRgb(0, 162, 232));

        /// <summary>
        /// The <see cref="Avalonia.Media.Color"/> that is currently selected.
        /// </summary>
        public Color Color
        {
            get { return GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        private Color InternalColor
        {
            get { return Color.FromArgb(A, R, G, B); }
            set
            {
                ProgrammaticChange = true;
                SetColor(value.R, value.G, value.B, value.A);
                BuildColorInterface(false, this);
                ProgrammaticChange = false;
            }
        }

        /// <summary>
        /// Defines the <see cref="PreviousColor"/> property.
        /// </summary>
        public static readonly StyledProperty<Color?> PreviousColorProperty = AvaloniaProperty.Register<CustomColorPicker, Color?>(nameof(PreviousColor), null);

        /// <summary>
        /// Represents the previously selected <see cref="Avalonia.Media.Color"/> (e.g. if the <see cref="CustomColorPicker"/> is being used to change the colour of an object, it would represent the previous colour of the object. Set to <see langword="null" /> to hide the previous colour display.
        /// </summary>
        public Color? PreviousColor
        {
            get { return GetValue(PreviousColorProperty); }
            set { SetValue(PreviousColorProperty, value); }
        }

        private void SetColor(byte r, byte g, byte b, byte a)
        {
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = a;

            (double _L, double _a, double _b) = Lab.ToLab(R, G, B);

            this.L = (byte)Math.Min(100, Math.Max(0, (_L * 100)));
            this.a = (int)Math.Min(100, Math.Max(-100, (_a * 100)));
            this.b = (int)Math.Min(100, Math.Max(-100, (_b * 100)));

            (double _H, double _S, double _V) = HSB.RGBToHSB(R / 255.0, G / 255.0, B / 255.0);

            this.H = (byte)Math.Min(255, Math.Max(0, (_H * 255)));
            this.S = (byte)Math.Min(255, Math.Max(0, (_S * 255)));
            this.V = (byte)Math.Min(255, Math.Max(0, (_V * 255)));

            this.Color = this.InternalColor;
        }

        private List<IColorCanvasControls> ColorCanvasControls = new List<IColorCanvasControls>();
        private List<IColorSpaceSelector> ColorSpaceSelectors = new List<IColorSpaceSelector>();
        private List<IColorSpacePreview> ColorSpacePreviews = new List<IColorSpacePreview>();
        private List<IColorComponentSelector> ColorComponentSelectors = new List<IColorComponentSelector>();
        private List<ICurrentColorPreview> CurrentColorPreviews = new List<ICurrentColorPreview>();
        private List<IPreviousColorPreview> PreviousColorPreviews = new List<IPreviousColorPreview>();
        private List<IRGBControls> RGBControls = new List<IRGBControls>();
        private List<IHSBControls> HSBControls = new List<IHSBControls>();
        private List<ICIELABControls> CIELABControls = new List<ICIELABControls>();
        private List<IAControl> AControls = new List<IAControl>();
        private List<IHexControl> HexControls = new List<IHexControl>();
        private List<IColorBlindnessSelector> ColorBlindnessSelectors = new List<IColorBlindnessSelector>();
        private List<IPaletteControl> PaletteControls = new List<IPaletteControl>();
        private IOutsideRGBWarning WarningTooltip = null;

        /// <summary>
        /// Create a new <see cref="ColorPicker"/> instance.
        /// </summary>
        public CustomColorPicker()
        {
            this.Initialized += (s, e) =>
            {
                ProcessChildren(this.GetLogicalDescendants());

                AddEventHandlers();

                BuildColorInterface(true, this);
            };



            ProgrammaticChange = true;

            this.R = 0;
            this.G = 162;
            this.B = 232;

            this.H = 140;
            this.S = 255;
            this.V = 232;

            this.L = 63;
            this.a = -10;
            this.b = -44;

            this.A = 255;

            BuildColorInterface(true, this);

            ProgrammaticChange = false;
        }

        /// <summary>
        /// Create  a new <see cref="ColorPicker"/> instance.
        /// </summary>
        /// <param name="controlElements">The controls that are used to select the colour.</param>
        public CustomColorPicker(IEnumerable<object> controlElements)
        {
            ProcessChildren(controlElements);

            AddEventHandlers();

            ProgrammaticChange = true;

            this.R = 0;
            this.G = 162;
            this.B = 232;

            this.H = 140;
            this.S = 255;
            this.V = 232;

            this.L = 63;
            this.a = -10;
            this.b = -44;

            this.A = 255;

            BuildColorInterface(true, this);

            ProgrammaticChange = false;
        }

        /// <summary>
        /// Create  a new <see cref="ColorPicker"/> instance.
        /// </summary>
        /// <param name="controlElements">The controls that are used to select the colour.</param>
        public CustomColorPicker(params object[] controlElements) : this((IEnumerable<object>)controlElements) { }

        private void ProcessChildren(IEnumerable<object> controlElements)
        {
            foreach (object visual in controlElements)
            {
                if (visual is IColorCanvasControls iccc)
                {
                    this.ColorCanvasControls.Add(iccc);
                }

                if (visual is IColorSpaceSelector icss)
                {
                    this.ColorSpaceSelectors.Add(icss);
                }

                if (visual is IColorSpacePreview icsp)
                {
                    this.ColorSpacePreviews.Add(icsp);
                }

                if (visual is IColorComponentSelector iccs)
                {
                    this.ColorComponentSelectors.Add(iccs);
                }

                if (visual is ICurrentColorPreview iccp)
                {
                    this.CurrentColorPreviews.Add(iccp);
                }

                if (visual is IPreviousColorPreview ipcp)
                {
                    this.PreviousColorPreviews.Add(ipcp);
                }

                if (visual is IRGBControls irc)
                {
                    this.RGBControls.Add(irc);
                }

                if (visual is IHSBControls ihc)
                {
                    this.HSBControls.Add(ihc);
                }

                if (visual is ICIELABControls icc)
                {
                    this.CIELABControls.Add(icc);
                }

                if (visual is IAControl iac)
                {
                    this.AControls.Add(iac);
                }

                if (visual is IHexControl ixc)
                {
                    this.HexControls.Add(ixc);
                }

                if (visual is IColorBlindnessSelector icbs)
                {
                    this.ColorBlindnessSelectors.Add(icbs);
                }

                if (visual is IOutsideRGBWarning iorw)
                {
                    this.WarningTooltip = iorw;
                }

                if (visual is IPaletteControl ipc)
                {
                    PaletteControls.Add(ipc);
                }
            }


            foreach (IPreviousColorPreview control in PreviousColorPreviews)
            {
                control.PreviousColor = this.PreviousColor ?? Colors.Transparent;
                if (control is Control ctrl)
                {
                    ctrl.IsVisible = this.PreviousColor != null;
                }
            }

            foreach (IColorCanvasControls control2 in this.ColorCanvasControls)
            {
                control2.ColorSpace = ColorSpace;
            }

            foreach (IColorSpaceSelector control2 in ColorSpaceSelectors)
            {
                control2.ColorSpace = ColorSpace;
            }

            foreach (IColorSpacePreview control2 in ColorSpacePreviews)
            {
                control2.ColorSpace = ColorSpace;
                control2.UpdatePreview(true, WarningTooltip);
            }

            foreach (IColorComponentSelector control2 in ColorComponentSelectors)
            {
                control2.ColorSpace = ColorSpace;
            }
        }


        bool programmaticChangeColorSpace = false;

        private void AddEventHandlers()
        {
            foreach (IColorCanvasControls control in this.ColorCanvasControls)
            {
                control.ColorUpdatedFromAlphaCanvas += (s, e) => { this.A = e.A; UpdateDependingOnAlpha(e.InstantTransition, s); this.Color = this.InternalColor; };
                control.ColorUpdatedFromCanvasOneD += (s, e) =>
                {
                    this.R = e.R;
                    this.G = e.G;
                    this.B = e.B;

                    this.H = e.H;
                    this.S = e.S;
                    this.V = e.V;

                    this.L = e.L;
                    this.a = e.a;
                    this.b = e.b;

                    this.A = e.A;

                    BuildColorInterface(e.InstantTransition, s);

                    this.Color = this.InternalColor;
                };
                control.ColorUpdatedFromCanvasTwoD += (s, e) =>
                {
                    this.R = e.R;
                    this.G = e.G;
                    this.B = e.B;

                    this.H = e.H;
                    this.S = e.S;
                    this.V = e.V;

                    this.L = e.L;
                    this.a = e.a;
                    this.b = e.b;

                    this.A = e.A;

                    BuildColorInterface(e.InstantTransition, s);

                    this.Color = this.InternalColor;
                };
            }

            foreach (IColorSpaceSelector control in ColorSpaceSelectors)
            {
                control.ColorSpaceChanged += (s, e) =>
                {
                    if (!programmaticChangeColorSpace)
                    {
                        programmaticChangeColorSpace = true;

                        this.ColorSpace = e.ColorSpace;

                        foreach (IColorCanvasControls control2 in this.ColorCanvasControls)
                        {
                            control2.ColorSpace = e.ColorSpace;
                        }

                        foreach (IColorSpaceSelector control2 in ColorSpaceSelectors)
                        {
                            control2.ColorSpace = e.ColorSpace;
                        }

                        foreach (IColorSpacePreview control2 in ColorSpacePreviews)
                        {
                            control2.ColorSpace = e.ColorSpace;
                            control2.UpdatePreview(true, WarningTooltip);
                        }

                        foreach (IColorComponentSelector control2 in ColorComponentSelectors)
                        {
                            control2.ColorSpace = e.ColorSpace;
                        }

                        programmaticChangeColorSpace = false;
                    }
                };
            }

            foreach (IColorComponentSelector control in ColorComponentSelectors)
            {
                control.ColorComponentChanged += (s, e) =>
                {
                    foreach (IColorCanvasControls control2 in this.ColorCanvasControls)
                    {
                        control2.ColorComponent = e.ColorComponent;
                    }

                    foreach (IColorSpacePreview control2 in ColorSpacePreviews)
                    {
                        control2.ColorComponent = e.ColorComponent;
                        control2.UpdatePreview(true, WarningTooltip);
                    }

                    foreach (IColorComponentSelector control2 in ColorComponentSelectors)
                    {
                        control2.ColorComponent = e.ColorComponent;
                    }
                };
            }

            foreach (IPreviousColorPreview control in PreviousColorPreviews)
            {
                control.ColorSelected += (s, e) =>
                {
                    this.InternalColor = e.Color;
                };
            }

            foreach (ICurrentColorPreview control in CurrentColorPreviews)
            {
                control.ColorSelected += (s, e) =>
                {
                    this.InternalColor = e.Color;
                };
            }

            foreach (IRGBControls control in RGBControls)
            {
                control.ColorChanged += (s, e) =>
                {
                    if (!ProgrammaticChange)
                    {
                        SetColor(e.R, e.G, e.B, A);

                        BuildColorInterface(false, s);
                    }
                };
            }

            foreach (IHSBControls control in HSBControls)
            {
                control.ColorChanged += (s, e) =>
                {
                    if (!ProgrammaticChange)
                    {
                        SetColorFromHSB(e.H, e.S, e.B);

                        BuildColorInterface(false, s);
                    }
                };
            }

            foreach (ICIELABControls control in CIELABControls)
            {
                control.ColorChanged += (s, e) =>
                {
                    if (!ProgrammaticChange)
                    {
                        SetColorFromLab(e.L, e.a, e.b);
                        BuildColorInterface(false, s);
                    }
                };
            }

            foreach (IAControl control in AControls)
            {
                control.AChanged += (s, e) =>
                {
                    if (!ProgrammaticChange)
                    {
                        this.A = e.A;
                        UpdateDependingOnAlpha(false, s);
                        this.Color = this.InternalColor;
                    }
                };
            }

            foreach (IHexControl control in HexControls)
            {
                control.TextChanged += (s, e) =>
                {
                    if (!ProgrammaticChange)
                    {
                        ProgrammaticChange = true;
                        try
                        {
                            string col = e.Text.Trim();
                            if (col?.StartsWith("#") == true)
                            {
                                col = col.Substring(1);
                            }

                            if (col?.Length == 6 || col?.Length == 8)
                            {
                                string _R = col.Substring(0, 2);
                                string _G = col.Substring(2, 2);
                                string _B = col.Substring(4, 2);

                                string _A = col.Length > 6 ? col.Substring(6, 2) : "FF";

                                byte r = byte.Parse(_R, NumberStyles.HexNumber);
                                byte g = byte.Parse(_G, NumberStyles.HexNumber);
                                byte b = byte.Parse(_B, NumberStyles.HexNumber);
                                byte a = byte.Parse(_A, NumberStyles.HexNumber);

                                SetColor(r, g, b, a);

                                BuildColorInterface(false, s);
                            }
                        }
                        catch
                        {

                        }
                        ProgrammaticChange = false;
                    }
                };
            }

            foreach (IColorBlindnessSelector control in ColorBlindnessSelectors)
            {
                control.ColorBlindnessModeChanged += (s, e) =>
                {
                    foreach (IPreviousColorPreview control2 in PreviousColorPreviews)
                    {
                        control2.ColorBlindnessMode = e.ColorBlindnessMode;
                    }

                    foreach (ICurrentColorPreview control2 in CurrentColorPreviews)
                    {
                        control2.ColorBlindnessMode = e.ColorBlindnessMode;
                    }

                    foreach (IColorBlindnessSelector control2 in ColorBlindnessSelectors)
                    {
                        control2.ColorBlindnessMode = e.ColorBlindnessMode;
                    }

                    foreach (IPaletteControl control2 in PaletteControls)
                    {
                        control2.ColorBlindnessMode = e.ColorBlindnessMode;
                    }
                };
            }

            foreach (IPaletteControl palette in PaletteControls)
            {
                palette.Owner = this;
                palette.ColorSelected += (s, e) =>
                {
                    this.InternalColor = e.Color;
                };
            }
        }

        /// <inheritdoc/>
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == ColorProperty)
            {
                Color newVal = change.GetNewValue<Color>();

                if (newVal != this.InternalColor)
                {
                    this.InternalColor = newVal;
                }
            }
            else if (change.Property == PreviousColorProperty)
            {
                foreach (IPreviousColorPreview control in PreviousColorPreviews)
                {
                    control.PreviousColor = this.PreviousColor ?? Colors.Transparent;
                    if (control is Control ctrl)
                    {
                        ctrl.IsVisible = this.PreviousColor != null;
                    }
                }
            }
            else if (change.Property == ColorSpaceProperty)
            {
                if (!programmaticChangeColorSpace)
                {
                    programmaticChangeColorSpace = true;

                    foreach (IColorCanvasControls control2 in this.ColorCanvasControls)
                    {
                        control2.ColorSpace = ColorSpace;
                    }

                    foreach (IColorSpaceSelector control2 in ColorSpaceSelectors)
                    {
                        control2.ColorSpace = ColorSpace;
                    }

                    foreach (IColorSpacePreview control2 in ColorSpacePreviews)
                    {
                        control2.ColorSpace = ColorSpace;
                        control2.UpdatePreview(true, WarningTooltip);
                    }

                    foreach (IColorComponentSelector control2 in ColorComponentSelectors)
                    {
                        control2.ColorSpace = ColorSpace;
                    }

                    programmaticChangeColorSpace = false;
                }
            }

        }

        private void BuildColorInterface(bool instantTransition, object sender)
        {
            ProgrammaticChange = true;

            foreach (IColorSpacePreview control in ColorSpacePreviews)
            {
                control.R = this.R;
                control.G = this.G;
                control.B = this.B;
                control.H = this.H;
                control.S = this.S;
                control.V = this.V;
                control.L = this.L;
                control.a = this.a;
                control.b = this.b;
                control.UpdatePreview(instantTransition, WarningTooltip);
            }

            foreach (IRGBControls control in RGBControls)
            {
                if (control != sender)
                {
                    control.R = this.R;
                    control.G = this.G;
                    control.B = this.B;
                }
            }

            foreach (IHSBControls control in HSBControls)
            {
                if (control != sender)
                {
                    control.H = this.H;
                    control.S = this.S;
                    control.B = this.V;
                }
            }

            foreach (ICIELABControls control in CIELABControls)
            {
                if (control != sender)
                {
                    control.L = this.L;
                    control.a = this.a;
                    control.b = this.b;
                }
            }

            ProgrammaticChange = false;

            UpdateDependingOnAlpha(instantTransition, sender);
        }

        private void UpdateDependingOnAlpha(bool instantTransition, object sender)
        {
            ProgrammaticChange = true;

            foreach (ICurrentColorPreview control in CurrentColorPreviews)
            {
                control.SetColor(this.InternalColor, instantTransition);
            }

            foreach (IColorCanvasControls control in this.ColorCanvasControls)
            {
                if (control != sender)
                {
                    control.R = this.R;
                    control.G = this.G;
                    control.B = this.B;
                    control.A = this.A;
                    control.H = this.H;
                    control.S = this.S;
                    control.V = this.V;
                    control.L = this.L;
                    control.a = this.a;
                    control.b = this.b;
                    control.UpdatePreview(instantTransition);
                }
            }

            foreach (IAControl control in AControls)
            {
                if (control != sender)
                {
                    control.A = this.A;
                }
            }

            string hexColor = R.ToString("X2");
            hexColor += G.ToString("X2");
            hexColor += B.ToString("X2");

            if (A < 255)
            {
                hexColor += A.ToString("X2");
            }

            foreach (IHexControl control in HexControls)
            {
                control.Text = hexColor;
            }

            ProgrammaticChange = false;
        }

        private bool ProgrammaticChange = false;

        private void SetColorFromHSB(byte H, byte S, byte V)
        {
            this.H = H;
            this.S = S;
            this.V = V;

            byte _R, _G, _B;
            HSB.HSVToRGB(H / 255.0, S / 255.0, V / 255.0, out _R, out _G, out _B);
            this.R = _R;
            this.G = _G;
            this.B = _B;

            (double _L, double _a, double _b) = Lab.ToLab(R, G, B);

            this.L = (byte)Math.Min(100, Math.Max(0, (_L * 100)));
            this.a = (int)Math.Min(100, Math.Max(-100, (_a * 100)));
            this.b = (int)Math.Min(100, Math.Max(-100, (_b * 100)));

            this.Color = this.InternalColor;
        }

        private void SetColorFromLab(byte L, int a, int b)
        {
            this.H = H;
            this.S = S;
            this.V = V;

            byte _R, _G, _B;
            Lab.FromLab(L / 100.0, a / 100.0, b / 100.0, out _R, out _G, out _B);
            this.R = _R;
            this.G = _G;
            this.B = _B;

            (double _H, double _S, double _V) = HSB.RGBToHSB(R / 255.0, G / 255.0, B / 255.0);

            this.H = (byte)Math.Min(255, Math.Max(0, (_H * 255)));
            this.S = (byte)Math.Min(255, Math.Max(0, (_S * 255)));
            this.V = (byte)Math.Min(255, Math.Max(0, (_V * 255)));

            this.Color = this.InternalColor;
        }
    }
}
