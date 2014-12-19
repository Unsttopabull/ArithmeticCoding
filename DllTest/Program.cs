using System;
using System.Diagnostics;
using System.IO;
using ArtimeticniKodirnik;
using ArtimeticniKodirnik.Dekodiranje;
using ArtimeticniKodirnik.Kodiranje;

namespace Vaja_1 {

    internal class Program {

        private static int velikost_segmenta = 8192;
        private static StBitov stevilo_bitov = StBitov.Bit64;
        private static Dekodirnik _dekodirnik;

        private static void Help() {
            Console.WriteLine("Program AC - coded by " + Author());
            Console.WriteLine("Usage: ACTester -[E/D] inputFile outputFile");
            Environment.Exit(0);
        }

        private static string Author() {
            return "Martin Kraner E5020649";
        }

        private static void Main(string[] args) {
            _dekodirnik = new Dekodirnik();
            // Nastavimo stevilo bitov za kodiranje:

            // Debugiranje/prikaz delovanja:
            //DEBUG();
            //args = new string[] { "-E", "test_in.txt", "test.bin" };
            //args = new string[] { "-D", "test.bin", "test_out.txt" };


            // Pomoč:
            if (args.Length != 3) {
                Help();
            }
            // Encode:
            if (args[0].ToUpper() == "-E") {
                string fileNameIn = args[1];
                string fileNameOut = args[2];

                try {
                    // Začetek merjenja časa:
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    // Zakodiramo:
                    ENCODE_SINGLE(fileNameIn, fileNameOut);
                    //ENCODE_SEGMENTS(fileNameIn, fileNameOut);

                    // Konec merjenje časa in izpis časa:
                    stopwatch.Stop();
                    Console.WriteLine("Zakodirano v: " + stopwatch.Elapsed);
                }
                catch (FileNotFoundException ex) {
                    Console.WriteLine("ERROR: Datoteka " + ex.FileName + " ne obstaja!");
                    Console.ReadLine();
                }
            }
                // Decode:
            else if (args[0].ToUpper() == "-D") {
                string fileNameIn = args[1];
                string fileNameOut = args[2];

                try {
                    // Začetek merjenja časa:
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    // Dekodiramo:
                    DECODE_SINGLE(fileNameIn, fileNameOut);
                    //DECODE_SEGMENTS(fileNameIn, fileNameOut);

                    // Konec merjenje časa in izpis časa:
                    stopwatch.Stop();
                    Console.WriteLine("Dekodirano v: " + stopwatch.Elapsed);
                    Console.ReadLine();
                }
                catch (FileNotFoundException ex) {
                    Console.WriteLine("ERROR: Datoteka \"" + ex.FileName + "\" ne obstaja!");
                    Console.ReadLine();
                }
            }
                // Pomoč:
            else {
                Help();
            }
        }

        #region Encode:

        // Kodiranje v datoteko brez segmentne strukture:
        private static byte[] ENCODE_SINGLE(string fileNameIn, string fileNameOut) {
            byte[] data;
            using (FileStream fs = new FileStream(fileNameIn, FileMode.Open, FileAccess.Read)) {
                using (BinaryReader br = new BinaryReader(fs)) {
                    long numBytes = new FileInfo(fileNameIn).Length;
                    data = br.ReadBytes((int) numBytes);
                }
            }

            Kodirnik kodirnik = new Kodirnik(stevilo_bitov);
            data = kodirnik.Kodiraj(data);

            File.WriteAllBytes(fileNameOut, data);
            return data;
        }

        #endregion

        #region Decode:

        // Dekodiranje datoteke brez segmentne strukture:
        private static byte[] DECODE_SINGLE(string fileNameIn, string fileNameOut) {
            byte[] data;
            FileStream fs = new FileStream(fileNameIn, FileMode.Open, FileAccess.Read);

            BinaryReader br = new BinaryReader(fs);
            long numBytes = new FileInfo(fileNameIn).Length;

            data = br.ReadBytes((int) numBytes);

            br.Close();
            fs.Close();

            data = _dekodirnik.Dekodiraj(data, fileNameOut);

            return data;
        }

        #endregion
    }

}