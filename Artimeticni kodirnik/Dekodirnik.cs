using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinIO;

namespace ArtimeticniKodirnik {

    public class Dekodirnik {
        private Dictionary<byte, Simbol> _tabelaFrekvenc;
        private readonly string _outFile;
        private readonly BinRead _vhod;
        private List<byte> _list;
        private ulong _cF;

        private ulong _prvaCetrtina;
        private ulong _drugaCetrtina;
        private ulong _tretjaCetrtina;

        private ulong _spodnjaMeja;
        private ulong _zgornjaMeja;

        private ulong _polje;

        public Dekodirnik(byte[] podatki, string outFile) {
            _outFile = outFile;
            _vhod = new BinRead(podatki);
        }

        public Dekodirnik(string potDoDatoteke, string outFile) {
            _vhod = new BinRead(File.ReadAllBytes(potDoDatoteke));
        }

        public void Dekodiraj() {
            _list = new List<byte>();
            _tabelaFrekvenc = new Dictionary<byte, Simbol>();

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

            NapolniTabelo();

            _polje = _vhod.ReadBits((byte) (stBitov - 1));

            ulong iter = 1;
            do {

                ulong korak = (_zgornjaMeja - _spodnjaMeja + 1) / _cF;
                double vrednost = (_polje - _spodnjaMeja) / (double) korak;

                KeyValuePair<byte, Simbol> simbol = _tabelaFrekvenc.FirstOrDefault(s => vrednost < s.Value.ZgornjaMeja);

                _list.Add(simbol.Key);

                _zgornjaMeja = _spodnjaMeja + korak * simbol.Value.SpodnjaMeja;
                _spodnjaMeja = _spodnjaMeja + korak * simbol.Value.ZgornjaMeja - 1;

                bool e1, e2;
                do {
                    e1 = false;
                    e2 = false;

                    if (_zgornjaMeja < _drugaCetrtina) {
                        //E1
                        e1 = true;

                        _spodnjaMeja *= 2;
                        _zgornjaMeja = (_zgornjaMeja * 2) + 1;

                        _polje = 2 * _polje + _vhod.ReadBits(1);
                    }
                    else if (_spodnjaMeja >= _drugaCetrtina) {
                        //E2
                        e2 = true;

                        _spodnjaMeja = 2 * (_spodnjaMeja - _drugaCetrtina);
                        _zgornjaMeja = 2 * (_zgornjaMeja - _drugaCetrtina) + 1;
                        _polje = 2 * (_polje - _drugaCetrtina) + _vhod.ReadBits(1);
                    }
                }
                while (e1 || e2); //dokler ne e1 in e2 nista izpolnjena

                while (_spodnjaMeja >= _prvaCetrtina && _zgornjaMeja < _tretjaCetrtina) {
                    //E3
                    _spodnjaMeja = 2 * (_spodnjaMeja - _prvaCetrtina);
                    _zgornjaMeja = 2 * (_zgornjaMeja - _prvaCetrtina) + 1;
                    
                    _polje = 2 * (_polje - _prvaCetrtina) + _vhod.ReadBits(1);
                }

                iter++;
            }
            while (iter >= _cF);
        }

        private void NapolniTabelo() {
            Dictionary<byte, ulong> frekvence = new Dictionary<byte, ulong>();
            for (int i = 0; i < byte.MaxValue; i++) {
                ulong freq = (ulong) _vhod.ReadLong();
                if (freq > 0) {
                    frekvence.Add((byte) i, freq);
                }
            }

            _cF = frekvence.Values.Aggregate((l, r) => l + r);

            ulong spMeja = 0;
            foreach (KeyValuePair<byte, ulong> par in frekvence) {
                ulong zgMeja = spMeja + par.Value;
                _tabelaFrekvenc.Add(par.Key, new Simbol(par.Value, par.Value / (double) _cF, zgMeja, spMeja));

                spMeja = zgMeja;
            }
        }
    }

}