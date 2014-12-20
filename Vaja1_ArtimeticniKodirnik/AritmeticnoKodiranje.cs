using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ArtimeticniKodirnik.Dekodiranje;
using ArtimeticniKodirnik.Kodiranje;

namespace ArtimeticniKodirnik.UI {

    public partial class AritmeticnoKodiranje : Form {
        private Kodirnik _kodirnik;
        private readonly Dekodirnik _dekodirnik;

        public AritmeticnoKodiranje() {
            InitializeComponent();

            cbBiti.SelectedIndex = (int) StBitov.Bit8;
            _kodirnik = new Kodirnik((StBitov) cbBiti.SelectedIndex, true);
            _kodirnik.SimbolZakodiran += SimbolZakodiran;
            _kodirnik.TabelaGenerirana += TabelaGenerirana;

            _dekodirnik = new Dekodirnik(true);
            _dekodirnik.SimbolDekodiran += SimbolDekodiran;
            _dekodirnik.TabelaGenerirana += TabelaGenerirana;
        }

        #region AC Dogodki

        private void TabelaGenerirana(IList<Simbol> tabela) {
            foreach (Simbol s in tabela) {
                if (s == null) {
                    continue;
                }

                string[] podatki = {
                    new string((char)s.Vrednost, 1), 
                    s.Frekvenca.ToString(CultureInfo.InvariantCulture),
                    s.SpodnjaMeja.ToString(CultureInfo.InvariantCulture),
                    s.ZgornjaMeja.ToString(CultureInfo.InvariantCulture),
                };

                lvTabela.Items.Add(new ListViewItem(podatki));
            }
        }

        private void SimbolZakodiran(byte simbol, ulong spMeja, ulong zgMeja, ulong korak, ulong novaSpMeja, ulong novaZgMeja, int e3Count, params Operacija[] operacije) {
            string[] podatki;
            if (e3Count == -1) {
                podatki = new[] {
                    "",
                    "",
                    "",
                    "",
                    string.Join<Operacija>("; ", operacije),
                    ""
                };
            }
            else {
                podatki = new[] {
                    new string((char)simbol, 1), 
                    string.Format("({0}, {1})", spMeja, zgMeja),
                    korak.ToString(CultureInfo.InvariantCulture),
                    string.Format("({0}, {1})", novaSpMeja, novaZgMeja),
                    string.Join<Operacija>("; ", operacije),
                    e3Count.ToString(CultureInfo.InvariantCulture)
                };
            }

            foreach (Operacija operacija in operacije) {
                tbRezultat.Text += operacija.Izhod;
            }

            lvPostopek.Items.Add(new ListViewItem(podatki));
        }

        private void SimbolDekodiran(string polje, ulong spMeja, ulong zgMeja, ulong korak, ulong vrednostSymb, byte simbol, ulong novaSpMeja, ulong novaZgMeja, params string[] operacije) {
            string[] podatki = {
                polje, 
                string.Format("({0}, {1})", spMeja, zgMeja),
                korak.ToString(CultureInfo.InvariantCulture),
                vrednostSymb.ToString(CultureInfo.InvariantCulture),
                new string((char)simbol, 1), 
                string.Format("({0}, {1})", novaSpMeja, novaZgMeja),
                string.Join("; ", operacije),
            };

            tbRezultat.Text += (char) simbol;

            lvPostopek.Items.Add(new ListViewItem(podatki));
        }

        #endregion

        private void CbBitiSelectedIndexChanged(object sender, EventArgs e) {
            if (_kodirnik == null) {
                return;
            }

            int biti = cbBiti.SelectedIndex;
            StBitov stBitov;
            switch (biti) {
                case 0:
                    stBitov = StBitov.Bit8;
                    break;
                case 1:
                    stBitov = StBitov.Bit16;
                    break;
                case 2:
                    stBitov = StBitov.Bit32;
                    break;
                case 3:
                    stBitov = StBitov.Bit64;
                    break;
                default:
                    stBitov = StBitov.Bit32;
                    break;
            }

            _kodirnik.SimbolZakodiran -= SimbolZakodiran;
            _kodirnik.TabelaGenerirana -= TabelaGenerirana;

            _kodirnik = new Kodirnik(stBitov, true);

            _kodirnik.SimbolZakodiran += SimbolZakodiran;
            _kodirnik.TabelaGenerirana += TabelaGenerirana;
        }

        private void BtnKodirajTextClick(object sender, EventArgs e) {
            tbRezultat.Text = "";
            NastaviStolpceZaKodiranje();

            byte[] podatki = Encoding.ASCII.GetBytes(tbText.Text);

            byte[] data = _kodirnik.Kodiraj(podatki);
            File.WriteAllBytes("tekst.ac", data);

            podatki = new byte[0];
            data = new byte[0];
            GC.Collect();
        }

        private void BtnDekodirajTextClick(object sender, EventArgs e) {
            _dekodirnik.Dekodiraj("tekst.ac");
        }

        private void NastaviStolpceZaKodiranje() {
            lvPostopek.Items.Clear();
            lvTabela.Items.Clear();

            lvPostopek.Columns[0].Text = "Symbol";
            lvPostopek.Columns[1].Text = "(L,H)";
            lvPostopek.Columns[2].Text = "Step";
            lvPostopek.Columns[3].Text = "(nL,nH)";
            lvPostopek.Columns[4].Text = "(Operations + Output)";
            lvPostopek.Columns[4].Width = 420;
            lvPostopek.Columns[5].Text = "E3 Cnt";
            lvPostopek.Columns[6].Text = "";
            lvPostopek.Columns[6].Width = 60;
        }

        private void NastaviStolpceZaDekodiranje() {
            lvPostopek.Items.Clear();
            lvTabela.Items.Clear();

            lvPostopek.Columns[0].Text = "V";
            lvPostopek.Columns[1].Text = "(L,H)";
            lvPostopek.Columns[2].Text = "Step";
            lvPostopek.Columns[3].Text = "S";
            lvPostopek.Columns[4].Text = "Symbol";
            lvPostopek.Columns[4].Width = 60;
            lvPostopek.Columns[5].Text = "(nL,nH)";
            lvPostopek.Columns[6].Text = "Operations";
            lvPostopek.Columns[6].Width = 420;
        }

        private void btnKodirajDatoteko_Click(object sender, EventArgs e) {
            tbRezultat.Text = "";
            NastaviStolpceZaKodiranje();

            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK) {
                return;
            }

            byte[] kodiraj = _kodirnik.Kodiraj(ofd.FileName);

            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() != DialogResult.OK) {
                MessageBox.Show("Niste izbrali izhodne datoteke");
                return;
            }

            File.WriteAllBytes(sfd.FileName, kodiraj);

            kodiraj = new byte[0];
            GC.Collect();
        }

        private void BtnDekodirajDatotekoClick(object sender, EventArgs e) {
            tbRezultat.Text = "";
            NastaviStolpceZaDekodiranje();

            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK) {
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() != DialogResult.OK) {
                MessageBox.Show("Niste izbrali izhodne datoteke");
                return;
            }
            byte[] dekodiraj = _dekodirnik.Dekodiraj(ofd.FileName);
            File.WriteAllBytes(sfd.FileName, dekodiraj);
            GC.Collect();
        }
    }

}