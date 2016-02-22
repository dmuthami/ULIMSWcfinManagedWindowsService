using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using ULIMSGISPython = ulimsgispython.ulims.com.na ;

namespace wcf.ulims.com.na
{
    public class ULIMSGISWindowsService : ServiceBase
    {
        #region Member Variables

        static ULIMSGISWindowsService uLIMSGISWindowsService = null;

        public ServiceHost serviceHost = null;

        #endregion

        /// <summary>
        /// Constructor : ULIMSGISWindowsService method
        /// </summary>
        public ULIMSGISWindowsService()
        {
            //Name the Windows Service Name
            ServiceName = "ULIMS WCF GIS Synch Service";
        }

        /// <summary>
        /// Main method: entry point of this program
        /// </summary>
        public static void Main()
        {
            uLIMSGISWindowsService = new ULIMSGISWindowsService();
            ServiceBase.Run(uLIMSGISWindowsService);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            if (serviceHost != null)
            {
                serviceHost.Close();
            }

            // Create a ServiceHost for the ULIMSGISService type and 
            // provide the base address.
            serviceHost = new ServiceHost(typeof(ULIMSGISService));

            // Open the ServiceHostBase to create listeners and start 
            // listening for messages.
            serviceHost.Open();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnStop()
        {
            if (serviceHost != null)
            {
                serviceHost.Close();
                serviceHost = null;
            }
        }

    }
}
