using System;
using System.Collections.Generic;
using System.Linq;
using BinIO;

namespace ArtimeticniKodirnik.DLL {

    public class Dekodirnik {
        private Simbol[] _tabelaFrekvenc;
        private BitReader _vhod;
        private LinkedList<byte> _list;
        private ulong _cF;
        private int _stBitov;

        private ulong _prvaCetrtina;
        private ulong _drugaCetrtina;
        private ulong _tretjaCetrtina;

        private ulong _spodnjaMeja;
        private ulong _zgornjaMeja;

        private ulong _polje;

        public byte[] Dekodiraj(string inFile) {
            return Dekodiraj(new BitReader(inFile));
        }

        public byte[] Dekodiraj(byte[] podatki) {
            return Dekodiraj(new BitReader(podatki));
        }

        private byte[] Dekodiraj(BitReader read) {
            _cF = 0;
            _vhod = read;

            _list = new LinkedList<byte>();

            ulong readBits = _vhod.ReadBits(2);

            switch (readBits) {
                case 0:
                    _stBitov = 8;
                    break;
                case 1:
                    _stBitov = 16;
                    break;
                case 3:
                    _stBitov = 64;
                    break;
                default:
                    _stBitov = 32;
                    break;
            }

            _spodnjaMeja = 0;
            _zgornjaMeja = (1UL << (_stBitov - 1)) - 1;

            _drugaCetrtina = (_zgornjaMeja + 1) >> 1;
            _prvaCetrtina = _drugaCetrtina >> 1;
            _tretjaCetrtina = _prvaCetrtina * 3;

            NapolniTabeloUrejeno();

            _polje = 0;
            for (int i = 0; i < _stBitov - 1; i++) {
                _polje = (_polje << 1) + _vhod.ReadBits(1);
            }

            ulong iter = 1;
            do {
                ulong korak = (_zgornjaMeja - _spodnjaMeja + 1) / _cF;
                ulong vrednost = (_polje - _spodnjaMeja) / korak;

                Simbol simbol = NajdiSimbol(vrednost);
                if (simbol == null) {
                    Console.Error.WriteLine("Ni bilo mogoče najti simbola");
                    return null;
                }

                _list.AddLast(simbol.Vrednost);

                _zgornjaMeja = _spodnjaMeja + korak * simbol.ZgornjaMeja - 1;
                _spodnjaMeja = _spodnjaMeja + korak * simbol.SpodnjaMeja;

                bool e1, e2;
                do {
                    e1 = false;
                    e2 = false;

                    if (_zgornjaMeja < _drugaCetrtina) {
                        //E1
                        e1 = true;

                        _spodnjaMeja <<= 1;
                        _zgornjaMeja = (_zgornjaMeja << 1) + 1;

                        _polje = (_polje << 1) + _vhod.ReadBits(1);
                    }
                    else if (_spodnjaMeja >= _drugaCetrtina) {
                        //E2
                        e2 = true;

                        _spodnjaMeja = (_spodnjaMeja - _drugaCetrtina) << 1;
                        _zgornjaMeja = ((_zgornjaMeja - _drugaCetrtina) << 1) + 1;
                        _polje = ((_polje - _drugaCetrtina) << 1) + _vhod.ReadBits(1);
                    }
                }
                while (e1 || e2); //dokler ne e1 in e2 nista izpolnjena

                while (_spodnjaMeja >= _prvaCetrtina && _zgornjaMeja < _tretjaCetrtina) {
                    //E3
                    _spodnjaMeja = (_spodnjaMeja - _prvaCetrtina) << 1;
                    _zgornjaMeja = ((_zgornjaMeja - _prvaCetrtina) << 1) + 1;

                    _polje = ((_polje - _prvaCetrtina) << 1) + _vhod.ReadBits(1);
                }
                iter++;
            }
            while (iter <= _cF);

            return _list.ToArray();
        }

        private Simbol NajdiSimbol(double vrednost) {
            int binarySearch = Array.BinarySearch(_tabelaFrekvenc, vrednost);
            if (binarySearch >= 0) {
                return _tabelaFrekvenc[binarySearch];
            }
            return null;
        }

        private void NapolniTabeloUrejeno() {
            ulong stBitovZaFreq = _vhod.ReadBits(2);

            int stSimbolov = 256;
            bool samoFreq = false;

            //če so zapisane samo frekvence (vse)
            ulong samoFreqBit = _vhod.ReadBits(1);
            if (samoFreqBit != 0) {
                samoFreq = true;
            }

            if (!samoFreq) {
                stSimbolov = _vhod.ReadByte();
                if (stSimbolov == 0) {
                    stSimbolov = 256;
                }
            }

            switch (stBitovZaFreq) {
                case 0:
                    ReadTableByte(stSimbolov, samoFreq);
                    break;
                case 1:
                    ReadTableUInt16(stSimbolov, samoFreq);
                    break;
                case 2:
                    ReadTableUInt32(stSimbolov, samoFreq);
                    break;
                case 3:
                    ReadTableUInt64(stSimbolov, samoFreq);
                    break;
            }
        }

        private void ReadTableByte(int stSimbolov, bool samoFreq) {
            LinkedList<Simbol> simboli = new LinkedList<Simbol>();

            ulong spMeja = 0;
            for (int i = 0; i < stSimbolov; i++) {
                byte vrednost;
                if (!samoFreq) {
                    vrednost = _vhod.ReadByte();
                }
                else {
                    vrednost = (byte) i;
                }

                ulong frekvenca = _vhod.ReadByte();

                _cF += frekvenca;

                ulong zgMeja = spMeja + frekvenca;
                simboli.AddLast(new Simbol(frekvenca, zgMeja, spMeja, vrednost));

                spMeja = zgMeja;
            }

            _tabelaFrekvenc = simboli.ToArray();
        }

        private void ReadTableUInt16(int stSimbolov, bool samoFreq) {
            LinkedList<Simbol> simboli = new LinkedList<Simbol>();

            ulong spMeja = 0;
            for (int i = 0; i < stSimbolov; i++) {
                byte vrednost;
                if (!samoFreq) {
                    vrednost = _vhod.ReadByte();
                }
                else {
                    vrednost = (byte) i;
                }

                ulong frekvenca = _vhod.ReadUInt16();

                _cF += frekvenca;

                ulong zgMeja = spMeja + frekvenca;
                simboli.AddLast(new Simbol(frekvenca, zgMeja, spMeja, vrednost));

                spMeja = zgMeja;
            }
            _tabelaFrekvenc = simboli.ToArray();
        }

        private void ReadTableUInt32(int stSimbolov, bool samoFreq) {
            LinkedList<Simbol> simboli = new LinkedList<Simbol>();

            ulong spMeja = 0;
            for (int i = 0; i < stSimbolov; i++) {
                byte vrednost;
                if (!samoFreq) {
                    vrednost = _vhod.ReadByte();
                }
                else {
                    vrednost = (byte) i;
                }

                ulong frekvenca = _vhod.ReadUInt32();

                _cF += frekvenca;

                ulong zgMeja = spMeja + frekvenca;
                simboli.AddLast(new Simbol(frekvenca, zgMeja, spMeja, vrednost));

                spMeja = zgMeja;
            }

            _tabelaFrekvenc = simboli.ToArray();
        }

        private void ReadTableUInt64(int stSimbolov, bool samoFreq) {
            LinkedList<Simbol> simboli = new LinkedList<Simbol>();

            ulong spMeja = 0;
            for (int i = 0; i < stSimbolov; i++) {
                byte vrednost;
                if (!samoFreq) {
                    vrednost = _vhod.ReadByte();
                }
                else {
                    vrednost = (byte) i;
                }

                ulong frekvenca = _vhod.ReadUIn64();

                _cF += frekvenca;

                ulong zgMeja = spMeja + frekvenca;
                simboli.AddLast(new Simbol(frekvenca, zgMeja, spMeja, vrednost));

                spMeja = zgMeja;
            }

            _tabelaFrekvenc = simboli.ToArray();
        }
    }

}