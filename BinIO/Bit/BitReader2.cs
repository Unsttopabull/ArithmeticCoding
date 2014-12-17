using System;
using System.Collections;
using System.IO;
using System.Linq;

namespace BinIO {

    public class BitReader2 {
        private readonly MemoryStream _ms;
        private byte _currByte;
        private byte _bitPos;

        private BitReader2(MemoryStream ms) {
            _ms = ms;
            GetByte();
        }

        public BitReader2(byte[] data) : this(new MemoryStream(data)){
        }

        public BitReader2(string imeDatoteke) : this(new MemoryStream(File.ReadAllBytes(imeDatoteke))){
        }

        public bool EOF { get; private set; }

        private bool GetByte() {
            int b = _ms.ReadByte();
            if (b == -1) {
                EOF = true;
                return false;
            }
            _currByte = (byte) b;
            return true;
        }

        public byte ReadCurrByte(byte numBits) {
            return ReadBits(numBits)[0];
        }

        public byte[] ReadBits(byte numbits) {
            int newBitPos = _bitPos + numbits;

            //prebrali bomo trenutni bajt do konca
            if (newBitPos == 8) {
                return new[] { TrenBajtDoKonca(numbits) };
            }

            if (newBitPos < 8) {
                //npr. želimo 4 bite 0000 1111
                byte maska = (byte) ((1 << numbits) - 1);

                //npr. BitPos = 1, (smo na 7 bitu)
                //želimo prebrati 4 bite (7 do 3 bita)
                //(8 - 1) - 4 = 3
                byte numShift = (byte) ((8 - _bitPos) - numbits);

                //0111 1000
                maska <<= numShift;

                _bitPos += numbits;

                byte biti = (byte) (_currByte & maska);

                //shiftamo na desno da prenesemo bite na LSB
                biti >>= numShift;

                return new[] { biti };
            }

            BitWriter bw = new BitWriter();

            //beremo čez trenutni bajt
            byte preostaloBitovVtrenBajtu = (byte) (8 - _bitPos);
            byte data = TrenBajtDoKonca(preostaloBitovVtrenBajtu);

            bw.WriteBits(data, preostaloBitovVtrenBajtu);

            int seZaBrati = numbits - preostaloBitovVtrenBajtu;

            //pomaknemo delni bajt na levi konec
            //data <<= (seZaBrati + 1);

            int stCelihBajtov = seZaBrati / 8;

            for (int i = 0; i < stCelihBajtov; i++) {
                if (i != 0) {
                    if (!GetByte()) {
                        break;
                    }
                }
                bw.WriteByte(_currByte);

                //spojimo bajte
                //postavimo "bajt" na mesto kjer bo nastopal v rezultatu branja
                //in ga z ALI vpišemo v rezultat
                //int dolzShiftaZaBajtI = (seZaBrati - 8 * i);

                //int dolzShiftaZaBajtI = (preostaloBitovVtrenBajtu + 8 * i);
                //data |= ((ulong) _currByte) << dolzShiftaZaBajtI;
            }

            if (!EOF) {
                byte preostaloBitovZaBrat = (byte) (seZaBrati - (stCelihBajtov * 8));

                //preberemo naslednji bajt
                GetByte();

                byte zacetek = ReadCurrByte(preostaloBitovZaBrat);
                bw.WriteBits(zacetek, preostaloBitovZaBrat);

                //ulong zacetek = ReadBits(preostaloBitovZaBrat);
                //zacetek <<= numbits - 1;

                //data |= zacetek;
            }
            return bw.GetData();
        }

        private byte TrenBajtDoKonca(byte numbits) {
            byte maska = (byte) ((1 << numbits) - 1);

            _bitPos = 0;
            GetByte();

            return (byte) (_currByte & maska);
        }

        public byte ReadByte() {
            return ReadBits(8)[0];
        }

        public sbyte ReadSByte() {
            return (sbyte) ReadBits(8)[0];
        }

        public short ReadInt16() {
            short data = BitConverter.ToInt16(ReadBits(16), 0);
            return data;
        }

        public ushort ReadUInt16() {
            ushort data = BitConverter.ToUInt16(ReadBits(16), 0);
            return data;
        }

        public int ReadInt32() {
            byte[] readBits = ReadBits(32);

            //ulong data = ReadBits(32);
            int int32 = BitConverter.ToInt32(readBits, 0);

            return int32;
            //return (int) ReadBits(32);
        }

        public int ReadInt32R() {
            //ulong ul = ReadBits(32);
            //byte[] array = BitConverter.GetBytes(ul).Reverse().ToArray();

            //int int32 = BitConverter.ToInt32(array, 0);
            //return int32;
            return 0;
        }

        public uint ReadUInt32() {
            return 0;
        }

        public long ReadLong() {
            return 0;
        }

        public ulong ReadULong() {
            return 0;
        }

        public float ReadFloat() {
            return 0;
        }

        public double ReadDouble() {
            return 0;
        }
    }

}