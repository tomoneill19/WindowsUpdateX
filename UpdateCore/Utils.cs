using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
namespace UpdateCore
{
    /********************************************************
     * Collection of utilities (utils) for formatting data
     * Formats dates and times for the database format etc.
     ********************************************************/
    public class Utils
    {
        //Properties
        public static string dateFormat = "dd/MM/yyyy HH:mm:ss";
        public static string dateFormatDisplay = "dd MMM yyyy";
        private static DateTime minDateTime = new DateTime(1970, 01, 01);

        public static DateTime FormatDateTime(string dateTimeString) //Takes a date string and returns a datetime object
        {
            if (string.IsNullOrEmpty(dateTimeString)) // If a null date was found, use unix epoch instead of absolute minimum (-7 days from 01/01/0001 is bad!)
            {
                return minDateTime;
            }

            DateTime dateTime;
            try
            {
                dateTime = DateTime.SpecifyKind(DateTime.ParseExact(dateTimeString, dateFormat, CultureInfo.InvariantCulture), DateTimeKind.Utc);
            }
            catch (FormatException)
            {
                return minDateTime;
            }

            return dateTime;
        }

        public static DateTime FormatDateTimeWMI(string dateTimeString) //Formats the impure date strings found by WMI into true datetimes for the object type
        {
            if (string.IsNullOrEmpty(dateTimeString))
            {
                return minDateTime;
            }

            DateTime dateTime;
            try
            {
                dateTime = new DateTime(Convert.ToInt32(dateTimeString.Substring(0,4)),Convert.ToInt32(dateTimeString.Substring(4,2)),Convert.ToInt32(dateTimeString.Substring(6,2)));
            }
            catch (FormatException)
            {
                return minDateTime;
            }

            return dateTime;
        }

        public static string FormatDateTimeDB(DateTime dateTime) //Takes a datetime and casts it to a string for use in SQL queries
        {
            if (dateTime == DateTime.MinValue)
            {
                return minDateTime.ToString(dateFormatDisplay);
            }
            return dateTime.ToString(dateFormatDisplay);
        }

        public static bool FormatBoolean(string BoolAsString) // Casts SQL bits (0 or 1 in string format) to booleans
        {
            if (string.IsNullOrEmpty(BoolAsString))
            {
                return false;
            }

            if (BoolAsString.Equals("0"))
            {
                return false;
            }
            if (BoolAsString.Equals("1"))
            {
                return true;
            }
            return Boolean.Parse(BoolAsString);
        }

        public static int FormatBit(bool boolval) // Casts booleans into SQL bits (0 or 1 in string format)
        {
            if (boolval == true)
            {
                return 1;
            }
            if (boolval == false)
            {
                return 0;
            }
            return 0;
        }

        public static string EscapeQuotes(string Text) // Escapes quotes from long-form text (E.G. when found by WUAPI (E.G. Update Description))
        {
            if (Text != null)
            {
                return Text.Replace("'", "''");
            }
                return Text;
        }


        public static string getDisplayDate(DateTime date) // Cast datetime objects into strings for displaying on front-end
        {
            if(date <= minDateTime)
            {
                return "-";
            }
            return date.ToString(dateFormatDisplay);
        }
    }
}
