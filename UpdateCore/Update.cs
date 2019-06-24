using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WUApiLib;
using System.Management;
using System.Windows.Forms;

namespace UpdateCore
{
    public class Update : Software
    {
        //Properties
        public string Name;
        public string KBID;
        public string Description;
        public bool isDownloaded;
        public bool isInstalled;
        public string url;
        public string neededDiskSpace;
        public DateTime DateTimeCreated;
        public DateTime DateTimeDownloaded;
        public DateTime DateTimeInstalled;
        public DateTime DateTimeRemoved;
        public Boolean IssueFlag;
        public String Identity;


        // Constructor for a new update that hasn't been assigned Install dates etc yet.
        public Update(string Name, string KBID, string Description, bool isDownloaded, string url, string neededDiskSpace, Boolean isInstalled, String Identity)
        {
            Logger.instance.Debug("Creating update");
            this.Name = Name;
            this.KBID = KBID; //Primary Key - Clustered Index
            this.Description = Description;
            this.isDownloaded = isDownloaded;
            this.url = url;
            this.neededDiskSpace = neededDiskSpace;
            this.isInstalled = isInstalled;
            this.Identity = Identity;
        }

        // Constructor for an update that has been pulled from the database with all of it's information.
        public Update(string Name, string KBID, string Description, bool isDownloaded, bool isInstalled, string url, string neededDiskSpace, DateTime DateTimeCreated, DateTime DateTimeDownloaded, DateTime DateTimeInstalled, DateTime DateTimeRemoved,Boolean IssueFlag, String Identity)
        {
            Logger.instance.Debug("Creating update");
            this.Name = Name;
            this.KBID = KBID;
            this.Description = Description;
            this.isDownloaded = isDownloaded;
            this.isInstalled = isInstalled;
            this.url = url;
            this.neededDiskSpace = neededDiskSpace;
            this.DateTimeCreated = DateTimeCreated;
            this.DateTimeDownloaded = DateTimeDownloaded;
            this.DateTimeInstalled = DateTimeInstalled;
            this.DateTimeRemoved = DateTimeRemoved;
            this.IssueFlag = IssueFlag;
            this.Identity = Identity;
        }

        // Constructor for when grabbing the top update name off of the DB without the other info being needed
        public Update(string Name)
        {
            Logger.instance.Debug("Updating from database");
            this.Name = Name;
            populate();
        }


        // Method for using WUAPI to download the named update - "Windows Update" service needs turning on before calling this method
        public void downloadUpdate()
        {
            try
            {
                Logger.instance.Debug("Downloading update");
                IUpdateSession uSession = new UpdateSession();

                Logger.instance.Debug("Creating searcher");
                IUpdateSearcher uSearcher = uSession.CreateUpdateSearcher(); // Search for available Updates

                Logger.instance.Debug("Creating downloader");
                UpdateDownloader downloader = uSession.CreateUpdateDownloader(); //Create a downloader

                Logger.instance.Debug("Creating holder for updates");
                UpdateCollection updatesToDownload = new UpdateCollection();

                Logger.instance.Debug("Searcing for updates");
                ISearchResult uResult = uSearcher.Search("Type='Software' And IsAssigned = 1"); //Save the result of the search

                for (int i = 0; i < uResult.Updates.Count; i++)
                {
                    if (uResult.Updates[i].Title.ToString().Equals(this.Name))
                    {
                        updatesToDownload.Add(uResult.Updates[i]);   //save all the queued updates to the downloader queue
                        break;
                    }
                }

                if (updatesToDownload.Count > 0)
                {
                    downloader.Updates = updatesToDownload;
                    downloader.Download(); //download queued updates (should be just one)

                    // If fails, will need to check here
                    this.isDownloaded = true;
                    this.DateTimeDownloaded = DateTime.UtcNow;

                    Logger.instance.Debug("Saving update");
                    save();   // Save the latest information
                }
            }
            catch(Exception e)
            {
                Logger.instance.Error(e);
            }
        }

        // Method for using WUAPI to install the named update - "Windows Update" service needs turning on before calling this method
        public void installUpdate()
        {
            try
            {
                Logger.instance.Debug("Installing update");
                List<Update> updates = new List<Update>();

                IUpdateSession uSession;
                IUpdateSearcher uSearcher;
                uSession = new UpdateSession(); //Create an Update interface in WUA

                Logger.instance.Debug("Creating searcher");
                uSearcher = uSession.CreateUpdateSearcher(); // Search for available Updates

                Logger.instance.Debug("Creating holder for updates");
                UpdateCollection updatesToDownload = new UpdateCollection();

                Logger.instance.Debug("Searching for specific update");
                ISearchResult uResult = uSearcher.Search("Type='Software' And IsAssigned = 1"); //Save the result of the search

                Logger.instance.Debug("Creating update collection");
                UpdateCollection updatesToInstall = new UpdateCollection();

                for (int i = 0; i < uResult.Updates.Count; i++)
                {
                    if (uResult.Updates[i].Title.ToString().Equals(this.Name))
                    {
                        updatesToInstall.Add(uResult.Updates[i]);   //save all the queued updates to the downloader queue
                        break;
                    }
                }

                if (updatesToInstall.Count > 0)
                {
                    Logger.instance.Debug("Creating update installer");
                    IUpdateInstaller installer = uSession.CreateUpdateInstaller(); //create an interface for installation of updates
                    installer.Updates = updatesToInstall;

                    Logger.instance.Debug("Installing update");
                    IInstallationResult installationRes = installer.Install();

                    if (installationRes.GetUpdateResult(0).HResult == 0) // 0 (zero) code means succeeded
                    {
                        this.isInstalled = true;
                        this.DateTimeInstalled = DateTime.UtcNow;
                        save();
                    }
                    else  // For status codes other than 0 (zero)
                    {
                        String result = "Failed to install update " + Name + ".  It returned windows update error code = " + installationRes.GetUpdateResult(0).HResult.ToString();
                        Insight ins = new Insight();
                        ins.InsightText = result;
                        ins.Save();

                        Logger.instance.Warning(result);
                    }

                }
            }
            catch (Exception e)
            {
                Logger.instance.Error(e);
            }
        }

        protected override void populate() // Pull all the data about the named update from the database
        {
            Logger.instance.Debug("Populating update");
            Database db = new Database();
            string sql = "Select Name, KBID, Description, isDownloaded, url, neededDiskSpace, isInstalled, DateTimeCreated, DateTimeDownloaded, DateTimeInstalled, DateTimeRemoved, IssueFlag, [Identity] from [dbo].[Update] Where Name = '" + Name + "'";

            Logger.instance.Debug("Reading update fom database");
            List<string> rows = db.Read(sql, 13);
            if (rows.Count > 0)
            {
                string[] cols = rows[0].Split(new Database().COLUMN_DELIMETER);

                this.Name = cols[0];  //Primary Key - Clustered Index
                this.KBID = cols[1];
                this.Description = cols[2];
                this.isDownloaded = Utils.FormatBoolean(cols[3]);
                this.url = cols[4];
                this.neededDiskSpace = cols[5];
                this.isInstalled = Utils.FormatBoolean(cols[6]);
                this.DateTimeCreated = Utils.FormatDateTime(cols[7]);
                this.DateTimeDownloaded = Utils.FormatDateTime(cols[8]);
                this.DateTimeInstalled = Utils.FormatDateTime(cols[9]);
                this.DateTimeRemoved = Utils.FormatDateTime(cols[10]);
                this.IssueFlag = Utils.FormatBoolean(cols[11]);
                this.Identity = cols[12];
            }
        }

        protected override void save()  //Save to database with Upsert
        {
            Logger.instance.Debug("Saving update to database");
            Database db = new Database();
            string sql = "";
            sql += "IF NOT EXISTS(SELECT * FROM [dbo].[Update] WHERE Name = '" + Name + "')" + Environment.NewLine;
            sql += "Insert into [dbo].[Update] (Name, KBID, Description, IsDownloaded, IsInstalled, Url, NeededDiskSpace, [Identity], DateTimeDownloaded, DateTimeInstalled) values ('" + Name + "','" + KBID + "','" + Utils.EscapeQuotes(Description) + "'," + Utils.FormatBit(isDownloaded) + "," + Utils.FormatBit(isInstalled) + ",'" + url + "','" + neededDiskSpace + "','" + Identity + "','" + Utils.FormatDateTimeDB(DateTimeDownloaded) + "','" + Utils.FormatDateTimeDB(DateTimeInstalled) + "')" + Environment.NewLine;
            sql += "ELSE" + Environment.NewLine;
            sql += "UPDATE [dbo].[Update]" + Environment.NewLine;
            sql += "SET Name = '" + Name + "', Description = '" + Utils.EscapeQuotes(Description) + "', IsDownloaded = " + Utils.FormatBit(isDownloaded) + ", IsInstalled = " + Utils.FormatBit(isInstalled) + ", Url = '" + url + "', NeededDiskSpace = '" + neededDiskSpace + "',[Identity]= '" + Identity + "', DateTimeDownloaded = '" + Utils.FormatDateTimeDB(DateTimeDownloaded) + "', DateTimeInstalled = '" + Utils.FormatDateTimeDB(DateTimeInstalled) + "'" + Environment.NewLine;
            sql += "WHERE Name = '" + Name + "'";
            db.Write(sql);
        }

        public static List<Update> GetHistory()  //Load informnation about the update from the database
        {
            Logger.instance.Debug("Getting update history");
            Database db = new Database();

            Logger.instance.Debug("Reading update hsitory from database");
            List<string> rows = db.Read("Select Name, KBID, Description, isDownloaded, url, neededDiskSpace, isInstalled,  DateTimeCreated, DateTimeDownloaded, DateTimeInstalled, DateTimeRemoved, IssueFlag, [Identity] From [dbo].[Update]", 13);
            List<Update> UpdateList = new List<Update>();

            Logger.instance.Debug("Iterating through updates");
            foreach (string row in rows)
            {
                string[] cols = row.Split(new Database().COLUMN_DELIMETER);
                String Name = cols[0];  //Primary Key - Clustered Index
                String KBID = cols[1];
                String Description = cols[2];
                Boolean isDownloaded = Utils.FormatBoolean(cols[3]);
                String url = cols[4];
                String neededDiskSpace = cols[5];
                bool isInstalled = Utils.FormatBoolean(cols[6]);
                DateTime DateTimeCreated = Utils.FormatDateTime(cols[7]);
                DateTime DateTimeDownloaded = Utils.FormatDateTime(cols[8]);
                DateTime DateTimeInstalled = Utils.FormatDateTime(cols[9]);
                DateTime DateTimeRemoved = Utils.FormatDateTime(cols[10]);
                Boolean IssueFlag = Utils.FormatBoolean(cols[11]);
                String Identity = cols[12];

                Update u = new Update(Name, KBID, Description, isDownloaded, isInstalled, url, neededDiskSpace, DateTimeCreated, DateTimeDownloaded, DateTimeInstalled, DateTimeRemoved,IssueFlag, Identity);
                UpdateList.Add(u);
            }
            return UpdateList;
        }

        public static Update getLatestUpdate() //Find the update most recently added to the DB
        {
            List<Update> UpdateList = RunUpdateQuery("Select Name, KBID, Description, isDownloaded, url, neededDiskSpace, isInstalled,  DateTimeCreated, DateTimeDownloaded, DateTimeInstalled, DateTimeRemoved, IssueFlag, Identity From [dbo].[Update] U Inner Join (Select max([UID]) As MAXUID from[dbo].[Update]) MX On U.UID = MX.MAXUID");
            return UpdateList[0];
        }

        private static List<Update> RunUpdateQuery(String SQL) //Static method to get a list of all the updates in the updates table
        {
            Database db = new Database();
            Logger.instance.Debug("Reading update history from database");
            List<string> rows = db.Read(SQL, 12);
            List<Update> UpdateList = new List<Update>();

            Logger.instance.Debug("Iterating through updates");
            foreach (string row in rows)
            {
                String[] cols = row.Split(new Database().COLUMN_DELIMETER);
                String Name = cols[0];  //Primary Key - Clustered Index
                String KBID = cols[1];
                String Description = cols[2];
                Boolean isDownloaded = Utils.FormatBoolean(cols[3]);
                String url = cols[4];
                String neededDiskSpace = cols[5];
                Boolean isInstalled = Utils.FormatBoolean(cols[6]);
                DateTime DateTimeCreated = Utils.FormatDateTime(cols[7]);
                DateTime DateTimeDownloaded = Utils.FormatDateTime(cols[8]);
                DateTime DateTimeInstalled = Utils.FormatDateTime(cols[9]);
                DateTime DateTimeRemoved = Utils.FormatDateTime(cols[10]);
                Boolean IssueFlag = Utils.FormatBoolean(cols[11]);
                String Identity = cols[12];

                Update u = new Update(Name, KBID, Description, isDownloaded, isInstalled, url, neededDiskSpace, DateTimeCreated, DateTimeDownloaded, DateTimeInstalled, DateTimeRemoved,IssueFlag, Identity);
                UpdateList.Add(u);
            }
            return UpdateList;
        }


        public static void Refresh() // Called to snapshot the data
        {
            Logger.instance.Debug("Refreshing update");
            saveUpdate();
        }

        static public void installUpdates() //Apply just the next update (oldest one)
        {
            Logger.instance.Debug("installing update");
            applyNextUpdate();
        }




        static private List<Update> getUpdates() //Static method to get a list of all the updates avaiable in WUA
        {
            try
            {
                Logger.instance.Debug("Getting updates");
                List<Update> updates = new List<Update>();
                IUpdateSession uSession;
                IUpdateSearcher uSearcher;
                ISearchResult uResult;
                uSession = new UpdateSession(); //Create an Update interface in WUA


                Logger.instance.Debug("Creating update");
                uSearcher = uSession.CreateUpdateSearcher();

                if (uSearcher.Online)
                {  
                    uResult = uSearcher.Search("Type='Software' and IsAssigned=1 and IsInstalled = 0"); //Save the result of the search

                    foreach (IUpdate update in uResult.Updates) //print the names of each update found
                    {
                        if (update.SupportUrl != null)
                        {
                           updates.Add(new Update(
                           update.Title.ToString(),
                           update.KBArticleIDs.ToString(),
                           update.Description.ToString(),
                           update.IsDownloaded,
                           update.SupportUrl.ToString(),
                           update.RecommendedHardDiskSpace.ToString(),
                           false,
                           update.Identity.ToString()));
                        }
                    }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Update server is OFFLINE!");
                }
                return updates;
            }
            catch(Exception e)
            {
                Logger.instance.Error(e);
                return null;
            }
        }

        static private void saveUpdate() //Save this update to the database
        {
            Logger.instance.Debug("Saving updates");
            List<Update> updateList = new List<Update>();
            updateList = getUpdates();
            foreach (Update u in updateList)
            {
                Logger.instance.Debug("Saving update");
                u.save();
            }
        }

        static private void applyNextUpdate()
        {
            Logger.instance.Debug("Apply next update");
            Database db = new Database();

            Logger.instance.Debug("Reading update from database");
            List<string> rows = db.Read("Select TOP(1) Name From [dbo].[Update] Where isInstalled = 0 ORDER BY DateTimeCreated ASC", 1);
            string Name = rows[0];

            Update u = new Update(Name);
            u.downloadUpdate();
            u.installUpdate();
        }
    }
}