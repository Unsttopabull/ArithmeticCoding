namespace ArtimeticniKodirnik {

    public class Simbol {

        public Simbol() {
            
        }

        public Simbol(ulong frekvenca, double verjetnost, ulong zgornjaMeja, ulong spodnjaMeja, byte vrednost) {
            Frekvenca = frekvenca;
            Verjetnost = verjetnost;
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
            return string.Format("{4} | F: {0}, P: {1}, SP: {2}, ZG: {3}", Frekvenca, Verjetnost, SpodnjaMeja, ZgornjaMeja, (char) Vrednost);
        }
    }

}