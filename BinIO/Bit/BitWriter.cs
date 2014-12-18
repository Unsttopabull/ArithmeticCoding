using System;
using System.IO;

namespace BinIO {

    public class BitWriter {
        private readonly MemoryStream _ms;
        private byte _currByte;
        private byte _bitPos;

        public BitWriter() {
            _ms = new MemoryStream();
        }

        public byte BitPos { get { return _bitPos; } }

        public byte[] GetData() {
            byte[] data = _ms.ToArray();
            if (_bitPos > 0) {
                Array.Resize(ref data, data.Length + 1);

                //potisnemo bite na levo za število neuporabljenih bitov
                //npr 0000 0010 -> 0100 0000
                data[data.Length -1] = (byte) (_currByte << (8 - _bitPos));
            }
            return data;
        }

        public void WriteBits(ulong data, byte bits) {
            int newBitPos = _bitPos + bits;

            if (newBitPos == 8) {
                FillByte(data, bits);
            }
            else if(newBitPos > 8) {
                //koliko bitov je še praznih
                byte bitsToFill = (byte) (8 - _bitPos);

                FillByte(data, bitsToFill);

                //odstranimo bite porabljene za polnjene prejšnjega bajta
                data >>= bitsToFill;
                WriteBits(data, (byte) (bits - bitsToFill));
            }
            //zapišemo v tren bajt
            else{
                //preberemo "bits" bitov
                byte byteData = (byte) (data & (ulong) ((1 << bits) - 1));

                //premaknemo že vpisano na levo
                _currByte <<= bits;
                //vpišemo nove podatke
                _currByte |= byteData;

                _bitPos += bits;
            }
        }

        private void FillByte(ulong data, byte bitsToFill) {
            //pridobimo toliko bitov iz "data"
            byte byteData = (byte) (data & (ulong) ((1 << bitsToFill) - 1));

            //potisnemo že zapisano na levo
            _currByte <<= bitsToFill;
            //vpišemo nove bite
            _currByte |= byteData;

            //shranimo poln bajt
            _ms.WriteByte(_currByte);

            //resetiramo števce
            _currByte = 0;
            _bitPos = 0;
        }

        public void WriteByte(byte data) {
            WriteBits(data, 8);
        }

        public void WriteSByte(sbyte data) {
            WriteBits((ulong) data, 8);
        }

        public void WriteInt16(short data) {
            WriteBits((ulong) data, 16);
        }

        public void WriteUInt16(ushort data) {
            WriteBits(data, 16);
        }

        public void WriteInt32(int data) {
            WriteBits((ulong) data, 32);
        }

        public void WriteUInt32(uint data) {
            WriteBits(data, 32);
        }

        public void WriteLong(long data) {
            WriteBits((ulong) data, 64);
        }

        public void WriteULong(ulong data) {
            WriteBits(data, 64);
        }

        public void WriteFloat(float data) {
        }

        public void WriteDouble(double data) {
        }
    }

}