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

        private static bool? isDarkCached = null;

        private static bool IsDark
        {
            get
            {
                if (isDarkCached == null)
                {
                    foreach (Avalonia.Styling.IStyle style in Avalonia.Application.Current.Styles)
                    {
                        //TODO: Sort out how we do this (Maybe just reference the fluent package?)
                        /*if (style is Avalonia.Themes.Fluent.FluentTheme theme)
                        {
                            if (theme.Mode == Avalonia.Themes.Fluent.FluentThemeMode.Dark)
                            {
                                isDarkCached = true;
                                break;
                            }
                            else if (theme.Mode == Avalonia.Themes.Fluent.FluentThemeMode.Light)
                            {
                                isDarkCached = false;
                                break;
                            }
                        }
                        else*/ if (style is Avalonia.Markup.Xaml.Styling.StyleInclude include)
                        {
                            if (include.Source.IsAbsoluteUri)
                            {
                                if (include.Source.AbsoluteUri.Contains("BaseLight"))
                                {
                                    isDarkCached = false;
                                    break;
                                }
                                else if (include.Source.AbsoluteUri.Contains("BaseDark"))
                                {
                                    isDarkCached = true;
                                    break;
                                }
                            }
                            else
                            {
                                if (include.Source.OriginalString.Contains("Light"))
                                {
                                    isDarkCached = false;
                                    break;
                                }
                                else if (include.Source.OriginalString.Contains("Dark"))
                                {
                                    isDarkCached = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (isDarkCached == null)
                    {
                        isDarkCached = false;
                    }
                }

                return isDarkCached.Value;
            }
        }
    }
}
