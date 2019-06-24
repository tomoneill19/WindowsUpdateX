using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using UpdateCore;
using System.Runtime.InteropServices;

namespace WindowsService
{
    public enum ServiceState
    {
        SERVICE_STOPPED = 1,
        SERVICE_START_PENDING = 2,
        SERVICE_STOP_PENDING = 3,
        SERVICE_RUNNING = 4,
        SERVICE_CONTINUE_PENDING = 5,
        SERVICE_PAUSE_PENDING = 6,
        SERVICE_PAUSED = 7,
    }

    public enum tickTypes //Enumerates how many service ticks (minutes) should pass before doing a given method
    {
        HARDWARE_TICKS = 3,
        APPLICATION_TICKS = 25,
        SAVE_UPDATE_TICKS = 60,
        APPLY_UPDATE_TICKS = 120,
        DO_ANALYSIS_TICS = 180
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ServiceStatus
    {
        public int serviceType;
        public ServiceState currentState;
        public int controlsAccepted;
        public int win32ExitCode;
        public int serviceSpecificExitCode;
        public int checkPoint;
        public int waitHint;
    };

    public partial class ServiceX : ServiceBase
    {
        Hardware h;
        Timer timer = new System.Timers.Timer();
        private long ticks;
        private int updatesDone;
        private int maxUpdates = 3;
        private int eventId = 1;
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus); // WHAT IS THIS???
        string serviceName = "WindowsUpdate"; // The name of the in-built windows update service so it can be toggled on and off for API usage

        public ServiceX()
        {
            InitializeComponent();
            ticks = 0;
            updatesDone = 0;
            eventLog = new EventLog();
            string eventSourceName = "updateLog"; // Name of event in eventviewer interface
            string logName = "updateLog"; // Name of file to store event logs
            eventLog = new EventLog();
            if (!EventLog.SourceExists(eventSourceName))
            {
                EventLog.CreateEventSource(eventSourceName, logName);
            }
            eventLog.Source = eventSourceName;
            eventLog.Log = logName;
            this.ServiceName = "WindowsUpdateX";
            Logger.SetLogDir(Environment.GetCommandLineArgs()[1]); //Get the install location from the command line (passed by the XML) to set the text log's location
        }

        protected override void OnStart(string[] args)
        {
            eventLog.WriteEntry("Starting...");
            // Update the service state to Start Pending.  
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.currentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.waitHint = 30000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            timer.Interval = 60000; // 60 seconds  
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();

            // Update the service state to Running.  
            serviceStatus.currentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            // Set up message logger
            UpdateCore.Logger.SetLogToDatabase(true);
            UpdateCore.Logger.setLogToTextFile(true);
            UpdateCore.Logger.setDatabaseConnection(new Database().getConnection());
            UpdateCore.Logger.setLoggingLevel(Logger.LogLevels.DEBUG);
            h = new Hardware();
           eventLog.WriteEntry("Started");
        }

        protected override void OnStop()
        {
            // Update the service state to pending a stop.  
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.currentState = ServiceState.SERVICE_STOP_PENDING;
            serviceStatus.waitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            // Update the service state to stopped.
            serviceStatus.currentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            eventLog.WriteEntry("Stopped");
        }

        private void eventLog_EntryWritten(object sender, EntryWrittenEventArgs e)
        {

        }

        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            try
            {
                eventLog.WriteEntry("Monitoring the System", EventLogEntryType.Information, eventId++);
                ticks++;
                if (ticks % Convert.ToInt64(tickTypes.HARDWARE_TICKS) == 0)
                {
                    eventLog.WriteEntry("About to save hardware...");
                    saveHardware();
                }
                if (ticks % Convert.ToInt64(tickTypes.APPLICATION_TICKS) == 0)
                {
                    saveApplications();
                }
                if (ticks % Convert.ToInt64(tickTypes.SAVE_UPDATE_TICKS) == 0)
                {
                    startWindowsService();
                    saveUpdates();
                }
                if (ticks % Convert.ToInt64(tickTypes.DO_ANALYSIS_TICS) == 0)
                {
                    DoAnalysis();
                }
                if (ticks % Convert.ToInt64(tickTypes.APPLY_UPDATE_TICKS) == 0)
                {
                    if (updatesDone < maxUpdates) //Only apply 3 updates between restarts
                    {
                        startWindowsService();
                        applyUpdates();
                        updatesDone++;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.instance.Error(e);
            }
            finally //When finished doing the timer tick, try turning the default windows update service (needed for WUAPI) off (in case it was turned on)
            {
                stopWindowsService();
            }
        }

        protected override void OnContinue()
        {
            eventLog.WriteEntry("In OnContinue.");
        }

        public void saveHardware() // 3 ticks
        {
            eventLog.WriteEntry("About to create hardware h...");
            eventLog.WriteEntry("Saving hardware stats...");
            h.Refresh();
            eventLog.WriteEntry("Saved stats!");
        }

        public void saveApplications() // 25 ticks
        {
            eventLog.WriteEntry("Saving an application...");
            Application.Refresh();
            eventLog.WriteEntry("Saved the application!");
        }

        public void saveUpdates() // 60 ticks
        {
            eventLog.WriteEntry("Saving updates...");
            Update.Refresh();
            eventLog.WriteEntry("Saved the updates!");
        }

        public void applyUpdates() // 120 ticks (Maximum 3 updates per system restart)
        {
            eventLog.WriteEntry("Applying an update...");
            Update.installUpdates();
            eventLog.WriteEntry("Applied the update!");
        }

        public void DoAnalysis()  // 180 ticks
        {
            eventLog.WriteEntry("Doing analysis...");
            Analytics.DoAnalysis();
            eventLog.WriteEntry("Done analysis!");
        }


        /****************************************************************************************************************************************************
         * Use the methods in the service class to ensure WUAPI is on before trying to do update api commands and then turned off afterward (to stop clashes).
         ****************************************************************************************************************************************************/
        private void startWindowsService()
        {
            Service wuapiService = new Service(this.serviceName);
            if (wuapiService.serviceRunning())
            {
                return;
            }
            wuapiService.StartService();
        }

        private void stopWindowsService()
        {
            Service wuapiService = new Service(this.serviceName);
            if (!(wuapiService.serviceRunning()))
            {
                return;
            }
            wuapiService.StopService();
        }

    }
}