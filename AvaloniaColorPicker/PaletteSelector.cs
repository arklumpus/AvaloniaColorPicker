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
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Styling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AvaloniaColorPicker
{
    /// <summary>
    /// Represents a control displaying a colour palette.
    /// </summary>
    public interface IPaletteControl
    {
        /// <summary>
        /// Invoked when a colour is selected from the palette.
        /// </summary>
        event EventHandler<ColorSelectedEventArgs> ColorSelected;

        /// <summary>
        /// The <see cref="IColorPicker"/> to which the <see cref="IPaletteControl"/> belongs.
        /// </summary>
        IColorPicker Owner { get; set; }

        /// <summary>
        /// The colour blindness simulation mode.
        /// </summary>
        ColorBlindnessModes ColorBlindnessMode { get; set; }
    }

    /// <summary>
    /// A control displaying a collection of pre-set and user defined palettes. These cannot be edited programmatically and are shared between all programs using this library.
    /// </summary>
    public class PaletteSelector : UserControl, IPaletteControl
    {
        private const string PaletteID = "e88b36a3734141738b6f8a5b605fb6f9";

        private static List<Palette> Palettes { get; }

        private static int DefaultIndex { get; } = 0;

        internal static string PaletteDirectory { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), PaletteID, "ColorPicker");

        /// <inheritdoc/>
        public IColorPicker Owner { get; set; }

        static PaletteSelector()
        {
            bool createPalettes = !Directory.Exists(PaletteDirectory) || Directory.GetFiles(PaletteDirectory, "*.palette").Length == 0;

            if (createPalettes)
            {
                ColorPicker.ResetDefaultPalettes();
            }
            // Replace an unmodified Wong palette file with the OkabeIto palette file (issue #9).
            else if (File.Exists(Path.Combine(PaletteDirectory, "Wong.palette")) && new FileInfo(Path.Combine(PaletteDirectory, "Wong.palette")).Length == 186)
            {
                try
                {
                    File.Delete(Path.Combine(PaletteDirectory, "Wong.palette"));
                }
                catch { }

                try
                {
                    using (System.IO.Stream resourceStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("AvaloniaColorPicker.Palettes.OkabeIto.palette"))
                    using (System.IO.Stream fileStream = System.IO.File.Create(System.IO.Path.Combine(PaletteSelector.PaletteDirectory, "OkabeIto.palette")))
                    {
                        resourceStream.CopyTo(fileStream);
                    }
                }
                catch { }
            }

            string[] paletteFiles = Directory.GetFiles(PaletteDirectory, "*.palette");

            Palettes = new List<Palette>();

            for (int i = 0; i < paletteFiles.Length; i++)
            {
                if (Path.GetFileName(paletteFiles[i]) == "OkabeIto.palette")
                {
                    DefaultIndex = i;
                }

                string[] lines = File.ReadAllLines(paletteFiles[i]);

                string name = lines[0].Substring(1);
                string description = lines[1].Substring(1);

                System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("#.*");

                try
                {
                    Palette palette = new Palette(name, description, paletteFiles[i], (from el in lines let line = reg.Replace(el, "") where !string.IsNullOrWhiteSpace(line) select ColorFromBytes((from el2 in line.Split(',') select byte.Parse(el2.Trim())).ToArray())).ToArray());
                    Palettes.Add(palette);
                }
                catch { }
            }
        }

        private static Color ColorFromBytes(params byte[] bytes)
        {
            if (bytes.Length == 3)
            {
                return Color.FromRgb(bytes[0], bytes[1], bytes[2]);
            }
            else if (bytes.Length == 4)
            {
                return Color.FromArgb(bytes[3], bytes[0], bytes[1], bytes[2]);
            }
            else
            {
                throw new ArgumentException("The colour should be expressed in R,G,B or R,G,B,A format!");
            }
        }

        private ComboBox PaletteSelectorBox { get; }

        /// <inheritdoc/>
        public event EventHandler<ColorSelectedEventArgs> ColorSelected;

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

        private static readonly StyledProperty<Func<Color, Color>> ColourBlindnessFunctionProperty = AvaloniaProperty.Register<PaletteSelector, Func<Color, Color>>(nameof(ColourBlindnessFunction), col => col);

        private Func<Color, Color> ColourBlindnessFunction
        {
            get { return GetValue(ColourBlindnessFunctionProperty); }
            set { SetValue(ColourBlindnessFunctionProperty, value); }
        }

        private Grid ScrollerContainer { get; }
        private TextBlock PaletteDescription { get; }

        /// <summary>
        /// Create a new <see cref="PaletteSelector"/>.
        /// </summary>
        public PaletteSelector()
        {
            SetStyles();

            Grid mainGrid = new Grid();
            mainGrid.RowDefinitions.Add(new RowDefinition(0, GridUnitType.Auto));
            mainGrid.RowDefinitions.Add(new RowDefinition(0, GridUnitType.Auto));

            Grid headerPanel = new Grid();
            headerPanel.ColumnDefinitions.Add(new ColumnDefinition(0, GridUnitType.Auto));
            headerPanel.ColumnDefinitions.Add(new ColumnDefinition(0, GridUnitType.Auto));
            headerPanel.ColumnDefinitions.Add(new ColumnDefinition(0, GridUnitType.Auto));
            headerPanel.ColumnDefinitions.Add(new ColumnDefinition(0, GridUnitType.Auto));
            headerPanel.ColumnDefinitions.Add(new ColumnDefinition(0, GridUnitType.Auto));
            headerPanel.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));
            mainGrid.Children.Add(headerPanel);

            headerPanel.Children.Add(new TextBlock() { Text = "Palette:", Margin = new Avalonia.Thickness(5), VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center });

            List<string> paletteNames = (from el in Palettes select el.Name).ToList();

            int defaultIndex = DefaultIndex;

            if (Palette.CurrentPalette != null && Palettes.Contains(Palette.CurrentPalette))
            {
                defaultIndex = Palettes.IndexOf(Palette.CurrentPalette);
            }

            PaletteSelectorBox = new ComboBox() { Items = paletteNames, SelectedIndex = defaultIndex, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center };
            Grid.SetColumn(PaletteSelectorBox, 1);
            headerPanel.Children.Add(PaletteSelectorBox);

            Canvas deleteCanvas = GetDeleteCanvas();
            deleteCanvas.Margin = new Thickness(5, 0, 0, 0);
            Grid.SetColumn(deleteCanvas, 2);
            headerPanel.Children.Add(deleteCanvas);
            deleteCanvas.PointerPressed += (s, e) =>
            {
                Palette palette = Palettes[PaletteSelectorBox.SelectedIndex];
                if (File.Exists(palette.FileName))
                {
                    File.Delete(palette.FileName);
                }

                Palettes.Remove(palette);

                if (Palettes.Count == 0)
                {
                    string id = Guid.NewGuid().ToString("N");
                    Palette newPalette = new Palette("Custom", "A custom palette", Path.Combine(PaletteDirectory, id + ".palette"));
                    newPalette.Save();
                    Palettes.Add(newPalette);
                }

                List<string> _paletteNames = (from el in Palettes select el.Name).ToList();
                PaletteSelectorBox.Items = _paletteNames;
                PaletteSelectorBox.SelectedIndex = 0;
            };

            Canvas saveCanvas = GetSaveCanvas();
            Grid.SetColumn(saveCanvas, 3);
            headerPanel.Children.Add(saveCanvas);
            saveCanvas.PointerPressed += (s, e) =>
            {
                Palettes[PaletteSelectorBox.SelectedIndex].Save();
            };

            Canvas addCanvas = GetAddCanvas();
            Grid.SetColumn(addCanvas, 4);
            headerPanel.Children.Add(addCanvas);
            addCanvas.PointerPressed += async (s, e) =>
            {
                await AddButtonClicked();
                addCanvas.Classes.Remove("pressed");
            };

            PaletteDescription = new TextBlock() { FontStyle = FontStyle.Italic, Foreground = new SolidColorBrush(Color.FromRgb(128, 128, 128)), VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center, Margin = new Thickness(5), TextWrapping = TextWrapping.Wrap };
            Grid.SetColumn(PaletteDescription, 5);
            headerPanel.Children.Add(PaletteDescription);

            ScrollViewer paletteScroller = new ScrollViewer() { HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto, VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled, HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left, Margin = new Thickness(5) };
            Grid.SetRow(paletteScroller, 1);
            mainGrid.Children.Add(paletteScroller);

            ScrollerContainer = new Grid() { HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left };

            ScrollerContainer.Children.Add(BuildPaletteCanvas(Palettes[PaletteSelectorBox.SelectedIndex]));

            paletteScroller.Content = ScrollerContainer;
            PaletteDescription.Text = Palettes[PaletteSelectorBox.SelectedIndex].Description;

            PaletteSelectorBox.SelectionChanged += (s, e) =>
            {
                ScrollerContainer.Children.Clear();
                if (PaletteSelectorBox.SelectedIndex >= 0 && PaletteSelectorBox.SelectedIndex < Palettes.Count)
                {
                    Palette.CurrentPalette = Palettes[PaletteSelectorBox.SelectedIndex];
                    ScrollerContainer.Children.Add(BuildPaletteCanvas(Palettes[PaletteSelectorBox.SelectedIndex]));
                    PaletteDescription.Text = Palettes[PaletteSelectorBox.SelectedIndex].Description;
                }
                else
                {
                    Palette.CurrentPalette = null;
                }
            };

            if (PaletteSelectorBox.SelectedIndex >= 0 && PaletteSelectorBox.SelectedIndex < Palettes.Count)
            {
                Palette.CurrentPalette = Palettes[PaletteSelectorBox.SelectedIndex];
            }
            else
            {
                Palette.CurrentPalette = null;
            }

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

            if (change.Property == ColourBlindnessFunctionProperty)
            {
                ScrollerContainer.Children.Clear();
                if (PaletteSelectorBox.SelectedIndex >= 0 && PaletteSelectorBox.SelectedIndex < Palettes.Count)
                {
                    ScrollerContainer.Children.Add(BuildPaletteCanvas(Palettes[PaletteSelectorBox.SelectedIndex]));
                    PaletteDescription.Text = Palettes[PaletteSelectorBox.SelectedIndex].Description;
                }
            }
        }

        const double HexagonRadius = 24;

        private Canvas BuildPaletteCanvas(Palette palette)
        {
            int n = palette.Colors.Count + 1;

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
                AnimatableColorBrush plusStroke = new AnimatableColorBrush(Colors.White, col => plusPath.Stroke = new SolidColorBrush(col));

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
                        plusStroke.Update(Colors.White, false);
                    }
                };

                centerPath.PointerPressed += async (s, e) =>
                {
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

                    palette.Colors.Add(col);
                    AddColorHexagon(palette.Colors.Count - 1, palette, tbr, this, infos, this.ColourBlindnessFunction);

                    leftFill.Update(Color.FromRgb(240, 240, 240), false);
                    rightFill.Update(Color.FromRgb(240, 240, 240), false);
                    centerFill.Update(Color.FromRgb(200, 200, 200), false);
                    plusStroke.Update(Colors.White, false);

                    tbr.Width += HexagonRadius * 2.5;
                    double deltaX = HexagonRadius * 2.5;
                    double deltaY = HexagonRadius * Math.Sin(Math.PI / 3) * (palette.Colors.Count % 2 == 0 ? -1 : 1);
                    transform.Matrix = new Matrix(1, 0, 0, 1, transform.Matrix.M31 + deltaX, transform.Matrix.M32 + deltaY);

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

        private static void AddColorHexagon(int i, Palette palette, Canvas container, PaletteSelector owner, List<PaletteColorInfo> infos, Func<Color, Color> colourBlindnessFunction)
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


            Color color = palette.Colors[i % palette.Colors.Count];
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
                fgStroke.Update(Colors.White, false);
            };

            myContainer.Children.Add(deleteBG.Path);
            myContainer.Children.Add(deleteFG);

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

                await Task.Delay(100);
                container.Width -= HexagonRadius * 2.5;

                infos.RemoveAt(info.Index);
                palette.Colors.RemoveAt(info.Index);

                /*container.Children.Remove(leftPath);
                container.Children.Remove(rightPath);
                container.Children.Remove(centerPath);
                container.Children.Remove(deleteBG.Path);
                container.Children.Remove(deleteFG);*/
                container.Children.Remove(myContainer);
            };
        }

        private static Canvas GetDeleteCanvas()
        {
            Canvas can = new Canvas() { Width = 24, Height = 24 };
            can.Classes.Add("ButtonBG");
            can.Classes.Add("DeleteButton");

            can.PointerPressed += (s, e) =>
            {
                can.Classes.Add("pressed");
            };

            can.PointerReleased += (s, e) =>
            {
                can.Classes.Remove("pressed");
            };

            PathGeometry deleteGeometry = new PathGeometry();
            PathFigure deleteFigure1 = new PathFigure() { StartPoint = new Point(6, 6), IsClosed = false };
            deleteFigure1.Segments.Add(new LineSegment() { Point = new Point(18, 18) });
            PathFigure deleteFigure2 = new PathFigure() { StartPoint = new Point(18, 6), IsClosed = false };
            deleteFigure2.Segments.Add(new LineSegment() { Point = new Point(6, 18) });
            deleteGeometry.Figures.Add(deleteFigure1);
            deleteGeometry.Figures.Add(deleteFigure2);

            Avalonia.Controls.Shapes.Path deletePath = new Avalonia.Controls.Shapes.Path() { Data = deleteGeometry, StrokeThickness = 4, IsHitTestVisible = false };

            can.Children.Add(deletePath);

            return can;
        }

        private static Canvas GetSaveCanvas()
        {
            Canvas can = new Canvas() { Width = 24, Height = 24 };
            can.Classes.Add("ButtonBG");
            can.Classes.Add("SaveButton");

            can.PointerPressed += (s, e) =>
            {
                can.Classes.Add("pressed");
            };

            can.PointerReleased += (s, e) =>
            {
                can.Classes.Remove("pressed");
            };

            PathGeometry saveGeometry = new PathGeometry();
            PathFigure saveFigure1 = new PathFigure() { StartPoint = new Point(4, 4), IsClosed = true };
            saveFigure1.Segments.Add(new LineSegment() { Point = new Point(8, 4) });
            saveFigure1.Segments.Add(new LineSegment() { Point = new Point(8, 10) });
            saveFigure1.Segments.Add(new LineSegment() { Point = new Point(16, 10) });
            saveFigure1.Segments.Add(new LineSegment() { Point = new Point(16, 4) });
            saveFigure1.Segments.Add(new LineSegment() { Point = new Point(17, 4) });
            saveFigure1.Segments.Add(new LineSegment() { Point = new Point(20, 7) });
            saveFigure1.Segments.Add(new LineSegment() { Point = new Point(20, 20) });
            saveFigure1.Segments.Add(new LineSegment() { Point = new Point(4, 20) });
            saveGeometry.Figures.Add(saveFigure1);
            PathFigure saveFigure2 = new PathFigure() { StartPoint = new Point(13, 4), IsClosed = true };
            saveFigure2.Segments.Add(new LineSegment() { Point = new Point(15, 4) });
            saveFigure2.Segments.Add(new LineSegment() { Point = new Point(15, 8) });
            saveFigure2.Segments.Add(new LineSegment() { Point = new Point(13, 8) });
            saveGeometry.Figures.Add(saveFigure2);

            PathFigure saveFigure3 = new PathFigure() { StartPoint = new Point(16, 18), IsClosed = true, IsFilled = false };
            saveFigure3.Segments.Add(new LineSegment() { Point = new Point(16, 13) });
            saveFigure3.Segments.Add(new LineSegment() { Point = new Point(8, 13) });
            saveFigure3.Segments.Add(new LineSegment() { Point = new Point(8, 18) });
            saveGeometry.Figures.Add(saveFigure3);

            Avalonia.Controls.Shapes.Path deletePath = new Avalonia.Controls.Shapes.Path() { Data = saveGeometry, IsHitTestVisible = false };

            can.Children.Add(deletePath);

            return can;
        }

        private static Canvas GetAddCanvas()
        {
            Canvas can = new Canvas() { Width = 24, Height = 24 };
            can.Classes.Add("ButtonBG");
            can.Classes.Add("AddButton");

            can.PointerPressed += (s, e) =>
            {
                can.Classes.Add("pressed");
            };

            can.PointerReleased += (s, e) =>
            {
                can.Classes.Remove("pressed");
            };

            PathGeometry deleteGeometry = new PathGeometry();
            PathFigure deleteFigure1 = new PathFigure() { StartPoint = new Point(12, 4), IsClosed = false };
            deleteFigure1.Segments.Add(new LineSegment() { Point = new Point(12, 20) });
            PathFigure deleteFigure2 = new PathFigure() { StartPoint = new Point(20, 12), IsClosed = false };
            deleteFigure2.Segments.Add(new LineSegment() { Point = new Point(4, 12) });
            deleteGeometry.Figures.Add(deleteFigure1);
            deleteGeometry.Figures.Add(deleteFigure2);

            Avalonia.Controls.Shapes.Path deletePath = new Avalonia.Controls.Shapes.Path() { Data = deleteGeometry, StrokeThickness = 4, IsHitTestVisible = false };

            can.Children.Add(deletePath);

            return can;
        }

        private void SetStyles()
        {
            Transitions buttonPathTransitions = null;

            if (!ColorPicker.TransitionsDisabled)
            {  
                buttonPathTransitions = new Transitions
                {
                    new SolidBrushTransition() { Property = Avalonia.Controls.Shapes.Path.StrokeProperty, Duration = new TimeSpan(0, 0, 0, 0, 100) }
                };
            }

            Style ButtonBG = new Style(x => x.OfType<Canvas>().Class("ButtonBG"));
            ButtonBG.Setters.Add(new Setter(Canvas.BackgroundProperty, new SolidColorBrush(Color.FromArgb(0, 180, 180, 180))));
            ButtonBG.Setters.Add(new Setter(Canvas.CursorProperty, new Cursor(StandardCursorType.Hand)));
            if (!ColorPicker.TransitionsDisabled)
            {
                Transitions buttonBGTransitions = new Transitions
                {
                    new SolidBrushTransition() { Property = Canvas.BackgroundProperty, Duration = new TimeSpan(0, 0, 0, 0, 100) }
                };

                ButtonBG.Setters.Add(new Setter(Canvas.TransitionsProperty, buttonBGTransitions));
            }
            this.Styles.Add(ButtonBG);

            Style DeleteButtonPath = new Style(x => x.OfType<Canvas>().Class("DeleteButton").Child().OfType<Avalonia.Controls.Shapes.Path>());
            DeleteButtonPath.Setters.Add(new Setter(Avalonia.Controls.Shapes.Path.StrokeProperty, new SolidColorBrush(Color.FromArgb(255, 237, 28, 36))));
            if (!ColorPicker.TransitionsDisabled)
            {
                DeleteButtonPath.Setters.Add(new Setter(Avalonia.Controls.Shapes.Path.TransitionsProperty, buttonPathTransitions));
            }
            this.Styles.Add(DeleteButtonPath);

            Style SaveButtonPath = new Style(x => x.OfType<Canvas>().Class("SaveButton").Child().OfType<Avalonia.Controls.Shapes.Path>());
            SaveButtonPath.Setters.Add(new Setter(Avalonia.Controls.Shapes.Path.FillProperty, new SolidColorBrush(Color.FromArgb(255, 86, 180, 233))));
            if (!ColorPicker.TransitionsDisabled)
            {
                SaveButtonPath.Setters.Add(new Setter(Avalonia.Controls.Shapes.Path.TransitionsProperty, buttonPathTransitions));
            }
            this.Styles.Add(SaveButtonPath);

            Style AddButtonPath = new Style(x => x.OfType<Canvas>().Class("AddButton").Child().OfType<Avalonia.Controls.Shapes.Path>());
            AddButtonPath.Setters.Add(new Setter(Avalonia.Controls.Shapes.Path.StrokeProperty, new SolidColorBrush(Color.FromArgb(255, 34, 177, 76))));
            if (!ColorPicker.TransitionsDisabled)
            {
                AddButtonPath.Setters.Add(new Setter(Avalonia.Controls.Shapes.Path.TransitionsProperty, buttonPathTransitions));
            }
            this.Styles.Add(AddButtonPath);

            Style ButtonBGOver = new Style(x => x.OfType<Canvas>().Class("ButtonBG").Class(":pointerover"));
            ButtonBGOver.Setters.Add(new Setter(Canvas.BackgroundProperty, new SolidColorBrush(Color.FromArgb(255, 180, 180, 180))));
            this.Styles.Add(ButtonBGOver);

            Style ButtonBGOverPath = new Style(x => x.OfType<Canvas>().Class("ButtonBG").Class(":pointerover").Child().OfType<Avalonia.Controls.Shapes.Path>());
            ButtonBGOverPath.Setters.Add(new Setter(Avalonia.Controls.Shapes.Path.StrokeProperty, Colours.BackgroundColour));
            ButtonBGOverPath.Setters.Add(new Setter(Avalonia.Controls.Shapes.Path.FillProperty, Colours.BackgroundColour));
            this.Styles.Add(ButtonBGOverPath);

            Style ButtonBGPressed = new Style(x => x.OfType<Canvas>().Class("ButtonBG").Class("pressed"));
            ButtonBGPressed.Setters.Add(new Setter(Canvas.BackgroundProperty, new SolidColorBrush(Color.FromArgb(255, 128, 128, 128))));
            this.Styles.Add(ButtonBGPressed);

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

        private async Task AddButtonClicked()
        {
            Window win = new Window() { Title = "Create new palette" };
            win.Width = 600;
            win.Height = 150;

            Grid mainGrid = new Grid() { Margin = new Thickness(10) };
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition(0, GridUnitType.Auto));
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));
            mainGrid.RowDefinitions.Add(new RowDefinition(1, GridUnitType.Star));
            mainGrid.RowDefinitions.Add(new RowDefinition(1, GridUnitType.Star));
            mainGrid.RowDefinitions.Add(new RowDefinition(0, GridUnitType.Auto));

            TextBlock nameBlock = new TextBlock() { Text = "Name:", HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center };
            mainGrid.Children.Add(nameBlock);

            TextBlock descriptionBlock = new TextBlock() { Text = "Description:", HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center };
            Grid.SetRow(descriptionBlock, 1);
            mainGrid.Children.Add(descriptionBlock);

            TextBox nameBox = new TextBox() { VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center, Margin = new Thickness(5, 0, 0, 0) };
            Grid.SetColumn(nameBox, 1);
            mainGrid.Children.Add(nameBox);

            TextBox descriptionBox = new TextBox() { VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center, Margin = new Thickness(5, 0, 0, 0) };
            Grid.SetRow(descriptionBox, 1);
            Grid.SetColumn(descriptionBox, 1);
            mainGrid.Children.Add(descriptionBox);


            Grid buttonGrid = new Grid() { Margin = new Thickness(0, 10, 0, 0) };
            buttonGrid.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));
            buttonGrid.ColumnDefinitions.Add(new ColumnDefinition(0, GridUnitType.Auto));
            buttonGrid.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));
            buttonGrid.ColumnDefinitions.Add(new ColumnDefinition(0, GridUnitType.Auto));
            buttonGrid.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));

            Grid.SetRow(buttonGrid, 2);
            Grid.SetColumnSpan(buttonGrid, 2);
            mainGrid.Children.Add(buttonGrid);

            Button okButton = new Button() { Content = "OK", Width = 100 };
            Grid.SetColumn(okButton, 1);
            buttonGrid.Children.Add(okButton);

            Button cancelButton = new Button() { Content = "Cancel", Width = 100 };
            Grid.SetColumn(cancelButton, 3);
            buttonGrid.Children.Add(cancelButton);

            bool result = false;

            cancelButton.Click += (s, e) =>
            {
                result = false;
                win.Close();
            };

            okButton.Click += (s, e) =>
            {
                result = true;
                win.Close();
            };

            win.Content = mainGrid;
            await win.ShowDialog(this.FindLogicalAncestorOfType<Window>());

            if (result)
            {
                string name = nameBox.Text;
                string description = descriptionBox.Text;

                string id = Guid.NewGuid().ToString("N");

                Palette newPalette = new Palette(name, description, Path.Combine(PaletteDirectory, id + ".palette"));
                Palettes.Add(newPalette);
                List<string> paletteNames = (from el in Palettes select el.Name).ToList();
                PaletteSelectorBox.Items = paletteNames;
                PaletteSelectorBox.SelectedIndex = Palettes.Count - 1;
            }

        }
    }

    internal class PaletteColorInfo
    {
        public int Index { get; set; }
        public int OriginalIndex { get; set; }
        public AnimatableTransform Transform { get; set; }
        public AnimatablePath4Points DeleteBGPath { get; set; }
        public AnimatableTransform DeleteFBTransform { get; set; }

    }

    /// <summary>
    /// <see cref="EventArgs"/> for events where a colour has been selected.
    /// </summary>
    public class ColorSelectedEventArgs : EventArgs
    {
        /// <summary>
        /// The selected colour.
        /// </summary>
        public Color Color { get; }

        /// <summary>
        /// Create a new <see cref="ColorSelectedEventArgs"/> object.
        /// </summary>
        /// <param name="color">The selected colour.</param>
        public ColorSelectedEventArgs(Color color)
        {
            this.Color = color;
        }
    }
}
