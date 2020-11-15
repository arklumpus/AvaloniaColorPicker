using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace LabColorSpaceCache
{
    class Program
    {
        static readonly int Resolution = 96;

        static void Main()
        {
            double[] zAxis = new double[] { -0.5, -0.5, -1 };
            double modulus = Math.Sqrt(zAxis[0] * zAxis[0] + zAxis[1] * zAxis[1] + zAxis[2] * zAxis[2]);
            zAxis[0] /= modulus;
            zAxis[1] /= modulus;
            zAxis[2] /= modulus;
            RotationMatrix = RotationToAlignWithZ(zAxis);

            byte[] basicShape = GetImageBytes(GetImagePositions(), MultiplyVector(RotationMatrix, Normalize(new double[] { 0, -0.5, 1 })));

            CreateCachedImages(basicShape);
        }

        static void CreateCachedImages(byte[] basicShape)
        {
            object lockObject = new object();
            int count = 0;
            int lastPerc = 0;

            Console.WriteLine("0%");

            byte[] labImageData = new byte[Resolution * Resolution * 4 * 129 * 3];

            Parallel.For(0, 129, L =>
            {
                int offset = Resolution * Resolution * 4 * L;

                double l = L / 128.0;

                byte[] rgbaValues = GetImageL(l, basicShape);


                for (int i = 0; i < Resolution * Resolution * 4; i++)
                {
                    labImageData[offset + i] = rgbaValues[i];
                }

                lock (lockObject)
                {
                    count++;
                    int perc = (int)Math.Round((double)count / (129 * 3) * 100);

                    if (perc > lastPerc)
                    {
                        lastPerc = perc;
                        Console.WriteLine(perc.ToString() + "%");
                    }
                }
            });

            Parallel.For(-64, 65, A =>
            {
                int offset = Resolution * Resolution * 4 * (A + 64 + 129);

                double a = A / 64.0;

                byte[] rgbaValues = GetImageA(a, basicShape);

                for (int i = 0; i < Resolution * Resolution * 4; i++)
                {
                    labImageData[offset + i] = rgbaValues[i];
                }

                lock (lockObject)
                {
                    count++;
                    int perc = (int)Math.Round((double)count / (129 * 3) * 100);

                    if (perc > lastPerc)
                    {
                        lastPerc = perc;
                        Console.WriteLine(perc.ToString() + "%");
                    }
                }
            });

            Parallel.For(-64, 65, B =>
            {
                int offset = Resolution * Resolution * 4 * (B + 64 + 129 + 129);

                double b = B / 64.0;

                byte[] rgbaValues = GetImageB(b, basicShape);

                for (int i = 0; i < Resolution * Resolution * 4; i++)
                {
                    labImageData[offset + i] = rgbaValues[i];
                }

                lock (lockObject)
                {
                    count++;
                    int perc = (int)Math.Round((double)count / (129 * 3) * 100);

                    if (perc > lastPerc)
                    {
                        lastPerc = perc;
                        Console.WriteLine(perc.ToString() + "%");
                    }
                }
            });

            using FileStream compressedFile = new FileStream("LabColorSpace.bin", FileMode.Create);
            using GZipStream compressionStream = new GZipStream(compressedFile, CompressionLevel.Optimal);
            compressionStream.Write(labImageData, 0, labImageData.Length);
        }

        static double[] Normalize(double[] vector)
        {
            double modulus = Math.Sqrt(vector[0] * vector[0] + vector[1] * vector[1] + vector[2] * vector[2]);

            return new double[] { vector[0] / modulus, vector[1] / modulus, vector[2] / modulus };
        }

        static byte[] GetImageL(double fixedL, byte[] basicShape)
        {
            int width = Resolution;
            int height = Resolution;
            int depth = 256;

            double minL = double.MaxValue;
            double maxL = double.MinValue;

            byte[] rgbaValues = new byte[width * height * 4];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    double X = (x / (double)(width - 1)) * 2 - 1;
                    double Y = (y / (double)(height - 1)) * 2 - 1;

                    for (int z = depth - 1; z >= 0; z--)
                    {
                        double Z = (z / (double)(depth - 1)) * 2 - 1;

                        double[] newVect = MultiplyVector(RotationMatrix, new double[] { X, Y, Z });

                        double L = newVect[2] + 0.75;

                        minL = Math.Min(L, minL);
                        maxL = Math.Max(L, maxL);


                        if (Math.Abs(L - fixedL) <= 0.01)
                        {
                            double a = (newVect[0]) / 1.25;
                            double b = (-newVect[1]) / 1.25;

                            FromLab(L, a, b, out byte R, out byte G, out byte B, out byte A);

                            if (A > 0)
                            {
                                Blend(R, G, B, A, ref rgbaValues[x * 4 + y * width * 4], ref rgbaValues[x * 4 + y * width * 4 + 1], ref rgbaValues[x * 4 + y * width * 4 + 2], ref rgbaValues[x * 4 + y * width * 4 + 3]);

                                if (rgbaValues[x * 4 + y * width * 4 + 3] == 255)
                                {
                                    break;
                                }
                            }
                        }
                        else if (L >= fixedL && L - fixedL <= 0.02)
                        {
                            double a = (newVect[0]) / 1.25;
                            double b = (-newVect[1]) / 1.25;

                            FromLab(L, a, b, out byte R, out byte G, out byte B, out byte A);

                            if (A > 0)
                            {
                                Blend(R, G, B, (byte)(A / 2), ref rgbaValues[x * 4 + y * width * 4], ref rgbaValues[x * 4 + y * width * 4 + 1], ref rgbaValues[x * 4 + y * width * 4 + 2], ref rgbaValues[x * 4 + y * width * 4 + 3]);

                                if (rgbaValues[x * 4 + y * width * 4 + 3] == 255)
                                {
                                    break;
                                }
                            }
                        }
                        else if (L >= fixedL)
                        {
                            double a = (newVect[0]) / 1.25;
                            double b = (-newVect[1]) / 1.25;

                            FromLab(L, a, b, out byte _, out byte _, out byte _, out byte A);

                            byte R = basicShape[x * 4 + y * width * 4];
                            byte G = basicShape[x * 4 + y * width * 4 + 1];
                            byte B = basicShape[x * 4 + y * width * 4 + 2];
                            byte A2 = basicShape[x * 4 + y * width * 4 + 3];

                            if (A > 0 && A2 > 0)
                            {
                                Blend(R, G, B, A, ref rgbaValues[x * 4 + y * width * 4], ref rgbaValues[x * 4 + y * width * 4 + 1], ref rgbaValues[x * 4 + y * width * 4 + 2], ref rgbaValues[x * 4 + y * width * 4 + 3]);

                                if (rgbaValues[x * 4 + y * width * 4 + 3] == 255)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return rgbaValues;
        }

        static byte[] GetImageA(double fixedA, byte[] basicShape)
        {
            int width = Resolution;
            int height = Resolution;
            int depth = 256;

            byte[] rgbaValues = new byte[width * height * 4];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    double X = (x / (double)(width - 1)) * 2 - 1;
                    double Y = (y / (double)(height - 1)) * 2 - 1;

                    for (int z = depth - 1; z >= 0; z--)
                    {
                        double Z = (z / (double)(depth - 1)) * 2 - 1;

                        double[] newVect = MultiplyVector(RotationMatrix, new double[] { X, Y, Z });

                        double L = newVect[2] + 0.75;

                        double a = (newVect[0]) / 1.25;
                        double b = (-newVect[1]) / 1.25;


                        if (Math.Abs(a - fixedA) <= 0.01)
                        {
                            FromLab(L, a, b, out byte R, out byte G, out byte B, out byte A);

                            if (A > 0)
                            {
                                Blend(R, G, B, A, ref rgbaValues[x * 4 + y * width * 4], ref rgbaValues[x * 4 + y * width * 4 + 1], ref rgbaValues[x * 4 + y * width * 4 + 2], ref rgbaValues[x * 4 + y * width * 4 + 3]);

                                if (rgbaValues[x * 4 + y * width * 4 + 3] == 255)
                                {
                                    break;
                                }
                            }
                        }
                        else if (a <= fixedA && a - fixedA >= -0.02)
                        {
                            FromLab(L, a, b, out byte R, out byte G, out byte B, out byte A);

                            if (A > 0)
                            {
                                Blend(R, G, B, (byte)(A / 2), ref rgbaValues[x * 4 + y * width * 4], ref rgbaValues[x * 4 + y * width * 4 + 1], ref rgbaValues[x * 4 + y * width * 4 + 2], ref rgbaValues[x * 4 + y * width * 4 + 3]);

                                if (rgbaValues[x * 4 + y * width * 4 + 3] == 255)
                                {
                                    break;
                                }
                            }
                        }
                        else if (a <= fixedA)
                        {
                            FromLab(L, a, b, out byte _, out byte _, out byte _, out byte A);

                            byte R = basicShape[x * 4 + y * width * 4];
                            byte G = basicShape[x * 4 + y * width * 4 + 1];
                            byte B = basicShape[x * 4 + y * width * 4 + 2];
                            byte A2 = basicShape[x * 4 + y * width * 4 + 3];

                            if (A > 0 && A2 > 0)
                            {
                                Blend(R, G, B, A, ref rgbaValues[x * 4 + y * width * 4], ref rgbaValues[x * 4 + y * width * 4 + 1], ref rgbaValues[x * 4 + y * width * 4 + 2], ref rgbaValues[x * 4 + y * width * 4 + 3]);

                                if (rgbaValues[x * 4 + y * width * 4 + 3] == 255)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return rgbaValues;
        }

        static byte[] GetImageB(double fixedB, byte[] basicShape)
        {
            int width = Resolution;
            int height = Resolution;
            int depth = 256;

            byte[] rgbaValues = new byte[width * height * 4];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    double X = (x / (double)(width - 1)) * 2 - 1;
                    double Y = (y / (double)(height - 1)) * 2 - 1;

                    for (int z = depth - 1; z >= 0; z--)
                    {
                        double Z = (z / (double)(depth - 1)) * 2 - 1;

                        double[] newVect = MultiplyVector(RotationMatrix, new double[] { X, Y, Z });

                        double L = newVect[2] + 0.75;

                        double a = (newVect[0]) / 1.25;
                        double b = (-newVect[1]) / 1.25;


                        if (Math.Abs(b - fixedB) <= 0.01)
                        {
                            FromLab(L, a, b, out byte R, out byte G, out byte B, out byte A);

                            if (A > 0)
                            {
                                Blend(R, G, B, A, ref rgbaValues[x * 4 + y * width * 4], ref rgbaValues[x * 4 + y * width * 4 + 1], ref rgbaValues[x * 4 + y * width * 4 + 2], ref rgbaValues[x * 4 + y * width * 4 + 3]);
                            }
                        }
                        else if (b >= fixedB && b - fixedB <= 0.02)
                        {
                            FromLab(L, a, b, out byte R, out byte G, out byte B, out byte A);

                            if (A > 0)
                            {
                                Blend(R, G, B, (byte)(A / 2), ref rgbaValues[x * 4 + y * width * 4], ref rgbaValues[x * 4 + y * width * 4 + 1], ref rgbaValues[x * 4 + y * width * 4 + 2], ref rgbaValues[x * 4 + y * width * 4 + 3]);
                            }
                        }
                        else if (b >= fixedB)
                        {
                            FromLab(L, a, b, out byte _, out byte _, out byte _, out byte A);

                            byte R = basicShape[x * 4 + y * width * 4];
                            byte G = basicShape[x * 4 + y * width * 4 + 1];
                            byte B = basicShape[x * 4 + y * width * 4 + 2];
                            byte A2 = basicShape[x * 4 + y * width * 4 + 3];

                            if (A > 0 && A2 > 0)
                            {
                                Blend(R, G, B, A, ref rgbaValues[x * 4 + y * width * 4], ref rgbaValues[x * 4 + y * width * 4 + 1], ref rgbaValues[x * 4 + y * width * 4 + 2], ref rgbaValues[x * 4 + y * width * 4 + 3]);
                            }
                        }
                    }
                }
            }
            return rgbaValues;
        }

        static void Blend(byte backgroundR, byte backgroundG, byte backgroundB, byte backgroundA, ref byte sourceR, ref byte sourceG, ref byte sourceB, ref byte sourceA)
        {
            byte outA = (byte)(sourceA + backgroundA * (255 - sourceA) / 255);

            if (outA == 0)
            {
                sourceR = sourceG = sourceB = sourceA = 0;
            }
            else
            {
                sourceR = (byte)Math.Max(0, Math.Min(255, ((sourceR * sourceA + backgroundR * backgroundA * (255 - sourceA) / 255) / outA)));
                sourceG = (byte)Math.Max(0, Math.Min(255, ((sourceG * sourceA + backgroundG * backgroundA * (255 - sourceA) / 255) / outA)));
                sourceB = (byte)Math.Max(0, Math.Min(255, ((sourceB * sourceA + backgroundB * backgroundA * (255 - sourceA) / 255) / outA)));
                sourceA = outA;
            }
        }

        static byte[] GetImageBytes(double[] imagePositions, double[] lightDirection)
        {
            int width = Resolution;
            int height = Resolution;


            byte[] rgbaValues = new byte[width * height * 4];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    double L = imagePositions[x * 3 + y * width * 3];
                    double a = imagePositions[x * 3 + y * width * 3 + 1];
                    double b = imagePositions[x * 3 + y * width * 3 + 2];

                    if (!double.IsNaN(L) && !double.IsNaN(a) && !double.IsNaN(b))
                    {
                        double[] p00 = GetPoint(imagePositions, width, height, x - 1, y - 1);
                        double[] p01 = GetPoint(imagePositions, width, height, x, y - 1);
                        double[] p02 = GetPoint(imagePositions, width, height, x + 1, y - 1);
                        double[] p10 = GetPoint(imagePositions, width, height, x - 1, y);
                        double[] p11 = GetPoint(imagePositions, width, height, x, y);
                        double[] p12 = GetPoint(imagePositions, width, height, x + 1, y);
                        double[] p20 = GetPoint(imagePositions, width, height, x - 1, y + 1);
                        double[] p21 = GetPoint(imagePositions, width, height, x, y + 1);
                        double[] p22 = GetPoint(imagePositions, width, height, x + 1, y + 1);

                        List<double[]> normals = new List<double[]>();

                        AddIfNotNull(normals, GetNormal(GetTriangle(p11, p00, p10)));
                        AddIfNotNull(normals, GetNormal(GetTriangle(p11, p01, p00)));
                        AddIfNotNull(normals, GetNormal(GetTriangle(p11, p02, p01)));
                        AddIfNotNull(normals, GetNormal(GetTriangle(p11, p12, p02)));
                        AddIfNotNull(normals, GetNormal(GetTriangle(p11, p22, p12)));
                        AddIfNotNull(normals, GetNormal(GetTriangle(p11, p21, p22)));
                        AddIfNotNull(normals, GetNormal(GetTriangle(p11, p20, p21)));
                        AddIfNotNull(normals, GetNormal(GetTriangle(p11, p10, p20)));


                        double[] normal = GetAverageNormal(normals);

                        double dot = normal[0] * lightDirection[0] + normal[1] * lightDirection[1] + normal[2] * lightDirection[2];

                        byte col = (byte)(Math.Max(0, Math.Min(255, 128 + Math.Round(-dot * 127))));

                        rgbaValues[x * 4 + y * width * 4] = col;
                        rgbaValues[x * 4 + y * width * 4 + 1] = col;
                        rgbaValues[x * 4 + y * width * 4 + 2] = col;
                        rgbaValues[x * 4 + y * width * 4 + 3] = (byte)(Math.Max(0, Math.Min(255, Math.Round((double)normals.Count * 255 / 8))));
                    }
                }
            }
            return rgbaValues;
        }

        static double[] GetPoint(double[] values, int width, int height, int x, int y)
        {
            if (x > 0 && y > 0 && x < width && y < height)
            {
                double L = values[x * 3 + y * width * 3];
                double a = values[x * 3 + y * width * 3 + 1];
                double b = values[x * 3 + y * width * 3 + 2];

                if (!double.IsNaN(L) && !double.IsNaN(a) && !double.IsNaN(b))
                {
                    return new double[] { L, a, b };
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        static double[][] GetTriangle(double[] p1, double[] p2, double[] p3)
        {
            if (p1 != null && p2 != null && p3 != null)
            {
                return new double[][] { p1, p2, p3 };
            }
            else
            {
                return null;
            }
        }

        static double[] GetNormal(double[][] triangle)
        {
            if (triangle == null)
            {
                return null;
            }

            double ax = triangle[1][0] - triangle[0][0];
            double ay = triangle[1][1] - triangle[0][1];
            double az = triangle[1][2] - triangle[0][2];

            double bx = triangle[2][0] - triangle[0][0];
            double by = triangle[2][1] - triangle[0][1];
            double bz = triangle[2][2] - triangle[0][2];

            double[] tbr = new double[]
            {
                ay * bz - az * by,
                az * bx - ax * bz,
                ax * by - ay * bx
            };

            double modulus = Math.Sqrt(tbr[0] * tbr[0] + tbr[1] * tbr[1] + tbr[2] * tbr[2]);
            tbr[0] /= modulus;
            tbr[1] /= modulus;
            tbr[2] /= modulus;
            return tbr;
        }

        static double[] GetAverageNormal(IEnumerable<double[]> normals)
        {
            int count = 0;

            double[] tbr = new double[3];

            foreach (double[] normal in normals)
            {
                tbr[0] += normal[0];
                tbr[1] += normal[1];
                tbr[2] += normal[2];
                count++;
            }

            tbr[0] /= count;
            tbr[1] /= count;
            tbr[2] /= count;

            double modulus = Math.Sqrt(tbr[0] * tbr[0] + tbr[1] * tbr[1] + tbr[2] * tbr[2]);
            tbr[0] /= modulus;
            tbr[1] /= modulus;
            tbr[2] /= modulus;
            return tbr;
        }

        static void AddIfNotNull<T>(List<T> list, T item)
        {
            if (item != null)
            {
                list.Add(item);
            }
        }

        static double[] GetImagePositions()
        {
            int width = Resolution;
            int height = Resolution;
            int depth = 4096;

            double[] values = new double[width * height * 3];

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = double.NaN;
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    double X = (x / (double)(width - 1)) * 2 - 1;
                    double Y = (y / (double)(height - 1)) * 2 - 1;


                    for (int z = 0; z < depth; z++)
                    {
                        double Z = (z / (double)(depth - 1)) * 2 - 1;

                        double[] newVect = MultiplyVector(RotationMatrix, new double[] { X, Y, Z });


                        double L = newVect[2] + 0.75;


                        double a = (newVect[0]) / 1.25;
                        double b = (-newVect[1]) / 1.25;

                        FromLab(L, a, b, out byte _, out byte _, out byte _, out byte A);

                        if (A > 0)
                        {
                            values[x * 3 + y * width * 3] = L;
                            values[x * 3 + y * width * 3 + 1] = a;
                            values[x * 3 + y * width * 3 + 2] = b;
                        }
                    }
                }
            }
            return values;
        }

        public static double[,] RotationToAlignWithZ(double[] vector, bool preferY = true)
        {
            if (vector[2] != -1)
            {
                double[] v = new double[] { vector[1], -vector[0], 0 };
                double c = 1 / (1 + vector[2]);

                return new double[3, 3]
                {
                    {1 - v[1] * v[1] * c, v[0] * v[1] * c, v[1] },
                    {v[0] * v[1] * c, 1 - v[0] * v[0] * c, -v[0] },
                    {-v[1], v[0], 1 - v[0] * v[0] * c- v[1] * v[1] * c }
                };
            }
            else
            {
                if (!preferY)
                {
                    return new double[3, 3]
                    {
                        { 1, 0, 0 },
                        { 0, -1, 0 },
                        { 0, 0, -1 }
                    };
                }
                else
                {
                    return new double[3, 3]
                    {
                        {-1, 0, 0 },
                        {0, 1, 0 },
                        {0, 0, -1 }
                    };
                }
            }
        }

        static double[,] RotationMatrix = new double[3, 3] { { 0.7071068, 0, 0.7071068 }, { 0.5000000, 0.7071068, -0.5000000 }, { -0.5000000, 0.7071068, 0.5000000 } };

        static double[] MultiplyVector(double[,] matrix, double[] vector)
        {
            return new double[]
            {
                vector[0] * matrix[0, 0] + vector[1] * matrix[0, 1] + vector[2] * matrix[0, 2],
                vector[0] * matrix[1, 0] + vector[1] * matrix[1, 1] + vector[2] * matrix[1, 2],
                vector[0] * matrix[2, 0] + vector[1] * matrix[2, 1] + vector[2] * matrix[2, 2],
            };
        }

        static void FromXYZ(double x, double y, double z, out byte R, out byte G, out byte B, out byte A)
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
            else if (r >= -0.01 && r <= 1.01 && g >= -0.01 && g <= 1.01 && b >= -0.01 && b <= 1.01)
            {
                r = Math.Max(0, Math.Min(1, r));
                g = Math.Max(0, Math.Min(1, g));
                b = Math.Max(0, Math.Min(1, b));

                A = 128;

                R = (byte)Math.Round(r * 255);
                G = (byte)Math.Round(g * 255);
                B = (byte)Math.Round(b * 255);
            }
            else
            {
                r = Math.Max(0, Math.Min(1, r));
                g = Math.Max(0, Math.Min(1, g));
                b = Math.Max(0, Math.Min(1, b));

                A = 0;

                R = (byte)Math.Round(r * 255);
                G = (byte)Math.Round(g * 255);
                B = (byte)Math.Round(b * 255);
            }
        }

        static void FromLab(double L, double a, double b, out byte R, out byte G, out byte B, out byte A)
        {
            static double f(double t)
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
    }
}
