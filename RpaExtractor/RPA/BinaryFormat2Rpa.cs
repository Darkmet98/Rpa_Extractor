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

using System.Globalization;
using System.Text;
using Yarhl.FileFormat;
using Yarhl.IO;
using System.IO;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace RpaExtractor
{
    class BinaryFormat2Rpa : IConverter<BinaryFormat, Rpa>
    {
        Rpa Result { get; set; }
        DataReader Reader { get; set; }

        public Rpa Convert(BinaryFormat source)
        {
            Result = new Rpa();
            Reader = new DataReader(source.Stream)
            {
                DefaultEncoding = new UTF8Encoding(),
                Endianness = EndiannessMode.LittleEndian,
            };

            //First, get the key
            GetKey();
            //Now, get the position
            GetHeaderPosition();
            //Deflate the header
            DecompressHeader();
            //Get the file list
            GetFileList();
            //Get the files
            GetFiles();

            return Result;
        }

        private void GetKey()
        {
            Reader.Stream.PushToPosition(0x19);
            Result.Key ^= ulong.Parse(Reader.ReadString(8, Encoding.UTF8), NumberStyles.HexNumber);
            Reader.Stream.PopPosition();
        }

        private void GetHeaderPosition()
        {
            Reader.Stream.PushToPosition(0x8);
            Result.HeaderPosition = long.Parse(Reader.ReadString(0x10, Encoding.UTF8), NumberStyles.HexNumber);
            Reader.Stream.PopPosition();
        }

        private void DecompressHeader()
        {
            Reader.Stream.Position = Result.HeaderPosition;
            
            Result.Header = Reader.ReadBytes(System.Convert.ToInt32(Reader.Stream.Length - Result.HeaderPosition));

            Stream temp = new MemoryStream();

            using (var dec = new InflaterInputStream(new MemoryStream(Result.Header)))
            {
                dec.CopyTo(temp);
                Result.HeaderStream = new DataStream(temp);
            }
        }


        private void GetFileList()
        {
            DataReader h_Reader = new DataReader(Result.HeaderStream)
            {
                DefaultEncoding = new UTF8Encoding(),
                Endianness = EndiannessMode.LittleEndian,
            };

            h_Reader.Stream.Position += 7;

            do
            {
                int name = h_Reader.ReadInt32();
                Result.Names.Add(h_Reader.ReadString(name));

                bool end = false;
                do
                {
                    byte value = h_Reader.ReadByte();
                    if (value == 0x8A)
                    {
                        if (h_Reader.ReadByte() == 0x04)
                            end = true;
                        else
                            h_Reader.Stream.Position -= 1;
                    }
                    else if (value == 0x04)
                    {
                        h_Reader.Stream.Position -= 2;
                        if (h_Reader.ReadByte() == 0x8A)
                            end = true;
                        else
                            h_Reader.Stream.Position += 1;
                    }

                }
                while (!end);


                Result.Positions.Add(Decrypt(h_Reader.ReadInt32()));
                h_Reader.Stream.Position += 1;
                Result.Sizes.Add(Decrypt(h_Reader.ReadInt32()));


                end = false;
                do
                {
                    byte value = h_Reader.ReadByte();
                    if (value == 0x61)
                    {
                        byte value2 = h_Reader.ReadByte();
                        if (value2 == 0x58)
                            end = true;
                        else if (value2 == 0x75)
                            if (h_Reader.ReadByte() == 0x2E)
                                    return;
                            else
                                h_Reader.Stream.Position -= 2;
                        else
                            h_Reader.Stream.Position -= 1;
                    }
                    else if (value == 0x58)
                    {
                        h_Reader.Stream.Position -= 2;
                        if (h_Reader.ReadByte() == 0x61)
                            end = true;
                        else
                            h_Reader.Stream.Position += 1;
                    }

                }
                while (!end);

            }
            while (!h_Reader.Stream.EndOfStream);
        }

        private void GetFiles()
        {
            for(int i = 0; i < Result.Names.Count; i++)
            {
                Reader.Stream.Position = Result.Positions[i];
                Result.Files.Add(Reader.ReadBytes(Result.Sizes[i]));
            }
        }

        private int Decrypt(int value)
        {
            return value ^ (int)Result.Key;
        }

    }
}