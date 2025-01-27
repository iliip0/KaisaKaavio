using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KaisaKaavio.Kayttoliittyma
{
    /// <summary>
    /// Työkalu joka muokkaa/mukauttaa annetun UI elementin käyttöliittymää ja
    /// grafiikkaa
    /// </summary>
    public class Mukauttaja
    {

        public static void Mukauta(Control control)
        {
            //using (new Testaus.Profileri("Mukauttaja.Mukauta"))
            {
                if (control is ComboBox)
                {
                    MukautaComboBox((ComboBox)control);
                }

                if (control is TextBox)
                {
                    MukautaTextBox((TextBox)control);
                }

                foreach (var child in control.Controls)
                {
                    if (child is Control)
                    {
                        try
                        {
                            Mukauta((Control)child);
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        public static void MukautaTextBox(TextBox textBox)
        {
            //textBox.KeyPress += textBox_KeyPress;
           // textBox.PreviewKeyDown += textBox_PreviewKeyDown;
           //textBox.KeyDown += textBox_KeyDown;
           //textBox.KeyUp += textBox_KeyUp;
            //textBox.AcceptsReturn = true;
            //textBox.
            // Tämä aiheuttaa sen, että comboboxin "databinding" päivittyy heti kun comboboxin valinta vaihtuu
            //comboBox.SelectionChangeCommitted += comboBox_SelectionChangeCommitted;

            //comboBox.Paint += comboBox_Paint;
            //comboBox.DrawMode = DrawMode.OwnerDrawFixed;
            //comboBox.DrawItem += comboBox_DrawItem;
        }

        static void textBox_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;
                }
            }
            catch
            {
            }
        }

        static void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;
                    //((TextBox)sender).
                    //((TextBox)sender).Enabled = true;
                }
            }
            catch
            { 
            }
        }

        static void textBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter)
            //{
            //    ((TextBox)sender).;
            //}
        }

        static void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
           // e.
            //throw new NotImplementedException();
        }

        public static void MukautaComboBox(ComboBox comboBox)
        {
            // Tämä aiheuttaa sen, että comboboxin "databinding" päivittyy heti kun comboboxin valinta vaihtuu
            comboBox.SelectionChangeCommitted += comboBox_SelectionChangeCommitted;

            //comboBox.Paint += comboBox_Paint;
            //comboBox.DrawMode = DrawMode.OwnerDrawFixed;
            //comboBox.DrawItem += comboBox_DrawItem;
        }

        static void comboBox_Paint(object sender, PaintEventArgs e)
        {
            //var rect = e.ClipRectangle;

            //var brush = new SolidBrush(Color.Green);
            //var pen = new Pen(brush, 2.0f);
            //e.Graphics.DrawRectangle(pen, rect);
        }

        static void comboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            //e.DrawBackground();

            /*
            switch (e.State)
            { 
                case DrawItemState.ComboBoxEdit:
                    e.BackColor = Color.Red;
                    e.ForeColor = Color.Yellow;
                    break;

                default:
                    e.BackColor = Color.White;
                    e.ForeColor = Color.Black;
                    break;
            }
             */
        }

        static void comboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                ((Control)sender).Parent.Focus();
            }
            catch
            { 
            }
        }
    }
}
