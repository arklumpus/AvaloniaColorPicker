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
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using System;
using System.Reactive.Linq;

namespace AvaloniaColorPicker
{
    internal class ColorVisualBrush : VisualBrush
    {
        private static IBrush AlphaBackgroundBrush { get; } = Brush.Parse("#DCDCDC");

        private Rectangle ColorRectangle { get; }

        public static readonly StyledProperty<Color> ColorProperty = AvaloniaProperty.Register<ColorVisualBrush, Color>(nameof(Color));

        public Color Color
        {
            get
            {
                return GetValue(ColorProperty);
            }
            set
            {
                SetValue(ColorProperty, value);
            }
        }

        public ColorVisualBrush(Color color) : base()
        {
            Canvas can = new Canvas() { Width = 16, Height = 16, Background = Brushes.White };
            can.Children.Add(new Rectangle() { Width = 8, Height = 8, Fill = AlphaBackgroundBrush });
            can.Children.Add(new Rectangle() { Width = 8, Height = 8, Fill = AlphaBackgroundBrush, Margin = new Avalonia.Thickness(8, 8, 0, 0) });
            ColorRectangle = new Rectangle() { Width = 16, Height = 16 };
            can.Children.Add(ColorRectangle);
            this.Visual = can;
            this.SourceRect = new Avalonia.RelativeRect(0, 0, 16, 16, Avalonia.RelativeUnit.Absolute);
            this.DestinationRect = new Avalonia.RelativeRect(0, 0, 16, 16, Avalonia.RelativeUnit.Absolute);
            this.TileMode = TileMode.Tile;
            this.Color = color;

            AffectsRender<ColorVisualBrush>(ColorProperty);
            
            if (!ColorPicker.TransitionsDisabled)
            {
                this.Transitions = new Avalonia.Animation.Transitions
                {
                    new ColorTransition() { Property = ColorProperty, Duration = new TimeSpan(0, 0, 0, 0, 100) }
                };
            }
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == ColorProperty)
            {
                ColorRectangle.Fill = new SolidColorBrush((change.NewValue as Color?).Value);
                this.RaiseInvalidated(EventArgs.Empty);
            }
        }
    }

    internal class AnimatableColorBrush : Animatable
    {
        public static readonly StyledProperty<Color> ColorProperty = AvaloniaProperty.Register<ColorVisualBrush, Color>(nameof(Color));

        public Color Color
        {
            get
            {
                return GetValue(ColorProperty);
            }
            private set
            {
                SetValue(ColorProperty, value);
            }
        }

        private Action<Color> ColorSetter { get; }

        private ColorTransition Transition { get; }

        public AnimatableColorBrush(Color color, Action<Color> colorSetter) : base()
        {
            this.ColorSetter = colorSetter;

            this.Color = color;

            if (!ColorPicker.TransitionsDisabled)
            {
                this.Transitions = new Avalonia.Animation.Transitions();
                Transition = new ColorTransition() { Property = ColorProperty, Duration = new TimeSpan(0, 0, 0, 0, 100) };
                this.Transitions.Add(Transition);
            }
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == ColorProperty)
            {
                ColorSetter((change.NewValue as Color?).Value);
            }
        }

        public void Update(Color color, bool instantTransition)
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

            this.Color = color;
        }
    }

    internal class ColorTransition : Transition<Color>
    {
        public override IObservable<Color> DoTransition(IObservable<double> progress, Color oldValue, Color newValue)
        {
            return progress.Select(p =>
            {
                (double L1, double a1, double b1) = Lab.ToLab(oldValue);
                (double L2, double a2, double b2) = Lab.ToLab(newValue);

                double f = Easing.Ease(p);

                byte A = (byte)(oldValue.A + ((double)newValue.A - oldValue.A) * f);

                Lab.FromLab(L1 + (L2 - L1) * f, a1 + (a2 - a1) * f, b1 + (b2 - b1) * f, out byte R, out byte G, out byte B);

                return Color.FromArgb(A, R, G, B);
            });
        }
    }

    internal class SolidBrushTransition : Transition<IBrush>
    {
        public override IObservable<IBrush> DoTransition(IObservable<double> progress, IBrush oldBrush, IBrush newBrush)
        {
            return progress.Select(p =>
            {
                if (oldBrush is SolidColorBrush oldSolidBrush && newBrush is SolidColorBrush newSolidBrush)
                {
                    Color oldValue = oldSolidBrush.Color;
                    Color newValue = newSolidBrush.Color;


                    (double L1, double a1, double b1) = Lab.ToLab(oldValue);
                    (double L2, double a2, double b2) = Lab.ToLab(newValue);

                    double f = Easing.Ease(p);

                    byte A = (byte)(oldValue.A + ((double)newValue.A - oldValue.A) * f);

                    Lab.FromLab(L1 + (L2 - L1) * f, a1 + (a2 - a1) * f, b1 + (b2 - b1) * f, out byte R, out byte G, out byte B);

                    return new SolidColorBrush(Color.FromArgb(A, R, G, B));
                }
                else
                {
                    return newBrush;
                }
            });
        }
    }

    internal class RGBTransition : Transition<Color>
    {
        public override IObservable<Color> DoTransition(IObservable<double> progress, Color oldValue, Color newValue)
        {
            return progress.Select(p =>
            {
                double f = Easing.Ease(p);

                byte R = (byte)(oldValue.R + ((double)newValue.R - oldValue.R) * f);
                byte G = (byte)(oldValue.G + ((double)newValue.G - oldValue.G) * f);
                byte B = (byte)(oldValue.B + ((double)newValue.B - oldValue.B) * f);
                byte A = (byte)(oldValue.A + ((double)newValue.A - oldValue.A) * f);

                return Color.FromArgb(A, R, G, B);
            });
        }
    }
}
