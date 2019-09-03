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

using Yarhl.FileFormat;
using Yarhl.FileSystem;

namespace RpaExtractor
{
    class Rpa2NodeContainer : IConverter<Rpa, NodeContainerFormat>
    {
        public NodeContainerFormat Convert(Rpa source)
        {
            NodeContainerFormat container = new NodeContainerFormat();

            for (int i = 0; i < source.Files.Count; i++)
            {
                Node child = NodeFactory.FromMemory(source.Names[i].Replace('/', '\\'));
                child.Stream.Write(source.Files[i], 0, source.Files[i].Length);
                container.Root.Add(child);
            }
            return container;
        }

    }
}
