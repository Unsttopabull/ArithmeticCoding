namespace ArtimeticniKodirnik.DLL {

    public static class AC {
        private const StBitov STEVILO_BITOV = StBitov.Bit64;

        public static byte[] Encode(byte[] data) {
            Kodirnik kodirnik = new Kodirnik(STEVILO_BITOV);
            return kodirnik.Kodiraj(data);
        }

        public static byte[] Decode(byte[] data) {
            Dekodirnik d = new Dekodirnik();
            return d.Dekodiraj(data);
        }

        public static string Author() {
            return "Martin Kraner E5020649";
        }
    }

}