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
    public partial class EmailForm : Form
    {
        public EmailForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                SaveData();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            } 
        }

        private void EmailForm_Load(object sender, EventArgs e)
        {
            Email em = new Email();

            this.SMTP_HOST_NAME.Text = em.SmtpHost;
            this.SMTP_PORT.Text = em.getSmtpPort().ToString();
            this.FROM_EMAIL.Text = em.FromEmailAddress;
            this.FROM_DISPLAY_NAME.Text = em.FromEmailDisplayName;
            this.FROM_PASSWORD.Text = em.FromPassword;
            this.DEFAULT_SEND_EMAIL.Text = em.getSendEmail();
        }


        private void SaveData()
        {
            UpdateCore.Email e = UpdateCore.Email.Instance;

            int Output = 0;
            if (!Int32.TryParse(this.SMTP_PORT.Text, out Output))
            {
                throw new Exception("The port must a positive integer.  Sorry but I can't save this data until it is changed");
            }
            else
            {
                e.SmtpHost = this.SMTP_HOST_NAME.Text;
                e.setSmtpPort(Int32.Parse(this.SMTP_PORT.Text));
                e.FromEmailAddress = this.FROM_EMAIL.Text;
                e.FromEmailDisplayName = this.FROM_DISPLAY_NAME.Text;
                e.FromPassword = this.FROM_PASSWORD.Text;
                e.setSendEmail(this.DEFAULT_SEND_EMAIL.Text);

                e.SaveConfig();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                SaveData();
                UpdateCore.Email em = UpdateCore.Email.Instance;
                bool sucess = em.SendEmail("Test Email", "This is a test email to check that your email is configured corectly for the Windows Update application.  Check your inbox and if you received this email then all is ok");
                SaveData();
                MessageBox.Show("An email has been sent to your inbox as a test");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
