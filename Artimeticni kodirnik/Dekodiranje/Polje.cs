using System;

namespace ArtimeticniKodirnik.Dekodiranje {

    public class Polje {
        private const ulong MASKA_8 = 0x7F;
        private const ulong MASKA_16 = 0x7FFF; 
        private const ulong MASKA_32 = 0x7FFFFFFF; 
        private const ulong MASKA_64 = 0x7FFFFFFFFFFFFFFF;
        private ulong _vrednost;
        private readonly ulong _maska;

        public Polje(StBitov stBitov) {
            switch (stBitov) {
                case StBitov.Bit8:
                    _maska = MASKA_8;
                    break;
                case StBitov.Bit16:
                    _maska = MASKA_16;
                    break;
                case StBitov.Bit32:
                    _maska = MASKA_32;
                    break;
                case StBitov.Bit64:
                    _maska = MASKA_64;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("stBitov");
            }
        }

        public Polje(StBitov stBitov, ulong zacetnaVrednost) : this(stBitov) {
            _vrednost = zacetnaVrednost;
        }

        public void AddNextBit(byte bit) {
            _vrednost = (_vrednost << 1) + bit;
        }

        public static implicit operator ulong(Polje p) {
            return p._vrednost & p._maska;
        }

        public static Polje operator -(Polje polje, ulong vrednost) {
            polje._vrednost -= vrednost;
            return polje;
        }

        public static Polje operator <<(Polje polje, int stBitov) {
            polje._vrednost <<= stBitov;
            return polje;
        }
    }

}