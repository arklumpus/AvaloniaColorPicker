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

using Avalonia.Media;
using System;

namespace AvaloniaColorPicker
{
    // Colour blindness simulation algorithm adapted from https://ixora.io/projects/colorblindness/color-blindness-simulation-research/
    internal static class ColourBlindness
    {
        private static (double l, double m, double s) ToLMS(Color col)
        {
            double dR = col.R / 255.0;
            double dG = col.G / 255.0;
            double dB = col.B / 255.0;

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

            double l = 0.31399022 * r + 0.63951294 * g + 0.04649755 * b;
            double m = 0.15537241 * r + 0.75789446 * g + 0.08670142 * b;
            double s = 0.01775239 * r + 0.10944209 * g + 0.87256922 * b;

            return (l, m, s);
        }

        private static Color FromLMS(double l, double m, double s)
        {
            double r = 5.47221206 * l - 4.6419601 * m + 0.16963708 * s;
            double g = -1.1252419 * l + 2.29317094 * m - 0.1678952 * s;
            double b = 0.02980165 * l - 0.19318073 * m + 1.16364789 * s;

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

            return Color.FromRgb((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
        }


        public static Color Protanopia(Color col)
        {
            (double _, double m, double s) = ToLMS(col);

            double l1 = 1.05118294 * m - 0.05116099 * s;
            double m1 = m;
            double s1 = s;

            return FromLMS(l1, m1, s1);
        }

        public static Color Deuteranopia(Color col)
        {
            (double l, double _, double s) = ToLMS(col);

            double l1 = l;
            double m1 = l * 0.9513092 + s * 0.04866992;
            double s1 = s;

            return FromLMS(l1, m1, s1);
        }

        public static Color Tritanopia(Color col)
        {
            (double l, double m, double _) = ToLMS(col);

            double l1 = l;
            double m1 = m;
            double s1 = -0.86744736 * l + 1.86727089 * m;

            return FromLMS(l1, m1, s1);
        }

        public static Color ConeAchromatopsia(Color col)
        {
            double dR = col.R / 255.0;
            double dG = col.G / 255.0;
            double dB = col.B / 255.0;

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

            double w = 0.01775 * r + 0.10945 * g + 0.87262 * b;

            r = w;
            g = w;
            b = w;

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

            return Color.FromRgb((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
        }

        public static Color RodAchromatopsia(Color col)
        {
            double dR = col.R / 255.0;
            double dG = col.G / 255.0;
            double dB = col.B / 255.0;

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

            double w = 0.212656 * r + 0.715158 * g + 0.072186 * b;

            r = w;
            g = w;
            b = w;

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

            return Color.FromRgb((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
        }


        public static Color NormalVision(Color col)
        {
            return col;
        }
    }
}
