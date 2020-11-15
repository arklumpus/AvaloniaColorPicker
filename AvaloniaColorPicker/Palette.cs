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

using Avalonia.Media;
using System.Collections.Generic;
using System.IO;

namespace AvaloniaColorPicker
{
    internal class Palette
    {
        public static Palette CurrentPalette { get; set; } = null;

        public string Name { get; }
        public string Description { get; }

        public string FileName { get; }

        public List<Color> Colors { get; }

        public Palette(string name, string description, string fileName)
        {
            this.Name = name;
            this.Description = description;
            this.FileName = fileName;

            this.Colors = new List<Color>();
        }

        public Palette(string name, string description, string fileName, IEnumerable<Color> colors)
        {
            this.Name = name;
            this.Description = description;
            this.FileName = fileName;

            this.Colors = new List<Color>(colors);
        }

        public void Save()
        {
            using (StreamWriter sw = new StreamWriter(this.FileName))
            {
                sw.WriteLine("#" + this.Name);
                sw.WriteLine("#" + this.Description);
                for (int i = 0; i < Colors.Count; i++)
                {
                    sw.Write(Colors[i].R);
                    sw.Write(",");
                    sw.Write(Colors[i].G);
                    sw.Write(",");
                    sw.Write(Colors[i].B);
                    sw.Write(",");
                    sw.Write(Colors[i].A);
                    sw.WriteLine();
                }
            }
        }
    }
}
