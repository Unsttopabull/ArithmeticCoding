using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using ArtimeticniKodirnik;
using ArtimeticniKodirnik.Dekodiranje;
using ArtimeticniKodirnik.Kodiranje;

namespace AritmeticniKodirnik.Konzola.Demo {

    internal class Program {
        private static void Help() {
            Console.WriteLine("Program AC - coded by {0}", "Martin Kraner E5020649");
            Console.WriteLine("Usage: ACDemo -F [-E/-D] inputFile");
            Console.WriteLine("Usage: ACDemo -T [-E/-D] GEMMA");
            Environment.Exit(0);
        }

        private static void Main(string[] args) {
            if (args.Length == 3) {
                Help();
            }

            switch (args[0].ToUpperInvariant()) {
                case "-F":
                    try {
                        ObdelajUkaz(args, datoteka: true);
                    }
                    catch (FileNotFoundException e) {
                        Console.Error.WriteLine("NAPAKA: {0}", e.Message);
                    }
                    break;
                case "-T":
                    ObdelajUkaz(args, false);
                    break;
                default:
                    Help();
                    break;
            }
        }

        private static void ObdelajUkaz(string[] args, bool datoteka) {
            byte[] data = datoteka
                              ? File.ReadAllBytes(args[2])
                              : Encoding.ASCII.GetBytes(args[2]);

            switch (args[1].ToUpperInvariant()) {
                case "-E":
                    Kodiraj(data);
                    break;
                case "-D":
                    Dekodiraj(data);
                    break;
            }
        }

        private static void Kodiraj(byte[] data) {
            Kodirnik k = new Kodirnik(StBitov.Bit64, true);
            data = k.Kodiraj(data);
        }

        private static void Dekodiraj(byte[] data) {
            Dekodirnik d = new Dekodirnik(true);
            data = d.Dekodiraj(data, "out.txt");
        }
    }

}