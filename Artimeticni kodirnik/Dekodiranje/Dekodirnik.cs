using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BinIO;

namespace ArtimeticniKodirnik.Dekodiranje {

    public class Dekodirnik {
        private const ulong MASKA_8 = 0x7F;
        private const ulong MASKA_16 = 0x7FFF; 
        private const ulong MASKA_32 = 0x7FFFFFFF; 
        private const ulong MASKA_64 = 0x7FFFFFFFFFFFFFFF;

        private byte _stBitov;
        private ulong _maska;

        private Simbol[] _tabelaFrekvenc;
        private LinkedList<string> _operacije;
        private string _outFile;
        private BitReader _vhod;
        private LinkedList<byte> _list;
        private ulong _cF;

        private ulong _prvaCetrtina;
        private ulong _drugaCetrtina;
        private ulong _tretjaCetrtina;

        private ulong _spodnjaMeja;
        private ulong _zgornjaMeja;

        //private Polje _polje;
        private ulong _polje;

        public delegate void SimbolDekodiranHandler(string v, ulong spMeja, ulong zgMeja, ulong korak, ulong vrednostSymb, byte simbol, ulong novaSpMeja, ulong novaZgMeja, params string[] operacije);
        public event SimbolDekodiranHandler SimbolDekodiran;

        public delegate void TabelaGeneriranaHandler(IList<Simbol> tabela);
        public event TabelaGeneriranaHandler TabelaGenerirana;

        public Dekodirnik(bool enableEvents = false) {
            EventsEnabled = enableEvents;
        }

        public bool EventsEnabled { get; set; }

        public byte[] Dekodiraj(string inFile, string outFile) {
            return Dekodiraj(new BitReader(inFile), outFile);
        }

        public byte[] Dekodiraj(byte[] podatki, string outFile) {
            return Dekodiraj(new BitReader(podatki), outFile);
        }

        private byte[] Dekodiraj(BitReader read, string outFile) {
            _cF = 0;
            _vhod = read;
			GC.Collect();
			
            _operacije = new LinkedList<string>();
            
            _outFile = outFile;

            _list = new LinkedList<byte>();

            ulong readBits = _vhod.ReadBits(2);

            switch (readBits) {
                case 0:
                    _stBitov = 8;
                    _maska = MASKA_8;
                    break;
                case 1:
                    _stBitov = 16;
                    _maska = MASKA_16;
                    break;
                case 3:
                    _stBitov = 64;
                    _maska = MASKA_64;
                    break;
                default:
                    _stBitov = 32;
                    _maska = MASKA_32;
                    break;
            }

            _spodnjaMeja = 0;
            _zgornjaMeja = ((ulong) Math.Pow(2, _stBitov - 1)) - 1;

            _drugaCetrtina = (_zgornjaMeja + 1) / 2;
            _prvaCetrtina = _drugaCetrtina / 2;
            _tretjaCetrtina = _prvaCetrtina * 3;

            NapolniTabeloUrejeno();

            if (EventsEnabled) {
                PosljiTabeloPosljusalcem();
            }

            //_polje = _vhod.ReadBits((byte) (_stBitov - 1));
            // Napolnimo polje z n-1 bitov:
            _polje = 0;
            for (int i = 0; i < _stBitov - 1; i++) {
                _polje = (_polje << 1) + _vhod.ReadBits(1);
            }

            //string poljeBin = BinUtils.ULong2Bin(_polje, _stBitov - 1);
            //_polje = new Polje((StBitov)readBits, polje);

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

                ulong spMeja = _spodnjaMeja;
                ulong zgMeja = _zgornjaMeja;

                _zgornjaMeja = _spodnjaMeja + korak * simbol.ZgornjaMeja - 1;
                _spodnjaMeja = _spodnjaMeja + korak * simbol.SpodnjaMeja;

                ulong nSpMeja = _spodnjaMeja;
                ulong nZgMeja = _zgornjaMeja;

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

                        if (EventsEnabled) {
                            _operacije.AddLast(string.Format("E1({0}, {1}) V={2}", _spodnjaMeja, _zgornjaMeja, _polje & _maska));
                        }
                    }
                    else if (_spodnjaMeja >= _drugaCetrtina) {
                        //E2
                        e2 = true;

                        _spodnjaMeja = (_spodnjaMeja - _drugaCetrtina) << 1;
                        _zgornjaMeja = ((_zgornjaMeja - _drugaCetrtina) << 1) + 1;
                        _polje = ((_polje - _drugaCetrtina) << 1) + _vhod.ReadBits(1);

                        if (EventsEnabled) {
                            _operacije.AddLast(string.Format("E2({0}, {1}) V={2}", _spodnjaMeja, _zgornjaMeja, _polje & _maska));
                        }
                    }
                }
                while (e1 || e2); //dokler ne e1 in e2 nista izpolnjena

                while (_spodnjaMeja >= _prvaCetrtina && _zgornjaMeja < _tretjaCetrtina) {
                    //E3
                    _spodnjaMeja = (_spodnjaMeja - _prvaCetrtina) << 1;
                    _zgornjaMeja = ((_zgornjaMeja - _prvaCetrtina) << 1) + 1;
                    
                    _polje = ((_polje - _prvaCetrtina) << 1) + _vhod.ReadBits(1);

                    if (EventsEnabled) {
                        _operacije.AddLast(string.Format("E3({0}, {1}) V={2}", _spodnjaMeja, _zgornjaMeja, _polje & _maska));
                    }
                }

                if (EventsEnabled) {
                    PosljiDekodiranSimbolPosljusalcem(spMeja, zgMeja, korak, nSpMeja, nZgMeja, vrednost, simbol.Vrednost);
                }
                iter++;
            }
            while (iter <= _cF);

            byte[] data = _list.ToArray();
            File.WriteAllBytes(outFile, data);
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

            if (stSimbolov == 0) {
                stSimbolov = 256;
            }

            List<Simbol> simboli = new List<Simbol>();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("S;F;SpX;ZgX;");

            ulong spMeja = 0;
            for (int i = 0; i < stSimbolov; i++) {
                byte vrednost = _vhod.ReadByte();
                ulong frekvenca = _vhod.ReadUIn64();

                _cF += frekvenca;

                ulong zgMeja = spMeja + frekvenca;
                simboli.Add(new Simbol(frekvenca, zgMeja, spMeja, vrednost));
                sb.AppendLine(string.Format("{0};{1};{2};{3};", vrednost, frekvenca, spMeja, zgMeja));

                spMeja = zgMeja;
            }
            sb.AppendLine(string.Format("CF;{0};;;", _cF));
            File.WriteAllText("tabela_dec.csv", sb.ToString());

            _tabelaFrekvenc = simboli.ToArray();
        }

        private void PosljiDekodiranSimbolPosljusalcem(ulong spMeja, ulong zgMeja, ulong korak, ulong nSpMeja, ulong nZgMeja, ulong vrednost, byte simbol) {
            if (SimbolDekodiran == null) {
                return;
            }

            string polje = BinUtils.ULong2Bin(_polje, _stBitov - 1);
            SimbolDekodiran(polje, spMeja, zgMeja, korak, vrednost, simbol, nSpMeja, nZgMeja, _operacije.ToArray());
            _operacije.Clear();
        }

        private void PosljiTabeloPosljusalcem() {
            if (TabelaGenerirana == null) {
                return;
            }

            TabelaGenerirana(_tabelaFrekvenc);
        }
    }

}