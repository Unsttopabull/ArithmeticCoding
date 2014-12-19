using System;

namespace ArtimeticniKodirnik.DLL {

    public class Simbol : IComparable {
        public Simbol(ulong frekvenca, ulong zgornjaMeja, ulong spodnjaMeja, byte vrednost) {
            Frekvenca = frekvenca;
            ZgornjaMeja = zgornjaMeja;
            SpodnjaMeja = spodnjaMeja;
            Vrednost = vrednost;
        }

        public byte Vrednost { get; set; }

        public ulong Frekvenca { get; set; }

        public ulong ZgornjaMeja { get; set; }

        public ulong SpodnjaMeja { get; set; }


        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() {
            return string.Format("{3} | F: {0}, SP: {1}, ZG: {2}", Frekvenca, SpodnjaMeja, ZgornjaMeja, (char) Vrednost);
        }

        /// <summary>Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.</summary>
        /// <returns>A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes <paramref name="obj"/> in the sort order. Zero This instance occurs in the same position in the sort order as <paramref name="obj"/>. Greater than zero This instance follows <paramref name="obj"/> in the sort order. </returns>
        /// <param name="obj">An object to compare with this instance. </param><exception cref="T:System.ArgumentException"><paramref name="obj"/> is not the same type as this instance. </exception>
        public int CompareTo(object obj) {
            if (obj is double) {
                double val = (double) obj;

                if (val >= ZgornjaMeja) {
                    return -1;
                }
                else if (SpodnjaMeja > val) {
                    return 1;
                }
                return 0;
            }
            return 0;
        }
    }

}