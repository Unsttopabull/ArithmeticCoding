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

            int stBitov;
            switch (readBits) {
                case 0:
                    stBitov = 8;
                    break;
                case 1:
                    stBitov = 16;
                    break;
                case 3:
                    stBitov = 64;
                    break;
                default:
                    stBitov = 32;
                    break;
            }

            _spodnjaMeja = 0;
            _zgornjaMeja = (1UL << (stBitov - 1)) - 1;

            _drugaCetrtina = (_zgornjaMeja + 1) >> 1;
            _prvaCetrtina = _drugaCetrtina >> 1;
            _tretjaCetrtina = _prvaCetrtina * 3;

            NapolniTabeloUrejeno();

            //_polje = _vhod.ReadBits((byte) (_stBitov - 1));
            // Napolnimo polje z n-1 bitov:
            _polje = 0;
            for (int i = 0; i < stBitov - 1; i++) {
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

            byte[] data = _list.ToArray();
            return data;
        }

        private Simbol NajdiSimbol(double vrednost) {
            //return _tabelaFrekvenc.FirstOrDefault(s => vrednost < s.ZgornjaMeja);

            Simbol sim = _tabelaFrekvenc.FirstOrDefault(s => s.ZgornjaMeja > vrednost && s.SpodnjaMeja <= vrednost);
            if (sim != null) {
                return sim;
            }

            //int binarySearch = Array.BinarySearch(_tabelaFrekvenc, vrednost);
            //if (binarySearch > 0) {
            //    return _tabelaFrekvenc[binarySearch];
            //}
            return null;
        }

        private void NapolniTabeloUrejeno() {
            int stSimbolov = _vhod.ReadByte();

            if (stSimbolov == 0) {
                stSimbolov = 256;
            }

            List<Simbol> simboli = new List<Simbol>();

            ulong spMeja = 0;
            for (int i = 0; i < stSimbolov; i++) {
                byte vrednost = _vhod.ReadByte();
                ulong frekvenca = _vhod.ReadUIn64();

                _cF += frekvenca;

                ulong zgMeja = spMeja + frekvenca;
                simboli.Add(new Simbol(frekvenca, zgMeja, spMeja, vrednost));

                spMeja = zgMeja;
            }

            _tabelaFrekvenc = simboli.ToArray();
        }
    }

}