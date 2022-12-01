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
using Avalonia.Media.Imaging;
using System;
using System.Reactive.Linq;

namespace AvaloniaColorPicker
{
    internal struct PointCollection4 : IEquatable<PointCollection4>
    {
        public Point P1;
        public Point P2;
        public Point P3;
        public Point P4;

        public PointCollection4(Point p1, Point p2, Point p3, Point p4)
        {
            P1 = p1;
            P2 = p2;
            P3 = p3;
            P4 = p4;
        }

        public bool Equals(PointCollection4 other)
        {
            return this.P1 == other.P1 && this.P2 == other.P2 && this.P3 == other.P3 && this.P4 == other.P4;
        }
    }

    internal class AnimatablePath4Points : Animatable
    {
        public static readonly StyledProperty<PointCollection4> PointsProperty = AvaloniaProperty.Register<AnimatablePath4Points, PointCollection4>(nameof(Points));

        public PointCollection4 Points
        {
            get
            {
                return GetValue(PointsProperty);
            }
            set
            {
                SetValue(PointsProperty, value);
            }
        }

        public Path Path { get; }
        private PathFigure Figure { get; }
        private LineSegment Segment1 { get; }
        private LineSegment Segment2 { get; }
        private LineSegment Segment3 { get; }

        private PointCollection4Transition Transition { get; }

        public IBrush Fill
        {
            get
            {
                return Path.Fill;
            }
            set
            {
                Path.Fill = value;
            }
        }

        public AnimatablePath4Points(Point p1, Point p2, Point p3, Point p4) : base()
        {
            PathGeometry geometry = new PathGeometry();
            Figure = new PathFigure() { StartPoint = p1, IsClosed = true };
            Segment1 = new LineSegment() { Point = p2 };
            Segment2 = new LineSegment() { Point = p3 };
            Segment3 = new LineSegment() { Point = p4 };
            Figure.Segments.Add(Segment1);
            Figure.Segments.Add(Segment2);
            Figure.Segments.Add(Segment3);
            geometry.Figures.Add(Figure);

            Path = new Path() { Data = geometry };

            this.Points = new PointCollection4(p1, p2, p3, p4);

            if (!ColorPicker.TransitionsDisabled)
            {
                this.Transitions = new Avalonia.Animation.Transitions();
                Transition = new PointCollection4Transition() { Property = PointsProperty, Duration = new TimeSpan(0, 0, 0, 0, 100) };
                this.Transitions.Add(Transition);
            }
        }

        public void Update(Point p1, Point p2, Point p3, Point p4, bool instantTransition)
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

            this.Points = new PointCollection4(p1, p2, p3, p4);
        }



        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == PointsProperty)
            {
                PointCollection4 newPoints = (change.NewValue as PointCollection4?).Value;

                this.Figure.StartPoint = newPoints.P1;
                this.Segment1.Point = newPoints.P2;
                this.Segment2.Point = newPoints.P3;
                this.Segment3.Point = newPoints.P4;

                PathGeometry geometry = new PathGeometry();
                geometry.Figures.Add(Figure);
                Path.Data = geometry;
            }
        }
    }

    internal class InstantEasing : Easing
    {
        public override double Ease(double progress)
        {
            return 1;
        }
    }

    internal class PointCollection4Transition : Transition<PointCollection4>
    {
        public override IObservable<PointCollection4> DoTransition(IObservable<double> progress, PointCollection4 oldValue, PointCollection4 newValue)
        {
            return progress.Select(p =>
            {
                double f = Easing.Ease(p);
                return new PointCollection4(oldValue.P1 + (newValue.P1 - oldValue.P1) * f, oldValue.P2 + (newValue.P2 - oldValue.P2) * f, oldValue.P3 + (newValue.P3 - oldValue.P3) * f, oldValue.P4 + (newValue.P4 - oldValue.P4) * f);
            });
        }
    }

    internal class AnimatablePathCylinder : Animatable
    {
        public Path TopPath { get; }
        private PathFigure TopFigure { get; }
        private LineSegment TopSegment1 { get; }
        private LineSegment TopSegment2 { get; }
        private ArcSegment TopSegment3 { get; }

        public Path CylinderPath { get; }

        private PathFigure LeftFigure { get; }
        private ArcSegment LeftSegment1 { get; }
        private LineSegment LeftSegment2 { get; }
        private ArcSegment LeftSegment3 { get; }

        private PathFigure RightFigure { get; }
        private ArcSegment RightSegment1 { get; }
        private LineSegment RightSegment2 { get; }
        private ArcSegment RightSegment3 { get; }


        public Path PlatePath { get; }
        private PathFigure PlateFigure { get; }
        private LineSegment PlateSegment1 { get; }
        private LineSegment PlateSegment2 { get; }
        private LineSegment PlateSegment3 { get; }


        private DoubleTransition Transition { get; }

        public static readonly StyledProperty<double> AngleProperty = AvaloniaProperty.Register<AnimatablePathCylinder, double>(nameof(Angle));

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

        public AnimatablePathCylinder(double angle)
        {
            TopFigure = new PathFigure() { IsClosed = true, IsFilled = true };
            TopSegment1 = new LineSegment();
            TopSegment2 = new LineSegment();
            TopSegment3 = new ArcSegment() { IsLargeArc = true, RotationAngle = 0, Size = new Size(64, 20), };

            TopFigure.Segments.Add(TopSegment1);
            TopFigure.Segments.Add(TopSegment2);
            TopFigure.Segments.Add(TopSegment3);

            TopPath = new Path();

            LeftFigure = new PathFigure() { IsClosed = true, IsFilled = true };
            LeftSegment1 = new ArcSegment() { Point = new Point(0, 20), IsLargeArc = false, RotationAngle = 0, Size = new Size(64, 20), SweepDirection = SweepDirection.Clockwise };
            LeftSegment2 = new LineSegment() { Point = new Point(0, 76) };
            LeftSegment3 = new ArcSegment() { IsLargeArc = false, RotationAngle = 0, Size = new Size(64, 20), SweepDirection = SweepDirection.CounterClockwise };

            LeftFigure.Segments.Add(LeftSegment1);
            LeftFigure.Segments.Add(LeftSegment2);
            LeftFigure.Segments.Add(LeftSegment3);

            RightFigure = new PathFigure() { IsClosed = true, IsFilled = true };
            RightSegment1 = new ArcSegment() { Point = new Point(128, 20), IsLargeArc = false, RotationAngle = 0, Size = new Size(64, 20), SweepDirection = SweepDirection.CounterClockwise };
            RightSegment2 = new LineSegment() { Point = new Point(128, 76) };
            RightSegment3 = new ArcSegment() { IsLargeArc = false, RotationAngle = 0, Size = new Size(64, 20), SweepDirection = SweepDirection.Clockwise };

            RightFigure.Segments.Add(RightSegment1);
            RightFigure.Segments.Add(RightSegment2);
            RightFigure.Segments.Add(RightSegment3);

            CylinderPath = new Path();

            PlateFigure = new PathFigure() { IsClosed = true, IsFilled = true };
            PlateSegment1 = new LineSegment() { Point = new Point(64, 20) };
            PlateSegment2 = new LineSegment() { Point = new Point(64, 76) };
            PlateSegment3 = new LineSegment();
            PlateFigure.Segments.Add(PlateSegment1);
            PlateFigure.Segments.Add(PlateSegment2);
            PlateFigure.Segments.Add(PlateSegment3);

            PlatePath = new Path();

            this.Angle = angle;

            if (!ColorPicker.TransitionsDisabled)
            {
                this.Transitions = new Transitions();
                this.Transition = new DoubleTransition() { Property = AngleProperty, Duration = new TimeSpan(0, 0, 0, 0, 100) };
                this.Transitions.Add(this.Transition);
            }
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == AngleProperty)
            {
                double angle = (change.NewValue as double?).Value;

                Point v23 = new Point(64 + 64 * Math.Cos(angle * 2 * Math.PI), 20 + 20 * Math.Sin(angle * 2 * Math.PI));
                Point v67 = new Point(64 + 64 * Math.Cos(angle * 2 * Math.PI), 76 + 20 * Math.Sin(angle * 2 * Math.PI));
                Point v23Opp;
                Point v67Opp;


                if (angle > 0 && angle <= 0.25 || angle > 0.5 && angle <= 0.75)
                {
                    v67Opp = new Point(64 + 64 * Math.Cos(angle * 2 * Math.PI + Math.PI / 2), 76 + 20 * Math.Sin(angle * 2 * Math.PI + Math.PI / 2));
                    v23Opp = new Point(64 + 64 * Math.Cos(angle * 2 * Math.PI + Math.PI / 2), 20 + 20 * Math.Sin(angle * 2 * Math.PI + Math.PI / 2));
                }
                else
                {
                    v67Opp = new Point(64 + 64 * Math.Cos(angle * 2 * Math.PI - Math.PI / 2), 76 + 20 * Math.Sin(angle * 2 * Math.PI - Math.PI / 2));
                    v23Opp = new Point(64 + 64 * Math.Cos(angle * 2 * Math.PI - Math.PI / 2), 20 + 20 * Math.Sin(angle * 2 * Math.PI - Math.PI / 2));
                }

                if (angle > 0 && angle <= 0.25)
                {
                    PathGeometry topGeometry = new PathGeometry();
                    TopFigure.StartPoint = v23Opp;
                    TopSegment1.Point = new Point(64, 20);
                    TopSegment2.Point = v23;
                    TopSegment3.Point = v23Opp;
                    TopSegment3.SweepDirection = SweepDirection.CounterClockwise;
                    topGeometry.Figures.Add(TopFigure);

                    TopPath.Data = topGeometry;


                    PathGeometry cylinderGeometry = new PathGeometry();
                    RightFigure.StartPoint = v23;
                    RightSegment3.Point = v67;
                    cylinderGeometry.Figures.Add(RightFigure);

                    LeftFigure.StartPoint = v23Opp;
                    LeftSegment3.Point = v67Opp;
                    cylinderGeometry.Figures.Add(LeftFigure);

                    CylinderPath.Data = cylinderGeometry;

                    PathGeometry plateGeometry = new PathGeometry();
                    PlateFigure.StartPoint = v23Opp;
                    PlateSegment3.Point = v67Opp;
                    plateGeometry.Figures.Add(PlateFigure);
                    PlatePath.Data = plateGeometry;

                }
                else if (angle > 0.25 && angle <= 0.5)
                {
                    PathGeometry topGeometry = new PathGeometry();
                    TopFigure.StartPoint = v23Opp;
                    TopSegment1.Point = new Point(64, 20);
                    TopSegment2.Point = v23;
                    TopSegment3.Point = v23Opp;
                    TopSegment3.SweepDirection = SweepDirection.Clockwise;
                    topGeometry.Figures.Add(TopFigure);

                    TopPath.Data = topGeometry;

                    PathGeometry cylinderGeometry = new PathGeometry();
                    RightFigure.StartPoint = v23Opp;
                    RightSegment3.Point = v67Opp;
                    cylinderGeometry.Figures.Add(RightFigure);

                    LeftFigure.StartPoint = v23;
                    LeftSegment3.Point = v67;
                    cylinderGeometry.Figures.Add(LeftFigure);

                    CylinderPath.Data = cylinderGeometry;

                    PathGeometry plateGeometry = new PathGeometry();
                    PlateFigure.StartPoint = v23Opp;
                    PlateSegment3.Point = v67Opp;
                    plateGeometry.Figures.Add(PlateFigure);
                    PlatePath.Data = plateGeometry;
                }
                else if (angle > 0.5 && angle <= 0.75)
                {
                    PathGeometry topGeometry = new PathGeometry();
                    TopFigure.StartPoint = v23;
                    TopSegment1.Point = new Point(64, 20);
                    TopSegment2.Point = new Point(64, 40);
                    TopSegment3.Point = v23;
                    TopSegment3.SweepDirection = SweepDirection.CounterClockwise;
                    topGeometry.Figures.Add(TopFigure);

                    TopPath.Data = topGeometry;


                    PathGeometry cylinderGeometry = new PathGeometry();
                    RightFigure.StartPoint = new Point(64, 40);
                    RightSegment3.Point = new Point(64, 96);
                    cylinderGeometry.Figures.Add(RightFigure);

                    CylinderPath.Data = cylinderGeometry;

                    PlatePath.Data = null;
                }
                else if (angle > 0.75 && angle <= 1 || angle == 0)
                {
                    PathGeometry topGeometry = new PathGeometry();
                    TopFigure.StartPoint = v23;
                    TopSegment1.Point = new Point(64, 20);
                    TopSegment2.Point = new Point(64, 40);
                    TopSegment3.Point = v23;
                    TopSegment3.SweepDirection = SweepDirection.Clockwise;
                    topGeometry.Figures.Add(TopFigure);

                    TopPath.Data = topGeometry;

                    PathGeometry cylinderGeometry = new PathGeometry();
                    LeftFigure.StartPoint = new Point(64, 40);
                    LeftSegment3.Point = new Point(64, 96);
                    cylinderGeometry.Figures.Add(LeftFigure);

                    CylinderPath.Data = cylinderGeometry;

                    PlatePath.Data = null;
                }
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
    }


    internal class AnimatableCylinderShaft : Animatable
    {
        public Path Path { get; }
        private PathFigure Figure { get; }
        private ArcSegment Segment1 { get; }
        private LineSegment Segment2 { get; }
        private ArcSegment Segment3 { get; }


        private DoubleTransition Transition { get; }

        public static readonly StyledProperty<double> HeightProperty = AvaloniaProperty.Register<AnimatablePathCylinder, double>(nameof(Height));

        public double Height
        {
            get
            {
                return GetValue(HeightProperty);
            }
            set
            {
                SetValue(HeightProperty, value);
            }
        }

        public AnimatableCylinderShaft(double height)
        {
            Figure = new PathFigure() { IsClosed = true };

            Segment1 = new ArcSegment() { Size = new Size(64, 20), IsLargeArc = true, RotationAngle = 0, SweepDirection = SweepDirection.CounterClockwise };
            Segment2 = new LineSegment() { Point = new Point(128, 76) };
            Segment3 = new ArcSegment() { Point = new Point(0, 76), Size = new Size(64, 20), IsLargeArc = true, RotationAngle = 0, SweepDirection = SweepDirection.Clockwise };

            Figure.Segments.Add(Segment1);
            Figure.Segments.Add(Segment2);
            Figure.Segments.Add(Segment3);

            Path = new Path();

            this.Height = height;

            if (!ColorPicker.TransitionsDisabled)
            {
                this.Transitions = new Transitions();
                this.Transition = new DoubleTransition() { Property = HeightProperty, Duration = new TimeSpan(0, 0, 0, 0, 100) };
                this.Transitions.Add(this.Transition);
            }
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == HeightProperty)
            {
                double height = (change.NewValue as double?).Value;

                PathGeometry cylinderGeometry = new PathGeometry();
                Figure.StartPoint = new Point(0, 76 - height * 56 - 1);
                Segment1.Point = new Point(128, 76 - height * 56 - 1);
                cylinderGeometry.Figures.Add(Figure);
                Path.Data = cylinderGeometry;
            }
        }

        public void Update(double height, bool instantTransition)
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

            this.Height = height;
        }
    }


    internal class AnimatableColouredCylinder : Animatable
    {
        public Image ColourImage { get; }
        public MatrixTransform ColourCanvasTransform { get; }
        public Ellipse TopEllipse { get; }
        public Ellipse BottomEllipse { get; }

        public Ellipse PositionEllipse { get; }

        private HSVTransition Transition { get; }

        public static readonly StyledProperty<HSV> HSVProperty = AvaloniaProperty.Register<AnimatableColouredCylinder, HSV>(nameof(HSV));

        public HSV HSV
        {
            get
            {
                return GetValue(HSVProperty);
            }
            set
            {
                SetValue(HSVProperty, value);
            }
        }

        private WriteableBitmap ColourBitmap { get; set; }

        public AnimatableColouredCylinder(double H, double S, double V, WriteableBitmap colourBitmap)
        {
            ColourImage = new Image();

            ColourCanvasTransform = new MatrixTransform();

            TopEllipse = new Ellipse();
            BottomEllipse = new Ellipse();

            PositionEllipse = new Ellipse();

            this.ColourBitmap = colourBitmap;

            this.HSV = new HSV(H, S, V);

            if (!ColorPicker.TransitionsDisabled)
            {
                this.Transition = new HSVTransition() { Property = HSVProperty, Duration = new TimeSpan(0, 0, 0, 0, 100) };
                this.Transitions = new Transitions
                {
                    this.Transition
                };
            }
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == HSVProperty)
            {
                HSV hsv = (change.NewValue as HSV?).Value;
                double H = hsv.H;
                double S = hsv.S;
                double V = hsv.V;

                ColourImage.Source = HSB.GetShiftedS(this.ColourBitmap, 20 * S, H < 0.5);

                Point v23 = new Point(128 - 64 * (1 - S), 20);
                Point v14 = new Point(64 * (1 - S), 20);
                Point v67 = new Point(128 - 64 * (1 - S), 76);

                double m11 = (v23.X - v14.X) / 128;
                double m12 = (v67.X - v23.X) / 128;
                double m21 = (v23.Y - v14.Y) / 256;
                double m22 = (v67.Y - v23.Y) / 256;

                if (S > 0)
                {
                    PositionEllipse.Width = 56.0 / 256 * 48 / S * Math.Abs(Math.Sin(H * 2 * Math.PI));
                    PositionEllipse.Height = 48;
                    PositionEllipse.Fill = (S < 0.5 && V > 0.5 ? Brushes.Black : Brushes.White);

                    if (H < 0.5)
                    {
                        PositionEllipse.RenderTransform = new TranslateTransform(64 - (56.0 / 256 * 48 / S) * 0.5 * Math.Abs(Math.Sin(H * 2 * Math.PI)) + Math.Cos(H * 2 * Math.PI) * 64, 256 - 256 * V - 24 + 256.0 / 56.0 * 20 * S * Math.Sin(H * 2 * Math.PI));
                    }
                    else
                    {
                        PositionEllipse.RenderTransform = new TranslateTransform(64 - (56.0 / 256 * 48 / S) * 0.5 * Math.Abs(Math.Sin(H * 2 * Math.PI)) + Math.Cos(H * 2 * Math.PI) * 64, 256 + 20 * S * 256 / 56 - 256 * V - 24 + 256.0 / 56.0 * 20 * S * Math.Sin(H * 2 * Math.PI));
                    }
                }
                else
                {
                    PositionEllipse.Fill = null;
                }

                ColourCanvasTransform.Matrix = new Matrix(m11, m21, m12, m22, v14.X, v14.Y - (H < 0.5 ? 0 : (20 * S)));

                TopEllipse.Width = 128 * S;
                TopEllipse.Height = 40 * S;
                TopEllipse.RenderTransform = new TranslateTransform(64 - 64 * S, 20 - 20 * S);

                BottomEllipse.Width = 128 * S;
                BottomEllipse.Height = 40 * S;
                BottomEllipse.RenderTransform = new TranslateTransform(64 - 64 * S, 76 - 20 * S);

                if (H < 0.5)
                {
                    TopEllipse.Fill = HSB.LightCylinderBrush;
                    BottomEllipse.Fill = null;
                }
                else
                {
                    BottomEllipse.Fill = HSB.DarkCylinderBrush;
                    TopEllipse.Fill = null;
                }
            }
        }

        public void Update(double H, double S, double V, WriteableBitmap colourBitmap, bool instantTransition)
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

            this.ColourBitmap = colourBitmap;

            this.HSV = new HSV(H, S, V);
        }
    }

    internal struct HSV
    {
        public double H;
        public double S;
        public double V;

        public HSV(double h, double s, double v)
        {
            this.H = h;
            this.S = s;
            this.V = v;
        }
    }

    internal class HSVTransition : Transition<HSV>
    {
        public override IObservable<HSV> DoTransition(IObservable<double> progress, HSV oldValue, HSV newValue)
        {
            return progress.Select(p =>
            {
                double f = Easing.Ease(p);
                return new HSV(oldValue.H + (newValue.H - oldValue.H) * f, oldValue.S + (newValue.S - oldValue.S) * f, oldValue.V + (newValue.V - oldValue.V) * f);
            });
        }
    }

    internal class ThicknessTransition : Transition<Thickness>
    {
        public override IObservable<Thickness> DoTransition(IObservable<double> progress, Thickness oldValue, Thickness newValue)
        {
            return progress.Select(p =>
            {
                double f = Easing.Ease(p);
                return new Thickness(oldValue.Left + (newValue.Left - oldValue.Left) * f, oldValue.Top + (newValue.Top - oldValue.Top) * f, oldValue.Right + (newValue.Right - oldValue.Right) * f, oldValue.Bottom + (newValue.Bottom - oldValue.Bottom) * f);
            });
        }
    }
}
