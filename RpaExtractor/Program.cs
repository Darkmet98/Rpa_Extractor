// Copyright (C) 2019 Pedro Garau Martínez
//
// This file is part of RpaExtractor.
//
// RpaExtractor is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// RpaExtractor is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with RpaExtractor. If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.IO;
using Yarhl.FileFormat;
using Yarhl.FileSystem;

namespace RpaExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            Disclaimer();
            if(args == null || args.Length == 0)
            {
                Console.WriteLine("\nUsage: RpaExtractor \"File.rpa\"");
                return;

            }
            else if (!File.Exists(args[0])) Console.WriteLine("Error, the file does not exist.");
            else
            {
                Console.WriteLine("Exporting " + args[0] + "...");
                // 1
                Node nodo = NodeFactory.FromFile(args[0]); // BinaryFormat

                // 2
                IConverter<BinaryFormat, Rpa> FadConverter = new BinaryFormat2Rpa { };
                Node nodoScript = nodo.Transform(FadConverter);

                // 3
                IConverter<Rpa, NodeContainerFormat> ContainerConverter = new Rpa2NodeContainer { };
                Node nodoContainer = nodoScript.Transform(ContainerConverter);

                //4

                string foldername = args[0].Remove(args[0].Length - 4);
                if (!Directory.Exists(foldername)) Directory.CreateDirectory(foldername);

                foreach (var child in Navigator.IterateNodes(nodoContainer))
                {
                    if (child.Stream == null)
                        continue;
                    string output = Path.Combine(foldername + Path.DirectorySeparatorChar + child.Name.Replace('\\', Path.DirectorySeparatorChar));
                    child.Stream.WriteTo(output);
                }
            }
        }

        private static void Disclaimer()
        {
            Console.WriteLine("RpaExtractor — A rpa extractor for renpy games by Darkmet98.\nVersion: 1.1");
            Console.WriteLine("Thanks to Pleonex for the Yarhl libraries and Shizmob for the python release (rpatool) (Some of work is based from their sources).");
            Console.WriteLine("This program is licensed with a GPL V3 license.");
        }
    }
}
