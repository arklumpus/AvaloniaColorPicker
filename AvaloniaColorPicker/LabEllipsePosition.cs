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

using System;
using System.Collections.Generic;
using System.Text;

namespace AvaloniaColorPicker
{
    internal static partial class Lab
    {
        public enum LabComponents { L, a, b };

        private static readonly double[,] RotationMatrixInverse = new double[3, 3] { { 0.09175170953613726, -0.9082482904638628, -0.40824829046386196 }, { -0.9082482904638628, 0.09175170953613725, -0.40824829046386196 }, { 0.40824829046386196, 0.40824829046386196, -0.8164965809277254 } };

        static double[] MultiplyVector(double[,] matrix, double[] vector)
        {
            return new double[]
            {
                vector[0] * matrix[0, 0] + vector[1] * matrix[0, 1] + vector[2] * matrix[0, 2],
                vector[0] * matrix[1, 0] + vector[1] * matrix[1, 1] + vector[2] * matrix[1, 2],
                vector[0] * matrix[2, 0] + vector[1] * matrix[2, 1] + vector[2] * matrix[2, 2],
            };
        }

        public static (double x, double y, double w, double h) GetEllipsePosition(double L, double a, double b, LabComponents constantComponent)
        {
            (double centerX, double centerY) = Project(L, a, b);

            double w = 0;
            double h = 0;

            const int stepCount = 8;
            const double r = 0.1;

            if (constantComponent == LabComponents.L)
            {
                double minX = double.MaxValue;
                double minY = double.MaxValue;
                double maxX = double.MinValue;
                double maxY = double.MinValue;

                for (int step = 0; step < stepCount; step++)
                {
                    double theta = 2 * Math.PI / stepCount * step;

                    (double projX, double projY) = Project(L, a + r * Math.Cos(theta), b + r * Math.Sin(theta));
                    minX = Math.Min(minX, projX);
                    maxX = Math.Max(maxX, projX);
                    minY = Math.Min(minY, projY);
                    maxY = Math.Max(maxY, projY);
                }

                w = maxX - minX;
                h = maxY - minY;
            }
            else if (constantComponent == LabComponents.a)
            {
                double minX = double.MaxValue;
                double minY = double.MaxValue;
                double maxX = double.MinValue;
                double maxY = double.MinValue;

                for (int step = 0; step < stepCount; step++)
                {
                    double theta = 2 * Math.PI / stepCount * step;

                    (double projX, double projY) = Project(L + r * Math.Cos(theta), a, b + r * Math.Sin(theta));
                    minX = Math.Min(minX, projX);
                    maxX = Math.Max(maxX, projX);
                    minY = Math.Min(minY, projY);
                    maxY = Math.Max(maxY, projY);
                }

                w = maxX - minX;
                h = maxY - minY;
            }
            else if (constantComponent == LabComponents.b)
            {
                double minX = double.MaxValue;
                double minY = double.MaxValue;
                double maxX = double.MinValue;
                double maxY = double.MinValue;

                for (int step = 0; step < stepCount; step++)
                {
                    double theta = 2 * Math.PI / stepCount * step;

                    (double projX, double projY) = Project(L + r * Math.Cos(theta), a + r * Math.Sin(theta), b);
                    minX = Math.Min(minX, projX);
                    maxX = Math.Max(maxX, projX);
                    minY = Math.Min(minY, projY);
                    maxY = Math.Max(maxY, projY);
                }

                w = maxX - minX;
                h = maxY - minY;
            }

            return (centerX, centerY, w, h);
        }

        static (double x, double y) Project(double L, double a, double b)
        {
            double[] newVect = new double[] { 1.25 * a, -1.25 * b, L - 0.75 };
            double[] XYZ = MultiplyVector(RotationMatrixInverse, newVect);

            double x = (1 + XYZ[0]) / 2 * 95;
            double y = (1 + XYZ[1]) / 2 * 95;

            return (x, y);
        }
    }
}
