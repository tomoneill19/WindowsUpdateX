using System;
using System.Collections.Generic;
using UpdateCore;


using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class UnitTests
    {
        // Data for the tests - will need adjusting from time to time manually ad apps are installed etc
        static double TOTAL_RAM = 15.8883;
        static double MAX_DISK = 463.7314;
        static int NUMBER_INSTALLED_APPLICATIONS = 418;

        int count = 0;

        [TestMethod]
        public void TestBasicHardwareReadWrite()
        {

            Init();

            // This tests that 5 rows are written returned and 2 fixed columns are what they should be
            Hardware h = new Hardware();

            // Generate some data rows to test

            //Clear down the table
            Database db = new Database();
            db.Write("Truncate Table dbo.Hardware");

            for (int i = 0; i < 30; i++)
            {
                h.Refresh();
                System.Threading.Thread.Sleep(1000);   // wait for a second and sleep
            }

            List<Hardware> HardwareList = Hardware.getHistory();

            count = 0;
            foreach (Hardware hware in HardwareList)
            {
                count++;

                //Skip first rows as it is 0 for max disk (need to identify why that is?)
                if (count > 1)
                {
                    Assert.AreEqual(Math.Round(hware.totalRam, 2), Math.Round(TOTAL_RAM, 2));
                    Assert.AreEqual(Math.Round(hware.maxDisk, 2), Math.Round(MAX_DISK, 2));
                }

            }
            Assert.AreEqual(HardwareList.Count,30);
        }

        [TestMethod]
        public void TestBasicApplicationReadWrite()
        {
            Init();

            //Clear down the table
            Database db = new Database();
            db.Write("Truncate Table dbo.Application");

            // Pull and save any new ones
            Application.Refresh();
            List<Application> ApplicationList = Application.GetHistory();

            Assert.AreEqual(ApplicationList.Count, NUMBER_INSTALLED_APPLICATIONS);
        }

        [TestMethod]
        public void TestBasicUpdateReadWrite()
        {
            Init();

            //Clear down the table
            Database db = new Database();
            //db.Write("Truncate Table [dbo].[Update]");

            // Pull and save any new ones
            Update.Refresh();

            Update u = new Update("2018-03 Security Update for Adobe Flash Player for Windows 10 Version 1709 for x64-based Systems (KB4088785)");
            u.downloadUpdate();
            u.installUpdate();

            //List<Update> UpdateList = Update.GetHistory();

            //Assert.AreEqual(UpdateList.Count, NUMBER_INSTALLED_APPLICATIONS);
        }

        [TestMethod]
        public void TestEmailSend()
        {

            Init();    // For stand alone unit testing

            Database db = new Database();
            string SQL = "SELECT SmtpHost, SmtpPort, FromEmailAddress, FromEmailDisplayName, FromPassword, DefaultSendEmail FROM dbo.[CONFIG]";
            List<string> rows = db.Read(SQL, 6);
            string[] cols = rows[0].Split(new Database().COLUMN_DELIMETER);

            UpdateCore.Email email = UpdateCore.Email.Instance;
            email.SmtpHost = cols[0];
            email.setSmtpPort(Int32.Parse(cols[1]));
            email.FromEmailAddress = cols[2];
            email.FromEmailDisplayName = cols[3];
            email.FromPassword = cols[4];
            email.SendEmail(cols[5], "This is a test email", "The body of this email is blank");
        }

        [TestMethod]
        public void TestAnalytics()
        {
            Init();

            for (int i = 0; i < 10; i++)
            {
                Analytics.DoAnalysis();  // Loop to se it we get info logged
            }
   
        }

        private void Init()
        {
            UpdateCore.Logger.SetLogToDatabase(true);
            UpdateCore.Logger.setLogToTextFile(false);
            UpdateCore.Logger.setDatabaseConnection((new UpdateCore.Database()).getConnection());
            UpdateCore.Logger.setLoggingLevel(Logger.LogLevels.DEBUG);
        }

        [TestMethod]
        public void TestEnvironment()
        {

            Init();    // For stand alone unit testing

            Database db = new Database();
            string SQL = "IF EXISTS(select * from sys.databases where name='WindowsUpdates') BEGIN ALTER DATABASE [WindowsUpdates] SET SINGLE_USER WITH ROLLBACK IMMEDIATE DROP DATABASE WindowsUpdates END GO CREATE DATABASE WindowsUpdates GO";
            db.Write(SQL);
        }

    }
}
