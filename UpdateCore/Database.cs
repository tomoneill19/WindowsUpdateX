using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;

namespace UpdateCore
{
    public class Database
    {
        //Properties
        public char COLUMN_DELIMETER = '|'; // Define pipeline as a string delimiter for database info
        string dbName = "WindowsUpdates";
        static SqlConnection conn;


        public SqlConnection getConnection()  // Try and get a connection to a local SQL Server database. If there's a connection open, don't get a new one.
        {

            string mdfFilename = dbName + ".mdf";
            string con = "Data Source=localhost;Initial Catalog=WindowsUpdates;Integrated Security=True;MultipleActiveResultSets=True";

            if (conn == null)
            {
                conn = new SqlConnection();
                conn.ConnectionString = con;
                conn.Open();
            }

            return conn;
        }

        public void Write(string SQL)  // Get a SQL connection and then execute a SQL query passed to the method
        {
            SqlCommand cmd = getConnection().CreateCommand();
            cmd.CommandText = SQL;
            cmd.ExecuteNonQuery();
        }

        public List<string> Read(string SQL, int NumberOfColumns)
        {
            SqlCommand cmd = getConnection().CreateCommand();
            cmd.CommandText = SQL;
            SqlDataReader reader = cmd.ExecuteReader(); //Reads all data returned by the SQL command "cmd"

            List<string> rows = new List<string>();
            String row = "";

            while (reader.Read())
            {
                row = String.Format("{0}", reader[0]);
                for (int i = 1; i < NumberOfColumns; i++)
                {
                    row = row + COLUMN_DELIMETER + String.Format("{0}", reader[i]);
                }
                rows.Add(row);
            }

            reader.Close();

            return rows;
        }

        public long ExecuteScalar(String SQL)  // Not sure why this is here...
        {
            SqlCommand cmd = getConnection().CreateCommand();
            cmd.CommandText = SQL;

            return Convert.ToInt64(cmd.ExecuteScalar());
        }
    }
}