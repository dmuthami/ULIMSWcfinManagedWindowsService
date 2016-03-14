using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace wcf.ulims.com.na
{
    [RunInstaller(true)]//allows 
    public class ProjectInstaller : Installer
    {

        #region Member Variables
        private ServiceProcessInstaller process;
        private ServiceInstaller service;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor Method: ProjectInstaller()
        /// </summary>
        public ProjectInstaller()
        {
            try
            {
                process = new ServiceProcessInstaller();
                process.Account = ServiceAccount.LocalSystem; //Run services as local system account
                service = new ServiceInstaller(); //Service installer
                service.ServiceName = "ULIMS WCF GIS Synch Service"; //Service Name
                Installers.Add(process); 
                Installers.Add(service);
            }
            catch (Exception ex)
            {

                //In case of an error then throws it explicitly up the stack trace and add a message to the re-thrown error
                throw new Exception("ProjectInstaller.ProjectInstaller() : ", ex);
            }
        }

        #endregion

    }
}
