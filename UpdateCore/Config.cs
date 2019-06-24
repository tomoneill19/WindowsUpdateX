using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateCore
{
    public class Config
    {
        //Properties
        public bool AlertsForErrors;
        public bool AlertsForWarnings;
        private int DaysEitherSideUpdateToDisplay;

        public static String configTableName = "dbo.Config";


        public static void setConfigTableName(string thisConfigTableName)   // Feels unnecessary...
        {
            configTableName = thisConfigTableName;
        }

        public void setDaysEitherSideUpdateToDisplay(int DaysEitherSideUpdateToDisplay)  // Set property for days either side of an update to be checked when analysing data
        {
            if (DaysEitherSideUpdateToDisplay > 0)
            {
                this.DaysEitherSideUpdateToDisplay = DaysEitherSideUpdateToDisplay;
            }
            else
            {
                throw new Exception("The Days Either Side Update To Display must be greater than zero");
            }
        }

        public int GetDaysEitherSideUpdateToDisplay() // get method for days either side of an update
        {
            return DaysEitherSideUpdateToDisplay;
        }

        public Config()
        {
            Populate();
        }

        public void Save()
        {
            Database d = new Database();
            d.Write("Update " + configTableName + " Set AlertsForErrors = '" + AlertsForErrors + "',AlertsForWarnings='" + AlertsForWarnings + "'" + ",DaysEitherSideUpdateToDisplay='" + DaysEitherSideUpdateToDisplay + "'");
        }


        private void Populate() //Populates the class with settings from the config table in the DB
        {
            Logger.instance.Debug("Populating Config");
            Database db = new Database();
            string sql = "Select AlertsForErrors, AlertsForWarnings, DaysEitherSideUpdateToDisplay from " + configTableName;

            Logger.instance.Debug("Reading Config");
            List<string> rows = db.Read(sql, 3);

            Logger.instance.Debug("Splitting application");
            string[] cols = rows[0].Split(new Database().COLUMN_DELIMETER);

            AlertsForErrors = Utils.FormatBoolean(cols[0]);
            AlertsForWarnings = Utils.FormatBoolean(cols[1]);
            DaysEitherSideUpdateToDisplay = Int32.Parse(cols[2]);
        }

    }
}
