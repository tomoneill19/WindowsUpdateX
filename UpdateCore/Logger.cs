using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Data.SqlClient;

namespace UpdateCore
{
    public class Logger
    {
        public enum LogLevels
        {
            DEBUG = 1,
            ERROR = 2
        }

        static String LogDir;
        static String LOGFILE;
        private static Logger _logger;
        private static bool LogToTextFile = true;
        private static bool LogToDatabase = true;
        private static LogLevels LogLevel = LogLevels.DEBUG;


        public static String RefMessageLogProcedure = "dbo.LogMessage";
        public static String RefMessageLogTableName = "dbo.MessageLog";

        private static SqlConnection conn;

        public static void setLoggingLevel(LogLevels ThisLogLevel)
        {
            LogLevel = ThisLogLevel;
        }

        public static void setMessageLogProcedure(String TheRefMessageLogProcedure)
        {
            RefMessageLogProcedure = TheRefMessageLogProcedure;
        }

        public static void setDatabaseConnection(SqlConnection Thisconn)
        {
            conn = Thisconn;
        }

        public static void setFullLogFileName(String FullLogFileName)
        {
            LOGFILE = FullLogFileName;
        }

        public static void setLogToTextFile(bool ThisLogToTextFile)
        {
            LogToTextFile = ThisLogToTextFile;
        }

        public static void SetLogToDatabase(bool ThisLogToDatabase)
        {
            LogToDatabase = ThisLogToDatabase;
        }

        public static void SetLogDir(String ThisLogDir)
        {
            LogDir = ThisLogDir;
            LOGFILE = LogDir + "\\log.txt";
        }

        public static Logger instance //Create an instance of the logger if there isn't already one
        {
            get
            {
                if (_logger == null)
                {
                    _logger = new Logger();
                }
                return _logger;
            }
        }

        private Logger()
        {
        }

        /*
         * /////////////////////
         * LOGGER INSTANCE TYPES
         * /////////////////////
         * Caller Types passed when the method is called and sets info about the line number that caused the problem etc.
         */

        // Logger for non-error based logs
        public void Debug(String Message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (LogLevel == LogLevels.DEBUG)
            {
                LogMessage(Message, "DEBUG", memberName, sourceFilePath, sourceLineNumber);
            }
            
        }

        // Logger for generic errors
        public void Error(Exception e,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogMessage(e.Message, "ERROR", memberName, sourceFilePath, sourceLineNumber);

        }

        // Logger for SQL exceptions (doesn't try and write to the database because if the database broke, we don't want to log to the broken DB)
        public void Error(SqlException e,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            String LogString = "ERROR" + ": " + e.Message + " - Called By: " + memberName + " | Calling Path: " + sourceFilePath + " | Line Number: " + sourceLineNumber.ToString();
            LogString = LogString.Replace("'", "''");    // Escape single quotes

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(LOGFILE, true))
            {
                file.WriteLine(DateTime.UtcNow.ToString() + LogString);
            }
        }

        public void Error(String Message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
                LogMessage(Message, "ERROR", memberName, sourceFilePath, sourceLineNumber);

            if (new Config().AlertsForErrors)
            {
                UpdateCore.Email email = UpdateCore.Email.Instance;   // need to use the users configured email address
                email.SendEmail("Error logged by Updater", Message);
            }
        }

        // Logger for issues that weren't caused by exceptions being thrown
        public void Warning(String Message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogMessage(Message, "WARNING", memberName, sourceFilePath, sourceLineNumber);

            if (new Config().AlertsForWarnings)
            {
                UpdateCore.Email email = UpdateCore.Email.Instance;   // need to use the users configured email address
                email.SendEmail("Possible Issue logged by Updater", Message);
            }
        }

        // Method for logging to both text files and the database - Does whichever ones are set to try in the properties
        private void LogMessage(String Message, String Severity, string memberName, string sourceFilePath, int sourceLineNumber)
        {
            if (LogToTextFile)
            {
                String LogString = Severity + ": " + Message + " - Called By: " + memberName + " | Calling Path: " + sourceFilePath + " | Line Number: " + sourceLineNumber.ToString();
                LogString = LogString.Replace("'", "''");    // Escape single quotes

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(LOGFILE, true))
                {
                    file.WriteLine(DateTime.UtcNow.ToString() + LogString);
                }
            }

            if (LogToDatabase)
            {
                String SQL = "Exec " + RefMessageLogProcedure + " '" + Message.Replace("'","") + "','" + Severity + "','"  + memberName + "','" + sourceFilePath + "','" + sourceLineNumber + "'";

                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = SQL;
                cmd.ExecuteNonQuery();
            }
        }
    }
}
