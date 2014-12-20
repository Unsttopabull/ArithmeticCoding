﻿using System;
using System.Diagnostics;
using System.IO;
using ArtimeticniKodirnik.DLL;

namespace AritmeticniKodirnik.Konzola.DllTest {

    internal class Program {

        private static void Help() {
            Console.WriteLine("Program AC - coded by {0}", AC.Author());
            Console.WriteLine("Usage: ACTester -[E/D] inputFile outputFile");
            Environment.Exit(0);
        }

        private static void Main(string[] args) {
            if (args.Length != 3) {
                Help();
            }

            switch (args[0].ToUpperInvariant()) {
                case "-E":
                    try {
                        Stopwatch stopwatch = Stopwatch.StartNew();

                        Kodiraj(args[1], args[2]);

                        stopwatch.Stop();
                        Console.WriteLine("Zakodirano v: {0}", stopwatch.Elapsed);
                    }
                    catch (FileNotFoundException e) {
                        Console.Error.WriteLine("NAPAKA: Datoteka {0} ni bila najdena!", e.FileName);
                        Console.ReadLine();
                    }
                    break;
                case "-D":
                    try {
                        Stopwatch stopwatch = Stopwatch.StartNew();

                        Dekodiraj(args[1], args[2]);

                        // Konec merjenje časa in izpis časa:
                        stopwatch.Stop();
                        Console.WriteLine("Dekodirano v: {0}", stopwatch.Elapsed);
                    }
                    catch (FileNotFoundException e) {
                        Console.Error.WriteLine("NAPAKA: Datoteka {0} ni bila najdena!", e.FileName);
                        Console.ReadLine();
                    }
                    break;
                default:
                    Help();
                    break;
            }
        }

        private static byte[] Kodiraj(string inputFile, string outputFile) {
            byte[] data = File.ReadAllBytes(inputFile);

            Stopwatch sw = Stopwatch.StartNew();

            data = AC.Encode(data);

            sw.Stop();
            Console.WriteLine("Samo kodiranje: " + sw.Elapsed);

            File.WriteAllBytes(outputFile, data);
            return data;
        }

        private static byte[] Dekodiraj(string inputFile, string outpuFile) {
            byte[] data = File.ReadAllBytes(inputFile);

            Stopwatch sw = Stopwatch.StartNew();

            data = AC.Decode(data);

            sw.Stop();
            Console.WriteLine("Samo kodiranje: " + sw.Elapsed);

            if (data != null) {
                File.WriteAllBytes(outpuFile, data);
            }

            return data;
        }
    }

}