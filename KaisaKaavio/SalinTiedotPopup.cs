using System.Windows.Forms;

namespace KaisaKaavio
{
    public partial class SalinTiedotPopup : Form
    {
        private SalinTiedotPanel panel = new SalinTiedotPanel();
        private Sali sali = null;
        private bool kaavioArvottu = false;

        public SalinTiedotPopup(Sali sali, bool kaavioArvottu)
        {
            this.sali = sali;
            this.sali.VarmistaAinakinYksiPoyta();
            this.kaavioArvottu = kaavioArvottu;

            InitializeComponent();

            this.panel.toimitsijaBindingSource.DataSource = this.sali.Toimitsijat;
            this.panel.poytaBindingSource.DataSource = this.sali.Poydat;
            this.panel.linkkiBindingSource.DataSource = this.sali.Linkit;
            this.panel.saliBindingSource.DataSource = this.sali;
            this.panel.Dock = DockStyle.Fill;

            this.Controls.Add(panel);

            if (this.kaavioArvottu)
            {
                this.panel.Lukitse();
            }
        }

        private void SalinTiedotPopup_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!this.kaavioArvottu)
            {
                this.sali.VarmistaAinakinYksiPoyta();
            }
        }

        private void SalinTiedotPopup_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.panel.EndEditing();
        }
    }
}
