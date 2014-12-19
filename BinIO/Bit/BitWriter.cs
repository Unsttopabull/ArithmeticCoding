using System;
using System.IO;

namespace BinIO {

    public class BitWriter {
        private readonly MemoryStream _ms;
        private byte _currByte;
        private byte _bitPos;

        public BitWriter() {
			GC.Collect();
            _ms = new MemoryStream();
        }
		
		public BitWriter(int initialCapacity){
			//GC.Collect();
			_ms = new MemoryStream(initialCapacity);
		}

        public byte BitPos { get { return _bitPos; } }

        //public string CurrByteStr {
        //    get { return BinUtils.Byte2BinBajti(_currByte); }
        //}

        public long NumBytes {
            get { return _bitPos > 0 ? _ms.Length + 1 : _ms.Length; }
        }

        public byte[] GetData() {
            byte[] data = _ms.ToArray();
            if (_bitPos > 0) {
                Array.Resize(ref data, data.Length + 1);

                //potisnemo bite na levo za število neuporabljenih bitov
                //npr (01) = 0000 0010 -> 0100 0000
                byte trenBajt = _currByte;
                data[data.Length -1] = (byte) (trenBajt << (8 - _bitPos));
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
                ulong maska = (ulong) ((1 << bits) - 1);
                //string maskaStr = BinUtils.ULong2BinBajti(maska, 8);

                byte byteData = (byte) (data & maska);

                //string byteDataStr = BinUtils.ULong2BinBajti(data, 8);

                //premaknemo že vpisano na levo
                _currByte <<= bits;
                //vpišemo nove podatke
                _currByte |= byteData;

                _bitPos += bits;
            }
        }

        private void FillByte(ulong data, byte bitsToFill) {
            //pridobimo toliko "bitsToFill" bitov iz "data"
            ulong maska = (ulong) ((1 << bitsToFill) - 1);
            //string maskaStr = BinUtils.ULong2BinBajti(maska, 8);

            //string dataStr = BinUtils.ULong2BinBajti(data, 64);

            byte byteData = (byte) (data & maska);

            //string byteDataStr = BinUtils.ULong2BinBajti(byteData, 8);

            //potisnemo že zapisano na levo
            _currByte <<= bitsToFill;
            //vpišemo nove bite
            _currByte |= byteData;

            //shranimo poln bajt
            _ms.WriteByte(_currByte);

            //resetiramo števce in buffer
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

        public void WriteInt64(long data) {
            WriteBits((ulong) data, 64);
        }

        public void WriteUInt64(ulong data) {
            WriteBits(data, 64);
        }

        public void WriteFloat(float data) {
            WriteInt32(BitConverter.ToInt32(BitConverter.GetBytes(data), 0));
        }

        public void WriteDouble(double data) {
            WriteInt64(BitConverter.ToInt64(BitConverter.GetBytes(data), 0));
        }
    }

}