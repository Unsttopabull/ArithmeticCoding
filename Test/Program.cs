using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ArtimeticniKodirnik;
using BinIO;

namespace Test {

    internal class Program {
        private static void Main(string[] args) {
            //byte[] arr = ZapisiTabelo();

            //File.WriteAllBytes("test.bin", arr);
            //byte[] arr2 = File.ReadAllBytes("test.bin");

            byte[] bytes = BitConverter.GetBytes(int.MinValue);
            Array.Resize(ref bytes, 8);

            ulong u = BitConverter.ToUInt64(bytes, 0);

            int a = 1689486;
            File.WriteAllBytes("int.out", BitConverter.GetBytes(a));

            int i = 63 << 26;
            string long2Bin = BinUtils.Long2Bin((ulong) i, 32);

            //PreberiTabelo(arr2);
            byte t = (1 << 8 - 2) - 1;
            byte t2 = (byte) ~t;
            string s = Convert.ToString(t, 2);
            string s2 = Convert.ToString(t2, 2);

            TestBitWriter();

            Console.ReadLine();
        }

        private static void TestBitWriter() {
            BitWriter bw = new BitWriter();

            bw.WriteBits(3, 2);

            Random r = new Random();
            int next = r.Next();

            bw.WriteInt32(next);

            //bw.WriteInt32(int.MaxValue/2); 
            //1073741823
            //0011 1111 1111 1111 1111 1111 1111 1111

            byte[] min = BitConverter.GetBytes(next);
            string nextBin = BinUtils.Long2BinBajti((ulong) next, 32);

            byte bitPos = bw.BitPos;
            byte[] bytes = bw.GetData();

            string zapisano = string.Join(" ", bytes.Take(5).Select(b => BinUtils.Long2BinBajti(b, 8)));

            File.WriteAllBytes("out.bin", bytes);

            byte[] readAllBytes = File.ReadAllBytes("out.bin");

            BitReader br = new BitReader(readAllBytes);

            ulong readBits = br.ReadBits(2);
            int readInt32 = br.ReadInt32();

            byte[] bytes1 = BitConverter.GetBytes(readInt32);
            string readBin = BinUtils.Long2BinBajti((ulong) readInt32, 32);

            //int int32 = BitConverter.ToInt32(readAllBytes, 0);
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
            tabela[2] = new Simbol(1, 1, 0, 71); //G
            tabela[1] = new Simbol(1, 2, 1, 69); //E
            tabela[3] = new Simbol(2, 4, 2, 77); //M
            tabela[0] = new Simbol(1, 5, 4, 65); //A

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