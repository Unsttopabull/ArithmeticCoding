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

        private readonly ulong _prvaCetrtina;
        private readonly ulong _drugaCetrtina;
        private readonly ulong _tretjaCetrtina;

        private ulong _spodnjaMeja;
        private ulong _zgornjaMeja;

        public Kodirnik(StBitov stBitov, bool enableEvents = false) {
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

            //št. simbolov
            byte length = (byte) tabela.Length;
            bw.WriteByte(length);
            foreach (Simbol simbol in tabela) {
                //kateri simbol predstavlja
                bw.WriteByte(simbol.Vrednost);
                //frekvenca
                bw.WriteUInt64(simbol.Frekvenca);
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
            for (int i = 0; i < 256; i++) {
                ulong frekvenca = frekvence[i];
                if (frekvenca == 0) {
                    continue;
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