using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using ArtimeticniKodirnik;
using ArtimeticniKodirnik.Dekodiranje;
using ArtimeticniKodirnik.Kodiranje;

namespace Vaja1_ArtimeticniKodirnik {

    public partial class AritmeticnoKodiranje : Form {
        private Kodirnik _kodirnik;
        private Dekodirnik _dekodirnik;

        public AritmeticnoKodiranje() {
            InitializeComponent();

            cbBiti.SelectedIndex = (int) StBitov.Bit8;
            _kodirnik = new Kodirnik((StBitov) cbBiti.SelectedIndex);
            _kodirnik.SimbolZakodiran += KodirnikSimbolZakodiran;
            _kodirnik.TabelaGenerirana += KodirnikTabelaGenerirana;

            _dekodirnik = new Dekodirnik();
        }

        private void KodirnikTabelaGenerirana(IList<Simbol> tabela) {
            for (int i = 0; i < tabela.Count; i++) {
                if (tabela[i] == null) {
                    continue;
                }

                Simbol simbol = tabela[i];
                string[] podatki = {
                    new string((char)simbol.Vrednost, 1), 
                    simbol.Frekvenca.ToString(CultureInfo.InvariantCulture),
                    simbol.SpodnjaMeja.ToString(CultureInfo.InvariantCulture),
                    simbol.ZgornjaMeja.ToString(CultureInfo.InvariantCulture),
                };

                lvTabela.Items.Add(new ListViewItem(podatki));
            }
        }

        private void KodirnikSimbolZakodiran(byte simbol, ulong spMeja, ulong zgMeja, ulong korak, ulong novaSpMeja, ulong novaZgMeja, string operacije, int e3Count) {
            string[] podatki;
            if (e3Count == -1) {
                podatki = new[] {
                    "",
                    "",
                    "",
                    "",
                    operacije,
                    ""
                };
            }
            else {
                podatki = new[] {
                    new string((char)simbol, 1), 
                    string.Format("({0}, {1})", spMeja, zgMeja),
                    korak.ToString(CultureInfo.InvariantCulture),
                    string.Format("({0}, {1})", novaSpMeja, novaZgMeja),
                    operacije,
                    e3Count.ToString(CultureInfo.InvariantCulture)
                };
            }

            lvPostopek.Items.Add(new ListViewItem(podatki));
        }

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

            _kodirnik.SimbolZakodiran -= KodirnikSimbolZakodiran;
            _kodirnik.TabelaGenerirana -= KodirnikTabelaGenerirana;

            _kodirnik = new Kodirnik(stBitov);

            _kodirnik.SimbolZakodiran += KodirnikSimbolZakodiran;
            _kodirnik.TabelaGenerirana += KodirnikTabelaGenerirana;
        }

        private void BtnKodirajTextClick(object sender, EventArgs e) {
            NastaviStolpceZaKodiranje();

            byte[] podatki = Encoding.ASCII.GetBytes(tbText.Text);
            _kodirnik.Kodiraj(podatki);
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

        private void BtnDekodirajTextClick(object sender, EventArgs e) {

        }

        private void btnKodirajDatoteko_Click(object sender, EventArgs e) {
            NastaviStolpceZaKodiranje();

            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK) {
                return;
            }          
  
            _kodirnik.Kodiraj(ofd.FileName);

            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() != DialogResult.OK) {
                MessageBox.Show("Niste izbrali izhodne datoteke");
                return;
            }
            _kodirnik.ZapisiDatoteko(sfd.FileName);
        }

        private void BtnDekodirajDatotekoClick(object sender, EventArgs e) {
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
            _dekodirnik.Dekodiraj(ofd.FileName, sfd.FileName);
        }
    }

}