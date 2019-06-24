using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;
using System.Diagnostics.Contracts;

namespace UpdateCore
{
    public class Service
    {
        string ServiceName;

        public Service(string name)     // Constructor - Takes the Windows name for the process to start (found in Services.msc)
        {
            this.ServiceName = name;
        }

        public ServiceController getService()
        {
            ServiceController[] services = ServiceController.GetServices(); //Get an array of all of the services installed.

            // try to match the ServiceName property to one of the services in the array
            foreach (ServiceController service in services)
            {
                if (service.ServiceName == this.ServiceName)
                    return service;
            }
            return null;
        }

        public bool serviceRunning()
        {
            ServiceController service = getService();
            ServiceControllerStatus status = service.Status;

            if (status == ServiceControllerStatus.Running){
                return true;
            }
            return false;
        }

        public bool serviceInstalled()
        {
            if(getService() != null) //Uses the getService() method that returns null if it's not in the array
            {
                return true;
            }
            return false;
        }


        /******************************************************************************************************
         * "Windows Update" needs starting in order to use Windows Update API and should be stopped after use *
         ******************************************************************************************************/

        public void StartService()
        {
            ServiceController controller = getService();
            if (controller == null)
            {
                Logger.instance.Error("No service to start exists with name: " + this.ServiceName);
                return;
            }
            controller.Start();
            controller.WaitForStatus(ServiceControllerStatus.Running);
        }

        public void StopService()
        {
            ServiceController controller = getService();
            if (controller == null)
            {
                Logger.instance.Error("No service to stop exists with name: " + this.ServiceName);
                return;
            }

            controller.Stop();
            controller.WaitForStatus(ServiceControllerStatus.Stopped);
        }
    }
}
