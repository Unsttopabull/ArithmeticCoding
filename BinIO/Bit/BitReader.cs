using System;
using System.Collections;
using System.IO;
using System.Linq;

namespace BinIO {

    public class BitReader {
        private readonly MemoryStream _ms;
        private byte _currByte;
        private byte _bitPos;

        private BitReader(MemoryStream ms) {
            _ms = ms;
            GetByte();
        }

        public BitReader(byte[] data) : this(new MemoryStream(data)){
        }

        public BitReader(string imeDatoteke) : this(new MemoryStream(File.ReadAllBytes(imeDatoteke))){
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

        public ulong ReadBits(byte numbits) {
            int newBitPos = _bitPos + numbits;

            //prebrali bomo trenutni bajt do konca
            if (newBitPos == 8) {
                return TrenBajtDoKonca(numbits);
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

                ulong biti = (ulong) (_currByte & maska);

                //shiftamo na desno da prenesemo bite na LSB
                biti >>= numShift;

                return biti;
            }

            //beremo čez trenutni bajt
            byte preostaloBitovVtrenBajtu = (byte) (8 - _bitPos);
            ulong data = TrenBajtDoKonca(preostaloBitovVtrenBajtu);

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

                //spojimo bajte
                //postavimo "bajt" na mesto kjer bo nastopal v rezultatu branja
                //in ga z ALI vpišemo v rezultat
                //int dolzShiftaZaBajtI = (seZaBrati - 8 * i);

                int dolzShiftaZaBajtI = (preostaloBitovVtrenBajtu + 8 * i);
                data |= ((ulong) _currByte) << dolzShiftaZaBajtI;
            }

            if (!EOF) {
                byte preostaloBitovZaBrat = (byte) (seZaBrati - (stCelihBajtov * 8));

                //preberemo naslednji bajt
                GetByte();

                ulong zacetek = ReadBits(preostaloBitovZaBrat);
                zacetek <<= numbits - preostaloBitovZaBrat;

                data |= zacetek;
            }
            return data;
        }

        private ulong TrenBajtDoKonca(byte numbits) {
            byte maska = (byte) ((1 << numbits) - 1);

            _bitPos = 0;
            GetByte();

            return (ulong) (_currByte & maska);
        }

        public byte ReadByte() {
            return (byte) ReadBits(8);
        }

        public sbyte ReadSByte() {
            return 0;
        }

        public short ReadInt16() {
            return 0;
        }

        public ushort ReadUInt16() {
            return 0;
        }

        public int ReadInt32() {
            ulong data = ReadBits(32);

            int int32 = BitConverter.ToInt32(BitConverter.GetBytes(data), 0);
            return int32;
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