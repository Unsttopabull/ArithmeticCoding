namespace ArtimeticniKodirnik {

    public class Simbol {

        public Simbol() {
            
        }

        public Simbol(ulong frekvenca, double verjetnost, ulong zgornjaMeja, ulong spodnjaMeja) {
            Frekvenca = frekvenca;
            Verjetnost = verjetnost;
            ZgornjaMeja = zgornjaMeja;
            SpodnjaMeja = spodnjaMeja;
        }

        public ulong Frekvenca { get; set; } 

        public double Verjetnost { get; set; }

        public ulong ZgornjaMeja { get; set; }

        public ulong SpodnjaMeja { get; set; }


        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() {
            return string.Format("F: {0}, P: {1}, SP: {2}, ZG: {3}", Frekvenca, Verjetnost, SpodnjaMeja, ZgornjaMeja);
        }
    }

}