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
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;
using System.Reactive.Linq;

namespace AvaloniaColorPicker
{
    internal class AnimatableLABCanvas : Animatable
    {
        public Image LabImage { get; }

        public Ellipse PositionEllipse { get; }

        private Lab.LabComponents LabComponent { get; set; }

        private LABTransition Transition { get; }

        public static readonly StyledProperty<LAB> LABProperty = AvaloniaProperty.Register<AnimatableLABCanvas, LAB>(nameof(LAB));

        public LAB LAB
        {
            get
            {
                return GetValue(LABProperty);
            }
            set
            {
                SetValue(LABProperty, value);
            }
        }

        public AnimatableLABCanvas(double L, double a, double b, Lab.LabComponents labComponent)
        {
            LabImage = new Image() { Width = 96, Height = 96 };
            PositionEllipse = new Ellipse();
            LabComponent = labComponent;

            this.LAB = new LAB(L, a, b);

            if (!ColorPicker.TransitionsDisabled)
            {
                this.Transitions = new Transitions();
                this.Transition = new LABTransition() { Property = LABProperty, Duration = new TimeSpan(0, 0, 0, 0, 100) };
                this.Transitions.Add(Transition);
            }
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == LABProperty)
            {
                LAB lab = (change.NewValue.Value as LAB?).Value;

                double L = lab.L;
                double a = lab.a;
                double b = lab.b;

                Update(L, a, b);
            }
        }

        private void Update(double L, double a, double b)
        {
            WriteableBitmap bitmap = new WriteableBitmap(new PixelSize(96, 96), new Vector(96, 96), Avalonia.Platform.PixelFormat.Rgba8888, Avalonia.Platform.AlphaFormat.Unpremul);

            int offset = -1;

            if (LabComponent == Lab.LabComponents.L)
            {
                int normL = Math.Max(0, Math.Min(128, (int)Math.Round(L * 128.0)));

                offset = 96 * 96 * 4 * normL;
            }
            else if (LabComponent == Lab.LabComponents.a)
            {
                int normA = Math.Max(0, Math.Min(128, 64 + (int)Math.Round(a * 64.0)));

                offset = 96 * 96 * 4 * normA + 96 * 96 * 129 * 4;
            }
            else if (LabComponent == Lab.LabComponents.b)
            {
                int normB = Math.Max(0, Math.Min(128, 64 + (int)Math.Round(b * 64.0)));

                offset = 96 * 96 * 4 * normB + 96 * 96 * 129 * 4 * 2;
            }

            using (var fb = bitmap.Lock())
            {
                unsafe
                {
                    byte* rgbaValues = (byte*)fb.Address;

                    for (int i = 0; i < 96 * 96 * 4; i++)
                    {
                        rgbaValues[i] = Lab.LabImageData[offset + i];
                    }
                }
            }

            LabImage.Source = bitmap;


            Lab.FromLab(L, a, b, out _, out _, out _, out byte A);

            if (A > 0)
            {
                (double x, double y, double w, double h) = Lab.GetEllipsePosition(L, a, b, LabComponent);

                double C = Math.Sqrt(a * a + b * b);
                double S = C / Math.Sqrt(C * C + L * L);
                PositionEllipse.Width = w;
                PositionEllipse.Height = h;
                PositionEllipse.Fill = (S < 0.5 && L > 0.5 ? Brushes.Black : Brushes.White);
                PositionEllipse.RenderTransform = new TranslateTransform(x - (w * 0.5), y - h * 0.5);
            }
            else
            {
                PositionEllipse.Fill = null;
            }
        }

        public void Update(double L, double a, double b, Lab.LabComponents labComponent, bool instantTransition)
        {
            if (!ColorPicker.TransitionsDisabled)
            {
                if (instantTransition)
                {
                    this.Transition.Easing = new InstantEasing();
                }
                else
                {
                    this.Transition.Easing = new LinearEasing();
                }
            }

            this.LabComponent = labComponent;

            if (this.LAB.L != L || this.LAB.a != a || this.LAB.b != b)
            {
                this.LAB = new LAB(L, a, b);
            }
            else
            {
                Update(L, a, b);
            }
        }
    }

    internal struct LAB
    {
        public double L;
        public double a;
        public double b;

        public LAB(double L, double a, double b)
        {
            this.L = L;
            this.a = a;
            this.b = b;
        }
    }

    internal class LABTransition : Transition<LAB>
    {
        public override IObservable<LAB> DoTransition(IObservable<double> progress, LAB oldValue, LAB newValue)
        {
            return progress.Select(p =>
            {
                double f = Easing.Ease(p);
                return new LAB(oldValue.L + (newValue.L - oldValue.L) * f, oldValue.a + (newValue.a - oldValue.a) * f, oldValue.b + (newValue.b - oldValue.b) * f);
            });
        }
    }
}
