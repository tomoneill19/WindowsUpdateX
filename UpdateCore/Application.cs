namespace UpdateCore
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management;
    using System;

    public class Application : Software
    {
        //Properties
        public string Name;
        public string Version;
        public string Description;
        public DateTime InstallDate;

        
        public Application(string Name) // Constructor for finding an application by name and finding its details
        {
            Logger.instance.Debug("Creating application");
            this.Name = Name;
            populate();
        }

        private Application(string Name, string Version, string Description, DateTime InstallDate) //Private constructor for use in static methods to get history
        {
            Logger.instance.Debug("Creating application");
            this.Name = Name;             //Primary Key - Clustered Index in DB
            this.Version = Version;
            this.Description = Description;
            this.InstallDate = InstallDate;
        }

        //Load history from database
        public static List<Application> GetHistory(Update up = null, int DayRange = 7)  //Optional parameters for history near to a specific update and/or in a specific range
        {
            Logger.instance.Debug("Getting application history");

            String SQL = "Select Name, Version, Description, InstallDate From dbo.Application ";
            String WhereClause = null;
            if (up != null)
            {
                DateTime max = up.DateTimeInstalled.AddDays(DayRange);
                DateTime min = up.DateTimeInstalled.AddDays(-DayRange);
                WhereClause = "Where InstallDate Between '" + min.ToString("dd-MMM-yyyy") + "' And '" + max.ToString("dd-MMM-yyyy") + "'";
                SQL = SQL + WhereClause;
            }

            List<Application> ApplicationList = RunApplicationQuery(SQL);
           
            Logger.instance.Debug("Returning application list");
            return ApplicationList;
        }

        private static List<Application> RunApplicationQuery(String SQL)  //Query applications table and return a list of applications from the database
        {
            Logger.instance.Debug("Getting application history");
            Database db = new Database();
            List<string> rows = db.Read(SQL, 4);
            List<Application> ApplicationList = new List<Application>();

            Logger.instance.Debug("Iterating through rows of applications");
            foreach (string row in rows)
            {
                string[] cols = row.Split(new Database().COLUMN_DELIMETER);
                string Name = cols[0];
                string Version = cols[1];
                string Description = cols[2];
                DateTime InstallDate = DateTime.Parse(cols[3]);

                Application a = new Application(Name, Version, Description, InstallDate);
                ApplicationList.Add(a);
            }

            Logger.instance.Debug("Returning application list");
            return ApplicationList;
        }

        protected override void populate()   //Populate an application's properties from the database using an already established name (using the public constructor)
        {
            Logger.instance.Debug("Populating application");
            Database db = new Database();
            string sql = "Select Version, Description, InstallDate from dbo.Application Where Name = " + Name;

            Logger.instance.Debug("Reading application");
            List<string> rows = db.Read(sql, 3);

            Logger.instance.Debug("Splitting application");
            string[] cols = rows[0].Split(new Database().COLUMN_DELIMETER);

            Version = cols[0];
            Description = cols[1];
            InstallDate = DateTime.Parse(cols[2]);
         }

        //Save to database with Upsert - Each application saves itself to the DB
        protected override void save()
        {
            Logger.instance.Debug("Saving application to database");
            Database db = new Database();
            string sql = "";
            sql += "IF NOT EXISTS(SELECT * FROM dbo.Application WHERE Name = '" + Name + "')" + Environment.NewLine;
            sql += "Insert into dbo.Application (Name, Version, Description, InstallDate) values ('" + Name + "','" + Version + "','" + Description + "','" + InstallDate.ToString("s") + "')" + Environment.NewLine;
            sql += "ELSE" + Environment.NewLine;
            sql += "UPDATE dbo.Application" + Environment.NewLine;
            sql += "SET Version = '" + Version + "', Description = '" + Description + "', InstallDate = '" + InstallDate.ToString("s") + "'" + Environment.NewLine;
            sql += "WHERE Name = '" + Name + "'";
            db.Write(sql);
        }

        public static void Refresh()   // Method for calling 
        {
            saveApplications(); 
        }




        private static void saveApplications()    // Find all the applications installed on the machine and save them to the DB
        {
            Logger.instance.Debug("Saving applications");
            List<Application> appList = new List<Application>();
            appList = getApplications();
            foreach (Application a in appList)
            {
                a.save();
            }
        }

        private static List<Application> getApplications() //Method to use WMI to find all the applications installed in a list
        {
            Logger.instance.Debug("Getting applications");
            List<Application> installedPrograms = new List<Application>();
            ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_Product");
            foreach (ManagementObject mo in mos.Get())
            {
                if (mo["Name"] != null)   // There seem to be nulls returned. Leave this
                {
                    installedPrograms.Add(new Application(
                        mo["Name"] == null ? string.Empty : mo["Name"].ToString(),
                        mo["Version"] == null ? string.Empty : mo["Version"].ToString(),
                        mo["Description"] == null ? string.Empty : mo["Description"].ToString(),
                        Utils.FormatDateTimeWMI(mo["InstallDate"].ToString())));
                }
            }
            return installedPrograms;
        }

    }
}