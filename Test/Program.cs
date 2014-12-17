using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using ArtimeticniKodirnik;
using BinIO;

namespace Test {

    internal class Program {
        private static void Main(string[] args) {
            byte[] arr = ZapisiTabelo();

            File.WriteAllBytes("test.bin", arr);
            byte[] arr2 = File.ReadAllBytes("test.bin");

            PreberiTabelo(arr2);

            Console.ReadLine();
        }

        private static void PreberiTabelo(byte[] arr) {
            BinRead br = new BinRead(arr);

            ulong stBitov = br.ReadBits(2);

            byte stSimbolov = br.ReadByte();

            List<object> obj = new List<object>();
            for (int i = 0; i < stSimbolov; i++) {
                byte bajt = br.ReadByte();
                ulong freq = br.ReadULong();

                obj.Add(new {Vrednost = bajt, Frekvenca = freq});
            }
        }

        private static byte[] ZapisiTabelo() {
            BinWrite bw = new BinWrite();
            bw.WriteBits(2, 2);

            Simbol[] tabela = new Simbol[4];
            tabela[2] = new Simbol(1, 1 / 5.0, 1, 0, 71); //G
            tabela[1] = new Simbol(1, 1 / 5.0, 2, 1, 69); //E
            tabela[3] = new Simbol(2, 2 / 5.0, 4, 2, 77); //M
            tabela[0] = new Simbol(1, 1 / 5.0, 5, 4, 65); //A

            //št. simbolov
            bw.WriteByte((byte) tabela.Length);
            for (int i = 0; i < tabela.Length; i++) {
                //kateri simbol predstavlja
                bw.WriteByte(tabela[i].Vrednost);
                //frekvenca
                bw.WriteULong(tabela[i].Frekvenca);
            }

            byte[] output = bw.GetOutput();
            StringBuilder sb = new StringBuilder();
            foreach (byte b in output) {
                sb.AppendLine(BinUtils.Long2Bin(b, 8));
            }
            string str = sb.ToString();

            return output;
        }

        private static void IzpisiBajte(IEnumerable<byte> bajti) {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bajti) {
                sb.Append(BinUtils.Long2Bin(b, 8));
            }
            Console.WriteLine(sb.ToString());
        }

    }

}