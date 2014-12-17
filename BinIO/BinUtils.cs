using System;
using System.Text;

namespace BinIO {

    public static class BinUtils {

        public static string Long2Bin(ulong data, int numBits) {
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

        public static ulong Bin2Long(string data) {
            return Convert.ToUInt64(data, 2);
        }

    }
}
