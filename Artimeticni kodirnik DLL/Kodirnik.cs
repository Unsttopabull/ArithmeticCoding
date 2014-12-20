using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinIO;

namespace ArtimeticniKodirnik.DLL {

    public class Kodirnik {
        private MemoryStream _ms;
        private BitWriter _izhod;
        private readonly StBitov _stBitov;
        private readonly Simbol[] _tabelaFrekvenc;
        private int _e3Counter;
        private ulong _cF;
        private ulong _maxFreq;

        private readonly ulong _prvaCetrtina;
        private readonly ulong _drugaCetrtina;
        private readonly ulong _tretjaCetrtina;

        private ulong _spodnjaMeja;
        private ulong _zgornjaMeja;

        public Kodirnik(StBitov stBitov) {
            _e3Counter = 0;
            _tabelaFrekvenc = new Simbol[256];

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
            _zgornjaMeja = (1UL << (stBitovNum - 1)) - 1;

            _drugaCetrtina = (_zgornjaMeja + 1) >> 1;
            _prvaCetrtina = _drugaCetrtina >> 1;
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

            _izhod = new BitWriter((int) ms.Length);

            switch (_stBitov) {
                case StBitov.Bit8:
                    _izhod.WriteBits(0, 2); //00
                    break;
                case StBitov.Bit16:
                    _izhod.WriteBits(1, 2); //01
                    break;
                case StBitov.Bit32:
                    _izhod.WriteBits(2, 2); //10
                    break;
                case StBitov.Bit64:
                    _izhod.WriteBits(3, 2); //11
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //if (!IzracunajTabelo()) {
            if (!IzracunajTabeloUrejeno()) {
                return null;
            }

            ZapisiTabelo(_izhod);

            int brano = _ms.ReadByte();
            do {
                Simbol simbol = _tabelaFrekvenc[brano];

                ulong korak = (_zgornjaMeja - _spodnjaMeja + 1) / _cF;
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

                        _izhod.WriteBits(0, 1);

                        if (_e3Counter > 0) {
                            ulong e3Bits = (ulong) ((1 << _e3Counter) - 1);
                            _izhod.WriteBits(e3Bits, (byte) _e3Counter);
                            _e3Counter = 0;
                        }
                    }
                    else if (_spodnjaMeja >= _drugaCetrtina) {
                        //E2
                        e2 = true;
                        _spodnjaMeja = (_spodnjaMeja - _drugaCetrtina) << 1;
                        _zgornjaMeja = ((_zgornjaMeja - _drugaCetrtina) << 1) + 1;

                        _izhod.WriteBits(1, 1);

                        if (_e3Counter > 0) {
                            _izhod.WriteBits(0, (byte) _e3Counter);
                            _e3Counter = 0;
                        }
                    }
                }
                while (e1 || e2); //dokler ne e1 in e2 nista izpolnjena

                while (_spodnjaMeja >= _prvaCetrtina && _zgornjaMeja < _tretjaCetrtina) {
                    //E3
                    _spodnjaMeja = (_spodnjaMeja - _prvaCetrtina) << 1;
                    _zgornjaMeja = ((_zgornjaMeja - _prvaCetrtina) << 1) + 1;
                    _e3Counter++;
                }

                brano = _ms.ReadByte();
            }
            while (brano != -1);


            if (_spodnjaMeja < _prvaCetrtina) {
                _izhod.WriteBits(1, 2); // "01"

                ulong e3Bits = (ulong) ((1 << _e3Counter) - 1); // 2^(e3 + 1) - 1
                _izhod.WriteBits(e3Bits, (byte) _e3Counter); //e3 * "1"
            }
            else {
                _izhod.WriteBits(2, 2); // "10"
                _izhod.WriteBits(0, (byte) _e3Counter); //e3 * "0"
            }

            byte[] output = _izhod.GetData();
            return output;
        }

        private void ZapisiTabelo(BitWriter bw) {
            Simbol[] tabela = _tabelaFrekvenc.Where(s => s != null).ToArray();

            ulong nacin;
            int shift, stBitov;
            int stSimbolov = tabela.Length;
            bool samoFreq = false;

            if (_maxFreq <= byte.MaxValue) {
                stBitov = 8;
                shift = 3;
                nacin = 0;
            }
            else if (_maxFreq <= ushort.MaxValue) {
                stBitov = 16;
                shift = 4;
                nacin = 1;
            }
            else if (_maxFreq <= uint.MaxValue) {
                stBitov = 32;
                shift = 5;
                nacin = 2;
            }
            else {
                stBitov = 64;
                shift = 6;
                nacin = 3;
            }

            if (stSimbolov == 256) {
                samoFreq = true;
                tabela = _tabelaFrekvenc;
                bw.WriteBits(nacin, 2); //00, 01, 10, 11
                bw.WriteBits(1, 1);
            }
            else {
                int samoFreqBits = stSimbolov << shift; //stSimbolov * 8/16/32/64
                int valInFreqBits = (stSimbolov << 3) + samoFreqBits; //stSimbolov * 8 + samoFreqBits

                bw.WriteBits(nacin, 2); //00, 01, 10, 11
                if (samoFreqBits >= valInFreqBits) {
                    bw.WriteBits(0, 1);
                    bw.WriteByte((byte) stSimbolov);
                }
                else {
                    samoFreq = true;
                    tabela = _tabelaFrekvenc;
                    bw.WriteBits(1, 1);
                }
            }

            foreach (Simbol simbol in tabela) {
                if (!samoFreq) {
                    if (simbol == null) {
                        bw.WriteByte(0);
                    }
                    else {
                        //kateri simbol predstavlja
                        bw.WriteByte(simbol.Vrednost);
                    }
                }

                ulong frekvenca = simbol == null ? 0 : simbol.Frekvenca;

                //frekvenca
                switch (stBitov) {
                    case 8:
                        bw.WriteByte((byte) frekvenca);
                        break;
                    case 16:
                        bw.WriteUInt16((ushort) frekvenca);
                        break;
                    case 32:
                        bw.WriteUInt32((uint) frekvenca);
                        break;
                    case 64:
                        bw.WriteUInt64(frekvenca);
                        break;
                }
            }
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
            _maxFreq = 0;
            for (int i = 0; i < 256; i++) {
                ulong frekvenca = frekvence[i];
                if (frekvenca == 0) {
                    continue;
                }

                if (frekvenca > _maxFreq) {
                    _maxFreq = frekvenca;
                }

                ulong zgMeja = spMeja + frekvenca;
                _tabelaFrekvenc[i] = new Simbol(frekvenca, zgMeja, spMeja, (byte) i);

                spMeja = zgMeja;
            }

            _ms.Seek(0, SeekOrigin.Begin);
            return true;
        }
    }

}