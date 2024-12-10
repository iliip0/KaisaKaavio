using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KaisaKaavio
{
    /// <summary>
    /// Popup ikkuna pelin detaljien editoimiseen. (pisimmät pötköt)
    /// </summary>
    public partial class PelinTiedotPopup : Form
    {
        Kilpailu kilpailu = null;
        Peli peli = null;

        public PelinTiedotPopup(Kilpailu kilpailu, Peli peli)
        {
            this.kilpailu = kilpailu;
            this.peli = peli;

            InitializeComponent();

            Shown += PelinTiedotPopup_Shown;
            FormClosing += PelinTiedotPopup_FormClosing;
        }

        void PelinTiedotPopup_Shown(object sender, EventArgs e)
        {
            this.pelidetaljiNimi1textBox.Text = peli.Pelaaja1;
            this.pelidetaljiNimi2textBox.Text = peli.Pelaaja2;

            this.Text = string.Format("{0}. kierroksen peli {1} {2} - {3} {4}",
                peli.Kierros,
                peli.Pelaaja1,
                peli.Pisteet1,
                peli.Pisteet2,
                peli.Pelaaja2);

            this.pelidetaljiPelaaja1Sarja1textBox.Text = peli.PisinSarja1;
            this.pelidetaljiPelaaja1Sarja2textBox.Text = peli.ToiseksiPisinSarja1;

            this.pelidetaljiPelaaja2Sarja1textBox.Text = peli.PisinSarja2;
            this.pelidetaljiPelaaja2Sarja2textBox.Text = peli.ToiseksiPisinSarja2;

            this.pelidetaljiPelaaja1Sarja2textBox.Enabled = !string.IsNullOrEmpty(this.pelidetaljiPelaaja1Sarja1textBox.Text);
            this.pelidetaljiPelaaja2Sarja2textBox.Enabled = !string.IsNullOrEmpty(this.pelidetaljiPelaaja2Sarja1textBox.Text);

            this.pelidetaljiPelaaja1Sarja1textBox.Focus();
        }

        void PelinTiedotPopup_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.peli.PisinSarja1 = this.pelidetaljiPelaaja1Sarja1textBox.Text;
            this.peli.PisinSarja2 = this.pelidetaljiPelaaja2Sarja1textBox.Text;
            this.peli.ToiseksiPisinSarja1 = this.pelidetaljiPelaaja1Sarja2textBox.Text;
            this.peli.ToiseksiPisinSarja2 = this.pelidetaljiPelaaja2Sarja2textBox.Text;
        }

        private void tallennaButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void pelidetaljiPelaaja1Sarja1textBox_TextChanged(object sender, EventArgs e)
        {
            this.pelidetaljiPelaaja1Sarja2textBox.Enabled = !string.IsNullOrEmpty(this.pelidetaljiPelaaja1Sarja1textBox.Text);
        }

        private void pelidetaljiPelaaja2Sarja1textBox_TextChanged(object sender, EventArgs e)
        {
            this.pelidetaljiPelaaja2Sarja2textBox.Enabled = !string.IsNullOrEmpty(this.pelidetaljiPelaaja2Sarja1textBox.Text);
        }
    }
}
