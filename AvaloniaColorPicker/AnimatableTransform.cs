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
using Avalonia.Animation.Easings;
using Avalonia.Media;
using System;
using System.Reactive.Linq;

namespace AvaloniaColorPicker
{
    internal class AnimatableTransform : Animatable
    {
        public static readonly StyledProperty<Matrix> MatrixProperty = AvaloniaProperty.Register<AnimatableTransform, Matrix>(nameof(Matrix));

        public Matrix Matrix
        {
            get
            {
                return GetValue(MatrixProperty);
            }
            set
            {
                SetValue(MatrixProperty, value);
            }
        }

        public MatrixTransform MatrixTransform { get; }
        private MatrixTransition Transition { get; }

        public AnimatableTransform(Matrix matrix)
        {
            this.MatrixTransform = new MatrixTransform() { Matrix = matrix };
            this.Matrix = matrix;

            if (!ColorPicker.TransitionsDisabled)
            {
                this.Transitions = new Transitions();
                Transition = new MatrixTransition() { Property = MatrixProperty, Duration = new TimeSpan(0, 0, 0, 0, 100) };
                this.Transitions.Add(Transition);
            }
        }

        public void Update(Matrix matrix, bool instantTransition)
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

            this.Matrix = matrix;
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == MatrixProperty)
            {
                this.MatrixTransform.Matrix = (change.NewValue.Value as Matrix?).Value;
            }
        }
    }

    internal class MatrixTransition : Transition<Matrix>
    {
        public override IObservable<Matrix> DoTransition(IObservable<double> progress, Matrix oldValue, Matrix newValue)
        {
            return progress.Select(p =>
            {
                double f = Easing.Ease(p);

                return new Matrix(oldValue.M11 + (newValue.M11 - oldValue.M11) * f,
                    oldValue.M12 + (newValue.M12 - oldValue.M12) * f,
                    oldValue.M21 + (newValue.M21 - oldValue.M21) * f,
                    oldValue.M22 + (newValue.M22 - oldValue.M22) * f,
                    oldValue.M31 + (newValue.M31 - oldValue.M31) * f,
                    oldValue.M32 + (newValue.M32 - oldValue.M32) * f);
            });
        }
    }



    internal class AnimatableAngleTransform : Animatable
    {
        public static readonly StyledProperty<double> AngleProperty = AvaloniaProperty.Register<AnimatableTransform, double>(nameof(Angle));

        public double Angle
        {
            get
            {
                return GetValue(AngleProperty);
            }
            set
            {
                SetValue(AngleProperty, value);
            }
        }

        public MatrixTransform MatrixTransform { get; }
        private DoubleTransition Transition { get; }

        public AnimatableAngleTransform(double angle)
        {
            this.MatrixTransform = new MatrixTransform();
            this.Angle = angle;
            if (!ColorPicker.TransitionsDisabled)
            {
                this.Transitions = new Transitions();
                Transition = new DoubleTransition() { Property = AngleProperty, Duration = new TimeSpan(0, 0, 0, 0, 100) };
                this.Transitions.Add(Transition);
            }
        }

        public void Update(double angle, bool instantTransition)
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

            this.Angle = angle;
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == AngleProperty)
            {
                double H = (change.NewValue.Value as double?).Value;

                Point v23 = new Point(64 + 64 * Math.Cos(H * 2 * Math.PI), 20 + 20 * Math.Sin(H * 2 * Math.PI));
                Point v14 = new Point(64, 20);
                Point v67 = new Point(64 + 64 * Math.Cos(H * 2 * Math.PI), 76 + 20 * Math.Sin(H * 2 * Math.PI));

                double m11 = (v23.X - v14.X) / 256;
                double m12 = (v67.X - v23.X) / 256;
                double m21 = (v23.Y - v14.Y) / 256;
                double m22 = (v67.Y - v23.Y) / 256;

                this.MatrixTransform.Matrix = new Matrix(m11, m21, m12, m22, v14.X, v14.Y);
            }
        }
    }
}
