using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using BinIO;

namespace ArtimeticniKodirnik.Kodiranje {

    public class Kodirnik {
        private MemoryStream _ms;
        private BinWrite _izhod;
        private readonly StBitov _stBitov;
        private readonly List<bool> _list;
        private readonly List<string> _operacije;
        private readonly Simbol[] _tabelaFrekvenc;
        private int _e3Counter;
        private ulong _cF;

        private readonly ulong _prvaCetrtina;
        private readonly ulong _drugaCetrtina;
        private readonly ulong _tretjaCetrtina;

        private ulong _spodnjaMeja;
        private ulong _zgornjaMeja;

        public delegate void SimbolZakodiranHandler(byte simbol, ulong spMeja, ulong zgMeja, ulong korak, ulong novaSpMeja, ulong novaZgMeja, string operacije, int e3Count);
        public event SimbolZakodiranHandler SimbolZakodiran;

        public delegate void TabelaGeneriranaHandler(IList<Simbol> tabela);
        public event TabelaGeneriranaHandler TabelaGenerirana;

        public Kodirnik(StBitov stBitov) {
            _e3Counter = 0;
            _tabelaFrekvenc = new Simbol[256];
            _list = new List<bool>();
            _operacije = new List<string>();

            _stBitov = stBitov;
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

        public byte[] Kodiraj(byte[] podatki) {
            return Kodiraj(new MemoryStream(podatki));
        }

        public byte[] Kodiraj(string inFile) {
            return Kodiraj(File.ReadAllBytes(inFile));
        }

        public byte[] Kodiraj(MemoryStream ms) {
            _ms = ms;

            //if (!IzracunajTabelo()) {
            if (!IzracunajTabeloUrejeno()) {
                return null;
            }

            ObvestiPoslusalceOTabeli();

            _izhod = new BinWrite();

            int brano = _ms.ReadByte();
            do {
                Simbol simbol = _tabelaFrekvenc[brano];

                ulong zgMeja = _zgornjaMeja;
                ulong spMeja = _spodnjaMeja;

                ulong korak = (_zgornjaMeja - _spodnjaMeja + 1) / _cF;
                _zgornjaMeja = _spodnjaMeja + korak * simbol.ZgornjaMeja - 1;
                _spodnjaMeja = _spodnjaMeja + korak * simbol.SpodnjaMeja;

                ulong nZgMeja = _zgornjaMeja;
                ulong nSpMeja = _spodnjaMeja;

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

                        IzpisiE1();

                        if (_e3Counter > 0) {
                            _izhod.WriteBits(1, _e3Counter);
                            _e3Counter = 0;
                        }
                    }
                    else if (_spodnjaMeja >= _drugaCetrtina) {
                        //E2
                        e2 = true;
                        _spodnjaMeja = 2 * (_spodnjaMeja - _drugaCetrtina);
                        _zgornjaMeja = 2 * (_zgornjaMeja - _drugaCetrtina) + 1;

                        _izhod.WriteBits(1, 1);

                        IzpisiE2();

                        if (_e3Counter > 0) {
                            _izhod.WriteBits(0, _e3Counter);
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

                    _operacije.Add(string.Format("E3({0}, {1})", _spodnjaMeja, _zgornjaMeja));
                }

                PosljiPoslusalcem(brano, spMeja, zgMeja, korak, nSpMeja, nZgMeja);

                brano = _ms.ReadByte();
            }
            while (brano != -1);


            if (_spodnjaMeja < _prvaCetrtina) {
                _izhod.WriteBits(1, 2);
                _izhod.WriteBits(1, _e3Counter);

                PosljiPoslusalcemOstanek("01" + string.Join("", Enumerable.Repeat(1, _e3Counter)));

            }
            else {
                _izhod.WriteBits(2, 2);
                _izhod.WriteBits(0, _e3Counter);

                PosljiPoslusalcemOstanek("10" + string.Join("", Enumerable.Repeat(0, _e3Counter)));
            }

            _izhod.Flush();

            string bin = _list.Aggregate("", (str, bit) => str + (bit ? "1" : "0"));

            return _izhod.GetOutput();
        }

        private void IzpisiE2() {
            string e3Bits = string.Join("", Enumerable.Repeat("0", _e3Counter));
            if (!string.IsNullOrEmpty(e3Bits)) {
                e3Bits = " " + e3Bits;
            }

            _operacije.Add(string.Format("{0}({1}, {2}) Out => {3}{4}", "E2", _spodnjaMeja, _zgornjaMeja, "1", e3Bits));
        }

        private void IzpisiE1() {
            string e3Bits = string.Join("", Enumerable.Repeat("1", _e3Counter));
            if (!string.IsNullOrEmpty(e3Bits)) {
                e3Bits = " " + e3Bits;
            }

            _operacije.Add(string.Format("{0}({1}, {2}) Out => {3}{4}", "E1", _spodnjaMeja, _zgornjaMeja, "0", e3Bits));
        }

        private void PosljiPoslusalcemOstanek(string ostanek) {
            if (SimbolZakodiran == null) {
                return;
            }

            SimbolZakodiran(0, 0, 0, 0, 0, 0, "Ostanek -> " + ostanek, -1);
        }

        private void ObvestiPoslusalceOTabeli() {
            if (TabelaGenerirana == null) {
                return;
            }

            Simbol[] tabela = _tabelaFrekvenc.Where(s => s != null).ToArray();
            TabelaGenerirana(tabela);
        }

        private void PosljiPoslusalcem(int brano, ulong spMeja, ulong zgMeja, ulong korak, ulong nSpMeja, ulong nZgMeja) {
            if (SimbolZakodiran == null) {
                return;
            }

            SimbolZakodiran((byte) brano, spMeja, zgMeja, korak, nSpMeja, nZgMeja, string.Join("; ", _operacije), _e3Counter);
            _operacije.Clear();
        }

        public void ZapisiDatoteko(string imeDatoteke) {
            BinWriteFile bw = new BinWriteFile(imeDatoteke);
            switch (_stBitov) {
                case StBitov.Bit8:
                    bw.WriteBits(0, 2); //00
                    break;
                case StBitov.Bit16:
                    bw.WriteBits(1, 2); //01
                    break;
                case StBitov.Bit32:
                    bw.WriteBits(2, 2); //10
                    break;
                case StBitov.Bit64:
                    bw.WriteBits(3, 2); //11
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            ZapisiTabelo(bw);

            bw.WriteBytes(_izhod.GetOutput());

            bw.Close();
            //File.WriteAllBytes(imeDatoteke, bw.GetOutput());
        }

        private void ZapisiTabelo(BinWriteFile bw) {
            Simbol[] tabela = _tabelaFrekvenc.Where(s => s != null).ToArray();

            //št. simbolov
            bw.WriteByte((byte) tabela.Length);
            for (int i = 0; i < tabela.Length; i++) {
                //kateri simbol predstavlja
                bw.WriteByte(tabela[i].Vrednost);
                //frekvenca
                bw.WriteULong(tabela[i].Frekvenca);
            }
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
                _cF++;
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

            ulong spMeja = 0;
            foreach (KeyValuePair<byte, ulong> par in frekvenca) {
                ulong zgMeja = spMeja + par.Value;
                _tabelaFrekvenc[par.Key] = new Simbol(par.Value, par.Value / (double) _cF, zgMeja, spMeja, par.Key);

                spMeja = zgMeja;
            }

            _ms.Seek(0, SeekOrigin.Begin);
            return true;
        }

        private bool IzracunajTabeloUrejeno() {
            _ms.Seek(0, SeekOrigin.Begin);

            int brano = _ms.ReadByte();
            if (brano == -1) {
                _ms.Seek(0, SeekOrigin.Begin);
                return false;
            }

            ulong[] frekvence = new ulong[256];
            do {
                byte bajt = (byte) brano;
                frekvence[bajt]++;
                _cF++;

                brano = _ms.ReadByte();
            }
            while (brano >= 0);

            ulong spMeja = 0;
            for (int i = 0; i < 256; i++) {
                ulong frekvenca = frekvence[i];
                if (frekvenca == 0) {
                    continue;
                }

                ulong zgMeja = spMeja + frekvenca;
                _tabelaFrekvenc[i] = new Simbol(frekvenca, frekvenca / (double) _cF, zgMeja, spMeja, (byte) i);

                spMeja = zgMeja;
            }

            _ms.Seek(0, SeekOrigin.Begin);
            return true;
        }
    }

}