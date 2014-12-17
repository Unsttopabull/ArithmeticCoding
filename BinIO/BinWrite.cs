using System;
using System.Collections.Generic;

namespace BinIO {

    public class BinWrite {
        private readonly List<byte> _output;
        private List<byte> _buffer;
        private readonly int _buffersize = 65536;
        private int _bytepos;
        private byte _bitpos;

        public BinWrite(int bufsize = 65536) {
            _buffer = new List<byte>(new byte[bufsize]);
            _buffersize = bufsize;
            _output = new List<byte>();
        }

        public int BytePos { get { return _bytepos; } }
        public byte BitPos { get { return _bitpos; } }

        public void Flush() {
            // Zapišemo napolnjene byte-e iz bufferja:
            _output.AddRange(_buffer.GetRange(0, _bytepos));
            // Če smo kateri byte le delno napolnili, ga tudi zapišemo:
            if (_bitpos > 0) {
                byte mask = (byte) ((1 << _bitpos) - 1);
                _output.Add((byte) (_buffer[_bytepos] & mask));
            }

            // Ponastavimo indexe:
            _bytepos = 0;
            _bitpos = 0;

            // Za vsak slučaj če nam je ostalo kaj v prejšni iteraciji branja v buffer.
            _buffer = new List<byte>(new byte[_buffersize]);
        }

        public byte[] GetBuffer() {
            // Zapišemo napolnjene byte-e iz bufferja:
            _output.AddRange(_buffer.GetRange(0, _bytepos));
            // Če smo kateri byte le delno napolnili, ga tudi zapišemo:
            if (_bitpos > 0) {
                byte mask = (byte) ((1 << _bitpos) - 1);
                _output.Add((byte) (_buffer[_bytepos] & mask));
            }

            return _output.ToArray();
        }

        public byte[] GetOutput() {
            Flush();
            return _output.ToArray();
        }

        // Funkcija ki vrne trenutno pozicijo:
        public void GetPosition(out int bytePos, out byte bitPos) {
            bytePos = _bytepos + _output.Count;
            bitPos = _bitpos;
        }

        // Funkcija ki piše na podano pozicijo bufferja: 
        //   ERROR CE PISE NA KONCU OUTPUT BUFFERJA, kar za primer števila znakov ni problem pri default velikosti bufferja, 
        //   ERROR CE NA POZICIJI NISO SAME NULE, kar ni problem če prvotno zapišemo nulto vrednost.
        //   Hitra rešitev ker imamo še druge obveznosti...
        public void WriteAt(ulong data, byte numbits, int bytePos, byte bitPos) {
            bool fieldSwitch = bytePos < _output.Count;
            List<byte> tBuffer = null;

            // Shranimo trenutne vrednosti:
            int tBytepos = _bytepos;
            byte tBitpos = _bitpos;
            if (fieldSwitch) {
                tBuffer = _buffer;
                _buffer = _output;
            }

            // Se prestavimo na podano mesto:
            _bytepos = bytePos;
            _bitpos = bitPos;

            // Obnovimo vrednosti:
            _bytepos = tBytepos;
            _bitpos = tBitpos;
            if (fieldSwitch) {
                _buffer = tBuffer;
            }
        }

        public void WriteBits(ulong data, int numbits) {
            if (numbits == 0) {
                return;
            }

            // V primeru da hočemo zapisati preveč bitov:
            if (numbits > 64) {
                Console.WriteLine("ERROR: Na enkrat lahko zapišete največ 64 bitov!");
                Console.ReadLine();
                Environment.Exit(0);
            }

            for (int i = 0; i < numbits; i++) {
                // Če je bit v data 1, ustrezni bit v bufferju postavimo na 1:
                if ((data & ((ulong) 1 << i)) != 0) // SPREMENJENO
                {
                    _buffer[_bytepos] += (byte) ((byte) 1 << _bitpos);
                }

                // Pomik na naslednji bit v bufferju:
                _bitpos++;
                if (_bitpos == 8) {
                    _bitpos = 0;
                    _bytepos++;
                    if (_bytepos == _buffersize) {
                        Flush();
                    }
                }
            }
        }

        #region Pisanje ostalih tipov:

        public void WriteByte(Byte data) {
            WriteBits(data, 8);
        }

        public void WriteSByte(SByte data) {
            WriteBits((byte) data, 8);
        }

        public void WriteInt16(Int16 data) {
            byte[] temp = BitConverter.GetBytes(data);
            foreach (byte b in temp) {
                WriteBits(b, 8);
            }
        }

        public void WriteUInt16(UInt16 data) {
            WriteBits((ulong) data, 16);
        }

        public void WriteInt32(Int32 data) {
            byte[] temp = BitConverter.GetBytes(data);
            foreach (byte b in temp) {
                WriteBits(b, 8);
            }
        }

        public void WriteUInt32(UInt32 data) {
            byte[] temp = BitConverter.GetBytes(data);
            foreach (byte b in temp) {
                WriteBits(b, 8);
            }
        }

        public void WriteLong(long data) {
            byte[] temp = BitConverter.GetBytes(data);
            foreach (byte b in temp) {
                WriteBits(b, 8);
            }
        }

        public void WriteULong(ulong data) {
            byte[] temp = BitConverter.GetBytes(data);
            foreach (byte b in temp) {
                WriteBits(b, 8);
            }
        }

        public void WriteFloat(float data) {
            byte[] temp = BitConverter.GetBytes(data);
            foreach (byte b in temp) {
                WriteBits(b, 8);
            }
        }

        public void WriteDouble(double data) {
            byte[] temp = BitConverter.GetBytes(data);
            foreach (byte b in temp) {
                WriteBits(b, 8);
            }
        }

        #endregion

        public void WriteBytes(byte[] bytes) {
            for (int i = 0; i < bytes.Length; i++) {
                WriteByte(bytes[i]);
            }
        }
    }

}