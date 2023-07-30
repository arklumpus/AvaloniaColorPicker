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
using System;

namespace AvaloniaColorPicker
{
    /// <summary>
    /// Represents a control providing a preview of the currently selected colour.
    /// </summary>
    public interface ICurrentColorPreview
    {
        /// <summary>
        /// The currently selected colour.
        /// </summary>
        Color CurrentColor { get; set; }

        /// <summary>
        /// The colour blindness simulation mode.
        /// </summary>
        ColorBlindnessModes ColorBlindnessMode { get; set; }

        /// <summary>
        /// Invoked when the user clicks on a colour.
        /// </summary>
        event EventHandler<ColorSelectedEventArgs> ColorSelected;

        /// <summary>
        /// Change the currently selected colour.
        /// </summary>
        /// <param name="color">The new colour.</param>
        /// <param name="instantTransition">Whether the change should happen instantly or not.</param>
        void SetColor(Color color, bool instantTransition);
    }

    /// <summary>
    /// Represents a control providing a preview of the previously selected colour.
    /// </summary>
    public interface IPreviousColorPreview
    {
        /// <summary>
        /// The previously selected colour.
        /// </summary>
        Color PreviousColor { get; set; }

        /// <summary>
        /// The colour blindness simulation mode.
        /// </summary>
        ColorBlindnessModes ColorBlindnessMode { get; set; }

        /// <summary>
        /// Invoked when the user clicks on a colour.
        /// </summary>
        event EventHandler<ColorSelectedEventArgs> ColorSelected;

        /// <summary>
        /// Change the previously selected colour.
        /// </summary>
        /// <param name="color">The new colour.</param>
        /// <param name="instantTransition">Whether the change should happen instantly or not.</param>
        void SetColor(Color color, bool instantTransition);
    }

    /// <summary>
    /// A control that provides a preview of the currently selected colour.
    /// </summary>
    public class CurrentColorPreview : ColorPreview, ICurrentColorPreview
    {
        /// <inheritdoc/>
        public Color CurrentColor
        {
            get
            {
                return Color;
            }

            set
            {
                Color = value;
            }
        }

        /// <summary>
        /// Create a new <see cref="CurrentColorPreview"/>.
        /// </summary>
        public CurrentColorPreview() : base(false) { }
    }

    /// <summary>
    /// A control that provides a preview of the previously selected colour.
    /// </summary>
    public class PreviousColorPreview : ColorPreview, IPreviousColorPreview
    {
        /// <inheritdoc/>
        public Color PreviousColor
        {
            get
            {
                return Color;
            }

            set
            {
                Color = value;
            }
        }

        /// <summary>
        /// Create a new <see cref="PreviousColorPreview"/>.
        /// </summary>
        public PreviousColorPreview() : base(true) { }
    }

    /// <summary>
    /// A control that provides a preview of a colour.
    /// </summary>
    public partial class ColorPreview : UserControl
    {
        /// <summary>
        /// Defines the <see cref="Color"/> property.
        /// </summary>
        public static readonly StyledProperty<Color> ColorProperty = AvaloniaProperty.Register<ColorPreview, Color>(nameof(Color), Color.FromRgb(0, 162, 232));

        /// <summary>
        /// The currently selected colour.
        /// </summary>
        public Color Color
        {
            get { return GetValue(ColorProperty); }
            set
            {
                SetValue(ColorProperty, value);
                UpdatePreview(false);
            }
        }

        /// <summary>
        /// Defines the <see cref="ColorBlindnessMode"/> property.
        /// </summary>
        public static readonly StyledProperty<ColorBlindnessModes> ColorBlindnessModeProperty = AvaloniaProperty.Register<ColorBlindnessSelector, ColorBlindnessModes>(nameof(ColorBlindnessMode), ColorBlindnessModes.Normal);

        /// <summary>
        /// Determines the colour-blindness preview mode.
        /// </summary>
        public ColorBlindnessModes ColorBlindnessMode
        {
            get { return GetValue(ColorBlindnessModeProperty); }
            set { SetValue(ColorBlindnessModeProperty, value); }
        }

        /// <summary>
        /// Invoked when the user clicks on a colour.
        /// </summary>
        public event EventHandler<ColorSelectedEventArgs> ColorSelected;

        /// <inheritdoc/>
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == ColorBlindnessModeProperty)
            {
                UpdatePreview(false);
            }
        }

        /// <summary>
        /// Change the colour.
        /// </summary>
        /// <param name="color">The new colour.</param>
        /// <param name="instantTransition">Whether the change should happen instantly or not.</param>
        public void SetColor(Color color, bool instantTransition)
        {
            SetValue(ColorProperty, color);
            UpdatePreview(instantTransition);
        }

        private void UpdatePreview(bool instantTransition)
        {
            Color contrasting = ColorPicker.GetContrastingColor(Color);
            Color light = ColorPicker.GetLighterColor(Color);
            Color dark = ColorPicker.GetDarkerColor(Color);

            Func<Color, Color> colourBlindnessFunction = ColorPicker.GetColourBlindnessFunction((int)ColorBlindnessMode);

            ColorVisualBrush.SetColor(this.FindControl<Path>("HexagonColor1"), colourBlindnessFunction(contrasting), instantTransition);
            this.FindControl<Path>("HexagonColor1").Tag = contrasting;
            ColorVisualBrush.SetColor(this.FindControl<Path>("HexagonColor2"), colourBlindnessFunction(Color), instantTransition);
            this.FindControl<Path>("HexagonColor2").Tag = Color;
            ColorVisualBrush.SetColor(this.FindControl<Path>("HexagonColor3"), colourBlindnessFunction(light), instantTransition);
            this.FindControl<Path>("HexagonColor3").Tag = light;
            ColorVisualBrush.SetColor(this.FindControl<Path>("HexagonColor4"), colourBlindnessFunction(dark), instantTransition);
            this.FindControl<Path>("HexagonColor4").Tag = dark;
        }

        private void HexagonPressed(object sender, PointerPressedEventArgs e)
        {
            Color col = (Color)((Path)sender).Tag;

            ColorSelected?.Invoke(this, new ColorSelectedEventArgs(col));
        }

        /// <summary>
        /// Create a new <see cref="ColorPreview"/>.
        /// </summary>
        /// <param name="canBeSelected">Determines whether the current colour (central hexagon) can be selected or not.</param>
        public ColorPreview(bool canBeSelected)
        {
            InitializeComponent();
            if (canBeSelected)
            {
                this.FindControl<Path>("HexagonColor2").Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand);
                this.FindControl<Path>("HexagonColor2").Stroke = Colours.BackgroundColour;

                this.FindControl<Path>("HexagonColor1").PointerPressed += HexagonPressed;
                this.FindControl<Path>("HexagonColor2").PointerPressed += HexagonPressed;
                this.FindControl<Path>("HexagonColor3").PointerPressed += HexagonPressed;
                this.FindControl<Path>("HexagonColor4").PointerPressed += HexagonPressed;
            }
            else
            {
                this.FindControl<Path>("HexagonColor1").PointerPressed += HexagonPressed;
                this.FindControl<Path>("HexagonColor3").PointerPressed += HexagonPressed;
                this.FindControl<Path>("HexagonColor4").PointerPressed += HexagonPressed;
            }


            UpdatePreview(true);
        }

        /// <summary>
        /// Create a new <see cref="ColorPreview"/>.
        /// </summary>
        public ColorPreview()
        {
            InitializeComponent();
            this.FindControl<Path>("HexagonColor2").Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand);
            this.FindControl<Path>("HexagonColor2").Stroke = Colours.BackgroundColour;

            this.FindControl<Path>("HexagonColor1").PointerPressed += HexagonPressed;
            this.FindControl<Path>("HexagonColor2").PointerPressed += HexagonPressed;
            this.FindControl<Path>("HexagonColor3").PointerPressed += HexagonPressed;
            this.FindControl<Path>("HexagonColor4").PointerPressed += HexagonPressed;
            UpdatePreview(true);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
