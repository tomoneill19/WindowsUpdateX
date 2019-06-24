using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;
using UpdateCore;
using System.Globalization;

namespace UpdateUI
{
    public partial class UI : Form
    {
        private static String DEFAULT_URL = "https://technet.microsoft.com/en-gb/ms772425.aspx";   // Replay with Tom website or product page

        public UI()
        {
            InitializeComponent();
            initUI();

            Task task = new Task(() => this.webBrowser1.Navigate(DEFAULT_URL));
            task.Start();
        }

        private void initUI()
        {
            tempInit();    // XXXX REMOVE

            List<Update> UpdateList = UpdateCore.Update.GetHistory();

            try
            {
                DataTable up = new DataTable("Updates");
                up.Columns.Add("Name");
                up.Columns.Add("KBID");
                up.Columns.Add("URL");
                up.Columns.Add("Description");
                up.Columns.Add("isDownloaded");
                up.Columns.Add("isInstalled");
                up.Columns.Add("neededDiskSpace");
                up.Columns.Add("DateTimeCreated");
                up.Columns.Add("DateTimeInstalled");
                up.Columns.Add("IssueFlag");
                foreach (Update u in UpdateList)
                {
                    DataRow row = up.NewRow();
                    row["Name"] = u.Name;
                    row["KBID"] = u.KBID;
                    row["URL"] = u.url;
                    row["Description"] = u.Description;
                    row["isDownloaded"] = u.isDownloaded.ToString();
                    row["isInstalled"] = u.isInstalled.ToString();
                    row["neededDiskSpace"] = u.neededDiskSpace.ToString();
                    row["DateTimeCreated"] = Utils.getDisplayDate(u.DateTimeCreated);
                    row["DateTimeInstalled"] = Utils.getDisplayDate(u.DateTimeInstalled);
                    row["IssueFlag"] = u.IssueFlag.ToString();
                    
                    up.Rows.Add(row);
                }
                dataGridView1.DataSource = up;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdateCore.Hardware hardware = new UpdateCore.Hardware();
            hardware.Refresh();
            UpdateCore.Hardware.getDailyHistory();
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void tempInit()
        {
            UpdateCore.Logger.SetLogToDatabase(true);
            UpdateCore.Logger.setLogToTextFile(false);
            UpdateCore.Logger.setDatabaseConnection(new Database().getConnection());
            UpdateCore.Logger.setLoggingLevel(Logger.LogLevels.DEBUG);
            UpdateCore.Logger.SetLogDir(Directory.GetCurrentDirectory());
        }

        private void toolStripButtonRefresh_Click(object sender, EventArgs e)
        {
            initUI();
        }

 
        // On select update
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            tempInit();    // XXXX REMOVE

            Update selUpdate = null;
            String KBID = "";
            String Name = "";
            if (dataGridView1.SelectedCells.Count > 0)
            {
                int selectedrowindex = dataGridView1.SelectedCells[0].RowIndex;
                DataGridViewRow selectedRow = dataGridView1.Rows[selectedrowindex];
                KBID = Convert.ToString(selectedRow.Cells["KBID"].Value);
                Name = Convert.ToString(selectedRow.Cells["Name"].Value);
                selUpdate = new Update(Name);
            }

            // do this first in a new thread so it gets built at the same time as the combos
            Task task = new Task(() => ShowWebBrowser(selUpdate));
            task.Start();

            List<Hardware> HardwareList = UpdateCore.Hardware.getDailyHistory(selUpdate, new Config().GetDaysEitherSideUpdateToDisplay());
            foreach (Hardware h in HardwareList)
            {
                this.Used_CPU.Series["Hardware History"].Points.AddXY(h.LogDate.ToString("dd MMM"), h.usedCpu.ToString());
                this.Avail_RAM.Series["Hardware History"].Points.AddXY(h.LogDate.ToString("dd MMM"), h.getUsedPercentageRam().ToString());
                this.Avail_DISK.Series["Hardware History"].Points.AddXY(h.LogDate.ToString("dd MMM"), h.GetUsedPercentageDiskSpace().ToString());
                this.Disk_READ.Series["Hardware History"].Points.AddXY(h.LogDate.ToString("dd MMM"), h.diskRead.ToString());
                this.Disk_WRITE.Series["Hardware History"].Points.AddXY(h.LogDate.ToString("dd MMM"), h.diskWrite.ToString());
            }

            // Perhaps set some defaults later
            this.Used_CPU.ChartAreas[0].AxisY.Maximum = 100;
            this.Used_CPU.ChartAreas[0].AxisY.Minimum = 0;
            this.Used_CPU.ChartAreas[0].AxisX.LabelStyle.Angle = 45;

            this.Avail_RAM.ChartAreas[0].AxisY.Maximum = 100;
            this.Avail_RAM.ChartAreas[0].AxisY.Minimum = 0;
            this.Avail_RAM.ChartAreas[0].AxisX.LabelStyle.Angle = 45;

            this.Avail_DISK.ChartAreas[0].AxisY.Maximum = 100;
            this.Avail_DISK.ChartAreas[0].AxisY.Minimum = 0;
            this.Avail_DISK.ChartAreas[0].AxisX.LabelStyle.Angle = 45;

            //this.Disk_READ.ChartAreas[0].AxisY.Maximum = 55;
            //this.Disk_READ.ChartAreas[0].AxisY.Minimum = 0;
            //this.Disk_READ.ChartAreas[0].AxisX.LabelStyle.Angle = 45;

            //this.Disk_WRITE.ChartAreas[0].AxisY.Maximum = 55;
            //this.Disk_WRITE.ChartAreas[0].AxisY.Minimum = 0;
            //this.Disk_WRITE.ChartAreas[0].AxisX.LabelStyle.Angle = 45;

            // Now display Apps
            List<UpdateCore.Application> ApplicationList = UpdateCore.Application.GetHistory(selUpdate, new Config().GetDaysEitherSideUpdateToDisplay());

            DataTable app = new DataTable("Applications");
            app.Columns.Add("Name");
            app.Columns.Add("Description");
            app.Columns.Add("Version");
            app.Columns.Add("InstallDate");
           
            foreach (UpdateCore.Application a in ApplicationList)
            {
                DataRow row = app.NewRow();
                row["Name"] = a.Name;
                row["Description"] = a.Description;
                row["Version"] = a.Version;
                row["InstallDate"] = Utils.getDisplayDate(a.InstallDate);
                app.Rows.Add(row);
            }

            dataGridView2.DataSource = app;

            // Now show the insights
            List<UpdateCore.Insight> InsightList = UpdateCore.Insight.GetHistory(selUpdate, new Config().GetDaysEitherSideUpdateToDisplay());

            DataTable ins = new DataTable("Insight");
            ins.Columns.Add("Description");
            ins.Columns.Add("DateLogged");

            foreach (UpdateCore.Insight i in InsightList)
            {
                DataRow row = ins.NewRow();
                row["Description"] = i.InsightText;
                row["DateLogged"] = Utils.getDisplayDate(i.DateLogged);
                ins.Rows.Add(row);
            }

            dataGridView3.DataSource = ins;
        }

        private void ShowWebBrowser(Update up)
        {
            // Now display the KB Article
            this.webBrowser1.Navigate(up.url);
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        protected void dataGridView1_RowDataBound(object sender, DataGridViewRowEventArgs e)
        {
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];
            if (dgv.Columns.Contains("Name") && dgv.Columns["Name"].Visible)
            {
                string  Name = Convert.ToString(selectedRow.Cells["Name"].Value);

                if (!String.IsNullOrEmpty(Name))
                {
                    Update selUpdate = new Update(Name);

                    if (selUpdate.IssueFlag == true)
                    {
                        dgv.Rows[e.RowIndex].DefaultCellStyle.Font = new Font("Tahoma", 8, FontStyle.Regular);
                        e.CellStyle.ForeColor = Color.Red;
                    }
                    else
                    {
                        // Perhaps set a default later
                    }
                 }
             }
        }

        private void dataGridView1_Load(object sender, DataGridViewRowEventArgs e)
        {
        }

        private void configureSupportEmailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EmailForm ef = new EmailForm();
            ef.Show();
        }

        private void setEmailAlertsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Alerts al = new Alerts();
            al.Show();
        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView3_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void technicalSupportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ContactUs cu = new ContactUs("Technical Support");
            cu.Show();
        }

        private void aboutProgramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutUs au = new AboutUs();
            au.Show();
        }

        private void ideasFeedbackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ContactUs cu = new ContactUs("Ideas/Feedback");
            cu.Show();
        }

        private void setDefaultsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Format f = new Format();
            f.Show();
        }
    }
}