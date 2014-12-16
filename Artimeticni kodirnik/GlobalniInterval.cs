using System;

namespace ArtimeticniKodirnik {

    public class GlobalniInterval {

        public GlobalniInterval(StBitov stBitov) {
            int stBitovNum;
            switch (stBitov) {
                case StBitov.Bit8:
                    stBitovNum = 8;
                    break;
                case StBitov.Bit16:
                    stBitovNum = 16;
                    break;
                case StBitov.Bit32:
                    stBitovNum = 32;
                    break;
                case StBitov.Bit64:
                    stBitovNum = 64;
                    break;
                default:
                    stBitovNum = 32;
                    break;
            }
            
            SpodnjaMeja = 0;
            ZgornjaMeja = ((int) Math.Pow(2, stBitovNum - 1)) - 1;
        }

        public double SpodnjaMeja { get; set; }
        public double ZgornjaMeja { get; set; }

        public double this[int numCetrtine] {
            get {
                double drugaCetrtina = (ZgornjaMeja + 1) / 2.0;
                switch (numCetrtine) {
                    case 1:
                        return drugaCetrtina / 2.0;
                    case 2:
                        return drugaCetrtina;
                    case 3:
                        return (drugaCetrtina / 2.0) * 3;
                    default:
                        throw new ArgumentException("Mora biti med 1 in 3.", "numCetrtine");
                }
            }
        }
    }

}