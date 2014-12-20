using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ArtimeticniKodirnik;
using BinIO;

namespace Test {

    internal class Program {
        private static void Main(string[] args) {
            int st = 5560;
            int div = 5560 / 8;
            int divBit = 5560 >> 3;

            byte[] arr = ZapisiGlavo();
            //File.WriteAllBytes("tabela.bin", arr);

            ulong cF;
            Simbol[] tabela = BeriGlavo(arr, out cF);

            //TestBitWriter();

            Console.ReadLine();
        }

        private static void TestBitWriter() {

            BitWriter bw = new BitWriter();
            bw.WriteBits(3, 2);


            var nextF = double.MinValue;
            byte[] nextBajti = BitConverter.GetBytes(nextF);

            bw.WriteDouble(nextF);

            //byte[] bajti = BitConverter.GetBytes(next);
            //string fBin = BinUtils.Bytes2Bin(bajti);

            byte[] bytes = bw.GetData();

            File.WriteAllBytes("out.bin", bytes);

            byte[] readAllBytes = File.ReadAllBytes("out.bin");

            BitReader br = new BitReader(readAllBytes);

            ulong readBits = br.ReadBits(2);
            var readVal = br.ReadDouble();

            byte[] readBajti = BitConverter.GetBytes(readVal);
            string fBinRead = BinUtils.Bytes2Bin(readBajti);
        }

        private static Simbol[] BeriGlavo(byte[] arr, out ulong cF) {
            BitReader br = new BitReader(arr);
            ulong readBits = br.ReadBits(2);

            int stSimbolov = br.ReadByte();

            List<Simbol> simboli = new List<Simbol>();

            cF = 0;
            ulong spMeja = 0;
            for (int i = 0; i < stSimbolov; i++) {
                byte vrednost = br.ReadByte();
                ulong frekvenca = br.ReadUIn64();

                cF += frekvenca;

                ulong zgMeja = spMeja + frekvenca;
                simboli.Add(new Simbol(frekvenca, zgMeja, spMeja, vrednost));

                spMeja = zgMeja;
            }

            return simboli.ToArray();
        }

        private static byte[] ZapisiGlavo() {
            BitWriter bw = new BitWriter();
            bw.WriteBits(2, 2);

            Simbol[] tabela = new Simbol[4];
            tabela[2] = new Simbol(1, 1, 0, 71); //G
            tabela[1] = new Simbol(1, 2, 1, 69); //E
            tabela[3] = new Simbol(2, 4, 2, 77); //M
            tabela[0] = new Simbol(1, 5, 4, 65); //A

            //št. simbolov
            bw.WriteByte((byte) tabela.Length);

            byte[] bytes = bw.GetData();
            string byteStr = BinUtils.Bytes2Bin(bytes);

            int a = 5;

            for (int i = 0; i < tabela.Length; i++) {
                //kateri simbol predstavlja
                bw.WriteByte(tabela[i].Vrednost);

                bytes = bw.GetData();
                byteStr = BinUtils.Bytes2Bin(bytes);

                //frekvenca
                bw.WriteUInt64(tabela[i].Frekvenca);

                bytes = bw.GetData();
                byteStr = BinUtils.Bytes2Bin(bytes);
            }

            //byte[] output = bw.GetData();
            //StringBuilder sb = new StringBuilder();
            //foreach (byte b in output) {
            //    sb.AppendLine(BinUtils.ULong2Bin(b, 8));
            //}
            //string str = sb.ToString();

            return bw.GetData();
        }

        private static string IzpisiBajte(IList<byte> bajti, int splitCount = 4) {
            if (splitCount <= 0) {
                splitCount = 4;
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bajti.Count; i++) {
                byte b = bajti[i];

                if (i % splitCount == 0) {
                    sb.Append(" ");
                }
                sb.Append(BinUtils.ULong2Bin(b, 8));
            }
            return sb.ToString().TrimStart(' ');
        }

    }

}