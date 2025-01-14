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
    public partial class SalinTiedotPopup : Form
    {
        private SalinTiedotPanel panel = new SalinTiedotPanel();
        private Sali sali = null;

        public SalinTiedotPopup(Sali sali)
        {
            this.sali = sali;

            InitializeComponent();

            this.panel.toimitsijaBindingSource.DataSource = this.sali.Toimitsijat;
            this.panel.poytaBindingSource.DataSource = this.sali.Poydat;
            this.panel.linkkiBindingSource.DataSource = this.sali.Linkit;
            this.panel.saliBindingSource.DataSource = this.sali;

            this.Controls.Add(panel);
        }
    }
}
