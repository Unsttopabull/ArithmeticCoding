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

        public string CurrByteStr {
            get { return BinUtils.Byte2BinBajti(_currByte); }
        }

        public void SeekToStart() {
            _ms.Seek(0, SeekOrigin.Begin);
            _bitPos = 0;
            GetByte();
        }

        public byte[] ToArray() {
            return _ms.ToArray();
        }

        private bool GetByte() {
            int b = _ms.ReadByte();
            if (b == -1) {
                EOF = true;
                return false;
            }
            _currByte = (byte) b;
            _bitPos = 0;
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
            byte bitsToFull = (byte) (8 - _bitPos);
            ulong data = TrenBajtDoKonca(bitsToFull);

            int seZaBrati = numbits - bitsToFull;

            int stCelihBajtov = seZaBrati / 8;

            for (int i = 0; i < stCelihBajtov; i++) {
                if (i != 0) {
                    if (!GetByte()) {
                        break;
                    }
                }

                //spojimo bajte
                //postavimo "_currByte" na mesto kjer bo nastopal v rezultatu branja
                //in ga z ALI vpišemo v rezultat
                int dolzShiftaZaBajtI = (bitsToFull + 8 * i);
                data |= ((ulong) _currByte) << dolzShiftaZaBajtI;
            }

            if (EOF) {
                return data;
            }

            byte preostaloBitovZaBrat = (byte) (seZaBrati - (stCelihBajtov * 8));

            //preberemo naslednji bajt če še ga nismo
            if (stCelihBajtov > 0) {
                GetByte();
            }

            ulong zacetek = ReadBits(preostaloBitovZaBrat);
            zacetek <<= numbits - preostaloBitovZaBrat;

            data |= zacetek;
            return data;
        }

        private ulong TrenBajtDoKonca(byte numbits) {
            byte maska = (byte) ((1 << numbits) - 1);

            ulong data = (ulong) (_currByte & maska);
            GetByte();

            return data;
        }

        public byte[] ReadBytes(int numBytes) {
            if (numBytes <= 0) {
                return new byte[0];
            }

            byte[] bajti = new byte[numBytes];
            for (int i = 0; i < numBytes; i++) {
                bajti[i] = ReadByte();
            }

            return bajti;
        }

        public byte ReadByte() {
            return (byte) ReadBits(8);
        }

        public sbyte ReadSByte() {
            return (sbyte) ReadBits(8);
        }

        public short ReadInt16() {
            return (short) ReadBits(16);
        }

        public ushort ReadUInt16() {
            return (ushort) ReadBits(16);
        }

        public int ReadInt32() {
            ulong data = ReadBits(32);

            //int int32 = BitConverter.ToInt32(BitConverter.GetBytes(data), 0);
            return (int) data;
        }

        public uint ReadUInt32() {
            return (uint) ReadBits(32);
        }

        public long ReadInt64() {
            return (long) ReadBits(64);
        }

        public ulong ReadUIn64() {
            return ReadBits(64);
        }

        public float ReadFloat() {
            return BitConverter.ToSingle(BitConverter.GetBytes(ReadBits(32)), 0);
        }

        public double ReadDouble() {
            return BitConverter.ToDouble(BitConverter.GetBytes(ReadBits(64)), 0);
        }
    }

}