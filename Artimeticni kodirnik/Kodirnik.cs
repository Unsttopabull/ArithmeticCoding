using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BinIO;

namespace ArtimeticniKodirnik {

    public class Kodirnik {
        private readonly MemoryStream _ms;
        private BinWrite _izhod;
        private readonly List<byte> _list = new List<byte>();
        private readonly Dictionary<byte, Simbol> _tabelaFrekvenc;
        private int _e3Counter;
        private ulong _cF;

        private readonly ulong _prvaCetrtina;
        private readonly ulong _drugaCetrtina;
        private readonly ulong _tretjaCetrtina;

        private ulong _spodnjaMeja;
        private ulong _zgornjaMeja;

        public Kodirnik(MemoryStream ms, StBitov stBitov) {
            _ms = ms;
            _e3Counter = 0;
            _tabelaFrekvenc = new Dictionary<byte, Simbol>();

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
            
            _spodnjaMeja = 0;
            _zgornjaMeja = ((ulong) Math.Pow(2, stBitovNum - 1)) - 1;

            _drugaCetrtina = (_zgornjaMeja + 1) / 2;
            _prvaCetrtina = _drugaCetrtina / 2;
            _tretjaCetrtina = _prvaCetrtina * 3;
        }

        public Kodirnik(byte[] podatki, StBitov stBitov) : this(new MemoryStream(podatki), stBitov) {
        }

        public byte[] Kodiraj() {
            Debug.Listeners.Add(new TextWriterTraceListener(new FileStream("debug.csv", FileMode.Create)));
            Debug.AutoFlush = true;

            if (!IzracunajTabelo()) {
                return null;
            }

            _izhod = new BinWrite();

            Debug.WriteLine("Iter;Simbol;Korak;S;Z;nS;nZ;Metoda;OUT;E3C;E3 OUT");

            int iter = 1;
            int brano = _ms.ReadByte();
            do {
                Simbol simbol = _tabelaFrekvenc[(byte) brano];

                ulong korak = (_zgornjaMeja - _spodnjaMeja + 1) / _cF;
                _zgornjaMeja = _spodnjaMeja + korak * simbol.ZgornjaMeja - 1;
                _spodnjaMeja = _spodnjaMeja + korak * simbol.SpodnjaMeja;

                Debug.WriteLine("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9}", iter, (char)brano, korak, _spodnjaMeja, _zgornjaMeja, "", "", "", "", "");

                bool e1, e2;
                do {
                    e1 = false;
                    e2 = false;

                    if (_zgornjaMeja < _drugaCetrtina) {
                        //E1
                        e1 = true;
                        _spodnjaMeja *= 2;
                        _zgornjaMeja = (_zgornjaMeja * 2) + 1;
                        _izhod.WriteBits(0, 1);

                        _list.Add(0);
                        var e3Out = Enumerable.Repeat("1", _e3Counter).ToList();
                        Debug.WriteLine("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10}", iter, (char)brano, "", "", "", _spodnjaMeja, _zgornjaMeja, "E1", "1", _e3Counter, e3Out.Any() ? e3Out.Aggregate("\"\"", (l,r) => l + r)+"\"\"" : "");

                        if (_e3Counter > 0) {
                            for (int i = 0; i < _e3Counter; i++) {
                                _izhod.WriteBits(1, 1);
                                _list.Add(1);
                            }
                            _e3Counter = 0;
                        }
                    }
                    else if (_spodnjaMeja >= _drugaCetrtina) {
                        //E2
                        e2 = true;
                        _spodnjaMeja = 2 * (_spodnjaMeja - _drugaCetrtina);
                        _zgornjaMeja = 2 * (_zgornjaMeja - _drugaCetrtina) + 1;
                        _izhod.WriteBits(1, 1);

                        _list.Add(1);
                        var e3Out = Enumerable.Repeat("0", _e3Counter).ToList();
                        Debug.WriteLine("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10}", iter, (char)brano, "", "", "", _spodnjaMeja, _zgornjaMeja, "E2", "0", _e3Counter, e3Out.Any() ? e3Out.Aggregate("\"\"", (l,r) => l + r)+"\"\"" : "");

                        if (_e3Counter > 0) {
                            for (int i = 0; i < _e3Counter; i++) {
                                _izhod.WriteBits(0, 1);
                                _list.Add(0);
                            }
                            _e3Counter = 0;
                        }
                    }
                }
                while (e1 || e2); //dokler ne e1 in e2 nista izpolnjena

                while (_spodnjaMeja >= _prvaCetrtina && _zgornjaMeja < _tretjaCetrtina) {
                    //E3
                    _spodnjaMeja = 2 * (_spodnjaMeja - _prvaCetrtina);
                    _zgornjaMeja = 2 * (_zgornjaMeja - _prvaCetrtina) + 1;
                    _e3Counter++;

                    Debug.WriteLine("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10}", iter, (char)brano, "", "", "", _spodnjaMeja, _zgornjaMeja, "E3", "", _e3Counter, "");
                }

                Debug.WriteLine(";;;;;;;;;;");
                Debug.WriteLine(";;;;;;;;;;");

                brano = _ms.ReadByte();
                iter++;
            }
            while (brano != -1);


            if (_spodnjaMeja < _prvaCetrtina) {
                _izhod.WriteBits(1, 2);

                _list.Add(0);
                _list.Add(1);

                var e3Out = Enumerable.Repeat("1", _e3Counter).ToList();
                for (int i = 0; i < _e3Counter; i++) {
                    _izhod.WriteBits(1, 1);
                    _list.Add(1);
                }

                Debug.WriteLine("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10}", iter, "EOF", "", "", "", "", "", "", "11", _e3Counter, e3Out.Any() ? e3Out.Aggregate("\"\"", (l,r) => l + r)+"\"\"" : "");
            }
            else {
               _izhod.WriteBits(0x2, 2);

                _list.Add(1);
                _list.Add(0);

                var e3Out = Enumerable.Repeat("0", _e3Counter).ToList();
                for (int i = 0; i < _e3Counter; i++) {
                    _izhod.WriteBits(0, 1);
                    _list.Add(0);
                }
                Debug.WriteLine("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10}", iter, "EOF", "", "", "", "", "", "", "10", _e3Counter, e3Out.Any() ? e3Out.Aggregate("\"\"", (l,r) => l + r)+"\"\"" : "");
            }

            _izhod.Flush();

            Debug.Flush();

            DebugKodiraj();

            return _ms.ToArray();
        }

        private void DebugKodiraj() {
            List<string> strs = new List<string>();

            int c = 0;
            string s = null;
            foreach (byte b in _list) {
                if (c >= 4) {
                    strs.Add(s);
                    c = 0;
                    s = null;
                }

                s += b;
                c++;
            }
            if (!string.IsNullOrEmpty(s)) {
                strs.Add(s);
            }

            Debug.WriteLine(";;;;;;;;;;");
            Debug.WriteLine(string.Format("IZHOD;{0}", string.Join(";", strs)));
        }

        public void ZapisiDatoteko(string imeDatoteke) {
            File.WriteAllBytes(imeDatoteke, _ms.ToArray());
        }

        private bool IzracunajTabelo() {
            _ms.Seek(0, SeekOrigin.Begin);

            Dictionary<byte, ulong> frekvenca = new Dictionary<byte, ulong>();

            int brano = _ms.ReadByte();
            if (brano == -1) {
                _ms.Seek(0, SeekOrigin.Begin);
                return false;
            }

            do {
                byte bajt = (byte) brano;
                if (frekvenca.ContainsKey(bajt)) {
                    frekvenca[bajt]++;
                }
                else {
                    frekvenca.Add(bajt, 1);
                }

                brano = _ms.ReadByte();
            }
            while (brano >= 0);

            _cF = frekvenca.Values.Aggregate((l, r) => l + r);

            ulong spMeja = 0;
            foreach (KeyValuePair<byte, ulong> par in frekvenca) {
                ulong zgMeja = spMeja + par.Value;
                _tabelaFrekvenc.Add(par.Key, new Simbol(par.Value, par.Value / (double) _cF, zgMeja, spMeja));

                spMeja = zgMeja;
            }

            _ms.Seek(0, SeekOrigin.Begin);
            return true;
        }
    }

}