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

using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Media.Transformation;
using Avalonia.Styling;
using System;
using System.Threading.Tasks;

namespace AvaloniaColorPicker
{
    internal class MyToggleButton : ToggleButton, IStyleable
    {
        Type IStyleable.StyleKey => typeof(ToggleButton);

        public void Unpress()
        {
            this.PseudoClasses.Remove(":pressed");
        }
    }

    /// <summary>
    /// A control that can be used to select a <see cref="Avalonia.Media.Color"/>.
    /// </summary>
    public class ColorButton : ColorButton<ColorPickerWindow> { }

    /// <summary>
    /// A control that can be used to select a <see cref="Avalonia.Media.Color"/>.
    /// </summary>
    public class ColorButton<T> : UserControl where T : Window, IColorPickerWindow
    {
        /// <summary>
        /// Defines the <see cref="Color"/> property.
        /// </summary>
        public static readonly StyledProperty<Color> ColorProperty = AvaloniaProperty.Register<ColorPicker, Color>(nameof(Color), Color.FromArgb(255, 0, 0, 0));

        /// <summary>
        /// The <see cref="Avalonia.Media.Color"/> that is currently selected.
        /// </summary>
        public Color Color
        {
            get { return GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        private Grid PaletteContainer { get; }

        private ColorVisualBrush ColorBrush { get; }

        private MyToggleButton ContentButton { get; }

        private static T CreateColourPickerWindow(Color? previousColour)
        {
            return (T)typeof(T).GetConstructor(new Type[] { typeof(Color?) }).Invoke(new object[] { previousColour });
        }

        /// <summary>
        /// Creates a new <see cref="ColorButton{T}"/> instance.
        /// </summary>
        public ColorButton()
        {
            SetStyles();

            ContentButton = new MyToggleButton();

            ContentButton.Classes.Add("ContentButton");
            ContentButton.Padding = new Thickness(4, 4, 0, 4);

            Grid mainGrid = new Grid();
            ContentButton.Content = mainGrid;

            mainGrid.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition(16, GridUnitType.Pixel));

            Border colorBorder = new Border() { MinHeight = 16, MinWidth = 24, BorderThickness = new Avalonia.Thickness(1), BorderBrush = Colours.ForegroundColour, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch };

            mainGrid.Children.Add(colorBorder);

            ColorBrush = new ColorVisualBrush(Color);

            colorBorder.Background = ColorBrush;

            Canvas arrowCanvas = new Canvas() { Width = 16, Height = 16 };
            arrowCanvas.Classes.Add("ArrowCanvas");
            Grid.SetColumn(arrowCanvas, 1);

            PathGeometry arrowGeometry = new PathGeometry();
            PathFigure arrowFigure = new PathFigure() { StartPoint = new Avalonia.Point(4, 6), IsClosed = true, IsFilled = true };
            arrowFigure.Segments.Add(new LineSegment() { Point = new Avalonia.Point(8, 10) });
            arrowFigure.Segments.Add(new LineSegment() { Point = new Avalonia.Point(12, 6) });
            arrowGeometry.Figures.Add(arrowFigure);
            Path arrowPath = new Path() { Data = arrowGeometry };
            arrowCanvas.Children.Add(arrowPath);
            mainGrid.Children.Add(arrowCanvas);

            arrowCanvas.RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative);

            Popup palettePopup = new Popup
            {
                Placement = PlacementMode.AnchorAndGravity,
                PlacementGravity = Avalonia.Controls.Primitives.PopupPositioning.PopupGravity.Bottom,
                PlacementAnchor = Avalonia.Controls.Primitives.PopupPositioning.PopupAnchor.Bottom,
                PlacementConstraintAdjustment = Avalonia.Controls.Primitives.PopupPositioning.PopupPositionerConstraintAdjustment.FlipY,
                PlacementTarget = ContentButton
            };
            mainGrid.Children.Add(palettePopup);

            Border paletteBorder = new Border() { BorderThickness = new Thickness(1), BorderBrush = Colours.BorderLowColour, Background = Colours.BackgroundColour };
            palettePopup.Child = paletteBorder;

            Grid paletteGrid = new Grid() { Margin = new Thickness(5) };
            paletteGrid.RowDefinitions.Add(new RowDefinition(0, GridUnitType.Auto));
            paletteGrid.RowDefinitions.Add(new RowDefinition(0, GridUnitType.Auto));

            Button openColorPickerButton = new Button() { Content = "More colours...", HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center, FontFamily = this.FontFamily, FontSize = this.FontSize };
            Grid.SetRow(openColorPickerButton, 1);
            paletteGrid.Children.Add(openColorPickerButton);

            PaletteContainer = new Grid() { Margin = new Thickness(0, 0, 0, 5) };

            paletteGrid.Children.Add(PaletteContainer);

            openColorPickerButton.Click += async (s, e) =>
            {
                palettePopup.Close();
                T win = CreateColourPickerWindow(Color);
                win.FontFamily = this.FontFamily;
                win.FontSize = this.FontSize;
                Color? newCol = await ((IColorPickerWindow)win).ShowDialog(this.FindLogicalAncestorOfType<Window>());
                if (newCol != null)
                {
                    this.Color = newCol.Value;
                }
                ContentButton.IsChecked = false;
            };

            paletteBorder.Child = paletteGrid;

            ContentButton.PropertyChanged += async (s, e) =>
            {
                if (e.Property == ToggleButton.IsCheckedProperty)
                {
                    if ((e.NewValue as bool?).Value == true)
                    {
                        if (Palette.CurrentPalette != null && Palette.CurrentPalette.Colors.Count > 0)
                        {
                            PaletteContainer.Children.Clear();
                            PaletteContainer.Children.Add(GetPaletteCanvas(Palette.CurrentPalette));
                            palettePopup.Open();
                        }
                        else
                        {
                            T win = CreateColourPickerWindow(Color);
                            win.FontFamily = this.FontFamily;
                            win.FontSize = this.FontSize;
                            Color? newCol = await ((IColorPickerWindow)win).ShowDialog(this.FindLogicalAncestorOfType<Window>());
                            if (newCol != null)
                            {
                                this.Color = newCol.Value;
                            }
                            ContentButton.IsChecked = false;
                        }
                    }
                    else
                    {
                        palettePopup.Close();
                    }
                }
            };

            this.Content = ContentButton;
        }

        /// <inheritdoc/>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            IInputElement element = FocusManager.Instance.Current;

            if (!HasParent(element as Control, ContentButton))
            {
                ContentButton.IsChecked = false;
            }
        }

        private static bool HasParent(Control child, Control parent)
        {
            while (child != null)
            {
                if (child == parent)
                {
                    return true;
                }

                child = child.Parent as Control;
            }

            return false;
        }

        private void SetStyles()
        {
            Style arrowColor = new Style(x => x.OfType<ToggleButton>().Class("ContentButton").Child().OfType<Grid>().Child().OfType<Canvas>().Class("ArrowCanvas").Child().OfType<Path>());
            arrowColor.Setters.Add(new Setter(Path.FillProperty, Colours.ForegroundColour));
            this.Styles.Add(arrowColor);

            Style arrowColorPressed = new Style(x => x.OfType<ToggleButton>().Class("ContentButton").Class(":pressed").Child().OfType<Grid>().Child().OfType<Canvas>().Class("ArrowCanvas").Child().OfType<Path>());
            arrowColorPressed.Setters.Add(new Setter(Path.FillProperty, Colours.ControlHighlightLowColour));
            this.Styles.Add(arrowColorPressed);

            Style arrowColorActive = new Style(x => x.OfType<ToggleButton>().Class("ContentButton").Class(":checked").Child().OfType<Grid>().Child().OfType<Canvas>().Class("ArrowCanvas").Child().OfType<Path>());
            arrowColorActive.Setters.Add(new Setter(Path.FillProperty, Colours.ControlHighlightLowColour));
            this.Styles.Add(arrowColorActive);

            Style canvasRotationActive = new Style(x => x.OfType<ToggleButton>().Class("ContentButton").Class(":checked").Child().OfType<Grid>().Child().OfType<Canvas>().Class("ArrowCanvas"));
            canvasRotationActive.Setters.Add(new Setter(Canvas.RenderTransformProperty, new RotateTransform(180)));
            this.Styles.Add(canvasRotationActive);

            if (!ColorPicker.TransitionsDisabled)
            {
                Transitions transformTransitions = new Transitions
                {
                    new TransformOperationsTransition() { Property = Path.RenderTransformProperty, Duration = new TimeSpan(0, 0, 0, 0, 100) }
                };

                Transitions strokeTransitions = new Transitions
                {
                    new DoubleTransition() { Property = Path.StrokeThicknessProperty, Duration = new TimeSpan(0, 0, 0, 0, 100) }
                };

                Style HexagonLeft = new Style(x => x.OfType<Path>().Class("HexagonLeftButton"));
                HexagonLeft.Setters.Add(new Setter(Path.TransitionsProperty, transformTransitions));
                HexagonLeft.Setters.Add(new Setter(Path.RenderTransformProperty, TransformOperations.Parse("none")));
                this.Styles.Add(HexagonLeft);

                Style HexagonRight = new Style(x => x.OfType<Path>().Class("HexagonRightButton"));
                HexagonRight.Setters.Add(new Setter(Path.TransitionsProperty, transformTransitions));
                HexagonRight.Setters.Add(new Setter(Path.RenderTransformProperty, TransformOperations.Parse("none")));
                this.Styles.Add(HexagonRight);

                Style HexagonCenter = new Style(x => x.OfType<Path>().Class("HexagonCenterButton"));
                HexagonCenter.Setters.Add(new Setter(Path.StrokeProperty, Colours.BackgroundColour));
                HexagonCenter.Setters.Add(new Setter(Path.StrokeThicknessProperty, 0.0));
                HexagonCenter.Setters.Add(new Setter(Path.TransitionsProperty, strokeTransitions));
                this.Styles.Add(HexagonCenter);
            }

            Style HexagonLeftOver = new Style(x => x.OfType<Path>().Class("HexagonLeftButton").Class(":pointerover"));
            HexagonLeftOver.Setters.Add(new Setter(Path.RenderTransformProperty, TransformOperations.Parse("translate(-4.33px, 0)")));
            this.Styles.Add(HexagonLeftOver);

            Style HexagonRightOver = new Style(x => x.OfType<Path>().Class("HexagonRightButton").Class(":pointerover"));
            HexagonRightOver.Setters.Add(new Setter(Path.RenderTransformProperty, TransformOperations.Parse("translate(4.33px, 0)")));
            this.Styles.Add(HexagonRightOver);

            Style HexagonCenterOver = new Style(x => x.OfType<Path>().Class("HexagonCenterButton").Class(":pointerover"));
            HexagonCenterOver.Setters.Add(new Setter(Path.StrokeThicknessProperty, 3.0));
            this.Styles.Add(HexagonCenterOver);

            Style rightOverBlurring = new Style(x => x.OfType<Path>().Class("rightOverBlurring"));
            rightOverBlurring.Setters.Add(new Setter(Path.ZIndexProperty, 9));
            this.Styles.Add(rightOverBlurring);

            Style leftOverBlurring = new Style(x => x.OfType<Path>().Class("leftOverBlurring"));
            leftOverBlurring.Setters.Add(new Setter(Path.ZIndexProperty, 9));
            this.Styles.Add(leftOverBlurring);

            Style centerOverBlurring = new Style(x => x.OfType<Path>().Class("centerOverBlurring"));
            centerOverBlurring.Setters.Add(new Setter(Path.ZIndexProperty, 9));
            this.Styles.Add(centerOverBlurring);

            Style rightOver = new Style(x => x.OfType<Path>().Class("rightOver"));
            rightOver.Setters.Add(new Setter(Path.ZIndexProperty, 10));
            this.Styles.Add(rightOver);

            Style leftOver = new Style(x => x.OfType<Path>().Class("leftOver"));
            leftOver.Setters.Add(new Setter(Path.ZIndexProperty, 10));
            this.Styles.Add(leftOver);

            Style centerOver = new Style(x => x.OfType<Path>().Class("centerOver"));
            centerOver.Setters.Add(new Setter(Path.ZIndexProperty, 10));
            this.Styles.Add(centerOver);


            Style rightOverBlurringCanvas = new Style(x => x.OfType<Canvas>().Class("rightOverBlurring"));
            rightOverBlurringCanvas.Setters.Add(new Setter(Canvas.ZIndexProperty, 9));
            this.Styles.Add(rightOverBlurringCanvas);

            Style leftOverBlurringCanvas = new Style(x => x.OfType<Canvas>().Class("leftOverBlurring"));
            leftOverBlurringCanvas.Setters.Add(new Setter(Canvas.ZIndexProperty, 9));
            this.Styles.Add(leftOverBlurringCanvas);

            Style centerOverBlurringCanvas = new Style(x => x.OfType<Canvas>().Class("centerOverBlurring"));
            centerOverBlurringCanvas.Setters.Add(new Setter(Canvas.ZIndexProperty, 9));
            this.Styles.Add(centerOverBlurringCanvas);

            Style rightOverCanvas = new Style(x => x.OfType<Canvas>().Class("rightOver"));
            rightOverCanvas.Setters.Add(new Setter(Canvas.ZIndexProperty, 10));
            this.Styles.Add(rightOverCanvas);

            Style leftOverCanvas = new Style(x => x.OfType<Canvas>().Class("leftOver"));
            leftOverCanvas.Setters.Add(new Setter(Canvas.ZIndexProperty, 10));
            this.Styles.Add(leftOverCanvas);

            Style centerOverCanvas = new Style(x => x.OfType<Canvas>().Class("centerOver"));
            centerOverCanvas.Setters.Add(new Setter(Canvas.ZIndexProperty, 10));
            this.Styles.Add(centerOverCanvas);
        }

        /// <inheritdoc/>
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == ColorProperty)
            {
                Color col = (change.NewValue as Color?).Value;
                ColorBrush.Color = col;
            }
        }

        private const double HexagonRadius = 20;
        private const int MaxColumns = 3;

        private Canvas GetPaletteCanvas(Palette palette)
        {
            Canvas can = new Canvas
            {
                Width = ((Math.Min(MaxColumns, palette.Colors.Count) - 1) * 2.5 + 3) * HexagonRadius + 8.66,
                Height = ((Math.Ceiling((double)palette.Colors.Count / MaxColumns) - 1) * 2 + 3) * HexagonRadius * Math.Sin(Math.PI / 3)
            };

            if (palette.Colors.Count % MaxColumns == 1)
            {
                can.Height -= HexagonRadius * Math.Sin(Math.PI / 3);
            }

            for (int i = 0; i < palette.Colors.Count; i++)
            {
                AddColorHexagon(i, palette, can);
            }

            return can;
        }


        private void AddColorHexagon(int i, Palette palette, Canvas container)
        {
            // Necessary to address https://github.com/AvaloniaUI/Avalonia/issues/6257
            Canvas myContainer = new Canvas() { Width = container.Width, Height = container.Height };

            int rowInd = i / MaxColumns;
            int colInd = i % MaxColumns;

            double centerX;
            double centerY;

            if (i == 0 && palette.Colors.Count == 1)
            {
                centerX = HexagonRadius * (1.5 + colInd * 2.5);
                centerY = HexagonRadius * Math.Sin(Math.PI / 3) * (2 + 2 * rowInd - 1);
            }
            else if (colInd == 0)
            {
                centerX = HexagonRadius * (1.5 + 2.5);
                centerY = HexagonRadius * Math.Sin(Math.PI / 3) * (2 + 2 * rowInd - 1);
            }
            else if (colInd == 1)
            {
                centerX = HexagonRadius * 1.5;
                centerY = HexagonRadius * Math.Sin(Math.PI / 3) * (2 + 2 * rowInd);
            }
            else
            {
                centerX = HexagonRadius * (1.5 + colInd * 2.5);
                centerY = HexagonRadius * Math.Sin(Math.PI / 3) * (2 + 2 * rowInd - colInd % 2);
            }

            centerX += 4.33;

            PathGeometry leftHexagon = GetHexagonPath(new Point(centerX - 0.5 * HexagonRadius, centerY), HexagonRadius);
            PathGeometry centerHexagon = GetHexagonPath(new Point(centerX, centerY), HexagonRadius);
            PathGeometry rightHexagon = GetHexagonPath(new Point(centerX + 0.5 * HexagonRadius, centerY), HexagonRadius);


            Color color = palette.Colors[i % palette.Colors.Count];
            Color lighterColor = ColorPicker.GetLighterColor(color);
            Color darkerColor = ColorPicker.GetDarkerColor(color);


            Avalonia.Controls.Shapes.Path leftPath = new Avalonia.Controls.Shapes.Path() { Data = leftHexagon, Fill = new ColorVisualBrush(lighterColor), Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand) };
            Avalonia.Controls.Shapes.Path rightPath = new Avalonia.Controls.Shapes.Path() { Data = rightHexagon, Fill = new ColorVisualBrush(darkerColor), Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand) };
            Avalonia.Controls.Shapes.Path centerPath = new Avalonia.Controls.Shapes.Path() { Data = centerHexagon, Fill = new ColorVisualBrush(color), Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand) };

            leftPath.Classes.Add("HexagonLeftButton");
            rightPath.Classes.Add("HexagonRightButton");
            centerPath.Classes.Add("HexagonCenterButton");

            rightPath.PointerEntered += (s, e) =>
            {
                rightPath.Classes.Add("rightOver");
                centerPath.Classes.Add("rightOver");
                leftPath.Classes.Add("rightOver");
                myContainer.Classes.Add("rightOver");
            };

            rightPath.PointerExited += async (s, e) =>
            {
                rightPath.Classes.Add("rightOverBlurring");
                centerPath.Classes.Add("rightOverBlurring");
                leftPath.Classes.Add("rightOverBlurring");
                myContainer.Classes.Add("rightOverBlurring");
                rightPath.Classes.Remove("rightOver");
                centerPath.Classes.Remove("rightOver");
                leftPath.Classes.Remove("rightOver");
                myContainer.Classes.Remove("rightOver");

                await Task.Delay(100);
                rightPath.Classes.Remove("rightOverBlurring");
                centerPath.Classes.Remove("rightOverBlurring");
                leftPath.Classes.Remove("rightOverBlurring");
                myContainer.Classes.Remove("rightOverBlurring");
            };

            leftPath.PointerEntered += (s, e) =>
            {
                rightPath.Classes.Add("leftOver");
                centerPath.Classes.Add("leftOver");
                leftPath.Classes.Add("leftOver");
                myContainer.Classes.Add("leftOver");
            };

            leftPath.PointerExited += async (s, e) =>
            {
                rightPath.Classes.Add("leftOverBlurring");
                centerPath.Classes.Add("leftOverBlurring");
                leftPath.Classes.Add("leftOverBlurring");
                myContainer.Classes.Add("leftOverBlurring");
                rightPath.Classes.Remove("leftOver");
                centerPath.Classes.Remove("leftOver");
                leftPath.Classes.Remove("leftOver");
                myContainer.Classes.Remove("leftOver");

                await Task.Delay(100);
                rightPath.Classes.Remove("leftOverBlurring");
                centerPath.Classes.Remove("leftOverBlurring");
                leftPath.Classes.Remove("leftOverBlurring");
                myContainer.Classes.Remove("leftOverBlurring");
            };

            centerPath.PointerEntered += (s, e) =>
            {
                rightPath.Classes.Add("centerOver");
                centerPath.Classes.Add("centerOver");
                leftPath.Classes.Add("centerOver");
                myContainer.Classes.Add("centerOver");
            };

            centerPath.PointerExited += async (s, e) =>
            {
                rightPath.Classes.Add("centerOverBlurring");
                centerPath.Classes.Add("centerOverBlurring");
                leftPath.Classes.Add("centerOverBlurring");
                myContainer.Classes.Add("centerOverBlurring");
                rightPath.Classes.Remove("centerOver");
                centerPath.Classes.Remove("centerOver");
                leftPath.Classes.Remove("centerOver");
                myContainer.Classes.Remove("centerOver");

                await Task.Delay(100);
                rightPath.Classes.Remove("centerOverBlurring");
                centerPath.Classes.Remove("centerOverBlurring");
                leftPath.Classes.Remove("centerOverBlurring");
                myContainer.Classes.Remove("centerOverBlurring");
            };

            leftPath.PointerPressed += async (s, e) =>
            {
                this.Color = lighterColor;
                ContentButton.IsChecked = false;

                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    ContentButton.Unpress();
                });
            };

            rightPath.PointerPressed += async (s, e) =>
            {
                this.Color = darkerColor;
                ContentButton.IsChecked = false;

                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    ContentButton.Unpress();
                });
            };

            centerPath.PointerPressed += async (s, e) =>
            {
                this.Color = color;
                ContentButton.IsChecked = false;

                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    ContentButton.Unpress();
                });
            };

            RelativePoint renderTransformOrigin = new RelativePoint(centerX, centerY, RelativeUnit.Absolute);

            leftPath.RenderTransformOrigin = renderTransformOrigin;
            rightPath.RenderTransformOrigin = renderTransformOrigin;
            centerPath.RenderTransformOrigin = renderTransformOrigin;

            myContainer.Children.Add(leftPath);
            myContainer.Children.Add(rightPath);
            myContainer.Children.Add(centerPath);

            container.Children.Add(myContainer);
        }

        private static PathGeometry GetHexagonPath(Point center, double radius)
        {
            PathFigure hexagonFigure = new PathFigure() { StartPoint = center + new Point(radius, 0), IsClosed = true, IsFilled = true };

            for (int i = 1; i < 6; i++)
            {
                hexagonFigure.Segments.Add(new LineSegment() { Point = center + new Point(radius * Math.Cos(Math.PI * i / 3.0), radius * Math.Sin(Math.PI * i / 3.0)) });
            }

            PathGeometry geometry = new PathGeometry();
            geometry.Figures.Add(hexagonFigure);
            return geometry;
        }
    }
}
