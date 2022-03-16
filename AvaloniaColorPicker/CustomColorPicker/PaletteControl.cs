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
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Immutable;

namespace AvaloniaColorPicker
{
    /// <summary>
    /// A control representing a colour palette.
    /// </summary>
    public class PaletteControl : UserControl, IPaletteControl
    {
        /// <inheritdoc/>
        public IColorPicker Owner { get; set; }

        /// <inheritdoc/>
        public event EventHandler<ColorSelectedEventArgs> ColorSelected;

        /// <summary>
        /// Invoked when the user adds a colour to the palette.
        /// </summary>
        public event EventHandler<ColorAddedRemovedEventArgs> ColorAdded;

        /// <summary>
        /// Invoked when the user removes a colour from the palette.
        /// </summary>
        public event EventHandler<ColorAddedRemovedEventArgs> ColorRemoved;

        private static readonly StyledProperty<Func<Color, Color>> ColourBlindnessFunctionProperty = AvaloniaProperty.Register<PaletteSelector, Func<Color, Color>>(nameof(ColourBlindnessFunction), col => col);

        private Func<Color, Color> ColourBlindnessFunction
        {
            get { return GetValue(ColourBlindnessFunctionProperty); }
            set { SetValue(ColourBlindnessFunctionProperty, value); }
        }

        /// <summary>
        /// Defines the <see cref="ColorBlindnessMode"/> property.
        /// </summary>
        public static readonly StyledProperty<ColorBlindnessModes> ColorBlindnessModeProperty = AvaloniaProperty.Register<ColorBlindnessSelector, ColorBlindnessModes>(nameof(ColorBlindnessMode), ColorBlindnessModes.Normal);

        /// <inheritdoc/>
        public ColorBlindnessModes ColorBlindnessMode
        {
            get { return GetValue(ColorBlindnessModeProperty); }
            set { SetValue(ColorBlindnessModeProperty, value); }
        }

        /// <summary>
        /// Defines the <see cref="Colors"/> property.
        /// </summary>
        public static readonly StyledProperty<ImmutableList<Color>> ColorsProperty = AvaloniaProperty.Register<PaletteControl, ImmutableList<Color>>(nameof(Colors), ImmutableList<Color>.Empty);

        /// <summary>
        /// The colours contained in the palette. Note that you cannot directly add, remove or change elements from this list;
        /// you always need to create a new <see cref="ImmutableList"/> and change the value of the property.
        /// </summary>
        public ImmutableList<Color> Colors
        {
            get { return GetValue(ColorsProperty); }
            set { SetValue(ColorsProperty, value); }
        }

        private bool ProgrammaticChange = false;

        /// <summary>
        /// Defines the <see cref="CanAddRemove"/> property.
        /// </summary>
        public static readonly StyledProperty<bool> CanAddRemoveProperty = AvaloniaProperty.Register<PaletteControl, bool>(nameof(CanAddRemove), true);

        /// <summary>
        /// Determines whether the user can add and remove colours from the palette.
        /// </summary>
        public bool CanAddRemove
        {
            get { return GetValue(CanAddRemoveProperty); }
            set { SetValue(CanAddRemoveProperty, value); }
        }

        private Grid ScrollerContainer { get; }

        /// <summary>
        /// Create a new <see cref="PaletteControl"/>.
        /// </summary>
        public PaletteControl()
        {
            SetStyles();

            Grid mainGrid = new Grid();
            mainGrid.RowDefinitions.Add(new RowDefinition(0, GridUnitType.Auto));

            ScrollViewer paletteScroller = new ScrollViewer() { HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto, VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled, HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left, Margin = new Thickness(5) };
            mainGrid.Children.Add(paletteScroller);

            ScrollerContainer = new Grid() { HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left };

            ScrollerContainer.Children.Add(BuildPaletteCanvas(Colors));

            paletteScroller.Content = ScrollerContainer;

            this.Content = mainGrid;
        }

        /// <inheritdoc/>
        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == ColorBlindnessModeProperty)
            {
                this.ColourBlindnessFunction = ColorPicker.GetColourBlindnessFunction((int)this.ColorBlindnessMode);
            }

            if (change.Property == ColourBlindnessFunctionProperty || (change.Property == ColorsProperty && !ProgrammaticChange))
            {
                ScrollerContainer.Children.Clear();
                ScrollerContainer.Children.Add(BuildPaletteCanvas(Colors));
            }
        }

        const double HexagonRadius = 24;

        private Canvas BuildPaletteCanvas(IReadOnlyList<Color> palette)
        {
            int n = palette.Count + 1;

            Canvas tbr = new Canvas() { Height = HexagonRadius * Math.Cos(Math.PI / 6) * 3, Width = HexagonRadius * (3 + 2.5 * (n - 1)) + 13.86, Margin = new Thickness(0, 0, 0, 5) };

            if (!ColorPicker.TransitionsDisabled)
            {
                tbr.Transitions = new Transitions
                {
                    new DoubleTransition() { Property = Canvas.WidthProperty, Duration = new TimeSpan(0, 0, 0, 0, 100) }
                };
            }

            List<PaletteColorInfo> infos = new List<PaletteColorInfo>();

            for (int i = 0; i < n - 1; i++)
            {
                AddColorHexagon(i, palette, tbr, this, infos, this.ColourBlindnessFunction);
            }

            if (CanAddRemove)
            {
                int i = n - 1;
                double centerX = (1.5 + 2.5 * i) * HexagonRadius + 6.93;
                double centerY = HexagonRadius * Math.Cos(Math.PI / 6) * (i % 2 == 0 ? 1 : 2);

                PathGeometry leftHexagon = GetHexagonPath(new Point(centerX - 0.5 * HexagonRadius, centerY), HexagonRadius);
                PathGeometry centerHexagon = GetHexagonPath(new Point(centerX, centerY), HexagonRadius);
                PathGeometry rightHexagon = GetHexagonPath(new Point(centerX + 0.5 * HexagonRadius, centerY), HexagonRadius);

                Avalonia.Controls.Shapes.Path leftPath = new Avalonia.Controls.Shapes.Path() { Data = leftHexagon, ZIndex = 2 };
                Avalonia.Controls.Shapes.Path rightPath = new Avalonia.Controls.Shapes.Path() { Data = rightHexagon, ZIndex = 2 };
                Avalonia.Controls.Shapes.Path centerPath = new Avalonia.Controls.Shapes.Path() { Data = centerHexagon, Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand), ZIndex = 2 };

                AnimatableColorBrush leftFill = new AnimatableColorBrush(Color.FromRgb(240, 240, 240), col => leftPath.Fill = new SolidColorBrush(col));
                AnimatableColorBrush rightFill = new AnimatableColorBrush(Color.FromRgb(240, 240, 240), col => rightPath.Fill = new SolidColorBrush(col));
                AnimatableColorBrush centerFill = new AnimatableColorBrush(Color.FromRgb(200, 200, 200), col => centerPath.Fill = new SolidColorBrush(col));

                tbr.Children.Add(leftPath);
                tbr.Children.Add(rightPath);
                tbr.Children.Add(centerPath);

                PathGeometry plusGeometry = new PathGeometry();
                PathFigure verticalPlusFigure = new PathFigure() { StartPoint = new Point(centerX, centerY - HexagonRadius * 0.5), IsClosed = false };
                verticalPlusFigure.Segments.Add(new LineSegment() { Point = new Point(centerX, centerY + HexagonRadius * 0.5) });
                PathFigure horizontalPlusFigure = new PathFigure() { StartPoint = new Point(centerX - HexagonRadius * 0.5, centerY), IsClosed = false };
                horizontalPlusFigure.Segments.Add(new LineSegment() { Point = new Point(centerX + HexagonRadius * 0.5, centerY) });
                plusGeometry.Figures.Add(verticalPlusFigure);
                plusGeometry.Figures.Add(horizontalPlusFigure);

                Avalonia.Controls.Shapes.Path plusPath = new Avalonia.Controls.Shapes.Path() { Data = plusGeometry, StrokeThickness = HexagonRadius * 0.2, IsHitTestVisible = false, ZIndex = 2 };
                AnimatableColorBrush plusStroke = new AnimatableColorBrush(Avalonia.Media.Colors.White, col => plusPath.Stroke = new SolidColorBrush(col));

                tbr.Children.Add(plusPath);

                AnimatableTransform transform = new AnimatableTransform(new Matrix(1, 0, 0, 1, 0, 0));

                leftPath.RenderTransform = transform.MatrixTransform;
                rightPath.RenderTransform = transform.MatrixTransform;
                centerPath.RenderTransform = transform.MatrixTransform;
                plusPath.RenderTransform = transform.MatrixTransform;

                bool disabled = false;

                centerPath.PointerEnter += (s, e) =>
                {
                    if (!disabled)
                    {
                        leftFill.Update(Color.FromRgb(180, 180, 180), false);
                        rightFill.Update(Color.FromRgb(180, 180, 180), false);
                        centerFill.Update(Color.FromRgb(240, 240, 240), false);
                        plusStroke.Update(Color.FromRgb(180, 180, 180), false);
                    }
                };
                centerPath.PointerLeave += (s, e) =>
                {
                    if (!disabled)
                    {
                        leftFill.Update(Color.FromRgb(240, 240, 240), false);
                        rightFill.Update(Color.FromRgb(240, 240, 240), false);
                        centerFill.Update(Color.FromRgb(200, 200, 200), false);
                        plusStroke.Update(Avalonia.Media.Colors.White, false);
                    }
                };

                centerPath.PointerPressed += async (s, e) =>
                {
                    int index = this.Colors.Count;

                    disabled = true;

                    Color col = this.Owner.Color;

                    leftFill.Update(ColorPicker.GetLighterColor(col), false);
                    rightFill.Update(ColorPicker.GetDarkerColor(col), false);
                    centerFill.Update(col, false);
                    plusStroke.Update(Color.FromArgb(0, 255, 255, 255), false);

                    await Task.Delay(100);

                    leftPath.ZIndex = -1;
                    centerPath.ZIndex = -1;
                    rightPath.ZIndex = -1;
                    plusPath.ZIndex = -1;

                    ProgrammaticChange = true;
                    this.Colors = this.Colors.Add(col);
                    palette = this.Colors;
                    ProgrammaticChange = false;

                    AddColorHexagon(palette.Count - 1, palette, tbr, this, infos, this.ColourBlindnessFunction);

                    leftFill.Update(Color.FromRgb(240, 240, 240), false);
                    rightFill.Update(Color.FromRgb(240, 240, 240), false);
                    centerFill.Update(Color.FromRgb(200, 200, 200), false);
                    plusStroke.Update(Avalonia.Media.Colors.White, false);

                    tbr.Width += HexagonRadius * 2.5;
                    double deltaX = HexagonRadius * 2.5;
                    double deltaY = HexagonRadius * Math.Sin(Math.PI / 3) * (palette.Count % 2 == 0 ? -1 : 1);
                    transform.Matrix = new Matrix(1, 0, 0, 1, transform.Matrix.M31 + deltaX, transform.Matrix.M32 + deltaY);

                    ColorAdded?.Invoke(this, new ColorAddedRemovedEventArgs(index, col));

                    await Task.Delay(100);

                    leftPath.ZIndex = 2;
                    centerPath.ZIndex = 2;
                    rightPath.ZIndex = 2;
                    plusPath.ZIndex = 2;

                    disabled = false;

                };

                infos.Add(new PaletteColorInfo() { Index = -1, Transform = transform });
            }


            return tbr;
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

        private static void AddColorHexagon(int i, IReadOnlyList<Color> palette, Canvas container, PaletteControl owner, List<PaletteColorInfo> infos, Func<Color, Color> colourBlindnessFunction)
        {
            // Needed to address https://github.com/AvaloniaUI/Avalonia/issues/6257
            Canvas myContainer = new Canvas() { Width = container.Width, Height = container.Height };

            AnimatableTransform transform = new AnimatableTransform(new Matrix(1, 0, 0, 1, 0, 0));

            PaletteColorInfo info = new PaletteColorInfo() { Index = i, OriginalIndex = i, Transform = transform };

            infos.Insert(i, info);

            double centerX = (1.5 + 2.5 * i) * HexagonRadius + 6.93;
            double centerY = HexagonRadius * Math.Cos(Math.PI / 6) * (i % 2 == 0 ? 1 : 2);

            PathGeometry leftHexagon = GetHexagonPath(new Point(centerX - 0.5 * HexagonRadius, centerY), HexagonRadius);
            PathGeometry centerHexagon = GetHexagonPath(new Point(centerX, centerY), HexagonRadius);
            PathGeometry rightHexagon = GetHexagonPath(new Point(centerX + 0.5 * HexagonRadius, centerY), HexagonRadius);


            Color color = palette[i % palette.Count];
            Color lighterColor = ColorPicker.GetLighterColor(color);
            Color darkerColor = ColorPicker.GetDarkerColor(color);


            Avalonia.Controls.Shapes.Path leftPath = new Avalonia.Controls.Shapes.Path() { Data = leftHexagon, Fill = new ColorVisualBrush(colourBlindnessFunction(lighterColor)), Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand) };
            Avalonia.Controls.Shapes.Path rightPath = new Avalonia.Controls.Shapes.Path() { Data = rightHexagon, Fill = new ColorVisualBrush(colourBlindnessFunction(darkerColor)), Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand) };
            Avalonia.Controls.Shapes.Path centerPath = new Avalonia.Controls.Shapes.Path() { Data = centerHexagon, Fill = new ColorVisualBrush(colourBlindnessFunction(color)), Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand) };

            if (!ColorPicker.TransitionsDisabled)
            {
                leftPath.Transitions = new Transitions
                {
                    new ThicknessTransition() { Property = Avalonia.Controls.Shapes.Path.MarginProperty, Duration = new TimeSpan(0, 0, 0, 0, 100) }
                };

                rightPath.Transitions = new Transitions
                {
                    new ThicknessTransition() { Property = Avalonia.Controls.Shapes.Path.MarginProperty, Duration = new TimeSpan(0, 0, 0, 0, 100) }
                };
            }

            leftPath.Classes.Add("HexagonLeftPalette");
            rightPath.Classes.Add("HexagonRightPalette");
            centerPath.Classes.Add("HexagonCenter");

            rightPath.PointerEnter += (s, e) =>
            {
                rightPath.ZIndex = 10;
                centerPath.ZIndex = 10;
                myContainer.ZIndex = 10;
            };

            rightPath.PointerLeave += async (s, e) =>
            {
                await Task.Delay(100);
                rightPath.ZIndex = 0;
                centerPath.ZIndex = 0;
                myContainer.ZIndex = 0;
            };

            leftPath.PointerPressed += (s, e) =>
            {
                owner.ColorSelected?.Invoke(owner, new ColorSelectedEventArgs(lighterColor));
            };

            rightPath.PointerPressed += (s, e) =>
            {
                owner.ColorSelected?.Invoke(owner, new ColorSelectedEventArgs(darkerColor));
            };

            centerPath.PointerPressed += (s, e) =>
            {
                owner.ColorSelected?.Invoke(owner, new ColorSelectedEventArgs(color));
            };

            Point p1, p2, p3, p4;

            if (i % 2 == 0)
            {
                p1 = new Point(centerX + HexagonRadius * Math.Cos(Math.PI * 2 / 3) - HexagonRadius * 0.5, centerY + HexagonRadius * Math.Sin(Math.PI * 2 / 3));
                p2 = new Point(centerX + HexagonRadius * Math.Cos(Math.PI * 1 / 3) + HexagonRadius * 0.5, centerY + HexagonRadius * Math.Sin(Math.PI * 1 / 3));
                p3 = p2 + new Point(-Math.Cos(Math.PI / 3), Math.Sin(Math.PI / 3)) * HexagonRadius * 0.6;
                p4 = p1 + new Point(Math.Cos(Math.PI / 3), Math.Sin(Math.PI / 3)) * HexagonRadius * 0.6;
            }
            else
            {
                p1 = new Point(centerX + HexagonRadius * Math.Cos(Math.PI * 4 / 3) - HexagonRadius * 0.5, centerY + HexagonRadius * Math.Sin(Math.PI * 4 / 3));
                p2 = new Point(centerX + HexagonRadius * Math.Cos(Math.PI * 5 / 3) + HexagonRadius * 0.5, centerY + HexagonRadius * Math.Sin(Math.PI * 5 / 3));
                p3 = p2 + new Point(-Math.Cos(Math.PI / 3), -Math.Sin(Math.PI / 3)) * HexagonRadius * 0.6;
                p4 = p1 + new Point(Math.Cos(Math.PI / 3), -Math.Sin(Math.PI / 3)) * HexagonRadius * 0.6;
            }


            AnimatablePath4Points deleteBG = new AnimatablePath4Points(p1, p2, p2, p1);

            AnimatableColorBrush bgFill = new AnimatableColorBrush(Color.FromRgb(180, 180, 180), col => deleteBG.Path.Fill = new SolidColorBrush(col));

            deleteBG.Path.Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand);

            PathGeometry deleteGeometry = new PathGeometry();
            PathFigure seg1 = new PathFigure() { StartPoint = new Point(centerX - HexagonRadius * 0.15, centerY - HexagonRadius * 0.15), IsClosed = false };
            seg1.Segments.Add(new LineSegment() { Point = seg1.StartPoint + new Point(HexagonRadius * 0.3, HexagonRadius * 0.3) });
            PathFigure seg2 = new PathFigure() { StartPoint = new Point(centerX + HexagonRadius * 0.15, centerY - HexagonRadius * 0.15), IsClosed = false };
            seg2.Segments.Add(new LineSegment() { Point = seg2.StartPoint + new Point(-HexagonRadius * 0.3, HexagonRadius * 0.3) });
            deleteGeometry.Figures.Add(seg1);
            deleteGeometry.Figures.Add(seg2);

            RelativePoint renderTransformOrigin = new RelativePoint(centerX, centerY, RelativeUnit.Absolute);


            leftPath.RenderTransform = transform.MatrixTransform;
            leftPath.RenderTransformOrigin = renderTransformOrigin;
            rightPath.RenderTransform = transform.MatrixTransform;
            rightPath.RenderTransformOrigin = renderTransformOrigin;
            centerPath.RenderTransform = transform.MatrixTransform;
            centerPath.RenderTransformOrigin = renderTransformOrigin;
            deleteBG.Path.RenderTransform = transform.MatrixTransform;
            deleteBG.Path.RenderTransformOrigin = renderTransformOrigin;
            AnimatableTransform fgTransform = new AnimatableTransform(new Matrix(1, 0, 0, 1, 0, (info.Index % 2 == 0 ? 1 : -1) * HexagonRadius * Math.Sin(Math.PI / 3) * 0.7));

            TransformGroup fgTransforms = new TransformGroup();
            fgTransforms.Children.Add(transform.MatrixTransform);
            fgTransforms.Children.Add(fgTransform.MatrixTransform);

            Avalonia.Controls.Shapes.Path deleteFG = new Avalonia.Controls.Shapes.Path() { Data = deleteGeometry, StrokeThickness = HexagonRadius * 0.1, IsHitTestVisible = false, RenderTransform = fgTransforms, RenderTransformOrigin = renderTransformOrigin };

            info.DeleteBGPath = deleteBG;
            info.DeleteFBTransform = fgTransform;

            AnimatableColorBrush fgStroke = new AnimatableColorBrush(Color.FromRgb(255, 255, 255), col => deleteFG.Stroke = new SolidColorBrush(col));

            deleteBG.Path.PointerEnter += (s, e) =>
            {
                bgFill.Update(Color.FromRgb(240, 240, 240), false);
                fgStroke.Update(Color.FromRgb(128, 128, 128), false);
            };

            deleteBG.Path.PointerLeave += (s, e) =>
            {
                bgFill.Update(Color.FromRgb(180, 180, 180), false);
                fgStroke.Update(Avalonia.Media.Colors.White, false);
            };

            if (owner.CanAddRemove)
            {
                myContainer.Children.Add(deleteBG.Path);
                myContainer.Children.Add(deleteFG);
            }

            bool deleteVisible = false;

            centerPath.PointerEnter += async (s, e) =>
            {
                if (!deleteVisible)
                {
                    double _centerX = (1.5 + 2.5 * info.OriginalIndex) * HexagonRadius + 6.93;
                    double _centerY = HexagonRadius * Math.Cos(Math.PI / 6) * (info.OriginalIndex % 2 == 0 ? 1 : 2);

                    Point _p1, _p2, _p3, _p4;

                    if (info.Index % 2 == 0)
                    {
                        _p1 = new Point(_centerX + HexagonRadius * Math.Cos(Math.PI * 2 / 3) - HexagonRadius * 0.5, _centerY + HexagonRadius * Math.Sin(Math.PI * 2 / 3));
                        _p2 = new Point(_centerX + HexagonRadius * Math.Cos(Math.PI * 1 / 3) + HexagonRadius * 0.5, _centerY + HexagonRadius * Math.Sin(Math.PI * 1 / 3));
                        _p3 = _p2 + new Point(-Math.Cos(Math.PI / 3), Math.Sin(Math.PI / 3)) * HexagonRadius * 0.6;
                        _p4 = _p1 + new Point(Math.Cos(Math.PI / 3), Math.Sin(Math.PI / 3)) * HexagonRadius * 0.6;
                    }
                    else
                    {
                        _p1 = new Point(_centerX + HexagonRadius * Math.Cos(Math.PI * 4 / 3) - HexagonRadius * 0.5, _centerY + HexagonRadius * Math.Sin(Math.PI * 4 / 3));
                        _p2 = new Point(_centerX + HexagonRadius * Math.Cos(Math.PI * 5 / 3) + HexagonRadius * 0.5, _centerY + HexagonRadius * Math.Sin(Math.PI * 5 / 3));
                        _p3 = _p2 + new Point(-Math.Cos(Math.PI / 3), -Math.Sin(Math.PI / 3)) * HexagonRadius * 0.6;
                        _p4 = _p1 + new Point(Math.Cos(Math.PI / 3), -Math.Sin(Math.PI / 3)) * HexagonRadius * 0.6;
                    }

                    deleteVisible = true;
                    deleteBG.Points = new PointCollection4(_p1, _p2, _p3, _p4);
                    fgTransform.Matrix = new Matrix(1, 0, 0, 1, 0, (info.Index % 2 == 0 ? 1 : -1) * HexagonRadius * Math.Sin(Math.PI / 3) * 1.3);
                    await Task.Delay(100);
                    deleteBG.Path.ZIndex = 11;
                    deleteFG.ZIndex = 11;
                    await Task.Delay(1000);
                    deleteBG.Path.ZIndex = 0;
                    deleteFG.ZIndex = 0;
                    myContainer.ZIndex = 0;
                    fgTransform.Matrix = new Matrix(1, 0, 0, 1, 0, (info.Index % 2 == 0 ? 1 : -1) * HexagonRadius * Math.Sin(Math.PI / 3) * 0.7);
                    deleteBG.Points = new PointCollection4(_p1, _p2, _p2, _p1);
                    deleteVisible = false;
                }
            };

            myContainer.Children.Add(leftPath);
            myContainer.Children.Add(rightPath);
            myContainer.Children.Add(centerPath);
            container.Children.Add(myContainer);


            deleteBG.Path.PointerPressed += async (s, e) =>
            {
                transform.Matrix = new Matrix(0, 0, 0, 0, transform.Matrix.M31, transform.Matrix.M32);

                for (int j = info.Index + 1; j < infos.Count; j++)
                {
                    infos[j].Transform.Matrix = new Matrix(1, 0, 0, 1, infos[j].Transform.Matrix.M31 - HexagonRadius * 2.5, infos[j].Transform.Matrix.M32 + HexagonRadius * Math.Sin(Math.PI / 3) * (j % 2 == 0 ? 1 : -1));
                    infos[j].Index--;

                    if (infos[j].DeleteBGPath != null)
                    {
                        double _centerX = (1.5 + 2.5 * infos[j].OriginalIndex) * HexagonRadius + 6.93;
                        double _centerY = HexagonRadius * Math.Cos(Math.PI / 6) * (infos[j].OriginalIndex % 2 == 0 ? 1 : 2);

                        Point _p1, _p2;

                        if (infos[j].Index % 2 == 0)
                        {
                            _p1 = new Point(_centerX + HexagonRadius * Math.Cos(Math.PI * 2 / 3) - HexagonRadius * 0.5, _centerY + HexagonRadius * Math.Sin(Math.PI * 2 / 3));
                            _p2 = new Point(_centerX + HexagonRadius * Math.Cos(Math.PI * 1 / 3) + HexagonRadius * 0.5, _centerY + HexagonRadius * Math.Sin(Math.PI * 1 / 3));
                        }
                        else
                        {
                            _p1 = new Point(_centerX + HexagonRadius * Math.Cos(Math.PI * 4 / 3) - HexagonRadius * 0.5, _centerY + HexagonRadius * Math.Sin(Math.PI * 4 / 3));
                            _p2 = new Point(_centerX + HexagonRadius * Math.Cos(Math.PI * 5 / 3) + HexagonRadius * 0.5, _centerY + HexagonRadius * Math.Sin(Math.PI * 5 / 3));
                        }

                        infos[j].DeleteBGPath.Points = new PointCollection4(_p1, _p2, _p2, _p1);
                        infos[j].DeleteFBTransform.Matrix = new Matrix(1, 0, 0, 1, 0, -infos[j].DeleteFBTransform.Matrix.M32);
                    }
                }

                Color col = owner.Colors[info.Index];

                owner.ProgrammaticChange = true;
                owner.Colors = owner.Colors.RemoveAt(info.Index);
                owner.ProgrammaticChange = false;

                owner.ColorRemoved?.Invoke(owner, new ColorAddedRemovedEventArgs(info.Index, col));

                await Task.Delay(100);
                container.Width -= HexagonRadius * 2.5;

                infos.RemoveAt(info.Index);

                container.Children.Remove(myContainer);
            };
        }

        private void SetStyles()
        {
            Style HexagonLeftPalette = new Style(x => x.OfType<Avalonia.Controls.Shapes.Path>().Class("HexagonLeftPalette"));
            HexagonLeftPalette.Setters.Add(new Setter(Avalonia.Controls.Shapes.Path.MarginProperty, new Avalonia.Thickness(0)));
            this.Styles.Add(HexagonLeftPalette);

            Style HexagonRightPalette = new Style(x => x.OfType<Avalonia.Controls.Shapes.Path>().Class("HexagonRightPalette"));
            HexagonRightPalette.Setters.Add(new Setter(Avalonia.Controls.Shapes.Path.MarginProperty, new Avalonia.Thickness(0)));
            this.Styles.Add(HexagonRightPalette);

            Style HexagonLeftPaletteOver = new Style(x => x.OfType<Avalonia.Controls.Shapes.Path>().Class("HexagonLeftPalette").Class(":pointerover"));
            HexagonLeftPaletteOver.Setters.Add(new Setter(Avalonia.Controls.Shapes.Path.MarginProperty, new Avalonia.Thickness(-6.93, 0, 0, 0)));
            this.Styles.Add(HexagonLeftPaletteOver);

            Style HexagonRightPaletteOver = new Style(x => x.OfType<Avalonia.Controls.Shapes.Path>().Class("HexagonRightPalette").Class(":pointerover"));
            HexagonRightPaletteOver.Setters.Add(new Setter(Avalonia.Controls.Shapes.Path.MarginProperty, new Avalonia.Thickness(6.93, 0, 0, 0)));
            this.Styles.Add(HexagonRightPaletteOver);
        }
    }

    /// <summary>
    /// <see cref="EventArgs"/> for the <see cref="PaletteControl.ColorAdded"/> and <see cref="PaletteControl.ColorRemoved"/> events.
    /// </summary>
    public class ColorAddedRemovedEventArgs : EventArgs
    {
        /// <summary>
        /// The index at which the colour was added or removed.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// The colour that was added or removed.
        /// </summary>
        public Color Color { get; }

        /// <summary>
        /// Create a new <see cref="ColorAddedRemovedEventArgs"/> instance.
        /// </summary>
        /// <param name="index">The index at which the colour was added or removed.</param>
        /// <param name="color">The colour that was added or removed.</param>
        public ColorAddedRemovedEventArgs(int index, Color color)
        {
            this.Index = index;
            this.Color = color;
        }
    }
}

