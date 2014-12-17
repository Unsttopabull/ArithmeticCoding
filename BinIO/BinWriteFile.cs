using System;
using System.IO;

namespace BinIO {

    public class BinWriteFile {
        private BinaryWriter bw;
        private byte[] buffer;
        private int buffersize = 65536;
        private int bytepos = 0;
        private byte bitpos = 0;

        public BinWriteFile(string filename, int bufsize = 65536) //65536 500
        {
            bw = new BinaryWriter(new FileStream(filename, FileMode.Create));
            buffer = new byte[bufsize];
            buffersize = bufsize;
        }

        public void Close() {
            Flush();
            bw.Close();
        }

        public void Flush() {
            // Zapišemo napolnjene byte-e iz bufferja:
            bw.Write(buffer, 0, bytepos);
            // Če smo kateri byte le delno napolnili, ga tudi zapišemo:
            if (bitpos > 0) //&& buffer[bytepos] != 0)
            {
                byte mask = (byte) ((1 << (int) bitpos) - 1);
                bw.Write((byte) (buffer[bytepos] & mask));
            }

            // Flushamo:
            bw.Flush();

            // Ponastavimo indexe:
            bytepos = 0;
            bitpos = 0;

            // Za vsak slučaj če nam je ostalo kaj v prejšni iteraciji branja v buffer.
            for (int i = 0; i < buffersize; i++) {
                buffer[i] = 0;
            }
        }

        public void WriteBits(ulong data, byte numbits) {
            // V primeru da hočemo zapisati preveč bitov:
            if (numbits > 64) {
                Console.WriteLine("ERROR: Na enkrat lahko zapišete največ 64 bitov!");
                Console.ReadLine();
                Environment.Exit(0);
            }

            for (int i = 0; i < numbits; i++) {
                // Če je bit v data 1, ustrezni bit v bufferju postavimo na 1:
                //SPREMENJENO: if ((data & (ulong)(1 << i)) != 0)
                if ((data & ((ulong) 1 << i)) != 0) {
                    
                    buffer[bytepos] += (byte) ((byte) 1 << bitpos);
                }

                // Pomik na naslednji bit v bufferju:
                bitpos++;
                if (bitpos == 8) {
                    bitpos = 0;
                    bytepos++;
                    if (bytepos == buffersize) {
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

        #region Še več tipov:

        public void WriteBytes(byte[] data) {
            foreach (byte b in data) {
                WriteByte(b);
            }
        }

        #endregion
    }

}