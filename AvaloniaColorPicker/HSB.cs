/*
    AvaloniaColorPicker - A color picker for Avalonia.
    Copyright (C) 2020  Giorgio Bianchini
 
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, version 3.
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.
    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;

namespace AvaloniaColorPicker
{
    internal static class HSB
    {
        internal static Avalonia.Media.Imaging.WriteableBitmap GetSB(double H)
        {
            Avalonia.Media.Imaging.WriteableBitmap bmp = new Avalonia.Media.Imaging.WriteableBitmap(new PixelSize(256, 256), new Vector(96, 96), Avalonia.Platform.PixelFormat.Rgba8888, Avalonia.Platform.AlphaFormat.Unpremul);

            using (var fb = bmp.Lock())
            {
                unsafe
                {
                    byte* rgbaValues = (byte*)fb.Address;
                    for (int x = 0; x < 256; x++)
                    {
                        for (int y = 0; y < 256; y++)
                        {
                            HSVToRGB(H, x / 255.0, 1 - y / 255.0,
                                out rgbaValues[x * 4 + y * 256 * 4],
                                out rgbaValues[x * 4 + y * 256 * 4 + 1],
                                out rgbaValues[x * 4 + y * 256 * 4 + 2]
                                );

                            rgbaValues[x * 4 + y * 256 * 4 + 3] = 255;
                        }
                    }
                }
            }
            return bmp;
        }

        internal static Avalonia.Media.Imaging.WriteableBitmap GetH()
        {
            Avalonia.Media.Imaging.WriteableBitmap bmp = new Avalonia.Media.Imaging.WriteableBitmap(new PixelSize(1, 256), new Vector(96, 96), Avalonia.Platform.PixelFormat.Rgba8888, Avalonia.Platform.AlphaFormat.Unpremul);

            using (var fb = bmp.Lock())
            {
                unsafe
                {
                    byte* rgbaValues = (byte*)fb.Address;
                    for (int y = 0; y < 256; y++)
                    {
                        HSVToRGB(1 - y / 255.0, 1, 1,
                            out rgbaValues[y * 4],
                            out rgbaValues[y * 4 + 1],
                            out rgbaValues[y * 4 + 2]
                            );

                        rgbaValues[y * 4 + 3] = 255;
                    }
                }
            }
            return bmp;
        }


        internal static Avalonia.Media.Imaging.WriteableBitmap GetHB(double S)
        {
            Avalonia.Media.Imaging.WriteableBitmap bmp = new Avalonia.Media.Imaging.WriteableBitmap(new PixelSize(256, 256), new Vector(96, 96), Avalonia.Platform.PixelFormat.Rgba8888, Avalonia.Platform.AlphaFormat.Unpremul);

            using (var fb = bmp.Lock())
            {
                unsafe
                {
                    byte* rgbaValues = (byte*)fb.Address;
                    for (int x = 0; x < 256; x++)
                    {
                        for (int y = 0; y < 256; y++)
                        {
                            HSVToRGB(x / 255.0, S, 1 - y / 255.0,
                                out rgbaValues[x * 4 + y * 256 * 4],
                                out rgbaValues[x * 4 + y * 256 * 4 + 1],
                                out rgbaValues[x * 4 + y * 256 * 4 + 2]
                                );

                            rgbaValues[x * 4 + y * 256 * 4 + 3] = 255;
                        }
                    }
                }
            }
            return bmp;
        }

        internal static Avalonia.Media.Imaging.WriteableBitmap GetS(double H, double B)
        {
            Avalonia.Media.Imaging.WriteableBitmap bmp = new Avalonia.Media.Imaging.WriteableBitmap(new PixelSize(1, 256), new Vector(96, 96), Avalonia.Platform.PixelFormat.Rgba8888, Avalonia.Platform.AlphaFormat.Unpremul);

            using (var fb = bmp.Lock())
            {
                unsafe
                {
                    byte* rgbaValues = (byte*)fb.Address;
                    for (int y = 0; y < 256; y++)
                    {
                        HSVToRGB(H, 1 - y / 255.0, B,
                            out rgbaValues[y * 4],
                            out rgbaValues[y * 4 + 1],
                            out rgbaValues[y * 4 + 2]
                            );

                        rgbaValues[y * 4 + 3] = 255;
                    }
                }
            }
            return bmp;
        }

        internal static Avalonia.Media.Imaging.WriteableBitmap GetHS(double B)
        {
            Avalonia.Media.Imaging.WriteableBitmap bmp = new Avalonia.Media.Imaging.WriteableBitmap(new PixelSize(256, 256), new Vector(96, 96), Avalonia.Platform.PixelFormat.Rgba8888, Avalonia.Platform.AlphaFormat.Unpremul);

            using (var fb = bmp.Lock())
            {
                unsafe
                {
                    byte* rgbaValues = (byte*)fb.Address;
                    for (int x = 0; x < 256; x++)
                    {
                        for (int y = 0; y < 256; y++)
                        {
                            double r = Math.Sqrt((x - 128) * (x - 128) + (y - 128) * (y - 128));

                            if (r <= 130)
                            {
                                double theta = Math.Atan2(y - 128, x - 128) / Math.PI;

                                HSVToRGB(theta * 0.5, Math.Min(1, r / 128.0), B,
                                    out rgbaValues[x * 4 + y * 256 * 4],
                                    out rgbaValues[x * 4 + y * 256 * 4 + 1],
                                    out rgbaValues[x * 4 + y * 256 * 4 + 2]
                                    );

                                if (r >= 126)
                                {
                                    rgbaValues[x * 4 + y * 256 * 4 + 3] = (byte)Math.Max(0, Math.Min(255, 32640 - 255 * r));
                                }
                                else
                                {
                                    rgbaValues[x * 4 + y * 256 * 4 + 3] = 255;
                                }
                            }
                        }
                    }
                }
            }
            return bmp;
        }

        internal static Avalonia.Media.Imaging.WriteableBitmap GetB(double H, double S)
        {
            Avalonia.Media.Imaging.WriteableBitmap bmp = new Avalonia.Media.Imaging.WriteableBitmap(new PixelSize(1, 256), new Vector(96, 96), Avalonia.Platform.PixelFormat.Rgba8888, Avalonia.Platform.AlphaFormat.Unpremul);

            using (var fb = bmp.Lock())
            {
                unsafe
                {
                    byte* rgbaValues = (byte*)fb.Address;
                    for (int y = 0; y < 256; y++)
                    {
                        HSVToRGB(H, S, 1 - y / 255.0,
                            out rgbaValues[y * 4],
                            out rgbaValues[y * 4 + 1],
                            out rgbaValues[y * 4 + 2]
                            );

                        rgbaValues[y * 4 + 3] = 255;
                    }
                }
            }
            return bmp;
        }

        internal static WriteableBitmap GetShiftedS(WriteableBitmap originalS, double vRadius, bool front)
        {
            int delta = (int)Math.Round(vRadius * 256 / 56);

            int[] shifts = new int[128];

            for (int i = 0; i < 128; i++)
            {
                double cos = (63.5 - i) / 63.5;
                shifts[i] = (int)Math.Round(Math.Sqrt(1 - cos * cos) * delta);
            }

            Avalonia.Media.Imaging.WriteableBitmap bmp = new Avalonia.Media.Imaging.WriteableBitmap(new PixelSize(128, 256 + delta), new Vector(96, 96), Avalonia.Platform.PixelFormat.Rgba8888, Avalonia.Platform.AlphaFormat.Unpremul);

            if (front)
            {
                using (var fb = bmp.Lock())
                {
                    using (var fb2 = originalS.Lock())
                    {
                        unsafe
                        {
                            byte* rgbaValues = (byte*)fb.Address;
                            byte* rgbaValuesOrig = (byte*)fb2.Address;
                            for (int x = 0; x < 128; x++)
                            {
                                for (int y = shifts[x]; y < 256 + shifts[x]; y++)
                                {
                                    rgbaValues[x * 4 + y * 128 * 4] = rgbaValuesOrig[(127 - x) * 4 + (y - shifts[x]) * 256 * 4];
                                    rgbaValues[x * 4 + y * 128 * 4 + 1] = rgbaValuesOrig[(127 - x) * 4 + (y - shifts[x]) * 256 * 4 + 1];
                                    rgbaValues[x * 4 + y * 128 * 4 + 2] = rgbaValuesOrig[(127 - x) * 4 + (y - shifts[x]) * 256 * 4 + 2];
                                    rgbaValues[x * 4 + y * 128 * 4 + 3] = 255;


                                }
                            }
                        }
                    }
                }
            }
            else
            {
                using (var fb = bmp.Lock())
                {
                    using (var fb2 = originalS.Lock())
                    {
                        unsafe
                        {
                            byte* rgbaValues = (byte*)fb.Address;
                            byte* rgbaValuesOrig = (byte*)fb2.Address;
                            for (int x = 0; x < 128; x++)
                            {
                                for (int y = delta - shifts[x]; y < 256 + delta - shifts[x]; y++)
                                {
                                    rgbaValues[x * 4 + y * 128 * 4] = rgbaValuesOrig[(128 + x) * 4 + (y - delta + shifts[x]) * 256 * 4];
                                    rgbaValues[x * 4 + y * 128 * 4 + 1] = rgbaValuesOrig[(128 + x) * 4 + (y - delta + shifts[x]) * 256 * 4 + 1];
                                    rgbaValues[x * 4 + y * 128 * 4 + 2] = rgbaValuesOrig[(128 + x) * 4 + (y - delta + shifts[x]) * 256 * 4 + 2];
                                    rgbaValues[x * 4 + y * 128 * 4 + 3] = 255;


                                }
                            }
                        }
                    }
                }
            }

            return bmp;
        }


        //Adapted formulae from https://en.wikipedia.org/wiki/HSL_and_HSV
        public static void HSVToRGB(double h, double s, double v, out byte r, out byte g, out byte b)
        {
            double k = (5 + h * 6) % 6;
            r = (byte)(255 * v * (1 - s * Math.Max(Math.Min(Math.Min(k, 4 - k), 1), 0)));

            k = (3 + h * 6) % 6;
            g = (byte)(255 * v * (1 - s * Math.Max(Math.Min(Math.Min(k, 4 - k), 1), 0)));

            k = (1 + h * 6) % 6;
            b = (byte)(255 * v * (1 - s * Math.Max(Math.Min(Math.Min(k, 4 - k), 1), 0)));
        }

        public static void HSVToRGB(double h, double s, double v, out double r, out double g, out double b)
        {
            double k = (5 + h * 6) % 6;
            r = v * (1 - s * Math.Max(Math.Min(Math.Min(k, 4 - k), 1), 0));

            k = (3 + h * 6) % 6;
            g = v * (1 - s * Math.Max(Math.Min(Math.Min(k, 4 - k), 1), 0));

            k = (1 + h * 6) % 6;
            b = v * (1 - s * Math.Max(Math.Min(Math.Min(k, 4 - k), 1), 0));
        }

        public static Color ColorFromHSV(double h, double s, double v)
        {
            HSVToRGB(h, s, v, out byte r, out byte g, out byte b);

            return Color.FromRgb(r, g, b);
        }

        public static (double H, double S, double B) RGBToHSB(double R, double G, double B)
        {
            double max = Math.Max(Math.Max(R, G), B);
            double min = Math.Min(Math.Min(R, G), B);

            double h = 0;

            if (max != min)
            {
                if (max == R)
                {
                    h = (G - B) / (max - min) / 6;
                }
                else if (max == G)
                {
                    h = (2 + (B - R) / (max - min)) / 6;
                }
                else if (max == B)
                {
                    h = (4 + (R - G) / (max - min)) / 6;
                }
            }

            while (h < 0)
            {
                h += 1;
            }

            double s = max == 0 ? 0 : (max - min) / max;
            double v = max;

            return (h, s, v);
        }

        public static (double H, double S, double B) RGBToHSB(Color color)
        {
            double R = color.R / 255.0;
            double G = color.G / 255.0;
            double B = color.B / 255.0;

            return RGBToHSB(R, G, B);
        }


        public static SolidColorBrush CubeBrush = new SolidColorBrush(Color.FromArgb(128, 128, 128, 128));
        public static SolidColorBrush BorderBrush = new SolidColorBrush(Color.FromArgb(255, 128, 128, 128));
        public static SolidColorBrush LightCylinderBrush = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));
        public static SolidColorBrush DarkCylinderBrush = new SolidColorBrush(Color.FromArgb(255, 170, 170, 170));
        public static LinearGradientBrush CylinderBrush = new LinearGradientBrush() { StartPoint = new RelativePoint(0, 64, RelativeUnit.Absolute), EndPoint = new RelativePoint(128, 64, RelativeUnit.Absolute) };


        static HSB()
        {
            for (int i = 0; i <= 10; i++)
            {
                double x = i / 10.0;
                double theta = Math.Acos((0.5 - x) * 2);
                double intensity = Math.Max(0, Math.Cos(theta - Math.PI / 4));
                byte col = (byte)Math.Round(170 + 60 * intensity);
                CylinderBrush.GradientStops.Add(new GradientStop(Color.FromRgb(col, col, col), x));
            }
        }

        public static Canvas GetHCanvas(double H, double S, double V, Bitmap colourBitmap, Canvas OldCanvas, bool instantTransition)
        {
            if (OldCanvas is HCanvas hCanvas)
            {
                hCanvas.Update(H, S, V, colourBitmap, instantTransition);
                return hCanvas;
            }
            else
            {
                return new HCanvas(H, S, V, colourBitmap);
            }
        }

        public static Canvas GetBCanvas(double H, double S, double V, Bitmap colourBitmap, Canvas OldCanvas, bool instantTransition)
        {
            if (OldCanvas is VCanvas vCanvas)
            {
                vCanvas.Update(H, S, V, colourBitmap, instantTransition);
                return vCanvas;
            }
            else
            {
                return new VCanvas(H, S, V, colourBitmap);
            }
        }

        public static Canvas GetSCanvas(double H, double S, double V, WriteableBitmap colourBitmap, Canvas OldCanvas, bool instantTransition)
        {
            if (OldCanvas is SCanvas sCanvas)
            {
                sCanvas.Update(H, S, V, colourBitmap, instantTransition);
                return sCanvas;
            }
            else
            {
                return new SCanvas(H, S, V, colourBitmap);
            }
        }
    }

    internal class HCanvas : Canvas
    {
        private Canvas ColourCanvas { get; }
        private Image ColourImage { get; }
        private Ellipse PositionEllipse { get; }
        private AnimatableAngleTransform ColourCanvasTransform { get; }
        private AnimatablePathCylinder PiePath { get; }

        public HCanvas(double H, double S, double V, Bitmap colourBitmap) : base()
        {
            this.Width = 128;
            this.Height = 96;

            ColourCanvas = new Canvas() { Width = 256, Height = 256, ClipToBounds = false };

            ColourImage = new Image() { Source = colourBitmap };
            ColourCanvas.Children.Add(ColourImage);

            PathGeometry topBorderGeometry = new PathGeometry();
            PathFigure topBorderFigure = new PathFigure() { StartPoint = new Point(0, 0), IsClosed = false, IsFilled = false };
            topBorderFigure.Segments.Add(new LineSegment() { Point = new Point(256, 0) });
            topBorderGeometry.Figures.Add(topBorderFigure);
            ColourCanvas.Children.Add(new Path() { Data = topBorderGeometry, StrokeThickness = 6, Stroke = HSB.LightCylinderBrush });

            PositionEllipse = new Ellipse() { Width = 48, Height = 48, Fill = (S < 0.5 && V > 0.5 ? Brushes.Black : Brushes.White) };
            PositionEllipse.RenderTransform = ColorPicker.GetTranslateTransform(S * 255 - 24, 255 - V * 255 - 24);
            ColourCanvas.Children.Add(PositionEllipse);

            ColourCanvasTransform = new AnimatableAngleTransform(H);

            ColourCanvas.RenderTransform = ColourCanvasTransform.MatrixTransform;
            ColourCanvas.RenderTransformOrigin = new RelativePoint(0, 0, RelativeUnit.Absolute);
            this.Children.Add(ColourCanvas);

            PiePath = new AnimatablePathCylinder(H);
            PiePath.TopPath.Fill = HSB.LightCylinderBrush;
            this.Children.Add(PiePath.TopPath);

            PiePath.CylinderPath.Fill = HSB.CylinderBrush;
            this.Children.Add(PiePath.CylinderPath);

            double plateIntensity = Math.Sin(H * 2 * Math.PI - Math.PI / 4);
            byte plateCol = (byte)Math.Round(170 + 60 * plateIntensity);

            PiePath.PlatePath.Fill = new SolidColorBrush(Color.FromRgb(plateCol, plateCol, plateCol));
            this.Children.Add(PiePath.PlatePath);
        }

        public void Update(double H, double S, double V, Bitmap colourBitmap, bool instantTransition)
        {
            ColourImage.Source = colourBitmap;

            ColorPicker.SetTranslateRenderTransform(PositionEllipse, S * 255 - 24, 255 - V * 255 - 24, instantTransition);

            ColourCanvasTransform.Update(H, instantTransition);

            PiePath.Update(H, instantTransition);

            double plateIntensity = Math.Sin(H * 2 * Math.PI - Math.PI / 4);
            byte plateCol = (byte)Math.Round(170 + 60 * plateIntensity);

            PiePath.PlatePath.Fill = new SolidColorBrush(Color.FromRgb(plateCol, plateCol, plateCol));
        }
    }

    internal class VCanvas : Canvas
    {
        private Canvas ColourCanvas { get; }
        private Image ColourImage { get; }
        private Ellipse PositionEllipse { get; }

        private AnimatableTransform ColourCanvasTransform { get; }

        private AnimatableCylinderShaft Shaft { get; }

        public VCanvas(double H, double S, double V, Bitmap colourBitmap)
        {
            this.Width = 128;
            this.Height = 96;

            Shaft = new AnimatableCylinderShaft(V);
            Shaft.Path.Fill = HSB.CylinderBrush;
            this.Children.Add(Shaft.Path);

            ColourCanvas = new Canvas() { Width = 256, Height = 256, ClipToBounds = false };

            ColourImage = new Image() { Source = colourBitmap };
            ColourCanvas.Children.Add(ColourImage);

            PositionEllipse = new Ellipse() { Width = 32, Height = 32, Fill = (S < 0.5 && V > 0.5 ? Brushes.Black : Brushes.White) };
            PositionEllipse.RenderTransform = ColorPicker.GetTranslateTransform(128 - 16 + S * Math.Cos(H * 2 * Math.PI) * 128, 128 - 16 + S * Math.Sin(H * 2 * Math.PI) * 128);
            ColourCanvas.Children.Add(PositionEllipse);

            Point v23 = new Point(128, 56 - V * 56);
            Point v14 = new Point(0, 56 - V * 56);
            Point v67 = new Point(128, 96 - V * 56);

            double m11 = (v23.X - v14.X) / 256;
            double m12 = (v67.X - v23.X) / 256;
            double m21 = (v23.Y - v14.Y) / 256;
            double m22 = (v67.Y - v23.Y) / 256;

            ColourCanvasTransform = new AnimatableTransform(new Matrix(m11, m21, m12, m22, v14.X, v14.Y));

            ColourCanvas.RenderTransform = ColourCanvasTransform.MatrixTransform;
            ColourCanvas.RenderTransformOrigin = new RelativePoint(0, 0, RelativeUnit.Absolute);
            this.Children.Add(ColourCanvas);
        }

        public void Update(double H, double S, double V, Bitmap colourBitmap, bool instantTransition)
        {
            Shaft.Update(V, instantTransition);

            ColourImage.Source = colourBitmap;
            ColorPicker.SetTranslateRenderTransform(PositionEllipse, 128 - 16 + S * Math.Cos(H * 2 * Math.PI) * 128, 128 - 16 + S * Math.Sin(H * 2 * Math.PI) * 128, instantTransition);

            Point v23 = new Point(128, 56 - V * 56);
            Point v14 = new Point(0, 56 - V * 56);
            Point v67 = new Point(128, 96 - V * 56);

            double m11 = (v23.X - v14.X) / 256;
            double m12 = (v67.X - v23.X) / 256;
            double m21 = (v23.Y - v14.Y) / 256;
            double m22 = (v67.Y - v23.Y) / 256;

            ColourCanvasTransform.Update(new Matrix(m11, m21, m12, m22, v14.X, v14.Y), instantTransition);
        }
    }

    internal class SCanvas : Canvas
    {
        private Canvas ColourCanvas { get; }

        private AnimatableColouredCylinder ColouredCylinder { get; }

        public SCanvas(double H, double S, double V, WriteableBitmap colourBitmap)
        {
            this.Width = 128;
            this.Height = 96;

            ColourCanvas = new Canvas() { Width = 128, Height = 256, ClipToBounds = false };

            ColouredCylinder = new AnimatableColouredCylinder(H, S, V, colourBitmap);

            ColourCanvas.Children.Add(ColouredCylinder.ColourImage);

            ColourCanvas.Children.Add(ColouredCylinder.PositionEllipse);

            ColourCanvas.RenderTransform = ColouredCylinder.ColourCanvasTransform;
            ColourCanvas.RenderTransformOrigin = new RelativePoint(0, 0, RelativeUnit.Absolute);
            this.Children.Add(ColourCanvas);

            this.Children.Add(ColouredCylinder.BottomEllipse);
            this.Children.Add(ColouredCylinder.TopEllipse);
        }

        public void Update(double H, double S, double V, WriteableBitmap colourBitmap, bool instantTransition)
        {
            ColouredCylinder.Update(H, S, V, colourBitmap, instantTransition);
        }
    }
}
