using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ArtimeticniKodirnik;
using ArtimeticniKodirnik.Dekodiranje;
using ArtimeticniKodirnik.Kodiranje;
using ConsoleTables.Core;

namespace AritmeticniKodirnik.Konzola.Demo {

    internal class Program {
        private static StringBuilder _rezultat;
        private static ConsoleTable _tabela;

        private static void Help() {
            Console.WriteLine("Program AC - coded by {0}", "Martin Kraner E5020649");
            Console.WriteLine("Usage: ACDemo -F [-E/-D] inputFile");
            Console.WriteLine("Usage: ACDemo -T [-E/-D] GEMMA");
            Environment.Exit(0);
        }

        private static void Main(string[] args) {
            if (args.Length != 3) {
                Help();
            }

            _rezultat = new StringBuilder();

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
            _tabela = new ConsoleTable("Symbol", "(L,H)", "Step", "(nL, nH)", "Operations + Output", "E3Cnt");

            Kodirnik k = new Kodirnik(StBitov.Bit64, true);
            k.SimbolZakodiran += SimbolZakodiran;
            k.TabelaGenerirana += TabelaGenerirana;

            k.Kodiraj(data);

            Console.WriteLine(_tabela.ToString());
            Console.WriteLine(_rezultat.ToString());
        }

        private static void Dekodiraj(byte[] data) {
            _tabela = new ConsoleTable("V", "(L,H)", "Step", "S", "Symbol", "(nL, nH)", "Operations");

            Dekodirnik d = new Dekodirnik(true);
            d.TabelaGenerirana += TabelaGenerirana;
            d.SimbolDekodiran += SimbolDekodiran;

            d.Dekodiraj(data);

            Console.WriteLine(_tabela.ToString());
            Console.WriteLine(_rezultat.ToString());
        }

        private static void TabelaGenerirana(IList<Simbol> tabela) {
            ConsoleTable consoleTable = new ConsoleTable("Symbol", "Freq", "l(x)", "h(x)");
            
            foreach (Simbol s in tabela.Where(s => s != null)) {
                consoleTable.AddRow(new string((char) s.Vrednost, 1), s.Frekvenca, s.SpodnjaMeja, s.ZgornjaMeja);
            }
            consoleTable.Write();
            Console.WriteLine();
        }

        private static void SimbolZakodiran(byte simbol, ulong spMeja, ulong zgMeja, ulong korak, ulong novaSpMeja, ulong novaZgMeja, int e3Count, params Operacija[] operacije) {
            if (e3Count == -1) {
                _tabela.AddRow("", "", "", "", string.Join<Operacija>("; ", operacije), "");
            }
            else {
                _tabela.AddRow(
                    new string((char) simbol, 1),
                    string.Format("({0}, {1})", spMeja, zgMeja),
                    korak,
                    string.Format("({0}, {1})", novaSpMeja, novaZgMeja),
                    string.Join<Operacija>("; ", operacije),
                    e3Count
                );
            }

            foreach (Operacija operacija in operacije) {
                _rezultat.Append(operacija.Izhod);
            }
        }

        private static void SimbolDekodiran(string polje, ulong spMeja, ulong zgMeja, ulong korak, ulong vrednostSymb, byte simbol, ulong novaSpMeja, ulong novaZgMeja, params string[] operacije) {
            _tabela.AddRow(
                polje,
                string.Format("({0}, {1})", spMeja, zgMeja),
                korak,
                vrednostSymb,
                new string((char)simbol, 1),
                string.Format("({0}, {1})", novaSpMeja, novaZgMeja),
                string.Join("; ", operacije)
            );

            _rezultat.Append((char) simbol);
        }
    }

}