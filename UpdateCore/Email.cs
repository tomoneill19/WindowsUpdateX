using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using System.Globalization;
using System.Text.RegularExpressions;

namespace UpdateCore
{
    public class Email
    {

        public string FromEmailAddress;
        public string FromEmailDisplayName;
        public string FromPassword;
        public string SmtpHost;
        private int SmtpPort;
        private string sendEmail;
        public bool IsValidated = false;   // Initially an email address entered isn't validated until the validation method is run

        public static string emailTableName = "dbo.Config";
        private static Email email;

        public void setSendEmail(string sendEmail)
        {
            if (IsValidEmail(sendEmail)) // Regex check if the address is valid before allowing it to be entered to the DB
            {
                this.sendEmail = sendEmail;
            }
            else
            {
                throw new Exception("The email is not valid and must use the set of allowed characters for an email");
            }
        }

        public void setSmtpPort(int SmtpPort)
        {
            if (SmtpPort > 0)
            {
                this.SmtpPort = SmtpPort;
            }
            else
            {
                throw new Exception("The SmtpPort must be greater than zero");
            }
        }

        public int getSmtpPort()
        {
            return SmtpPort;
        }

        public String getSendEmail()
        {
            return sendEmail;
        }

        public Email() //Constructor for email class
        {
            populate();
        }


        public static Email Instance  // Create an instance of the email class if one doesn't already exist.
        {
            get
            {
                if (email == null)
                {
                    email = new Email();
                }
                return email;
            }
        }

        public bool SendEmail(string subject, string body) //Method for sending an email to the defined send address
        {
            IsValidated = Validate(); //Check email address is valid and set instead of the default values before sending
            if (!String.IsNullOrEmpty(sendEmail) && IsValidated)
            {
                return SendEmail(sendEmail, subject, body);
            }
            else
            {
                return false;
            }
            
        }

        private bool Validate()  // Check that all values in the database for email config have been set instead of their defaults
        {
            if (SmtpHost.Equals("<NOT SET>"))
            {
                return false;
            }

            if (SmtpPort == 0)
            {
                return false;
            }

            if(FromEmailAddress.Equals("<NOT SET>"))
            {
                return false;
            }

            if(FromEmailDisplayName.Equals("<NOT SET>"))
            {
                return false;
            }
            if (FromPassword.Equals("<NOT SET>"))
            {
                return false;
            }
            if (sendEmail.Equals("<NOT SET>"))
            {
                return false;
            }
            return true;
        }

        public bool SendEmail(string toEmailAddress, string subject, string body) //Method called when sending an email has been validated. Retuns a bool for success
        {
            if (!IsValidEmail(toEmailAddress))
                throw new Exception("The email is not a valid format and needs to use the list of allowed characters for an email");

            if (!(!String.IsNullOrEmpty(sendEmail) && IsValidated))
            {
                return false;
            }

            MailAddress fromAddress = new MailAddress(FromEmailAddress, FromEmailDisplayName);
            MailAddress toAddress = new MailAddress(toEmailAddress);

            try
            {
                var smtp = new SmtpClient
                {
                    Host = SmtpHost,
                    Port = SmtpPort,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, FromPassword)
                };

                using (var message = new MailMessage()
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                {
                    message.From = fromAddress;
                    message.To.Add(toEmailAddress);

                    smtp.Send(message);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public void SaveConfig() //Push config changes to the database from the class instance
        {
            Database d = new Database();
            d.Write("Update " + emailTableName + " Set SmtpHost = '" + SmtpHost + "',SmtpPort=" + SmtpPort.ToString() + ",FromEmailAddress='" + FromEmailAddress + "',FromEmailDisplayName='" + FromEmailDisplayName + "',FromPassword='" + FromPassword + "'" + ",DefaultSendEmail='" + sendEmail + "'" + ",IsValidated='" + IsValidated + "'");    
        }

        private void populate() // Populate the class with the current email config from the database
        {
            Logger.instance.Debug("Populating Email");
            Database db = new Database();
            string sql = "Select SmtpHost, SmtpPort, FromEmailAddress, FromEmailDisplayName, FromPassword, DefaultSendEmail, IsValidated from " + emailTableName;

            Logger.instance.Debug("Reading Email");
            List<string> rows = db.Read(sql, 7);

            Logger.instance.Debug("Splitting application");
            string[] cols = rows[0].Split(new Database().COLUMN_DELIMETER);

            SmtpHost = cols[0];
            SmtpPort = Int32.Parse(cols[1]);
            FromEmailAddress = cols[2];
            FromEmailDisplayName = cols[3];
            FromPassword = cols[4];
            sendEmail = cols[5];
            IsValidated = Utils.FormatBoolean(cols[6]);
        }

        // https://msdn.microsoft.com/en-us/library/01escwtf(v=vs.100).aspx
        // Above URL is MS docuemntation for validating email addressing using regex


        private bool IsValidEmail(string strIn)
        {
            if (String.IsNullOrEmpty(strIn))
                return false;

            try // Use dommain mapping class to convert Unicode domain names.
            {
                strIn = Regex.Replace(strIn, @"(@)(.+)$", this.DomainMapper);
            }
            catch (ArgumentException) //Thrown when the domain mapper didn't succeed so the argument in the method was wrong
            {
                return false;
            }

            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(strIn,
                   @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                   @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$",
                   RegexOptions.IgnoreCase);
        }

        private string DomainMapper(Match match) // Resolves unicode domain names when sending an email
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException ae)
            {
                // The email is invalid so throw exception // WHY?
                throw ae;
            }
            return match.Groups[1].Value + domainName;
        }

        public static void setEmailTableName(string thisEmailTableName) //Set the name of the config table
        {
            emailTableName = thisEmailTableName;
        }
    }
}

