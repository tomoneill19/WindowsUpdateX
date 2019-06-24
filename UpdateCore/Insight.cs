using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateCore
{
    public class Insight
    {
        private static String RefInsightTableName = "dbo.Insight";

        public long UID = 0;
        public String InsightText = "";
        public DateTime DateLogged;

        public Insight(long UID, String InsightText, DateTime DateLogged)
        {
            this.UID = UID;
            this.InsightText = InsightText;
            this.DateLogged = DateLogged;
        }

        public Insight(long UID)
        {
            Populate();
        }

        public Insight()
        {

        }

        public static void SetRefInsightTableName(String ThisRefInsightTableName)
        {
            RefInsightTableName = ThisRefInsightTableName;
        }

        //Load from database
        public static List<Insight> GetHistory(Update up = null, int DayRange = 7)
        {
            Logger.instance.Debug("Getting insight history");

            String SQL = "Select UID, [Insight], DateLogged From dbo.Insight ";
            String WhereClause = null;
            if (up != null)
            {
                DateTime max = up.DateTimeInstalled.AddDays(DayRange);
                DateTime min = up.DateTimeInstalled.AddDays(-DayRange);
                WhereClause = "Where DateLogged Between '" + min.ToString("dd-MMM-yyyy") + "' And '" + max.ToString("dd-MMM-yyyy") + "'";
                SQL = SQL + WhereClause;
            }

            List<Insight> InsightList = RunInsightQuery(SQL);

            Logger.instance.Debug("Returning insight list");
            return InsightList;
        }

        private static List<Insight> RunInsightQuery(String SQL)
        {
            Logger.instance.Debug("Getting insight history");
            Database db = new Database();
            List<string> rows = db.Read(SQL, 3);
            List<Insight> InsightList = new List<Insight>();

            Logger.instance.Debug("Iterating through rows of insights");
            foreach (string row in rows)
            {
                string[] cols = row.Split(new Database().COLUMN_DELIMETER);
                long UID = Int64.Parse(cols[0]);
                String Insight = cols[1];
                DateTime DateLogged = Utils.FormatDateTime(cols[2]);

                Insight i = new Insight(UID, Insight, DateLogged);
                InsightList.Add(i);
            }

            Logger.instance.Debug("Returning insight list");
            return InsightList;
        }

        public void Save()
        {
            Database d = new Database();

            if (UID == 0)  // Create new one
            {
                String SQL;
                SQL = "Insert Into " + RefInsightTableName + " ([Insight]) Values ('" + InsightText + "'" + ") SELECT SCOPE_IDENTITY();\n";
                UID = d.ExecuteScalar(SQL);   // escape single quotes
            }
            else  // Update
            {
                d.Write("Update " + RefInsightTableName + "  Set [Insight] = '" + InsightText + "' Where [UID] = " + UID.ToString());
            }
 
        }


        //Populate from the database
        private void Populate()
        {
            Logger.instance.Debug("Populating Insight");
            Database db = new Database();
            string sql = "Select [UID], [Insight], DateLogged from " + RefInsightTableName + " Where [UID] = " + UID;

            Logger.instance.Debug("Reading Insight");
            List<string> rows = db.Read(sql, 3);

            Logger.instance.Debug("Splitting Insight");
            string[] cols = rows[0].Split(new Database().COLUMN_DELIMETER);

            UID = Int64.Parse(cols[0]); 
            InsightText = cols[1];
            DateLogged = Utils.FormatDateTime(cols[2]);
        }
    }

    

}
