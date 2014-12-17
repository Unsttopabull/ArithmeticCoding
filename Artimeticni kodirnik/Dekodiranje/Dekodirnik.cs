using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinIO;

namespace ArtimeticniKodirnik.Dekodiranje {

    public class Dekodirnik {
        private Simbol[] _tabelaFrekvenc;
        private string _outFile;
        private BinRead _vhod;
        private List<byte> _list;
        private ulong _cF;

        private ulong _prvaCetrtina;
        private ulong _drugaCetrtina;
        private ulong _tretjaCetrtina;

        private ulong _spodnjaMeja;
        private ulong _zgornjaMeja;

        //private Polje _polje;
        private ulong _polje;

        public Dekodirnik() {
        }

        public void Dekodiraj(string inFile, string outFile) {
            Dekodiraj(File.ReadAllBytes(inFile), outFile);
        }

        public void Dekodiraj(byte[] podatki, string outFile) {
            _vhod = new BinRead(podatki);
            _outFile = outFile;

            _list = new List<byte>();

            ulong readBits = _vhod.ReadBits(2);

            byte stBitov;
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
            _zgornjaMeja = ((ulong) Math.Pow(2, stBitov)) - 1;

            _drugaCetrtina = (_zgornjaMeja + 1) / 2;
            _prvaCetrtina = _drugaCetrtina / 2;
            _tretjaCetrtina = _prvaCetrtina * 3;

            NapolniTabeloUrejeno();

            _polje = _vhod.ReadBits((byte) (stBitov - 1));
            //_polje = new Polje((StBitov)readBits, polje);

            ulong iter = 1;
            do {

                ulong korak = (_zgornjaMeja - _spodnjaMeja + 1) / _cF;
                double vrednost = (_polje - _spodnjaMeja) / (double) korak;

                Simbol simbol = NajdiSimbol(vrednost);
                if (simbol == null) {
                    Console.Error.WriteLine("Ni bilo mogoče najti simbola");
                    return;
                }

                _list.Add(simbol.Vrednost);

                _zgornjaMeja = _spodnjaMeja + korak * simbol.SpodnjaMeja;
                _spodnjaMeja = _spodnjaMeja + korak * simbol.ZgornjaMeja - 1;

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
            while (iter >= _cF);

            File.WriteAllBytes(outFile, _list.ToArray());
        }

        private Simbol NajdiSimbol(double vrednost) {
            //return _tabelaFrekvenc.FirstOrDefault(s => vrednost < s.ZgornjaMeja);

            int binarySearch = Array.BinarySearch(_tabelaFrekvenc, vrednost, new RangeComparer());
            if (binarySearch > 0) {
                return _tabelaFrekvenc[binarySearch];
            }
            return null;
        }

        //private void NapolniTabelo() {
        //    Dictionary<byte, ulong> frekvence = new Dictionary<byte, ulong>();
        //    for (int i = 0; i < byte.MaxValue; i++) {
        //        ulong freq = (ulong) _vhod.ReadLong();
        //        if (freq > 0) {
        //            frekvence.Add((byte) i, freq);
        //        }
        //    }

        //    _cF = frekvence.Values.Aggregate((l, r) => l + r);

        //    ulong spMeja = 0;
        //    foreach (KeyValuePair<byte, ulong> par in frekvence) {
        //        ulong zgMeja = spMeja + par.Value;
        //        _tabelaFrekvenc.Add(par.Key, new Simbol(par.Value, par.Value / (double) _cF, zgMeja, spMeja, (char)par.Key));

        //        spMeja = zgMeja;
        //    }
        //}

        private void NapolniTabeloUrejeno() {
            int stSimbolov = _vhod.ReadByte();

            List<Simbol> simboli = new List<Simbol>();

            ulong spMeja = 0;
            for (int i = 0; i < stSimbolov; i++) {
                byte vrednost = _vhod.ReadByte();
                ulong frekvenca = _vhod.ReadULong();

                _cF += frekvenca;

                ulong zgMeja = spMeja + frekvenca;
                simboli.Add(new Simbol(frekvenca, 0, zgMeja, spMeja, vrednost));

                spMeja = zgMeja;
            }

            foreach (Simbol simbol in simboli) {
                simbol.Verjetnost = simbol.Frekvenca / (double) _cF;
            }

            _tabelaFrekvenc = simboli.ToArray();
        }
    }

}