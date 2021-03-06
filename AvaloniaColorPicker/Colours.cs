﻿using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace AvaloniaColorPicker
{
    internal static class Colours
    {
        private static SolidColorBrush ForegroundColourLight = new SolidColorBrush(0xFF000000);
        private static SolidColorBrush ForegroundColourDark = new SolidColorBrush(0xFFDEDEDE);

        public static SolidColorBrush ForegroundColour
        {
            get
            {
                return IsDark ? ForegroundColourDark : ForegroundColourLight;
            }
        }

        private static SolidColorBrush BorderLowColourLight = new SolidColorBrush(0xFFAAAAAA);
        private static SolidColorBrush BorderLowColourDark = new SolidColorBrush(0xFF505050);

        public static SolidColorBrush BorderLowColour
        {
            get
            {
                return IsDark ? BorderLowColourDark : BorderLowColourLight;
            }
        }

        private static SolidColorBrush ControlHighlightLowColourLight = new SolidColorBrush(0xFFF0F0F0);
        private static SolidColorBrush ControlHighlightLowColourDark = new SolidColorBrush(0xFFA8A8A8);

        public static SolidColorBrush ControlHighlightLowColour
        {
            get
            {
                return IsDark ? ControlHighlightLowColourDark : ControlHighlightLowColourLight;
            }
        }

        private static SolidColorBrush BackgroundColourLight = new SolidColorBrush(0xFFFFFFFF);
        private static SolidColorBrush BackgroundColourDark = new SolidColorBrush(0xFF282828);

        public static SolidColorBrush BackgroundColour
        {
            get
            {
                return IsDark ? BackgroundColourDark : BackgroundColourLight;
            }
        }

        private static SolidColorBrush BorderHighColourLight = new SolidColorBrush(0xFF333333);
        private static SolidColorBrush BorderHighColourDark = new SolidColorBrush(0xFFA0A0A0);

        public static SolidColorBrush BorderHighColour
        {
            get
            {
                return IsDark ? BorderHighColourDark : BorderHighColourLight;
            }
        }

        private static bool IsDark
        {
            get
            {
                foreach (Avalonia.Styling.IStyle style in Avalonia.Application.Current.Styles)
                {
                    if (style is Avalonia.Themes.Fluent.FluentTheme theme)
                    {
                        if (theme.Mode == Avalonia.Themes.Fluent.FluentThemeMode.Dark)
                        {
                            return true;
                        }
                        else if (theme.Mode == Avalonia.Themes.Fluent.FluentThemeMode.Light)
                        {
                            return false;
                        }
                    }
                    else if (style is Avalonia.Markup.Xaml.Styling.StyleInclude include)
                    {
                        if (include.Source.AbsoluteUri.Contains("BaseLight"))
                        {
                            return false;
                        }
                        else if (include.Source.AbsoluteUri.Contains("BaseDark"))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }


    }
}
