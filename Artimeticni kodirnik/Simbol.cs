namespace ArtimeticniKodirnik {

    public class Simbol {

        public Simbol(ulong frekvenca, ulong zgornjaMeja, ulong spodnjaMeja, byte vrednost) {
            Frekvenca = frekvenca;
            ZgornjaMeja = zgornjaMeja;
            SpodnjaMeja = spodnjaMeja;
            Vrednost = vrednost;
        }

        public byte Vrednost { get; set; }

        public ulong Frekvenca { get; set; } 

        public double Verjetnost { get; set; }

        public ulong ZgornjaMeja { get; set; }

        public ulong SpodnjaMeja { get; set; }


        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() {
            return string.Format("{3} | F: {0}, P: {1}, SP: {1}, ZG: {2}", Frekvenca, SpodnjaMeja, ZgornjaMeja, (char) Vrednost);
        }
    }

}