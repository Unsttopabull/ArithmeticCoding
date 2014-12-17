using System;
using System.Collections.Generic;
using System.IO;

namespace BinIO {

    public class BinReadFile {
        private BinaryReader br;
        private byte[] buffer;
        private int buffersize = 65536;
        private int bytepos = 0;
        private byte bitpos = 0;


        public BinReadFile(string filename, int bufsize = 65536) {
            br = new BinaryReader(new FileStream(filename, FileMode.Open));
            buffer = new byte[bufsize];
            buffersize = bufsize;

            FillBuffer();
        }


        public void CloseFile() {
            br.Close();
        }


        public ulong ReadBits(byte numbits) {
            if (numbits > 64) {
                Console.WriteLine("ERROR: Na enkrat lahko preberete največ 64 bitov.");
                Console.ReadLine();
                Environment.Exit(0);
            }

            ulong rezultat = 0;
            for (int i = 0; (i < numbits) & !EOF(); i++) {
                // Če je bit postavljen na 1, prištevejemo ekvivalentno vrednost k rezultatu:
                if (((buffer[bytepos]) & (byte) (1 << bitpos)) != 0) {
                    rezultat += ((ulong) 1 << i);
                }

                // Pomik na naslednji bit v bufferju:
                bitpos++;
                if (bitpos == 8) {
                    bitpos = 0;
                    bytepos++;
                    if (bytepos == buffersize) {
                        FillBuffer();
                    }
                }
            }

            return rezultat;
        }

        #region Branje podatkovnih tipov:

        public Byte ReadByte() {
            return (byte) ReadBits(8);
        }

        public SByte ReadSByte() {
            return (SByte) ReadBits(8);
        }

        public Int16 ReadInt16() {
            return (Int16) ReadBits(16);
        }

        public UInt16 ReadUInt16() {
            return (UInt16) ReadBits(16);
            ;
        }

        public Int32 ReadInt32() {
            return (Int32) ReadBits(32);
            ;
        }

        public UInt32 ReadUInt32() {
            return (UInt32) ReadBits(32);
            ;
        }

        public long ReadLong() {
            return (long) ReadBits(64);
        }

        public ulong ReadULong() {
            return ReadBits(64);
        }

        public float ReadFloat() {
            ulong temp = ReadBits(32);
            byte[] temp2 = BitConverter.GetBytes(temp);
            return System.BitConverter.ToSingle(temp2, 0);
        }

        public double ReadDouble() {
            ulong temp = ReadBits(64);
            byte[] temp2 = BitConverter.GetBytes(temp);
            return System.BitConverter.ToDouble(temp2, 0);
            ;
        }

        #endregion

        #region Še več tipov:

        public byte[] ReadBytes(int number_of_bytes) {
            List<byte> output = new List<Byte>();
            for (int i = 0; i < number_of_bytes; i++) {
                output.Add(ReadByte());
            }
            return output.ToArray();
        }

        #endregion

        #region Dodatne metode za delovanje:

        // Metoda za polnjenja bufferja:
        private void FillBuffer() {
            // Ce se imamo toliko za prebrat da lahko cel buffer napolnimo:
            if (br.BaseStream.Position + (long) buffersize <= br.BaseStream.Length) {
                buffer = br.ReadBytes(buffersize);
            }
                // Drugac napolnimo samo kolikor lahko preberemo in si zapomnimo koliko byteov smo prebrali:
            else {
                buffersize = (int) (br.BaseStream.Length - br.BaseStream.Position);
                buffer = br.ReadBytes(buffersize);
            }

            // Ponastavimo pozicije branja bufferja:
            bitpos = 0;
            bytepos = 0;
        }

        // Metoda za preverjanje ce smo prisli do konca datoteke:
        public bool EOF() {
            if (br.BaseStream.Position == br.BaseStream.Length) {
                if (bytepos == buffersize) {
                    return true;
                }
            }
            return false;
        }

        // Metoda, ki vrne koliko bitov se lahko preberemo:
        public ulong BitsTillEOF() {
            ulong izhod = (ulong) (br.BaseStream.Length - br.BaseStream.Position) * 8;
            izhod += (ulong) (buffersize - bytepos - 1) * 8;
            izhod += (ulong) 7 - bitpos;

            return izhod;
        }

        #endregion
    }

}