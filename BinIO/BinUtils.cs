using System;
using System.Collections.Generic;
using System.Text;

namespace BinIO {

    public static class BinUtils {

        public static string Bytes2Bin(IList<byte> bajti) {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bajti) {
                sb.Append(ULong2BinBajti(b, 8) + " ");
            }
            return sb.ToString().TrimStart(' ');
        }

        public static string ULong2Bin(ulong data, int numBits) {
            if (numBits <= 0) {
                return null;
            }

            StringBuilder sb = new StringBuilder(numBits);

            //npr. 25 = 11001, numBits = 4
            //
            // potek grajenja niza:
            // _ _ _ _
            // _ _ _ 1
            // _ _ 1 0
            // _ 1 0 0
            // 1 0 0 1

            while (numBits > 0) {
                //če je več od 1 bit prižgan, drugače ugašnjen
                sb.Append((data & ((ulong) 0x01 << --numBits)) != 0 ? "1" : "0");
            }

            return sb.ToString();
        }

        public static string Long2Bin(long data) {
            return ULong2Bin((ulong) data, 64);
        }

        public static string Int2Bin(int data) {
            return ULong2Bin((ulong) data, 32);
        }

        public static string UInt2Bin(uint data) {
            return ULong2Bin(data, 32);
        }

        public static string Short2Bin(short data) {
            return ULong2Bin((ulong) data, 16);
        }

        public static string UShort2Bin(ushort data) {
            return ULong2Bin(data, 16);
        }

        public static string Byte2Bin(byte data) {
            return ULong2Bin(data, 8);
        }       

        public static string SByte2Bin(sbyte data) {
            return ULong2Bin((ulong) data, 8);
        }



        public static string ULong2BinBajti(ulong data, int numBits) {
            if (numBits <= 0) {
                return null;
            }

            StringBuilder sb = new StringBuilder(numBits);

            //npr. 25 = 11001, numBits = 4
            //
            // potek grajenja niza:
            // _ _ _ _
            // _ _ _ 1
            // _ _ 1 0
            // _ 1 0 0
            // 1 0 0 1

            while (numBits > 0) {
                if (numBits % 4 == 0) {
                    sb.Append(" ");
                }

                //če je več od 1 bit prižgan, drugače ugašnjen
                sb.Append((data & ((ulong) 0x01 << --numBits)) != 0 ? "1" : "0");
            }

            return sb.ToString().TrimStart(' ');
        }

        public static string Long2BinBajti(long data) {
            return ULong2Bin((ulong) data, 64);
        }

        public static string Int2BinBajti(int data) {
            return ULong2BinBajti((ulong) data, 32);
        }

        public static string UInt2BinBajti(uint data) {
            return ULong2BinBajti(data, 32);
        }

        public static string Short2BinBajti(short data) {
            return ULong2BinBajti((ulong) data, 16);
        }

        public static string UShort2BinBajti(ushort data) {
            return ULong2BinBajti(data, 16);
        }

        public static string Byte2BinBajti(byte data) {
            return ULong2BinBajti(data, 8);
        }       

        public static string SByte2BinBajti(sbyte data) {
            return ULong2BinBajti((ulong) data, 8);
        }

        public static ulong Bin2Long(string data) {
            return Convert.ToUInt64(data, 2);
        }

    }
}
