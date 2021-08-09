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
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace AvaloniaColorPicker
{
    internal static class RGB
    {
        public static Avalonia.Media.Imaging.WriteableBitmap GetGB(byte R)
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

                            rgbaValues[x * 4 + y * 256 * 4] = R;
                            rgbaValues[x * 4 + y * 256 * 4 + 1] = (byte)x;
                            rgbaValues[x * 4 + y * 256 * 4 + 2] = (byte)(255 - y);

                            rgbaValues[x * 4 + y * 256 * 4 + 3] = 255;
                        }
                    }
                }
            }
            return bmp;
        }

        public static Avalonia.Media.Imaging.WriteableBitmap GetR(byte G, byte B)
        {
            Avalonia.Media.Imaging.WriteableBitmap bmp = new Avalonia.Media.Imaging.WriteableBitmap(new PixelSize(1, 256), new Vector(96, 96), Avalonia.Platform.PixelFormat.Rgba8888, Avalonia.Platform.AlphaFormat.Unpremul);

            using (var fb = bmp.Lock())
            {
                unsafe
                {
                    byte* rgbaValues = (byte*)fb.Address;
                    for (int y = 0; y < 256; y++)
                    {
                        rgbaValues[y * 4] = (byte)(255 - y);
                        rgbaValues[y * 4 + 1] = G;
                        rgbaValues[y * 4 + 2] = B;

                        rgbaValues[y * 4 + 3] = 255;
                    }
                }
            }
            return bmp;
        }

        public static Avalonia.Media.Imaging.WriteableBitmap GetRB(byte G)
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

                            rgbaValues[x * 4 + y * 256 * 4] = (byte)(255 - x);
                            rgbaValues[x * 4 + y * 256 * 4 + 1] = G;
                            rgbaValues[x * 4 + y * 256 * 4 + 2] = (byte)(255 - y);

                            rgbaValues[x * 4 + y * 256 * 4 + 3] = 255;
                        }
                    }
                }
            }
            return bmp;
        }

        public static Avalonia.Media.Imaging.WriteableBitmap GetG(byte R, byte B)
        {
            Avalonia.Media.Imaging.WriteableBitmap bmp = new Avalonia.Media.Imaging.WriteableBitmap(new PixelSize(1, 256), new Vector(96, 96), Avalonia.Platform.PixelFormat.Rgba8888, Avalonia.Platform.AlphaFormat.Unpremul);

            using (var fb = bmp.Lock())
            {
                unsafe
                {
                    byte* rgbaValues = (byte*)fb.Address;
                    for (int y = 0; y < 256; y++)
                    {
                        rgbaValues[y * 4] = R;
                        rgbaValues[y * 4 + 1] = (byte)(255 - y);
                        rgbaValues[y * 4 + 2] = B;

                        rgbaValues[y * 4 + 3] = 255;
                    }
                }
            }
            return bmp;
        }

        public static Avalonia.Media.Imaging.WriteableBitmap GetRG(byte B)
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

                            rgbaValues[x * 4 + y * 256 * 4] = (byte)y;
                            rgbaValues[x * 4 + y * 256 * 4 + 1] = (byte)x;
                            rgbaValues[x * 4 + y * 256 * 4 + 2] = B;

                            rgbaValues[x * 4 + y * 256 * 4 + 3] = 255;
                        }
                    }
                }
            }
            return bmp;
        }

        public static Avalonia.Media.Imaging.WriteableBitmap GetB(byte R, byte G)
        {
            Avalonia.Media.Imaging.WriteableBitmap bmp = new Avalonia.Media.Imaging.WriteableBitmap(new PixelSize(1, 256), new Vector(96, 96), Avalonia.Platform.PixelFormat.Rgba8888, Avalonia.Platform.AlphaFormat.Unpremul);

            using (var fb = bmp.Lock())
            {
                unsafe
                {
                    byte* rgbaValues = (byte*)fb.Address;
                    for (int y = 0; y < 256; y++)
                    {
                        rgbaValues[y * 4] = R;
                        rgbaValues[y * 4 + 1] = G;
                        rgbaValues[y * 4 + 2] = (byte)(255 - y);

                        rgbaValues[y * 4 + 3] = 255;
                    }
                }
            }
            return bmp;
        }

        public static Avalonia.Media.Imaging.WriteableBitmap GetA(byte R, byte G, byte B)
        {
            Avalonia.Media.Imaging.WriteableBitmap bmp = new Avalonia.Media.Imaging.WriteableBitmap(new PixelSize(1, 256), new Vector(96, 96), Avalonia.Platform.PixelFormat.Rgba8888, Avalonia.Platform.AlphaFormat.Unpremul);

            using (var fb = bmp.Lock())
            {
                unsafe
                {
                    byte* rgbaValues = (byte*)fb.Address;
                    byte A;
                    for (int y = 0; y < 256; y++)
                    {
                        A = (byte)(255 - y);
                        rgbaValues[y * 4] = R;
                        rgbaValues[y * 4 + 1] = G;
                        rgbaValues[y * 4 + 2] = B;

                        rgbaValues[y * 4 + 3] = A;
                    }
                }
            }
            return bmp;
        }


        public static SolidColorBrush CubeBrush = new SolidColorBrush(Color.FromArgb(128, 128, 128, 128));
        public static SolidColorBrush BorderBrush = new SolidColorBrush(Color.FromArgb(255, 128, 128, 128));
        public static SolidColorBrush LightCubeBrush = new SolidColorBrush(Color.FromRgb(220, 220, 220));
        public static SolidColorBrush MiddleCubeBrush = new SolidColorBrush(Color.FromRgb(200, 200, 200));
        public static SolidColorBrush DarkCubeBrush = new SolidColorBrush(Color.FromRgb(180, 180, 180));

        public static Canvas GetRCanvas(byte R, byte G, byte B, Bitmap colourBitmap, Canvas OldCanvas, bool instantTransition)
        {
            if (OldCanvas is RCanvas rCanvas)
            {
                rCanvas.Update(R, G, B, colourBitmap, instantTransition);
                return rCanvas;
            }
            else
            {
                return new RCanvas(R, G, B, colourBitmap);
            }
        }

        public static Canvas GetGCanvas(byte R, byte G, byte B, Bitmap colourBitmap, Canvas OldCanvas, bool instantTransition)
        {
            if (OldCanvas is GCanvas gCanvas)
            {
                gCanvas.Update(R, G, B, colourBitmap, instantTransition);
                return gCanvas;
            }
            else
            {
                return new GCanvas(R, G, B, colourBitmap);
            }
        }

        public static Canvas GetBCanvas(byte R, byte G, byte B, Bitmap colourBitmap, Canvas OldCanvas, bool instantTransition)
        {
            if (OldCanvas is BCanvas bCanvas)
            {
                bCanvas.Update(R, G, B, colourBitmap, instantTransition);
                return bCanvas;
            }
            else
            {
                return new BCanvas(R, G, B, colourBitmap);
            }
        }
    }



    internal class RCanvas : Canvas
    {
        private AnimatablePath4Points Fill1 { get; }
        private AnimatablePath4Points Fill2 { get; }

        private AnimatableTransform ColourCanvasTransform { get; }

        private Image ColourImage { get; }
        private Canvas ColourCanvas { get; }

        private Ellipse PositionEllipse { get; }

        public RCanvas(byte R, byte G, byte B, Bitmap colourBitmap) : base()
        {
            (double _, double S, double V) = HSB.RGBToHSB(R / 255.0, G / 255.0, B / 255.0);

            this.Width = 128;
            this.Height = 96;

            Point v1 = new Point(51.673, 0);
            Point v2 = new Point(104, 15.955);
            Point v3 = new Point(76.327, 46.125);
            Point v4 = new Point(24, 30.170);

            Point v6 = new Point(104, 65.830);
            Point v7 = new Point(76.327, 96);

            Point v23 = v2 * (1 - R / 255.0) + v3 * (R / 255.0);
            Point v14 = v1 * (1 - R / 255.0) + v4 * (R / 255.0);
            Point v67 = v6 * (1 - R / 255.0) + v7 * (R / 255.0);

            Fill1 = new AnimatablePath4Points(v1, v2, v23, v14) { Fill = RGB.LightCubeBrush };
            this.Children.Add(Fill1.Path);

            Fill2 = new AnimatablePath4Points(v2, v6, v67, v23) { Fill = RGB.DarkCubeBrush };
            this.Children.Add(Fill2.Path);

            ColourCanvas = new Canvas() { Width = 256, Height = 256, ClipToBounds = false };

            ColourImage = new Image() { Source = colourBitmap };
            ColourCanvas.Children.Add(ColourImage);

            PathGeometry border2Geometry = new PathGeometry();
            PathFigure border2Figure = new PathFigure() { StartPoint = new Point(0, 0), IsClosed = false };
            border2Figure.Segments.Add(new LineSegment() { Point = new Point(256, 0) });
            border2Geometry.Figures.Add(border2Figure);
            ColourCanvas.Children.Add(new Path() { Data = border2Geometry, StrokeThickness = 4, Stroke = RGB.LightCubeBrush });

            PathGeometry border3Geometry = new PathGeometry();
            PathFigure border3Figure = new PathFigure() { StartPoint = new Point(256, 0), IsClosed = false };
            border3Figure.Segments.Add(new LineSegment() { Point = new Point(256, 256) });
            border3Geometry.Figures.Add(border3Figure);
            ColourCanvas.Children.Add(new Path() { Data = border3Geometry, StrokeThickness = 4, Stroke = RGB.DarkCubeBrush });


            PositionEllipse = new Ellipse() { Width = 48, Height = 48, Fill = (S < 0.5 && V > 0.5 ? Brushes.Black : Brushes.White) };
            PositionEllipse.RenderTransform = ColorPicker.GetTranslateTransform(G - 24, 255 - B - 24);
            ColourCanvas.Children.Add(PositionEllipse);

            double m11 = (v23.X - v14.X) / 256;
            double m12 = (v67.X - v23.X) / 256;
            double m21 = (v23.Y - v14.Y) / 256;
            double m22 = (v67.Y - v23.Y) / 256;

            ColourCanvasTransform = new AnimatableTransform(new Matrix(m11, m21, m12, m22, v14.X, v14.Y));

            ColourCanvas.RenderTransform = ColourCanvasTransform.MatrixTransform;
            ColourCanvas.RenderTransformOrigin = new RelativePoint(0, 0, RelativeUnit.Absolute);
            this.Children.Add(ColourCanvas);
        }


        public void Update(byte R, byte G, byte B, Bitmap colourBitmap, bool instantTransition)
        {
            (double _, double S, double V) = HSB.RGBToHSB(R / 255.0, G / 255.0, B / 255.0);

            Point v1 = new Point(51.673, 0);
            Point v2 = new Point(104, 15.955);
            Point v3 = new Point(76.327, 46.125);
            Point v4 = new Point(24, 30.170);

            Point v6 = new Point(104, 65.830);
            Point v7 = new Point(76.327, 96);

            Point v23 = v2 * (1 - R / 255.0) + v3 * (R / 255.0);
            Point v14 = v1 * (1 - R / 255.0) + v4 * (R / 255.0);
            Point v67 = v6 * (1 - R / 255.0) + v7 * (R / 255.0);

            Fill1.Update(v1, v2, v23, v14, instantTransition);
            Fill2.Update(v2, v6, v67, v23, instantTransition);

            ColourImage.Source = colourBitmap;

            PositionEllipse.Fill = (S < 0.5 && V > 0.5 ? Brushes.Black : Brushes.White);
            ColorPicker.SetTranslateRenderTransform(PositionEllipse, G - 24, 255 - B - 24, instantTransition);

            double m11 = (v23.X - v14.X) / 256;
            double m12 = (v67.X - v23.X) / 256;
            double m21 = (v23.Y - v14.Y) / 256;
            double m22 = (v67.Y - v23.Y) / 256;

            ColourCanvasTransform.Update(new Matrix(m11, m21, m12, m22, v14.X, v14.Y), instantTransition);
        }
    }


    internal class GCanvas : Canvas
    {
        private AnimatablePath4Points Fill1 { get; }
        private AnimatablePath4Points Fill2 { get; }

        private AnimatableTransform ColourCanvasTransform { get; }

        private Image ColourImage { get; }
        private Canvas ColourCanvas { get; }

        private Ellipse PositionEllipse { get; }

        public GCanvas(byte R, byte G, byte B, Bitmap colourBitmap) : base()
        {
            (double _, double S, double V) = HSB.RGBToHSB(R / 255.0, G / 255.0, B / 255.0);

            this.Width = 128;
            this.Height = 96;

            Point v1 = new Point(51.673, 0);
            Point v2 = new Point(104, 15.955);
            Point v3 = new Point(76.327, 46.125);
            Point v4 = new Point(24, 30.170);

            Point v5 = new Point(51.673, 49.875);
            Point v6 = new Point(104, 65.830);
            Point v7 = new Point(76.327, 96);
            Point v8 = new Point(24, 80.045);

            Point v12 = v1 * (1 - G / 255.0) + v2 * (G / 255.0);
            Point v43 = v4 * (1 - G / 255.0) + v3 * (G / 255.0);
            Point v56 = v5 * (1 - G / 255.0) + v6 * (G / 255.0);
            Point v87 = v8 * (1 - G / 255.0) + v7 * (G / 255.0);


            Fill1 = new AnimatablePath4Points(v1, v12, v43, v4)
            {
                Fill = RGB.LightCubeBrush
            };
            this.Children.Add(Fill1.Path);

            Fill2 = new AnimatablePath4Points(v4, v43, v87, v8)
            {
                Fill = RGB.MiddleCubeBrush
            };
            this.Children.Add(Fill2.Path);

            ColourCanvas = new Canvas() { Width = 256, Height = 256, ClipToBounds = false };

            ColourImage = new Image() { Source = colourBitmap };
            ColourCanvas.Children.Add(ColourImage);

            PathGeometry border2Geometry = new PathGeometry();
            PathFigure border2Figure = new PathFigure() { StartPoint = new Point(0, 0), IsClosed = false };
            border2Figure.Segments.Add(new LineSegment() { Point = new Point(256, 0) });
            border2Geometry.Figures.Add(border2Figure);
            ColourCanvas.Children.Add(new Path() { Data = border2Geometry, StrokeThickness = 4, Stroke = RGB.LightCubeBrush });

            PathGeometry border3Geometry = new PathGeometry();
            PathFigure border3Figure = new PathFigure() { StartPoint = new Point(0, 0), IsClosed = false };
            border3Figure.Segments.Add(new LineSegment() { Point = new Point(0, 256) });
            border3Geometry.Figures.Add(border3Figure);
            ColourCanvas.Children.Add(new Path() { Data = border3Geometry, StrokeThickness = 4, Stroke = RGB.MiddleCubeBrush });

            PositionEllipse = new Ellipse() { Width = 48, Height = 48, Fill = (S < 0.5 && V > 0.5 ? Brushes.Black : Brushes.White) };
            PositionEllipse.RenderTransform = ColorPicker.GetTranslateTransform(255 - R - 24, 255 - B - 24);
            ColourCanvas.Children.Add(PositionEllipse);

            double m11 = (v12.X - v43.X) / 256;
            double m12 = (v56.X - v12.X) / 256;
            double m21 = (v12.Y - v43.Y) / 256;
            double m22 = (v56.Y - v12.Y) / 256;

            ColourCanvasTransform = new AnimatableTransform(new Matrix(m11, m21, m12, m22, v43.X, v43.Y));

            ColourCanvas.RenderTransform = ColourCanvasTransform.MatrixTransform;
            ColourCanvas.RenderTransformOrigin = new RelativePoint(0, 0, RelativeUnit.Absolute);
            this.Children.Add(ColourCanvas);
        }


        public void Update(byte R, byte G, byte B, Bitmap colourBitmap, bool instantTransition)
        {
            (double _, double S, double V) = HSB.RGBToHSB(R / 255.0, G / 255.0, B / 255.0);

            Point v1 = new Point(51.673, 0);
            Point v2 = new Point(104, 15.955);
            Point v3 = new Point(76.327, 46.125);
            Point v4 = new Point(24, 30.170);

            Point v5 = new Point(51.673, 49.875);
            Point v6 = new Point(104, 65.830);
            Point v7 = new Point(76.327, 96);
            Point v8 = new Point(24, 80.045);

            Point v12 = v1 * (1 - G / 255.0) + v2 * (G / 255.0);
            Point v43 = v4 * (1 - G / 255.0) + v3 * (G / 255.0);
            Point v56 = v5 * (1 - G / 255.0) + v6 * (G / 255.0);
            Point v87 = v8 * (1 - G / 255.0) + v7 * (G / 255.0);

            Fill1.Update(v1, v12, v43, v4, instantTransition);
            Fill2.Update(v4, v43, v87, v8, instantTransition);

            ColourImage.Source = colourBitmap;

            PositionEllipse.Fill = (S < 0.5 && V > 0.5 ? Brushes.Black : Brushes.White);
            ColorPicker.SetTranslateRenderTransform(PositionEllipse, 255 - R - 24, 255 - B - 24, instantTransition);

            double m11 = (v12.X - v43.X) / 256;
            double m12 = (v56.X - v12.X) / 256;
            double m21 = (v12.Y - v43.Y) / 256;
            double m22 = (v56.Y - v12.Y) / 256;

            ColourCanvasTransform.Update(new Matrix(m11, m21, m12, m22, v43.X, v43.Y), instantTransition);
        }
    }


    internal class BCanvas : Canvas
    {
        private AnimatablePath4Points Fill1 { get; }
        private AnimatablePath4Points Fill2 { get; }

        private AnimatableTransform ColourCanvasTransform { get; }

        private Image ColourImage { get; }
        private Canvas ColourCanvas { get; }

        private Ellipse PositionEllipse { get; }

        public BCanvas(byte R, byte G, byte B, Bitmap colourBitmap) : base()
        {
            (double _, double S, double V) = HSB.RGBToHSB(R / 255.0, G / 255.0, B / 255.0);

            this.Width = 128;
            this.Height = 96;

            Point v1 = new Point(51.673, 0);
            Point v2 = new Point(104, 15.955);
            Point v3 = new Point(76.327, 46.125);
            Point v4 = new Point(24, 30.170);

            Point v5 = new Point(51.673, 49.875);
            Point v6 = new Point(104, 65.830);
            Point v7 = new Point(76.327, 96);
            Point v8 = new Point(24, 80.045);

            Point v15 = v1 * (B / 255.0) + v5 * (1 - B / 255.0);
            Point v26 = v2 * (B / 255.0) + v6 * (1 - B / 255.0);
            Point v37 = v3 * (B / 255.0) + v7 * (1 - B / 255.0);
            Point v48 = v4 * (B / 255.0) + v8 * (1 - B / 255.0);

            Fill1 = new AnimatablePath4Points(v48, v8, v7, v37)
            {
                Fill = RGB.MiddleCubeBrush
            };
            this.Children.Add(Fill1.Path);

            Fill2 = new AnimatablePath4Points(v37, v7, v6, v26)
            {
                Fill = RGB.DarkCubeBrush
            };
            this.Children.Add(Fill2.Path);

            ColourCanvas = new Canvas() { Width = 256, Height = 256, ClipToBounds = false };

            ColourImage = new Image() { Source = colourBitmap };
            ColourCanvas.Children.Add(ColourImage);

            PathGeometry border2Geometry = new PathGeometry();
            PathFigure border2Figure = new PathFigure() { StartPoint = new Point(0, 256), IsClosed = false };
            border2Figure.Segments.Add(new LineSegment() { Point = new Point(256, 256) });
            border2Geometry.Figures.Add(border2Figure);
            ColourCanvas.Children.Add(new Path() { Data = border2Geometry, StrokeThickness = 4, Stroke = RGB.MiddleCubeBrush });

            PathGeometry border3Geometry = new PathGeometry();
            PathFigure border3Figure = new PathFigure() { StartPoint = new Point(256, 256), IsClosed = false };
            border3Figure.Segments.Add(new LineSegment() { Point = new Point(256, 0) });
            border3Geometry.Figures.Add(border3Figure);
            ColourCanvas.Children.Add(new Path() { Data = border3Geometry, StrokeThickness = 4, Stroke = RGB.DarkCubeBrush });

            PositionEllipse = new Ellipse() { Width = 48, Height = 48, Fill = (S < 0.5 && V > 0.5 ? Brushes.Black : Brushes.White) };
            PositionEllipse.RenderTransform = ColorPicker.GetTranslateTransform(G - 24, R - 24);
            ColourCanvas.Children.Add(PositionEllipse);

            double m11 = (v26.X - v15.X) / 256;
            double m12 = (v37.X - v26.X) / 256;
            double m21 = (v26.Y - v15.Y) / 256;
            double m22 = (v37.Y - v26.Y) / 256;

            ColourCanvasTransform = new AnimatableTransform(new Matrix(m11, m21, m12, m22, v15.X, v15.Y));

            ColourCanvas.RenderTransform = ColourCanvasTransform.MatrixTransform;
            ColourCanvas.RenderTransformOrigin = new RelativePoint(0, 0, RelativeUnit.Absolute);
            this.Children.Add(ColourCanvas);
        }


        public void Update(byte R, byte G, byte B, Bitmap colourBitmap, bool instantTransition)
        {
            (double _, double S, double V) = HSB.RGBToHSB(R / 255.0, G / 255.0, B / 255.0);

            Point v1 = new Point(51.673, 0);
            Point v2 = new Point(104, 15.955);
            Point v3 = new Point(76.327, 46.125);
            Point v4 = new Point(24, 30.170);

            Point v5 = new Point(51.673, 49.875);
            Point v6 = new Point(104, 65.830);
            Point v7 = new Point(76.327, 96);
            Point v8 = new Point(24, 80.045);

            Point v15 = v1 * (B / 255.0) + v5 * (1 - B / 255.0);
            Point v26 = v2 * (B / 255.0) + v6 * (1 - B / 255.0);
            Point v37 = v3 * (B / 255.0) + v7 * (1 - B / 255.0);
            Point v48 = v4 * (B / 255.0) + v8 * (1 - B / 255.0);

            Fill1.Update(v48, v8, v7, v37, instantTransition);
            Fill2.Update(v37, v7, v6, v26, instantTransition);

            ColourImage.Source = colourBitmap;

            PositionEllipse.Fill = (S < 0.5 && V > 0.5 ? Brushes.Black : Brushes.White);
            ColorPicker.SetTranslateRenderTransform(PositionEllipse, G - 24, R - 24, instantTransition);

            double m11 = (v26.X - v15.X) / 256;
            double m12 = (v37.X - v26.X) / 256;
            double m21 = (v26.Y - v15.Y) / 256;
            double m22 = (v37.Y - v26.Y) / 256;


            ColourCanvasTransform.Update(new Matrix(m11, m21, m12, m22, v15.X, v15.Y), instantTransition);
        }
    }
}
