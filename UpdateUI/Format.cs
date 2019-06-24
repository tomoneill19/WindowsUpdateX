using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UpdateCore;

namespace UpdateUI
{
    public partial class Format : Form
    {
        public Format()
        {
            InitializeComponent();
        }

        private void Format_Load(object sender, EventArgs e)
        {
            try
            {
                Config c = new Config();
                this.DAYS_TO_DISPLAY.Text = c.GetDaysEitherSideUpdateToDisplay().ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Config c = new Config();

                int Output = 0;
                if (!Int32.TryParse(this.DAYS_TO_DISPLAY.Text, out Output))
                {
                    throw new Exception("The Days Either Side Update To Display must be a positive integer");
                }

                c.setDaysEitherSideUpdateToDisplay(Int32.Parse(this.DAYS_TO_DISPLAY.Text));
                c.Save();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
