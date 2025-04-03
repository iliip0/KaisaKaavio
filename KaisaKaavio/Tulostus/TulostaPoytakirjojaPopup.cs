using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KaisaKaavio.Tulostus
{
    public partial class TulostaPoytakirjojaPopup : Form
    {
        private Kilpailu kilpailu = null;
        private Loki loki = null;
        private BindingList<Peli> pelit = new BindingList<Peli>();

        private List<Peli> tulostettavatPelit = new List<Peli>();

        private Brush mustaHarja = new SolidBrush(Color.Black);
        private Brush harmaaHarja = new SolidBrush(Color.Gray);
        private Pen mustaPaksuKyna = null;
        private Pen mustaOhutKyna = null;

        private Font isoPaksuFontti = new Font(FontFamily.GenericSansSerif, 14.0f, FontStyle.Bold);
        private Font isoOhutFontti = new Font(FontFamily.GenericSansSerif, 12.0f, FontStyle.Bold);
        private Font paksuFontti = new Font(FontFamily.GenericSansSerif, 12.0f, FontStyle.Bold);
        private Font ohutFontti = new Font(FontFamily.GenericSansSerif, 12.0f, FontStyle.Regular);
        private Font paksuPieniFontti = new Font(FontFamily.GenericSerif, 10.0f, FontStyle.Bold);
        private Font ohutPieniFontti = new Font(FontFamily.GenericSerif, 10.0f, FontStyle.Regular);

        public int TaytettyjaLappuja
        {
            get
            {
                int i = 0;
                foreach (var r in this.dataGridView1.Rows)
                {
                    try
                    {
                        DataGridViewRow row = (DataGridViewRow)r;
                        DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)row.Cells[TulostaColumn.Index];
                        if ((cell.Value != null) && ((bool)(cell.Value)))
                        {
                            i++;
                        }
                    }
                    catch
                    { 
                    }
                }
                return i;
            }
        }

        public int TyhjiaLappuja
        {
            get
            {
                return this.tyhjiaLappujaTrackBar.Value;
            }
        }

        public TulostaPoytakirjojaPopup(Kilpailu kilpailu, Loki loki)
        {
            this.kilpailu = kilpailu;
            this.loki = loki;

            this.mustaOhutKyna = new Pen(this.mustaHarja, 1.0f);
            this.mustaPaksuKyna = new Pen(this.mustaHarja, 2.0f);

            InitializeComponent();

            this.printDocument1.PrintPage += printDocument1_PrintPage;
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int w = e.PageBounds.Width;
            int h = e.PageBounds.Height;

            int lappujaX = 2;
            int lappujaY = 5;

            int lapunLeveys = w / lappujaX;
            int lapunKorkeus = h / lappujaY;

            for (int i = 0; i < lappujaX; ++i)
            {
                int x = i * lapunLeveys;

                for (int j = 0; j < lappujaY; ++j)
                {
                    if (this.tulostettavatPelit.Any())
                    {
                        var peli = this.tulostettavatPelit.First();

                        int y = j * lapunKorkeus;

                        PiirraOttelupoytakirja(e, x, y, lapunLeveys, lapunKorkeus, peli);

                        this.tulostettavatPelit.RemoveAt(0);
                    }
                }
            }

            e.HasMorePages = this.tulostettavatPelit.Any();
        }

        private void PiirraTeksti(
            System.Drawing.Printing.PrintPageEventArgs e, 
            Font font,
            Brush brush,
            string teksti, 
            int x, 
            int y, 
            int w, 
            int h, 
            HorizontalAlignment align)
        {
            e.Graphics.Clip = new System.Drawing.Region(new Rectangle(x, y, w, h));

            var koko = e.Graphics.MeasureString(teksti, font);

            if (koko.Width > w)
            {
                align = HorizontalAlignment.Left;
            }
            /*
            if (koko.Width > w)
            {
                font = this.ohutPieniFontti;
                koko = e.Graphics.MeasureString(teksti, font);
            }
             */

            switch (align)
            {
                case HorizontalAlignment.Left:
                    e.Graphics.DrawString(teksti, font, brush, (float)x, (float)y);
                    break;

                case HorizontalAlignment.Right:
                    e.Graphics.DrawString(teksti, font, brush, (float)(x + w) - koko.Width, (float)y);
                    break;

                case HorizontalAlignment.Center:
                    e.Graphics.DrawString(teksti, font, brush, (float)(x + w / 2.0f) - koko.Width / 2.0f, (float)y);
                    break;
            }

            e.Graphics.ResetClip();
        }

        private void PiirraOttelupoytakirja(System.Drawing.Printing.PrintPageEventArgs e, int x, int y, int w, int h, Peli peli)
        {
            e.Graphics.DrawRectangle(this.mustaOhutKyna, new Rectangle(x + 5, y + 5, w - 10, h - 10));

            x += 10;
            y += 10;
            w -= 20;
            h -= 20;

            // Ylärivi
            PiirraTeksti(e, this.isoPaksuFontti, this.mustaHarja, "PELIPÖYTÄKIRJA", x, y, w, h, HorizontalAlignment.Left);
            PiirraTeksti(e, this.ohutPieniFontti, this.mustaHarja, "Peli nro:", x, y, w - 30, h, HorizontalAlignment.Right);
            PiirraTeksti(e, this.isoPaksuFontti, this.mustaHarja, peli != null ? peli.PeliNumero.ToString() : "__", x, y, w, h, HorizontalAlignment.Right);

            // Tulosrivin otsikot
            int riviY = y + 26;
            int riviH = 18;
            int vs = 26;
            int tulos = 52;
            int nimi = (w - vs - 2 * tulos) / 2;

            PiirraTeksti(e, this.ohutPieniFontti, this.mustaHarja, "Pelaaja 1", x, riviY, nimi, riviH, HorizontalAlignment.Right);
            PiirraTeksti(e, this.ohutPieniFontti, this.mustaHarja, "Tulos", x + nimi, riviY, tulos, riviH, HorizontalAlignment.Center);
            PiirraTeksti(e, this.ohutPieniFontti, this.mustaHarja, "vs", x + nimi + tulos, riviY, vs, riviH, HorizontalAlignment.Center);
            PiirraTeksti(e, this.ohutPieniFontti, this.mustaHarja, "Tulos", x + nimi + tulos + vs, riviY, tulos, riviH, HorizontalAlignment.Center);
            PiirraTeksti(e, this.ohutPieniFontti, this.mustaHarja, "Pelaaja 2", x + nimi + vs + tulos + tulos + 2, riviY, nimi, riviH, HorizontalAlignment.Left);

            e.Graphics.DrawLine(this.mustaOhutKyna, new Point(x + 10, riviY + riviH), new Point(x + w - 20, riviY + riviH));

            // Tulosrivi
            riviY += 22;
            riviH = 40;

            if (peli != null)
            {
                if (this.kilpailu.KilpaSarja == KilpaSarja.MixedDoubles || this.kilpailu.KilpaSarja == KilpaSarja.Parikilpailu)
                {
                    var pelaaja1 = this.kilpailu.Osallistujat.FirstOrDefault(k => k.Id == peli.Id1);
                    var pelaaja2 = this.kilpailu.Osallistujat.FirstOrDefault(k => k.Id == peli.Id2);

                    PiirraTeksti(e, this.isoOhutFontti, this.mustaHarja, pelaaja1.Pelaajan1Nimi, x - 6, riviY - 2, nimi, riviH, HorizontalAlignment.Right);
                    PiirraTeksti(e, this.isoOhutFontti, this.mustaHarja, pelaaja1.Pelaajan2Nimi, x - 6, riviY + 18, nimi, riviH, HorizontalAlignment.Right);

                    PiirraTeksti(e, this.isoOhutFontti, this.mustaHarja, pelaaja2.Pelaajan1Nimi, x + nimi + vs + tulos + tulos + 2, riviY - 2, nimi, riviH, HorizontalAlignment.Left);
                    PiirraTeksti(e, this.isoOhutFontti, this.mustaHarja, pelaaja2.Pelaajan2Nimi, x + nimi + vs + tulos + tulos + 2, riviY + 18, nimi, riviH, HorizontalAlignment.Left);
                }
                else
                {
                    PiirraTeksti(e, this.isoOhutFontti, this.mustaHarja, peli.PelaajanNimi1, x - 6, riviY, nimi, riviH, HorizontalAlignment.Right);
                    PiirraTeksti(e, this.ohutPieniFontti, this.mustaHarja, peli.Seura1, x - 3, riviY + 20, nimi, riviH, HorizontalAlignment.Right);

                    PiirraTeksti(e, this.isoOhutFontti, this.mustaHarja, peli.PelaajanNimi2, x + nimi + vs + tulos + tulos + 2, riviY, nimi, riviH, HorizontalAlignment.Left);
                    PiirraTeksti(e, this.ohutPieniFontti, this.mustaHarja, peli.Seura2, x + nimi + vs + tulos + tulos + 2, riviY + 20, nimi, riviH, HorizontalAlignment.Left);
                }
            }
            PiirraTeksti(e, this.isoOhutFontti, this.mustaHarja, "-", x + nimi + tulos, riviY, vs, riviH, HorizontalAlignment.Center);

            e.Graphics.DrawLine(this.mustaOhutKyna, new Point(x + nimi, riviY - 30), new Point(x + nimi, riviY + riviH));
            e.Graphics.DrawLine(this.mustaOhutKyna, new Point(x + nimi + tulos, riviY - 30), new Point(x + nimi + tulos, riviY + riviH));
            e.Graphics.DrawLine(this.mustaOhutKyna, new Point(x + nimi + tulos + vs, riviY - 30), new Point(x + nimi + tulos + vs, riviY + riviH));
            e.Graphics.DrawLine(this.mustaOhutKyna, new Point(x + nimi + tulos + vs + tulos, riviY - 30), new Point(x + nimi + tulos + vs + tulos, riviY + riviH));

            e.Graphics.DrawLine(this.mustaOhutKyna, new Point(x + 10, riviY + riviH), new Point(x + w - 20, riviY + riviH));

            // Alarivi
            riviY = y + h - 22;
            PiirraTeksti(e, this.ohutPieniFontti, this.mustaHarja, "Pisin sarja/nimi:", x, riviY, w, riviH, HorizontalAlignment.Left);
            PiirraTeksti(e, this.ohutPieniFontti, this.mustaHarja, "Kierros:", x, riviY, w - 30, riviH, HorizontalAlignment.Right);
            PiirraTeksti(e, this.isoPaksuFontti, this.mustaHarja, peli != null ? peli.Kierros.ToString() : "__", x, riviY, w, riviH, HorizontalAlignment.Right);

            // Pöytä numero
            int r = 26;
            e.Graphics.DrawEllipse(this.mustaOhutKyna, new Rectangle(x + w / 2 - r, y + h - r * 2, 2 * r, 2 * r));
            PiirraTeksti(e, this.ohutPieniFontti, this.mustaHarja, "Pöytä", x + w / 2 - r, y + h - r + 2, 2 * r, 2 * r, HorizontalAlignment.Center);
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.tulostettavatPelit.Clear();

            foreach (var r in this.dataGridView1.Rows)
            {
                try
                {
                    DataGridViewRow row = (DataGridViewRow)r;
                    DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)row.Cells[TulostaColumn.Index];
                    if ((cell.Value != null) && ((bool)(cell.Value)))
                    {
                        Peli peli = (Peli)row.DataBoundItem;
                        tulostettavatPelit.Add(peli);
                    }                    
                }
                catch
                {
                }
            }

            for (int i = 0; i < TyhjiaLappuja; ++i)
            {
                tulostettavatPelit.Add(null);
            }

            this.printPreviewDialog1.ClientSize = new System.Drawing.Size(800, 600);
            this.printPreviewDialog1.Document.DefaultPageSettings.Landscape = true;
            this.printPreviewDialog1.UseAntiAlias = true;
            this.printPreviewDialog1.TopLevel = true;

#if DEBUG
            var result = printPreviewDialog1.ShowDialog();
#else
            this.printDocument1.Print();
#endif

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void peruutaButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void TulostaPoytakirjojaPopup_Load(object sender, EventArgs e)
        {
            foreach (var peli in this.kilpailu.Pelit.Where(x => 
                x.Tilanne == PelinTilanne.Tyhja ||
                x.Tilanne == PelinTilanne.ValmiinaAlkamaan ||
                x.Tilanne == PelinTilanne.Kaynnissa))
            {
                this.pelit.Add(peli);
            }

            this.peliBindingSource.DataSource = this.pelit;
        }

        private void tyhjiaLappujaTrackBar_Scroll(object sender, EventArgs e)
        {
            PaivitaLuvut();
        }

        private void PaivitaLuvut()
        {
            try
            {
                int tyhjia = TyhjiaLappuja;
                int taytettyja = TaytettyjaLappuja;

                this.tyhjiaLappujaTextBox.Text = tyhjia.ToString();
                this.taytettyjaLappujaTextBox.Text = taytettyja.ToString();
                this.lappujaTextBox.Text = (tyhjia + taytettyja).ToString();
                this.sivujaTextBox.Text = ((int)(Math.Ceiling((tyhjia + taytettyja) / 8.0))).ToString();

                this.okButton.Enabled = tyhjia + taytettyja > 0;
            }
            catch (Exception ex)
            {
                if (this.loki != null)
                {
                    this.loki.Kirjoita("Lukujen päivitys pöytäkirjoja tulostettaessa epäonnistui", ex);
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == TulostaColumn.Index)
            {
                this.label1.Focus();
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (this.Visible)
            {
                PaivitaLuvut();
            }
        }

        private void TulostaPoytakirjojaPopup_Shown(object sender, EventArgs e)
        {
            foreach (var r in this.dataGridView1.Rows)
            {
                try
                {
                    DataGridViewRow row = (DataGridViewRow)r;
                    DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)row.Cells[TulostaColumn.Index];
                    Peli peli = (Peli)row.DataBoundItem;
                    cell.Value = peli.Tilanne != PelinTilanne.Kaynnissa;
                }
                catch
                {
                }
            }
        }
    }
}
