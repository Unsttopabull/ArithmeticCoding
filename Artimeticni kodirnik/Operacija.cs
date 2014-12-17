namespace ArtimeticniKodirnik {

    public class Operacija {
        private readonly string _operacija;

        public Operacija(string operacija, string izhod) {
            _operacija = operacija;
            Izhod = izhod;
        }

        public Operacija(string metoda, ulong spodnjaMeja, ulong zgornjaMeja, string bit = null, string e3Bits = null) {
            if (metoda.ToLowerInvariant() == "e3") {
                _operacija = string.Format("E3({0}, {1})", spodnjaMeja, zgornjaMeja);
            }
            else {
                Izhod = bit + e3Bits;
                _operacija = string.Format("{0}({1}, {2}) Out => {3}{4}", metoda, spodnjaMeja, zgornjaMeja, bit, e3Bits);
            }
        }

        public string Izhod { get; set; }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() {
            return _operacija;
        }
    }

}