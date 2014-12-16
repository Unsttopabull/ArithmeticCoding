using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using AritmeticniKodirnik;
using AritmeticniKodirnik.Read;
using AritmeticniKodirnik.Write;
using ArtimeticniKodirnik;

namespace Test {

    internal class Program {

        private static void Main(string[] args) {
            Debug.Listeners.Add(new TextWriterTraceListener(new FileStream("binWriteTest.txt", FileMode.Create)));
            Debug.AutoFlush = true;

            byte[] bajti = Encoding.ASCII.GetBytes("GEMMA");
            //byte[] bajti = Encoding.ASCII.GetBytes("Krasna si bistra hci planin");

            Kodirnik k = new Kodirnik(bajti, StBitov.Bit32);
            byte[] kodiraj = k.Kodiraj();
            IzpisiBajte(kodiraj);

            //k.ZapisiDatoteko("gemma_32.ac");

            //TestGemma();

            //BinWriteFile bwf = new BinWriteFile("text.bin");
            //bwf.WriteBits(0, 1);
            //bwf.WriteBits(0, 1);
            //bwf.WriteBits(0, 1);

            //bwf.WriteBits(1, 1);

            //bwf.WriteBits(0, 1);
            //bwf.WriteBits(0, 1);
            //bwf.WriteBits(0, 1);

            //bwf.WriteBits(1, 2);

            //bwf.WriteBits(1, 1);
            //bwf.WriteBits(1, 1);

            //bwf.Close();

            //BinaryReader br = new BinaryReader(new FileStream("text.bin", FileMode.Open));
            //byte[] readBytes = br.ReadBytes(2);
            Console.ReadLine();
        }

        private static void IzpisiBajte(IEnumerable<byte> bajti) {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bajti) {
                sb.Append(BinUtils.Long2Bin(b, 8));
            }
            Console.WriteLine(sb.ToString());
        }

        private static void TestGemma() {
            BinWriteFile bwf = new BinWriteFile("gemma.bin");
            //bwf.WriteBits(1, 2); //8-bit
            bwf.WriteBits(1, 2);
            bwf.WriteInt32(3232);

            //for (int i = 0; i < byte.MaxValue; i++) {
                //switch ((char)i) {
                //    case 'G':
                        //bwf.WriteDouble(1.0 / 5.0);
                    //    break;
                    //case 'E':
                        //bwf.WriteDouble(1.0 /5.0);
                    //    break;
                    //case 'M':
                        //bwf.WriteDouble(2.0 /5.0);
                    //    break;
                    //case 'A':
                        //bwf.WriteDouble(1.0 /5.0);
                        //break;
                    //default:
                        //bwf.WriteDouble(0);
                //        break;
                //}
            //}

            //bwf.WriteByte(1);
            //bwf.WriteByte(0);
            //bwf.WriteByte(14);
            bwf.Close();

            BinReadFile brf = new BinReadFile("gemma.bin");
            //ulong bitEncoding = brf.ReadBits(2);
            //Debug.WriteLine(string.Format("BitEncoding: {0};", BinUtils.Long2Bin(bitEncoding, 2)));
            Debug.WriteLine(";");

            
            ulong readBits = brf.ReadBits(2);
            var readByte = brf.ReadInt32();

            //for (int i = 0; i < 4; i++) {
                //double freq = brf.ReadDouble();
                //Debug.WriteLine("{0};{1}", i, freq);
            //}
            Debug.WriteLine(";");

            //while (!brf.Eof()) {
            //    byte b = brf.ReadByte();
            //    Debug.WriteLine(BinUtils.Long2Bin(b, 8));
            //}

            brf.Close();

            Debug.Flush();
        }
    }

}