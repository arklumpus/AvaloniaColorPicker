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
using System;
using System.IO.Compression;
using System.Reflection;

namespace AvaloniaColorPicker
{
    internal static partial class Lab
    {
        public static byte[] LabImageData = new byte[96 * 96 * 4 * 129 * 3];

        static Lab()
        {
            using (GZipStream decompressionStream = new GZipStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("AvaloniaColorPicker.LabColorSpace.bin"), CompressionMode.Decompress))
            {
                int countRead = 0;

                while (countRead < LabImageData.Length)
                {
                    countRead += decompressionStream.Read(LabImageData, countRead, LabImageData.Length - countRead);
                }
            }
        }

        internal static Avalonia.Media.Imaging.WriteableBitmap GetAB(double L)
        {
            Avalonia.Media.Imaging.WriteableBitmap bmp = new Avalonia.Media.Imaging.WriteableBitmap(new PixelSize(256, 256), new Vector(96, 96), Avalonia.Platform.PixelFormat.Rgba8888, Avalonia.Platform.AlphaFormat.Unpremul);

            using (var fb = bmp.Lock())
            {
                unsafe
                {
                    byte* rgbaValues = (byte*)fb.Address;

                    byte[] mask = new byte[256 * 256];

                    for (int x = 0; x < 256; x++)
                    {
                        for (int y = 0; y < 256; y++)
                        {
                            FromLab(L, (1 - 2 * y / 255.0) * 1, (2 * x / 255.0 - 1) * 1,
                                out rgbaValues[x * 4 + y * 256 * 4],
                                out rgbaValues[x * 4 + y * 256 * 4 + 1],
                                out rgbaValues[x * 4 + y * 256 * 4 + 2],
                                out rgbaValues[x * 4 + y * 256 * 4 + 3]
                                );
                        }
                    }


                    FillMask(rgbaValues, mask);

                    for (int x = 0; x < 256; x++)
                    {
                        for (int y = 0; y < 256; y++)
                        {
                            if (mask[x + y * 256] == 255)
                            {
                                byte R = (byte)(255 - rgbaValues[x * 4 + y * 256 * 4]);
                                byte G = (byte)(255 - rgbaValues[x * 4 + y * 256 * 4 + 1]);
                                byte B = (byte)(255 - rgbaValues[x * 4 + y * 256 * 4 + 2]);

                                rgbaValues[x * 4 + y * 256 * 4] = R;
                                rgbaValues[x * 4 + y * 256 * 4 + 1] = G;
                                rgbaValues[x * 4 + y * 256 * 4 + 2] = B;

                                rgbaValues[x * 4 + y * 256 * 4 + 3] = 255;
                            }
                            else if (mask[x + y * 256] > 0)
                            {
                                rgbaValues[x * 4 + y * 256 * 4 + 3] = 255;
                                byte R = (byte)(255 - rgbaValues[x * 4 + y * 256 * 4]);
                                byte G = (byte)(255 - rgbaValues[x * 4 + y * 256 * 4 + 1]);
                                byte B = (byte)(255 - rgbaValues[x * 4 + y * 256 * 4 + 2]);
                                Blend(ref rgbaValues[x * 4 + y * 256 * 4], ref rgbaValues[x * 4 + y * 256 * 4 + 1], ref rgbaValues[x * 4 + y * 256 * 4 + 2], ref rgbaValues[x * 4 + y * 256 * 4 + 3], R, G, B, mask[x + y * 256]);
                            }
                            else
                            {
                                rgbaValues[x * 4 + y * 256 * 4 + 3] = 255;
                            }
                        }
                    }
                }
            }
            return bmp;
        }

        internal static Avalonia.Media.Imaging.WriteableBitmap GetL(double a, double b)
        {
            Avalonia.Media.Imaging.WriteableBitmap bmp = new Avalonia.Media.Imaging.WriteableBitmap(new PixelSize(1, 256), new Vector(96, 96), Avalonia.Platform.PixelFormat.Rgba8888, Avalonia.Platform.AlphaFormat.Unpremul);

            int minY = int.MaxValue;
            int maxY = int.MinValue;

            using (var fb = bmp.Lock())
            {
                unsafe
                {
                    byte* rgbaValues = (byte*)fb.Address;
                    for (int y = 0; y < 256; y++)
                    {
                        FromLab(1 - y / 255.0, a, b,
                            out rgbaValues[y * 4],
                            out rgbaValues[y * 4 + 1],
                            out rgbaValues[y * 4 + 2],
                            out rgbaValues[y * 4 + 3]
                            );

                        if (rgbaValues[y * 4 + 3] > 0)
                        {
                            minY = Math.Min(minY, y);
                            maxY = Math.Max(maxY, y);
                        }

                        rgbaValues[y * 4 + 3] = 255;
                    }

                    if (minY < int.MaxValue && minY > 2 && minY < 255)
                    {
                        byte R = (byte)(255 - rgbaValues[(minY - 1) * 4]);
                        byte G = (byte)(255 - rgbaValues[(minY - 1) * 4 + 1]);
                        byte B = (byte)(255 - rgbaValues[(minY - 1) * 4 + 2]);

                        rgbaValues[(minY - 1) * 4] = R;
                        rgbaValues[(minY - 1) * 4 + 1] = G;
                        rgbaValues[(minY - 1) * 4 + 2] = B;

                        R = (byte)(255 - rgbaValues[(minY - 2) * 4]);
                        G = (byte)(255 - rgbaValues[(minY - 2) * 4 + 1]);
                        B = (byte)(255 - rgbaValues[(minY - 2) * 4 + 2]);

                        rgbaValues[(minY - 2) * 4] = R;
                        rgbaValues[(minY - 2) * 4 + 1] = G;
                        rgbaValues[(minY - 2) * 4 + 2] = B;
                    }

                    if (maxY < int.MaxValue && maxY < 254 && maxY > 0)
                    {
                        byte R = (byte)(255 - rgbaValues[(maxY + 1) * 4]);
                        byte G = (byte)(255 - rgbaValues[(maxY + 1) * 4 + 1]);
                        byte B = (byte)(255 - rgbaValues[(maxY + 1) * 4 + 2]);

                        rgbaValues[(maxY + 1) * 4] = R;
                        rgbaValues[(maxY + 1) * 4 + 1] = G;
                        rgbaValues[(maxY + 1) * 4 + 2] = B;

                        R = (byte)(255 - rgbaValues[(maxY + 2) * 4]);
                        G = (byte)(255 - rgbaValues[(maxY + 2) * 4 + 1]);
                        B = (byte)(255 - rgbaValues[(maxY + 2) * 4 + 2]);

                        rgbaValues[(maxY + 2) * 4] = R;
                        rgbaValues[(maxY + 2) * 4 + 1] = G;
                        rgbaValues[(maxY + 2) * 4 + 2] = B;
                    }
                }
            }
            return bmp;
        }

        internal static Avalonia.Media.Imaging.WriteableBitmap GetLB(double a)
        {
            Avalonia.Media.Imaging.WriteableBitmap bmp = new Avalonia.Media.Imaging.WriteableBitmap(new PixelSize(256, 256), new Vector(96, 96), Avalonia.Platform.PixelFormat.Rgba8888, Avalonia.Platform.AlphaFormat.Unpremul);

            using (var fb = bmp.Lock())
            {
                unsafe
                {
                    byte* rgbaValues = (byte*)fb.Address;

                    byte[] mask = new byte[256 * 256];

                    for (int x = 0; x < 256; x++)
                    {
                        for (int y = 0; y < 256; y++)
                        {
                            FromLab((1 - y / 255.0) * 1, a, (2 * x / 255.0 - 1) * 1,
                                out rgbaValues[x * 4 + y * 256 * 4],
                                out rgbaValues[x * 4 + y * 256 * 4 + 1],
                                out rgbaValues[x * 4 + y * 256 * 4 + 2],
                                out rgbaValues[x * 4 + y * 256 * 4 + 3]
                                );
                        }
                    }


                    FillMask(rgbaValues, mask);

                    for (int x = 0; x < 256; x++)
                    {
                        for (int y = 0; y < 256; y++)
                        {
                            if (mask[x + y * 256] == 255)
                            {
                                byte R = (byte)(255 - rgbaValues[x * 4 + y * 256 * 4]);
                                byte G = (byte)(255 - rgbaValues[x * 4 + y * 256 * 4 + 1]);
                                byte B = (byte)(255 - rgbaValues[x * 4 + y * 256 * 4 + 2]);

                                rgbaValues[x * 4 + y * 256 * 4] = R;
                                rgbaValues[x * 4 + y * 256 * 4 + 1] = G;
                                rgbaValues[x * 4 + y * 256 * 4 + 2] = B;
                                rgbaValues[x * 4 + y * 256 * 4 + 3] = 255;
                            }
                            else if (mask[x + y * 256] > 0)
                            {
                                rgbaValues[x * 4 + y * 256 * 4 + 3] = 255;
                                byte R = (byte)(255 - rgbaValues[x * 4 + y * 256 * 4]);
                                byte G = (byte)(255 - rgbaValues[x * 4 + y * 256 * 4 + 1]);
                                byte B = (byte)(255 - rgbaValues[x * 4 + y * 256 * 4 + 2]);
                                Blend(ref rgbaValues[x * 4 + y * 256 * 4], ref rgbaValues[x * 4 + y * 256 * 4 + 1], ref rgbaValues[x * 4 + y * 256 * 4 + 2], ref rgbaValues[x * 4 + y * 256 * 4 + 3], R, G, B, mask[x + y * 256]);
                            }
                            else
                            {
                                rgbaValues[x * 4 + y * 256 * 4 + 3] = 255;
                            }
                        }
                    }
                }
            }
            return bmp;
        }

        internal static Avalonia.Media.Imaging.WriteableBitmap GetA(double L, double b)
        {
            Avalonia.Media.Imaging.WriteableBitmap bmp = new Avalonia.Media.Imaging.WriteableBitmap(new PixelSize(1, 256), new Vector(96, 96), Avalonia.Platform.PixelFormat.Rgba8888, Avalonia.Platform.AlphaFormat.Unpremul);

            int minY = int.MaxValue;
            int maxY = int.MinValue;

            using (var fb = bmp.Lock())
            {
                unsafe
                {
                    byte* rgbaValues = (byte*)fb.Address;
                    for (int y = 0; y < 256; y++)
                    {
                        FromLab(L, 1 - 2 * y / 255.0, b,
                            out rgbaValues[y * 4],
                            out rgbaValues[y * 4 + 1],
                            out rgbaValues[y * 4 + 2],
                            out rgbaValues[y * 4 + 3]
                            );
                        if (rgbaValues[y * 4 + 3] > 0)
                        {
                            minY = Math.Min(minY, y);
                            maxY = Math.Max(maxY, y);
                        }

                        rgbaValues[y * 4 + 3] = 255;
                    }

                    if (minY < int.MaxValue && minY > 2 && minY < 255)
                    {
                        byte R = (byte)(255 - rgbaValues[(minY - 1) * 4]);
                        byte G = (byte)(255 - rgbaValues[(minY - 1) * 4 + 1]);
                        byte B = (byte)(255 - rgbaValues[(minY - 1) * 4 + 2]);

                        rgbaValues[(minY - 1) * 4] = R;
                        rgbaValues[(minY - 1) * 4 + 1] = G;
                        rgbaValues[(minY - 1) * 4 + 2] = B;

                        R = (byte)(255 - rgbaValues[(minY - 2) * 4]);
                        G = (byte)(255 - rgbaValues[(minY - 2) * 4 + 1]);
                        B = (byte)(255 - rgbaValues[(minY - 2) * 4 + 2]);

                        rgbaValues[(minY - 2) * 4] = R;
                        rgbaValues[(minY - 2) * 4 + 1] = G;
                        rgbaValues[(minY - 2) * 4 + 2] = B;
                    }

                    if (maxY < int.MaxValue && maxY < 254 && maxY > 0)
                    {
                        byte R = (byte)(255 - rgbaValues[(maxY + 1) * 4]);
                        byte G = (byte)(255 - rgbaValues[(maxY + 1) * 4 + 1]);
                        byte B = (byte)(255 - rgbaValues[(maxY + 1) * 4 + 2]);

                        rgbaValues[(maxY + 1) * 4] = R;
                        rgbaValues[(maxY + 1) * 4 + 1] = G;
                        rgbaValues[(maxY + 1) * 4 + 2] = B;

                        R = (byte)(255 - rgbaValues[(maxY + 2) * 4]);
                        G = (byte)(255 - rgbaValues[(maxY + 2) * 4 + 1]);
                        B = (byte)(255 - rgbaValues[(maxY + 2) * 4 + 2]);

                        rgbaValues[(maxY + 2) * 4] = R;
                        rgbaValues[(maxY + 2) * 4 + 1] = G;
                        rgbaValues[(maxY + 2) * 4 + 2] = B;
                    }
                }
            }
            return bmp;
        }

        internal static Avalonia.Media.Imaging.WriteableBitmap GetLA(double b)
        {
            Avalonia.Media.Imaging.WriteableBitmap bmp = new Avalonia.Media.Imaging.WriteableBitmap(new PixelSize(256, 256), new Vector(96, 96), Avalonia.Platform.PixelFormat.Rgba8888, Avalonia.Platform.AlphaFormat.Unpremul);

            using (var fb = bmp.Lock())
            {
                unsafe
                {
                    byte* rgbaValues = (byte*)fb.Address;

                    byte[] mask = new byte[256 * 256];

                    for (int x = 0; x < 256; x++)
                    {
                        for (int y = 0; y < 256; y++)
                        {
                            FromLab((1 - x / 255.0) * 1, (1 - 2 * y / 255.0) * 1, b,
                                out rgbaValues[x * 4 + y * 256 * 4],
                                out rgbaValues[x * 4 + y * 256 * 4 + 1],
                                out rgbaValues[x * 4 + y * 256 * 4 + 2],
                                out rgbaValues[x * 4 + y * 256 * 4 + 3]
                                );
                        }
                    }


                    FillMask(rgbaValues, mask);

                    for (int x = 0; x < 256; x++)
                    {
                        for (int y = 0; y < 256; y++)
                        {
                            if (mask[x + y * 256] == 255)
                            {
                                byte R = (byte)(255 - rgbaValues[x * 4 + y * 256 * 4]);
                                byte G = (byte)(255 - rgbaValues[x * 4 + y * 256 * 4 + 1]);
                                byte B = (byte)(255 - rgbaValues[x * 4 + y * 256 * 4 + 2]);

                                rgbaValues[x * 4 + y * 256 * 4] = R;
                                rgbaValues[x * 4 + y * 256 * 4 + 1] = G;
                                rgbaValues[x * 4 + y * 256 * 4 + 2] = B;
                                rgbaValues[x * 4 + y * 256 * 4 + 3] = 255;
                            }
                            else if (mask[x + y * 256] > 0)
                            {
                                rgbaValues[x * 4 + y * 256 * 4 + 3] = 255;
                                byte R = (byte)(255 - rgbaValues[x * 4 + y * 256 * 4]);
                                byte G = (byte)(255 - rgbaValues[x * 4 + y * 256 * 4 + 1]);
                                byte B = (byte)(255 - rgbaValues[x * 4 + y * 256 * 4 + 2]);
                                Blend(ref rgbaValues[x * 4 + y * 256 * 4], ref rgbaValues[x * 4 + y * 256 * 4 + 1], ref rgbaValues[x * 4 + y * 256 * 4 + 2], ref rgbaValues[x * 4 + y * 256 * 4 + 3], R, G, B, mask[x + y * 256]);
                            }
                            else
                            {
                                rgbaValues[x * 4 + y * 256 * 4 + 3] = 255;
                            }
                        }
                    }
                }
            }
            return bmp;
        }

        internal static Avalonia.Media.Imaging.WriteableBitmap GetB(double L, double a)
        {
            Avalonia.Media.Imaging.WriteableBitmap bmp = new Avalonia.Media.Imaging.WriteableBitmap(new PixelSize(1, 256), new Vector(96, 96), Avalonia.Platform.PixelFormat.Rgba8888, Avalonia.Platform.AlphaFormat.Unpremul);

            int minY = int.MaxValue;
            int maxY = int.MinValue;

            using (var fb = bmp.Lock())
            {
                unsafe
                {
                    byte* rgbaValues = (byte*)fb.Address;
                    for (int y = 0; y < 256; y++)
                    {
                        FromLab(L, a, 1 - 2 * y / 255.0,
                            out rgbaValues[y * 4],
                            out rgbaValues[y * 4 + 1],
                            out rgbaValues[y * 4 + 2],
                            out rgbaValues[y * 4 + 3]
                            );
                        if (rgbaValues[y * 4 + 3] > 0)
                        {
                            minY = Math.Min(minY, y);
                            maxY = Math.Max(maxY, y);
                        }

                        rgbaValues[y * 4 + 3] = 255;
                    }

                    if (minY < int.MaxValue && minY > 2 && minY < 255)
                    {
                        byte R = (byte)(255 - rgbaValues[(minY - 1) * 4]);
                        byte G = (byte)(255 - rgbaValues[(minY - 1) * 4 + 1]);
                        byte B = (byte)(255 - rgbaValues[(minY - 1) * 4 + 2]);

                        rgbaValues[(minY - 1) * 4] = R;
                        rgbaValues[(minY - 1) * 4 + 1] = G;
                        rgbaValues[(minY - 1) * 4 + 2] = B;

                        R = (byte)(255 - rgbaValues[(minY - 2) * 4]);
                        G = (byte)(255 - rgbaValues[(minY - 2) * 4 + 1]);
                        B = (byte)(255 - rgbaValues[(minY - 2) * 4 + 2]);

                        rgbaValues[(minY - 2) * 4] = R;
                        rgbaValues[(minY - 2) * 4 + 1] = G;
                        rgbaValues[(minY - 2) * 4 + 2] = B;
                    }

                    if (maxY < int.MaxValue && maxY < 254 && maxY > 0)
                    {
                        byte R = (byte)(255 - rgbaValues[(maxY + 1) * 4]);
                        byte G = (byte)(255 - rgbaValues[(maxY + 1) * 4 + 1]);
                        byte B = (byte)(255 - rgbaValues[(maxY + 1) * 4 + 2]);

                        rgbaValues[(maxY + 1) * 4] = R;
                        rgbaValues[(maxY + 1) * 4 + 1] = G;
                        rgbaValues[(maxY + 1) * 4 + 2] = B;

                        R = (byte)(255 - rgbaValues[(maxY + 2) * 4]);
                        G = (byte)(255 - rgbaValues[(maxY + 2) * 4 + 1]);
                        B = (byte)(255 - rgbaValues[(maxY + 2) * 4 + 2]);

                        rgbaValues[(maxY + 2) * 4] = R;
                        rgbaValues[(maxY + 2) * 4 + 1] = G;
                        rgbaValues[(maxY + 2) * 4 + 2] = B;
                    }
                }
            }
            return bmp;
        }


        internal static unsafe void FillMask(byte* rgbaValues, byte[] mask)
        {
            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < 256; y++)
                {
                    if (rgbaValues[x * 4 + y * 256 * 4 + 3] > 0)
                    {
                        bool left = x > 0 && rgbaValues[(x - 1) * 4 + y * 256 * 4 + 3] > 0;
                        bool top = y > 0 && rgbaValues[x * 4 + (y - 1) * 256 * 4 + 3] > 0;
                        bool right = x < 255 && rgbaValues[(x + 1) * 4 + y * 256 * 4 + 3] > 0;
                        bool bottom = y < 255 && rgbaValues[x * 4 + (y + 1) * 256 * 4 + 3] > 0;

                        if (!left)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                if ((x - 1 - i) >= 0)
                                {
                                    mask[(x - 1 - i) + y * 256] = 255;
                                }
                            }
                        }

                        if (!top)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                if ((y - 1 - i) >= 0)
                                {
                                    mask[x + (y - 1 - i) * 256] = 255;
                                }
                            }
                        }

                        if (!right)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                if ((x + 1 + i) <= 255)
                                {
                                    mask[(x + 1 + i) + y * 256] = 255;
                                }
                            }
                        }

                        if (!bottom)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                if ((y + 1 + i) <= 255)
                                {
                                    mask[x + (y + 1 + i) * 256] = 255;
                                }
                            }
                        }

                        if (!top && !left)
                        {
                            for (int i = 0; i < 1; i++)
                            {
                                if ((x - 1 - i) >= 0 && (y - 1 - i) >= 0)
                                {
                                    mask[(x - 1 - i) + (y - 1 - i) * 256] = 255;
                                }
                            }
                        }

                        if (!top && !right)
                        {
                            for (int i = 0; i < 1; i++)
                            {
                                if ((x + 1 + i) <= 255 && (y - 1 - i) >= 0)
                                {
                                    mask[(x + 1 + i) + (y - 1 - i) * 256] = 255;
                                }
                            }
                        }

                        if (!bottom && !left)
                        {
                            for (int i = 0; i < 1; i++)
                            {
                                if ((x - 1 - i) >= 0 && (y + 1 + i) <= 255)
                                {
                                    mask[(x - 1 - i) + (y + 1 + i) * 256] = 255;
                                }
                            }
                        }

                        if (!bottom && !right)
                        {
                            for (int i = 0; i < 1; i++)
                            {
                                if ((x + 1 + i) <= 255 && (y + 1 + i) <= 255)
                                {
                                    mask[(x + 1 + i) + (y + 1 + i) * 256] = 255;
                                }
                            }
                        }
                    }
                }
            }

            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < 256; y++)
                {
                    bool left = x > 0 && mask[(x - 1) + y * 256] == 255;
                    bool top = y > 0 && mask[x + (y - 1) * 256] == 255;
                    bool right = x < 255 && mask[(x + 1) + y * 256] == 255;
                    bool bottom = y < 255 && mask[x + (y + 1) * 256] == 255;

                    if (top && left)
                    {
                        if ((x - 1) >= 0 && (y - 1) >= 0)
                        {
                            mask[(x - 1) + (y - 1) * 256] = Math.Max(mask[(x - 1) + (y - 1) * 256], (byte)128);
                        }
                    }

                    if (top && right)
                    {
                        if ((x + 1) <= 255 && (y - 1) >= 0)
                        {
                            mask[(x + 1) + (y - 1) * 256] = Math.Max(mask[(x + 1) + (y - 1) * 256], (byte)128);
                        }
                    }

                    if (bottom && left)
                    {
                        if ((x - 1) >= 0 && (y + 1) <= 255)
                        {
                            mask[(x - 1) + (y + 1) * 256] = Math.Max(mask[(x - 1) + (y + 1) * 256], (byte)128);
                        }
                    }

                    if (bottom && right)
                    {
                        if ((x + 1) <= 255 && (y + 1) <= 255)
                        {
                            mask[(x + 1) + (y + 1) * 256] = Math.Max(mask[(x + 1) + (y + 1) * 256], (byte)128);
                        }
                    }
                }
            }

            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < 256; y++)
                {
                    bool left = x > 0 && mask[(x - 1) + y * 256] >= 128;
                    bool top = y > 0 && mask[x + (y - 1) * 256] >= 128;
                    bool right = x < 255 && mask[(x + 1) + y * 256] >= 128;
                    bool bottom = y < 255 && mask[x + (y + 1) * 256] >= 128;

                    if (top && left)
                    {
                        if ((x - 1) >= 0 && (y - 1) >= 0)
                        {
                            mask[(x - 1) + (y - 1) * 256] = Math.Max(mask[(x - 1) + (y - 1) * 256], (byte)64);
                        }
                    }

                    if (top && right)
                    {
                        if ((x + 1) <= 255 && (y - 1) >= 0)
                        {
                            mask[(x + 1) + (y - 1) * 256] = Math.Max(mask[(x + 1) + (y - 1) * 256], (byte)64);
                        }
                    }

                    if (bottom && left)
                    {
                        if ((x - 1) >= 0 && (y + 1) <= 255)
                        {
                            mask[(x - 1) + (y + 1) * 256] = Math.Max(mask[(x - 1) + (y + 1) * 256], (byte)64);
                        }
                    }

                    if (bottom && right)
                    {
                        if ((x + 1) <= 255 && (y + 1) <= 255)
                        {
                            mask[(x + 1) + (y + 1) * 256] = Math.Max(mask[(x + 1) + (y + 1) * 256], (byte)64);
                        }
                    }
                }
            }
        }

        static void Blend(ref byte backgroundR, ref byte backgroundG, ref byte backgroundB, ref byte backgroundA, byte sourceR, byte sourceG, byte sourceB, byte sourceA)
        {
            byte outA = (byte)(sourceA + backgroundA * (255 - sourceA) / 255);

            if (outA == 0)
            {
                backgroundR = backgroundG = backgroundB = backgroundA = 0;
            }
            else
            {
                backgroundR = (byte)Math.Max(0, Math.Min(255, ((sourceR * sourceA + backgroundR * backgroundA * (255 - sourceA) / 255) / outA)));
                backgroundG = (byte)Math.Max(0, Math.Min(255, ((sourceG * sourceA + backgroundG * backgroundA * (255 - sourceA) / 255) / outA)));
                backgroundB = (byte)Math.Max(0, Math.Min(255, ((sourceB * sourceA + backgroundB * backgroundA * (255 - sourceA) / 255) / outA)));
                backgroundA = outA;
            }
        }

        public static (double X, double Y, double Z) ToXYZ(Color color)
        {
            return ToXYZ(color.R, color.G, color.B);
        }

        public static (double X, double Y, double Z) ToXYZ(byte R, byte G, byte B)
        {
            double dR = R / 255.0;
            double dG = G / 255.0;
            double dB = B / 255.0;

            double r, g, b;

            if (dR <= 0.04045)
            {
                r = 25 * dR / 323;
            }
            else
            {
                r = Math.Pow((200 * dR + 11) / 211, 2.4);
            }

            if (dG <= 0.04045)
            {
                g = 25 * dG / 323;
            }
            else
            {
                g = Math.Pow((200 * dG + 11) / 211, 2.4);
            }

            if (dB <= 0.04045)
            {
                b = 25 * dB / 323;
            }
            else
            {
                b = Math.Pow((200 * dB + 11) / 211, 2.4);
            }

            double x = 0.41239080 * r + 0.35758434 * g + 0.18048079 * b;
            double y = 0.21263901 * r + 0.71516868 * g + 0.07219232 * b;
            double z = 0.01933082 * r + 0.11919478 * g + 0.95053215 * b;

            return (x, y, z);
        }

        public static (double R, double G, double B) FromXYZ(double x, double y, double z)
        {
            double r = +3.24096994 * x - 1.53738318 * y - 0.49861076 * z;
            double g = -0.96924364 * x + 1.8759675 * y + 0.04155506 * z;
            double b = 0.05563008 * x - 0.20397696 * y + 1.05697151 * z;

            if (r <= 0.0031308)
            {
                r = 323 * r / 25;
            }
            else
            {
                r = (211 * Math.Pow(r, 1 / 2.4) - 11) / 200;
            }

            if (g <= 0.0031308)
            {
                g = 323 * g / 25;
            }
            else
            {
                g = (211 * Math.Pow(g, 1 / 2.4) - 11) / 200;
            }

            if (b <= 0.0031308)
            {
                b = 323 * b / 25;
            }
            else
            {
                b = (211 * Math.Pow(b, 1 / 2.4) - 11) / 200;
            }

            r = Math.Min(Math.Max(0, r), 1);
            g = Math.Min(Math.Max(0, g), 1);
            b = Math.Min(Math.Max(0, b), 1);

            return (r, g, b);
        }

        public static void FromXYZ(double x, double y, double z, out byte R, out byte G, out byte B)
        {
            double r = +3.24096994 * x - 1.53738318 * y - 0.49861076 * z;
            double g = -0.96924364 * x + 1.8759675 * y + 0.04155506 * z;
            double b = 0.05563008 * x - 0.20397696 * y + 1.05697151 * z;

            if (r <= 0.0031308)
            {
                r = 323 * r / 25;
            }
            else
            {
                r = (211 * Math.Pow(r, 1 / 2.4) - 11) / 200;
            }

            if (g <= 0.0031308)
            {
                g = 323 * g / 25;
            }
            else
            {
                g = (211 * Math.Pow(g, 1 / 2.4) - 11) / 200;
            }

            if (b <= 0.0031308)
            {
                b = 323 * b / 25;
            }
            else
            {
                b = (211 * Math.Pow(b, 1 / 2.4) - 11) / 200;
            }

            r = Math.Max(0, Math.Min(1, r));
            g = Math.Max(0, Math.Min(1, g));
            b = Math.Max(0, Math.Min(1, b));

            R = (byte)Math.Round(r * 255);
            G = (byte)Math.Round(g * 255);
            B = (byte)Math.Round(b * 255);
        }

        public static void FromXYZ(double x, double y, double z, out byte R, out byte G, out byte B, out byte A)
        {
            double r = +3.24096994 * x - 1.53738318 * y - 0.49861076 * z;
            double g = -0.96924364 * x + 1.8759675 * y + 0.04155506 * z;
            double b = 0.05563008 * x - 0.20397696 * y + 1.05697151 * z;

            if (r <= 0.0031308)
            {
                r = 323 * r / 25;
            }
            else
            {
                r = (211 * Math.Pow(r, 1 / 2.4) - 11) / 200;
            }

            if (g <= 0.0031308)
            {
                g = 323 * g / 25;
            }
            else
            {
                g = (211 * Math.Pow(g, 1 / 2.4) - 11) / 200;
            }

            if (b <= 0.0031308)
            {
                b = 323 * b / 25;
            }
            else
            {
                b = (211 * Math.Pow(b, 1 / 2.4) - 11) / 200;
            }



            if (r >= 0 && r <= 1 && g >= 0 && g <= 1 && b >= 0 && b <= 1)
            {
                A = 255;
                R = (byte)Math.Round(r * 255);
                G = (byte)Math.Round(g * 255);
                B = (byte)Math.Round(b * 255);
            }
            else
            {
                r = Math.Max(0, Math.Min(1, r));
                g = Math.Max(0, Math.Min(1, g));
                b = Math.Max(0, Math.Min(1, b));
                R = (byte)Math.Round(r * 255);
                G = (byte)Math.Round(g * 255);
                B = (byte)Math.Round(b * 255);
                A = 0;
            }


        }

        public static (double L, double a, double b) ToLab(Color color)
        {
            return ToLab(color.R, color.G, color.B);
        }

        public static (double L, double a, double b) ToLab(byte R, byte G, byte B)
        {
            double f(double t)
            {
                const double d = 6.0 / 29;

                if (t > d * d * d)
                {
                    return Math.Pow(t, 1.0 / 3);
                }
                else
                {
                    return t / (3 * d * d) + 4.0 / 29;
                }
            }

            const double xN = 0.950489;
            const double yN = 1;
            const double zN = 1.088840;

            (double x, double y, double z) = ToXYZ(R, G, B);

            double fY = f(y / yN);

            double l = 1.16 * fY - 0.16;
            double a = 5 * (f(x / xN) - fY);
            double b = 2 * (fY - f(z / zN));

            return (l, a, b);
        }

        public static (double R, double G, double B) FromLab(double L, double a, double b)
        {
            double f(double t)
            {
                const double d = 6.0 / 29;

                if (t > d)
                {
                    return t * t * t;
                }
                else
                {
                    return 3 * d * d * (t - 4.0 / 29);
                }
            }

            const double xN = 0.950489;
            const double yN = 1;
            const double zN = 1.088840;

            double x = xN * f((L + 0.16) / 1.16 + a / 5);
            double y = yN * f((L + 0.16) / 1.16);
            double z = zN * f((L + 0.16) / 1.16 - b / 2);

            return FromXYZ(x, y, z);
        }

        public static void FromLab(double L, double a, double b, out byte R, out byte G, out byte B)
        {
            double f(double t)
            {
                const double d = 6.0 / 29;

                if (t > d)
                {
                    return t * t * t;
                }
                else
                {
                    return 3 * d * d * (t - 4.0 / 29);
                }
            }

            const double xN = 0.950489;
            const double yN = 1;
            const double zN = 1.088840;

            double x = xN * f((L + 0.16) / 1.16 + a / 5);
            double y = yN * f((L + 0.16) / 1.16);
            double z = zN * f((L + 0.16) / 1.16 - b / 2);

            FromXYZ(x, y, z, out R, out G, out B);
        }

        public static void FromLab(double L, double a, double b, out byte R, out byte G, out byte B, out byte A)
        {
            double f(double t)
            {
                const double d = 6.0 / 29;

                if (t > d)
                {
                    return t * t * t;
                }
                else
                {
                    return 3 * d * d * (t - 4.0 / 29);
                }
            }

            const double xN = 0.950489;
            const double yN = 1;
            const double zN = 1.088840;

            double x = xN * f((L + 0.16) / 1.16 + a / 5);
            double y = yN * f((L + 0.16) / 1.16);
            double z = zN * f((L + 0.16) / 1.16 - b / 2);

            FromXYZ(x, y, z, out R, out G, out B, out A);
        }


        public static Brush WarningBrush = new SolidColorBrush(Color.FromRgb(255, 204, 77));


        public static Canvas GetLCanvas(double L, double a, double b, Canvas OldCanvas, bool instantTransition, IOutsideRGBWarning warningTooltip)
        {
            if (OldCanvas is LabCanvas lCanvas)
            {
                lCanvas.Update(L, a, b, LabComponents.L, instantTransition);
                return lCanvas;
            }
            else
            {
                return new LabCanvas(L, a, b, LabComponents.L, warningTooltip);
            }
        }

        public static Canvas GetACanvas(double L, double a, double b, Canvas OldCanvas, bool instantTransition, IOutsideRGBWarning warningTooltip)
        {
            if (OldCanvas is LabCanvas aCanvas)
            {
                aCanvas.Update(L, a, b, LabComponents.a, instantTransition);
                return aCanvas;
            }
            else
            {
                return new LabCanvas(L, a, b, LabComponents.a, warningTooltip);
            }
        }

        public static Canvas GetBCanvas(double L, double a, double b, Canvas OldCanvas, bool instantTransition, IOutsideRGBWarning warningTooltip)
        {
            if (OldCanvas is LabCanvas bCanvas)
            {
                bCanvas.Update(L, a, b, LabComponents.b, instantTransition);
                return bCanvas;
            }
            else
            {
                return new LabCanvas(L, a, b, LabComponents.b, warningTooltip);
            }
        }


        public static Canvas GetACanvasOld(double L, double a, double b)
        {
            Canvas can = new Canvas() { Width = 128, Height = 96 };

            Canvas colourCanvas = new Canvas() { Width = 96, Height = 96, Margin = new Thickness(16, 0, 16, 0) };
            WriteableBitmap bitmap = new WriteableBitmap(new PixelSize(96, 96), new Vector(96, 96), Avalonia.Platform.PixelFormat.Rgba8888, Avalonia.Platform.AlphaFormat.Unpremul);

            int normA = Math.Max(0, Math.Min(128, 64 + (int)Math.Round(a * 64.0)));

            int offset = 96 * 96 * 4 * normA + 96 * 96 * 129 * 4;

            using (var fb = bitmap.Lock())
            {
                unsafe
                {
                    byte* rgbaValues = (byte*)fb.Address;

                    for (int i = 0; i < 96 * 96 * 4; i++)
                    {
                        rgbaValues[i] = LabImageData[offset + i];
                    }
                }
            }

            Image labImage = new Image() { Width = 96, Height = 96, Source = bitmap };
            colourCanvas.Children.Add(labImage);
            can.Children.Add(colourCanvas);

            Lab.FromLab(L, a, b, out _, out _, out _, out byte A);

            if (A > 0)
            {
                (double x, double y, double w, double h) = GetEllipsePosition(L, a, b, LabComponents.a);

                double C = Math.Sqrt(a * a + b * b);
                double S = C / Math.Sqrt(C * C + L * L);
                Ellipse ell = new Ellipse() { Width = w, Height = h, Fill = (S < 0.5 && L > 0.5 ? Brushes.Black : Brushes.White) };
                ell.RenderTransform = new TranslateTransform(x - w * 0.5, y - h * 0.5);
                colourCanvas.Children.Add(ell);
            }
            else
            {
                Canvas warningCanvas = new Canvas() { Width = 32, Height = 32, RenderTransform = new TranslateTransform(0, 64) };

                PathGeometry triangleGeometry = new PathGeometry();
                PathFigure triangleFigure = new PathFigure() { IsClosed = true, IsFilled = true, StartPoint = new Point(2, 29) };
                triangleFigure.Segments.Add(new LineSegment() { Point = new Point(29, 29) });
                triangleFigure.Segments.Add(new LineSegment() { Point = new Point(16, 2) });
                triangleGeometry.Figures.Add(triangleFigure);

                warningCanvas.Children.Add(new Avalonia.Controls.Shapes.Path() { Data = triangleGeometry, StrokeThickness = 3, Fill = WarningBrush, Stroke = WarningBrush, StrokeLineCap = PenLineCap.Round, StrokeJoin = PenLineJoin.Round });

                PathGeometry exclamationMark = new PathGeometry();
                PathFigure shaftFigure = new PathFigure() { IsClosed = false, IsFilled = false, StartPoint = new Point(16, 11.5) };
                shaftFigure.Segments.Add(new LineSegment() { Point = new Point(16, 20) });
                exclamationMark.Figures.Add(shaftFigure);
                warningCanvas.Children.Add(new Avalonia.Controls.Shapes.Path() { Data = exclamationMark, StrokeThickness = 3, Stroke = Brushes.Black, StrokeLineCap = PenLineCap.Round });

                warningCanvas.Children.Add(new Ellipse() { Width = 3, Height = 3, RenderTransform = new TranslateTransform(16 - 1.5, 24), Fill = Brushes.Black });

                //ToolTip.SetTip(warningCanvas, "The colour falls outside of the sRGB colour space!");

                warningCanvas.PointerEntered += (s, e) =>
                {
                    ((Control)can.Parent).FindControl<Grid>("WarningTooltip").Opacity = 1;
                };

                warningCanvas.PointerExited += (s, e) =>
                {
                    ((Control)can.Parent).FindControl<Grid>("WarningTooltip").Opacity = 0;
                };

                can.Children.Add(warningCanvas);
            }

            return can;
        }

        public static Canvas GetBCanvasOld(double L, double a, double b)
        {
            Canvas can = new Canvas() { Width = 128, Height = 96 };

            Canvas colourCanvas = new Canvas() { Width = 96, Height = 96, Margin = new Thickness(16, 0, 16, 0) };
            WriteableBitmap bitmap = new WriteableBitmap(new PixelSize(96, 96), new Vector(96, 96), Avalonia.Platform.PixelFormat.Rgba8888, Avalonia.Platform.AlphaFormat.Unpremul);

            int normB = Math.Max(0, Math.Min(128, 64 + (int)Math.Round(b * 64.0)));

            int offset = 96 * 96 * 4 * normB + 96 * 96 * 129 * 4 * 2;

            using (var fb = bitmap.Lock())
            {
                unsafe
                {
                    byte* rgbaValues = (byte*)fb.Address;

                    for (int i = 0; i < 96 * 96 * 4; i++)
                    {
                        rgbaValues[i] = LabImageData[offset + i];
                    }
                }
            }

            Image labImage = new Image() { Width = 96, Height = 96, Source = bitmap };
            colourCanvas.Children.Add(labImage);
            can.Children.Add(colourCanvas);

            Lab.FromLab(L, a, b, out _, out _, out _, out byte A);

            if (A > 0)
            {
                (double x, double y, double w, double h) = GetEllipsePosition(L, a, b, LabComponents.b);

                double C = Math.Sqrt(a * a + b * b);
                double S = C / Math.Sqrt(C * C + L * L);
                Ellipse ell = new Ellipse() { Width = w, Height = h, Fill = (S < 0.5 && L > 0.5 ? Brushes.Black : Brushes.White) };
                ell.RenderTransform = new TranslateTransform(x - w * 0.5, y - h * 0.5);
                colourCanvas.Children.Add(ell);
            }
            else
            {
                Canvas warningCanvas = new Canvas() { Width = 32, Height = 32, RenderTransform = new TranslateTransform(0, 64) };

                PathGeometry triangleGeometry = new PathGeometry();
                PathFigure triangleFigure = new PathFigure() { IsClosed = true, IsFilled = true, StartPoint = new Point(2, 29) };
                triangleFigure.Segments.Add(new LineSegment() { Point = new Point(29, 29) });
                triangleFigure.Segments.Add(new LineSegment() { Point = new Point(16, 2) });
                triangleGeometry.Figures.Add(triangleFigure);

                warningCanvas.Children.Add(new Avalonia.Controls.Shapes.Path() { Data = triangleGeometry, StrokeThickness = 3, Fill = WarningBrush, Stroke = WarningBrush, StrokeLineCap = PenLineCap.Round, StrokeJoin = PenLineJoin.Round });

                PathGeometry exclamationMark = new PathGeometry();
                PathFigure shaftFigure = new PathFigure() { IsClosed = false, IsFilled = false, StartPoint = new Point(16, 11.5) };
                shaftFigure.Segments.Add(new LineSegment() { Point = new Point(16, 20) });
                exclamationMark.Figures.Add(shaftFigure);
                warningCanvas.Children.Add(new Avalonia.Controls.Shapes.Path() { Data = exclamationMark, StrokeThickness = 3, Stroke = Brushes.Black, StrokeLineCap = PenLineCap.Round });

                warningCanvas.Children.Add(new Ellipse() { Width = 3, Height = 3, RenderTransform = new TranslateTransform(16 - 1.5, 24), Fill = Brushes.Black });

                //ToolTip.SetTip(warningCanvas, "The colour falls outside of the sRGB colour space!");

                warningCanvas.PointerEntered += (s, e) =>
                {
                    ((Control)can.Parent).FindControl<Grid>("WarningTooltip").Opacity = 1;
                };

                warningCanvas.PointerExited += (s, e) =>
                {
                    ((Control)can.Parent).FindControl<Grid>("WarningTooltip").Opacity = 0;
                };

                can.Children.Add(warningCanvas);
            }


            return can;
        }


        public static (int size, double a, double b) GetSizeAndCentroid(double L)
        {
            int size = 0;

            int meanA = 0;
            int meanB = 0;


            for (int a = -100; a <= 100; a++)
            {
                for (int b = -100; b <= 100; b++)
                {
                    Lab.FromLab(L, a / 100.0, b / 100.0, out _, out _, out _, out byte A);

                    if (A > 0)
                    {
                        meanA += a;
                        meanB += b;
                        size++;
                    }
                }
            }

            return (size, meanA / 100.0 / size, meanB / 100.0 / size);
        }

        // CIEDE2000 adapted from https://en.wikipedia.org/wiki/Color_difference#CIELAB_%CE%94E* 
        public static double DeltaE(double L1, double a1, double b1, double L2, double a2, double b2)
        {
            L1 *= 100;
            a1 *= 100;
            b1 *= 100;

            L2 *= 100;
            a2 *= 100;
            b2 *= 100;

            double DLp = L2 - L1;
            double LpBar = (L1 + L2) * 0.5;

            double C1 = Math.Sqrt(a1 * a1 + b1 * b1);
            double C2 = Math.Sqrt(a2 * a2 + b2 * b2);

            double CBar = (C1 + C2) * 0.5;

            double a1p = a1 + 0.5 * a1 * (1 - Math.Sqrt(Math.Pow(CBar, 7) / (Math.Pow(CBar, 7) + Math.Pow(25, 7))));
            double a2p = a2 + 0.5 * a2 * (1 - Math.Sqrt(Math.Pow(CBar, 7) / (Math.Pow(CBar, 7) + Math.Pow(25, 7))));

            double C1p = Math.Sqrt(a1p * a1p + b1 * b1);
            double C2p = Math.Sqrt(a2p * a2p + b2 * b2);

            double CpBar = (C1p + C2p) * 0.5;

            double DCp = C2p - C1p;

            double h1p = Math.Atan2(b1, a1p);
            if (h1p < 0)
            {
                h1p += 2 * Math.PI;
            }
            h1p *= 180 / Math.PI;

            if (b1 == 0 && a1p == 0)
            {
                h1p = 0;
            }

            double h2p = Math.Atan2(b2, a2p);
            if (h2p < 0)
            {
                h2p += 2 * Math.PI;
            }
            h2p *= 180 / Math.PI;

            if (b2 == 0 && a2p == 0)
            {
                h2p = 0;
            }

            double Dhp;

            if (C1p == 0 || C2p == 0)
            {
                Dhp = 0;
            }
            else
            {
                if (Math.Abs(h1p - h2p) <= 180)
                {
                    Dhp = h2p - h1p;
                }
                else if (h2p <= h1p)
                {
                    Dhp = h2p - h1p + 360;
                }
                else
                {
                    Dhp = h2p - h1p - 360;
                }
            }

            double DHp = 2 * Math.Sqrt(C1p * C2p) * Math.Sin(Dhp * 0.5 * Math.PI / 180);

            double Hpbar;

            if (C1p == 0 || C2p == 0)
            {
                Hpbar = h1p + h2p;
            }
            else
            {
                if (Math.Abs(h1p - h2p) <= 180)
                {
                    Hpbar = (h2p + h1p) * 0.5;
                }
                else if (h2p + h1p < 360)
                {
                    Hpbar = (h2p + h1p + 360) * 0.5;
                }
                else
                {
                    Hpbar = (h2p + h1p - 360) * 0.5;
                }
            }

            double T = 1 - 0.17 * Math.Cos((Hpbar - 30) / 180 * Math.PI) + 0.24 * Math.Cos(2 * Hpbar / 180 * Math.PI) + 0.32 * Math.Cos((3 * Hpbar + 6) / 180 * Math.PI) - 0.20 * Math.Cos((4 * Hpbar - 63) / 180 * Math.PI);

            double SL = 1 + (0.015 * (LpBar - 50) * (LpBar - 50)) / Math.Sqrt(20 + (LpBar - 50) * (LpBar - 50));

            double SC = 1 + 0.045 * CpBar;

            double SH = 1 + 0.015 * CpBar * T;

            double RT = -2 * Math.Sqrt(Math.Pow(CpBar, 7) / (Math.Pow(CpBar, 7) + Math.Pow(25, 7))) * Math.Sin((60 * Math.Exp(-((Hpbar - 275) / 25) * ((Hpbar - 275) / 25))) / 180 * Math.PI);

            double DeltaE = Math.Sqrt(DLp / SL * DLp / SL + DCp / SC * DCp / SC + DHp / SH * DHp / SH + RT * DCp * DHp / SC / SH);

            return DeltaE;
        }

        static double Ease(double x)
        {
            const double g = 0.01;
            return Math.Log(x - Math.Sqrt(0.2e1) * (System.Double)g / (System.Double)(-1 + g)) / Math.Log((System.Double)g) - (0.2e1 * Math.Log(-0.1e1 / (System.Double)(-1 + g)) + Math.Log(0.2e1)) / Math.Log((System.Double)g) / 0.2e1;
        }

        public static (double L, double a, double b) GetContrastingColor(double L1, double a1, double b1)
        {
            int LSteps = 51;
            const int HSteps = 51;

            double C = Math.Sqrt(a1 * a1 + b1 * b1);

            double minL = L1 - L1 * Ease(C);
            double maxL = L1 + (1 - L1) * Ease(C);

            FromLab(L1, a1, b1, out byte R1, out byte G1, out byte B1);
            Color original = Color.FromRgb(R1, G1, B1);
            Color protanopia = ColourBlindness.Protanopia(original);
            Color deuteranopia = ColourBlindness.Deuteranopia(original);

            (double L1P, double a1P, double b1P) = ToLab(protanopia);
            (double L1D, double a1D, double b1D) = ToLab(deuteranopia);

            double maxDist = 0;
            (double L, double a, double b) maxDistCol = (L1, a1, b1);

            for (int l = 0; l < LSteps; l++)
            {

                for (int h = 0; h < HSteps; h++)
                {
                    double L = minL + (maxL - minL) * l / (LSteps - 1);
                    double H = (double)h / (HSteps - 1) * 2 * Math.PI;

                    double a = C * Math.Cos(H);
                    double b = C * Math.Sin(H);

                    FromLab(L, a, b, out byte R, out byte G, out byte B);

                    Color newCol = Color.FromRgb(R, G, B);
                    Color newProtanopia = ColourBlindness.Protanopia(newCol);
                    Color newDeuteranopia = ColourBlindness.Deuteranopia(newCol);

                    (L, a, b) = ToLab(R, G, B);

                    (double LP, double aP, double bP) = ToLab(newProtanopia);
                    (double LD, double aD, double bD) = ToLab(newDeuteranopia);

                    double dist1 = DeltaE(L1, a1, b1, L, a, b);
                    double dist2 = DeltaE(L1P, a1P, b1P, LP, aP, bP);
                    double dist3 = DeltaE(L1D, a1D, b1D, LD, aD, bD);

                    double dist = dist1 + dist2 + dist3;

                    if (dist >= maxDist)
                    {
                        maxDist = dist;
                        maxDistCol = (L, a, b);
                    }
                }
            }

            return maxDistCol;
        }
    }

    internal class LabCanvas : Canvas
    {
        Canvas WarningCanvas { get; }

        AnimatableLABCanvas LABCanvas { get; }

        public LabCanvas(double L, double a, double b, Lab.LabComponents labComponent, IOutsideRGBWarning warningTooltip)
        {
            this.Width = 128;
            this.Height = 96;

            Canvas colourCanvas = new Canvas() { Width = 96, Height = 96, Margin = new Thickness(16, 0, 16, 0) };

            LABCanvas = new AnimatableLABCanvas(L, a, b, labComponent);
            colourCanvas.Children.Add(LABCanvas.LabImage);
            this.Children.Add(colourCanvas);

            Lab.FromLab(L, a, b, out _, out _, out _, out byte A);

            WarningCanvas = new Canvas() { Width = 32, Height = 32, RenderTransform = new TranslateTransform(0, 64), IsVisible = false };

            PathGeometry triangleGeometry = new PathGeometry();
            PathFigure triangleFigure = new PathFigure() { IsClosed = true, IsFilled = true, StartPoint = new Point(2, 29) };
            triangleFigure.Segments.Add(new LineSegment() { Point = new Point(29, 29) });
            triangleFigure.Segments.Add(new LineSegment() { Point = new Point(16, 2) });
            triangleGeometry.Figures.Add(triangleFigure);

            WarningCanvas.Children.Add(new Avalonia.Controls.Shapes.Path() { Data = triangleGeometry, StrokeThickness = 3, Fill = Lab.WarningBrush, Stroke = Lab.WarningBrush, StrokeLineCap = PenLineCap.Round, StrokeJoin = PenLineJoin.Round });

            PathGeometry exclamationMark = new PathGeometry();
            PathFigure shaftFigure = new PathFigure() { IsClosed = false, IsFilled = false, StartPoint = new Point(16, 11.5) };
            shaftFigure.Segments.Add(new LineSegment() { Point = new Point(16, 20) });
            exclamationMark.Figures.Add(shaftFigure);
            WarningCanvas.Children.Add(new Avalonia.Controls.Shapes.Path() { Data = exclamationMark, StrokeThickness = 3, Stroke = Brushes.Black, StrokeLineCap = PenLineCap.Round });

            WarningCanvas.Children.Add(new Ellipse() { Width = 3, Height = 3, RenderTransform = new TranslateTransform(16 - 1.5, 24), Fill = Brushes.Black });

            WarningCanvas.PointerEntered += (s, e) =>
            {
                if (warningTooltip != null)
                {
                    warningTooltip.Opacity = 1;
                }
            };

            WarningCanvas.PointerExited += (s, e) =>
            {
                if (warningTooltip != null)
                {
                    warningTooltip.Opacity = 0;
                }
            };

            this.Children.Add(WarningCanvas);

            colourCanvas.Children.Add(LABCanvas.PositionEllipse);

            if (A > 0)
            {
                WarningCanvas.Opacity = 1;
            }
            else
            {
                WarningCanvas.Opacity = 0;
            }
        }

        public void Update(double L, double a, double b, Lab.LabComponents labComponent, bool instantTransition)
        {
            LABCanvas.Update(L, a, b, labComponent, instantTransition);

            Lab.FromLab(L, a, b, out _, out _, out _, out byte A);

            if (A > 0)
            {
                WarningCanvas.IsVisible = false;
            }
            else
            {
                WarningCanvas.IsVisible = true;
            }
        }
    }
}
