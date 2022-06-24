# AvaloniaColorPicker: a color picker for Avalonia

<img src="icon.svg" width="256" align="right">

[![License: LGPL v3](https://img.shields.io/badge/License-LGPL_v3-blue.svg)](https://www.gnu.org/licenses/lgpl-3.0)
[![Version](https://img.shields.io/nuget/v/AvaloniaColorPicker)](https://nuget.org/packages/AvaloniaColorPicker)

**AvaloniaColorPicker** is a colour picker control for Avalonia, with support for RGB, HSB and CIELAB colour spaces, palettes and colour blindness simulation.

The library contains three main controls:

* The `ColorPicker` class represents the core of the library: a color picker control that lets users specify a color in RGB, HSB or CIELAB components, either as raw values, or by using a 1-D slider combined with a 2-D surface. It can simulate how the colours would look when viewed by people with various kinds of color-blindness and it can suggest, for any given colour, a lighter and darker shade of the colour, as well as a "contrasting colour" that stands up against it, even when viewed by colour-blind people. It has a "palette" feature to store user-defined colours that persist between sessions and are shared between all applications using this control). Seven predefined palettes are also provided.
* The `ColorPickerWindow` class represents a window containing a `ColorPicker` and two buttons (`OK` and `Cancel`); this can be used as a dialog window to let users choose a colour.
* The `ColorButton` class represents a button displaying the currently selected colour, which can be clicked to choose another colour from the current palette or from a `ColorPickerWindow`.

In addition, the library contains a number of controls and interfaces that can be used to customise the interface of the colour picker.

The library is released under the [LGPLv3](https://www.gnu.org/licenses/lgpl-3.0.html) licence.

<img src="screenshot.png">

## Getting started

The library targets .NET Standard 2.0, thus it can be used in projects that target .NET Standard 2.0+ and .NET Core 2.0+.

To use the library in your project, you should install the [AvaloniaColorPicker Nuget package](https://www.nuget.org/packages/AvaloniaColorPicker/).

This repository also contains three very simple demo projects, one using the `ColorButton` control, one using the `ColorPicker` control, and one showing how to create a custom colour picker interface using the `CustomColorPicker` control.

## Usage

You will need to add the relevant `using` directive (in C# code) or the XML namespace (in the XAML code). You can then add controls from the AvaloniaColorPicker namespace. For example

```XAML
<Window ...
        xmlns:colorpicker="clr-namespace:AvaloniaColorPicker;assembly=AvaloniaColorPicker">
  ...
    <colorpicker:ColorButton Color="#56B4E9"></colorpicker:ColorButton>
  ...
</Window>
```

The [Wiki](https://github.com/arklumpus/AvaloniaColorPicker/wiki) for this repository contains more information about the controls included in the library and how they can be used.

## Source code

The source code for the library is available in this repository. In addition to the `AvaloniaColorPicker` library project and the three demo applications, the repository also contains a project for an application that is used to generate the `LabColorSpace.bin` cache file that is used by `AvaloniaColorPicker` to display 3D "sections" of the CIELAB colour space.