using System;

namespace BinIO {

    public class BinRead {
        private readonly byte[] _buffer;
        private readonly int _buffersize;
        private int _bytepos;
        private byte _bitpos;

        public BinRead(byte[] data) {
            _buffer = data;
            _buffersize = data.Length;
        }

        public ulong ReadBits(byte numbits) {
            if (numbits > 64) {
                Console.WriteLine("ERROR: Na enkrat lahko preberete največ 64 bitov.");
                Console.ReadLine();
                Environment.Exit(0);
            }

            //if (bytepos == 88)
            //    bytepos = bytepos;

            ulong rezultat = 0;
            for (int i = 0; (i < numbits) & !Eof(); i++) {
                // Če je bit postavljen na 1, prištevejemo ekvivalentno vrednost k rezultatu:
                if (((_buffer[_bytepos]) & (byte) (1 << _bitpos)) != 0) {
                    rezultat += ((ulong) 1 << i);
                }

                // Pomik na naslednji bit v bufferju:
                _bitpos++;
                if (_bitpos != 8) {
                    continue;
                }

                _bitpos = 0;
                _bytepos++;
                if (_bytepos == _buffersize) {
                    //return 0;   // FillBuffer
                }
            }

            return rezultat;
        }

        #region Branje podatkovnih tipov:

        public Byte ReadByte() {
            return (byte) ReadBits(8);
        }

        public SByte ReadSByte() {
            return (SByte) ReadBits(8);
        }

        public Int16 ReadInt16() {
            return (Int16) ReadBits(16);
        }

        public UInt16 ReadUInt16() {
            return (UInt16) ReadBits(16);
        }

        public Int32 ReadInt32() {
            return (Int32) ReadBits(32);
        }

        public UInt32 ReadUInt32() {
            return (UInt32) ReadBits(32);
        }

        public long ReadLong() {
            return (long) ReadBits(64);
        }

        public ulong ReadULong() {
            return ReadBits(64);
        }

        public float ReadFloat() {
            ulong temp = ReadBits(32);
            byte[] temp2 = BitConverter.GetBytes(temp);
            return BitConverter.ToSingle(temp2, 0);
        }

        public double ReadDouble() {
            ulong temp = ReadBits(64);
            byte[] temp2 = BitConverter.GetBytes(temp);

            return BitConverter.ToDouble(temp2, 0);
        }

        #endregion

        #region Dodatne metode za delovanje:

        // Metoda za preverjanje ce smo prisli do konca datoteke:
        public bool Eof() {
            return _bytepos == _buffersize;
        }

        // Metoda, ki vrne koliko bitov se lahko preberemo:
        public ulong BitsTillEof() {
            ulong izhod = (ulong) (_buffersize - _bytepos - 1) * 8;
            izhod += (ulong) 7 - _bitpos;

            return izhod;
        }

        #endregion
    }

}