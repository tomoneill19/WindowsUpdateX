using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.Diagnostics;

namespace UpdateCore
{
    public class Hardware
    {
        // Properties
        public DateTime LogDate;
        public double totalRam;
        public double availableRam;
        public double usedCpu;
        public double availableDisk;
        public double maxDisk;
        public float diskRead;
        public float diskWrite;



        public Hardware()     // Constructor with no parameters to prepare for WMI data gathering
        {
            Logger.instance.Debug("Creating hardware class with no parameters");
        }

        // Constructor for hardware class using data gathered from the database
        public Hardware(double totalRam, double availableRam, double usedCpu, double availableDisk, double maxDisk, float diskRead, float diskWrite, DateTime LogDate)
        {
            Logger.instance.Debug("Creating hardware object");
            this.totalRam = totalRam;
            this.availableRam = availableRam;
            this.usedCpu = usedCpu;
            this.availableDisk = availableDisk;
            this.maxDisk = maxDisk;
            this.diskRead = diskRead; 
            this.diskWrite = diskWrite;
            this.LogDate = LogDate;
            Logger.instance.Debug("Created hardware object");
        }


        public void Refresh() // Refresher method to be called on tick by the service and save to the DB
        {
            Logger.instance.Debug("Refreshing hardware object");

            populate();
            Logger.instance.Debug("Populated hardware object");

            save();
            Logger.instance.Debug("Saved  hardware object");
        }

        public void populate() //Populate all of the data into the properties using WMI
        {
            Logger.instance.Debug("Getting Total Ram");
            totalRam = getTotalRam();

            Logger.instance.Debug("Getting Available Ram");
            availableRam = getAvailableRam();

            Logger.instance.Debug("Getting Used CPU");
            usedCpu = getUsedCPU();

            Logger.instance.Debug("Getting Free Disk Space");
            availableDisk = getFreeDiskSpace().getNumber();

            Logger.instance.Debug("Getting Total Disk Space");
            maxDisk = getTotalDiskSpace().getNumber();

            Logger.instance.Debug("Getting Disk Read");
            diskRead = diskReadIO(); //Not using PCNumber class as it handles double types and these 2 are floats

            Logger.instance.Debug("Getting Disk Write");
            diskWrite = diskWriteIO();

            Logger.instance.Debug("Getting LogDate");
            LogDate = Utils.FormatDateTime(null);    // Not yet set

            Logger.instance.Debug("Finished WMI Calls");
        }

        //Save to database
        private void save() // Save to database all of the data gathered from WMI
        {
            Logger.instance.Debug("Saving hardware object");
            Database db = new Database();

            db.Write("Insert into dbo.Hardware (TotalRam, AvailableRam, UsedCPU, AvailableDisk, MaxDisk, DiskRead, DiskWrite) values (" + totalRam + ", " + availableRam + ", " + usedCpu + ", " + availableDisk + ", " + maxDisk + ", " + diskRead + ", " + diskWrite + ")");
            Logger.instance.Debug("Saved hardware object");
        }


        //Load from database
        public static List<Hardware> getHistory()  // Get all of the data history from the database
        {
            Logger.instance.Debug("In getHistory() hardware object");
            List<Hardware> HardWareList = RunHardwareQuery("Select LogDate, TotalRam, AvailableRam, UsedCPU, AvailableDisk, MaxDisk, DiskRead, DiskWrite From dbo.Hardware");
            Logger.instance.Debug("Returning history");
            return HardWareList;
        }


        //Load from database
        public static List<Hardware> getDailyHistory(Update up = null, int DayRange = 7) //Pull history from the database about a specific update within a date range
        {
            Logger.instance.Debug("In getHistory() hardware object");

            if (up == null) //If no specific update was supplied, use the most recent one.
            {
                up = Update.getLatestUpdate();
            }

            DateTime max = up.DateTimeInstalled.AddDays(DayRange);
            DateTime min = up.DateTimeInstalled.AddDays(-DayRange);

            List<Hardware> HardWareList = new List<Hardware>();
            HardWareList = RunHardwareQuery("Select Cast(LogDate as Date) As LogDate, avg(TotalRam) As TotalRam, avg(AvailableRam) as AvailableRam, avg(UsedCPU) as UsedCPU, avg(AvailableDisk) as AvailableDisk, avg(MaxDisk) as MaxDisk, avg(DiskRead) as DiskRead, avg(DiskWrite) as DiskWrite From dbo.Hardware Where LogDate Between '" + min.ToString("dd-MMM-yyyy") + "' And '" + max.ToString("dd-MMM-yyyy") + "'" + " Group by Cast(LogDate as Date)");

            Logger.instance.Debug("Returning history");
            return HardWareList;
        }


        private static List<Hardware> RunHardwareQuery(String SQL) // Query the hardware table and return all of the history
        {
            Logger.instance.Debug("In RunHardwareQuery() ");

            Database db = new Database();

            Logger.instance.Debug(SQL);
            List<string> rows = db.Read(SQL, 8);
            List<Hardware> HardWareList = new List<Hardware>();

            foreach (string row in rows)
            {
                string[] cols = row.Split(new Database().COLUMN_DELIMETER);
                DateTime dateTime = DateTime.Parse(cols[0]);
                double totalRam = double.Parse(cols[1]);
                double availableRam = double.Parse(cols[2]);
                double usedCpu = double.Parse(cols[3]);
                double availableDisk = double.Parse(cols[4]);
                double maxDisk = double.Parse(cols[5]);
                float diskRead = float.Parse(cols[6]);
                float diskWrite = float.Parse(cols[7]);

                Hardware h = new Hardware(totalRam, availableRam, usedCpu, availableDisk, maxDisk, diskRead, diskWrite, dateTime);
                HardWareList.Add(h);
            }

            Logger.instance.Debug("Returning history");
            return HardWareList;
        }


        /*
         * ////////////////////////////////////////
         * WMI GET METHODS FOR HARDWARE INFORMATION
         * ////////////////////////////////////////
         */

        public DateTime getLogDate()
        {
            return LogDate;
        }

        public double getUsedPercentageRam()
        {
            return 100*(totalRam-availableRam) / totalRam;
        }

        private double getTotalRam()
        {
            Logger.instance.Debug("Getting total ram");
            return new PCNumber(new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory).getNumber();
        }

        private double getAvailableRam()
        {
            Logger.instance.Debug("Getting available ram");
            PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            return Math.Round(ramCounter.NextValue() / 1000, 0);
        }


        private double getUsedCPU()
        {
            PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpuCounter.NextValue();
            System.Threading.Thread.Sleep(1000);
            return cpuCounter.NextValue();
        }

        public double GetUsedPercentageDiskSpace()
        {
            return 100*(maxDisk = availableDisk) / maxDisk;
        }

        private PCNumber getFreeDiskSpace()
        {
            return getDiskSpace("FreeSpace");
        }

        private PCNumber getTotalDiskSpace()
        {
            return getDiskSpace("TotalSpace");
        }

        private float diskReadIO()
        {
            return diskIO("Read");
        }

        private float diskWriteIO()
        {
            return diskIO("Write");
        }

        private float diskIO(string type)
        {
            if (type == "Read") 
            {
                PerformanceCounter diskRead = new PerformanceCounter();
                diskRead.CategoryName = "PhysicalDisk";
                diskRead.CounterName = "Disk Reads/sec";
                diskRead.InstanceName = "_Total";
                diskRead.NextValue();
                System.Threading.Thread.Sleep(1000); //Wait a second after taking a reference and get another to compare against the original (Without this, returns 0)
                return diskRead.NextValue();
            }
            else if (type == "Write")
            {
                PerformanceCounter diskWrite = new PerformanceCounter();
                diskWrite.CategoryName = "PhysicalDisk";
                diskWrite.CounterName = "Disk Writes/sec";
                diskWrite.InstanceName = "_Total";
                diskWrite.NextValue();
                System.Threading.Thread.Sleep(1000); //Wait a second after taking a reference and get another to compare against the original (Without this, returns 0)
                return diskWrite.NextValue();
            }
            else
            {
                throw new NotImplementedException();
            }
        }



        private PCNumber getDiskSpace(string type)
        {
            ManagementObject disk = new ManagementObject("win32_logicaldisk.deviceid=\"c:\"");
            disk.Get();
            if (type == "FreeSpace")
            {
                string diskFreeSpace = disk["FreeSpace"].ToString();
                double diskFreeSpaceReal = Convert.ToInt64(diskFreeSpace);
                PCNumber unitText = new PCNumber(diskFreeSpaceReal);
                return unitText;
            }
            else if (type == "TotalSpace")
            {
                string diskTotalSpace = disk["Size"].ToString();
                double diskTotalSpaceReal = Convert.ToInt64(diskTotalSpace);
                PCNumber unitText = new PCNumber(diskTotalSpaceReal);
                return unitText;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
