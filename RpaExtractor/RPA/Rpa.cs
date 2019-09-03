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

using System.Collections.Generic;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace RpaExtractor
{
    class Rpa : Format
    {
        public ulong Key { get; set; }
        public long HeaderPosition { get; set; }
        public byte[] Header { get; set; }
        public DataStream HeaderStream { get; set; }
        public List<int> Positions { get; set; }
        public List<int> Sizes { get; set; }
        public List<string> Names { get; set; }
        public List<byte[]> Files { get; set; }


        public Rpa()
        {
            Positions = new List<int>();
            Sizes = new List<int>();
            Names = new List<string>();
            Files = new List<byte[]>();
        }
    }
}
