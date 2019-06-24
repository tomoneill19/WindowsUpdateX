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
    public partial class Alerts : Form
    {
        public Alerts()
        {
            InitializeComponent();
        }

        private void Alerts_Load(object sender, EventArgs e)
        {
            UpdateCore.Config conf = new UpdateCore.Config();

            if(conf.AlertsForErrors)
            {
                this.RECEIVE_ALERT_ERRORS.Checked = true;
            }
            else
            {
                this.RECEIVE_ALERT_ERRORS.Checked = false;
            }

            if (conf.AlertsForWarnings)
            {
                this.RECEIVE_ALERT_FOR_WARNING.Checked = true;
            }
            else
            {
                this.RECEIVE_ALERT_FOR_WARNING.Checked = false;
            }
        }

        private void SaveData()
        {
            UpdateCore.Config conf = new UpdateCore.Config();

            if (this.RECEIVE_ALERT_ERRORS.Checked == true)
            {
                conf.AlertsForErrors = true;
            }
            else
            {
                conf.AlertsForErrors = false;
            }

            if (this.RECEIVE_ALERT_FOR_WARNING.Checked == true)
            {
                conf.AlertsForWarnings = true;
            }
            else
            {
                conf.AlertsForWarnings = false;
            }

            conf.Save();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Email.Instance.IsValidated)
            {
                SaveData();
                this.Close();
            }
            else
            {
                MessageBox.Show("Can't setup alerts until email server is configured","ERROR",MessageBoxButtons.OK);
            }
        }
    }
}
