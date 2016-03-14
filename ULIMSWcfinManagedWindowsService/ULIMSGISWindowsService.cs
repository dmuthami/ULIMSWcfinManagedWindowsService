using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using ULIMSGISPython = ulimsgispython.ulims.com.na;

//Utility Clacc library
using Utility.ulims.com.na;

namespace wcf.ulims.com.na
{
    public class ULIMSGISWindowsService : ServiceBase
    {
        #region Member Variables

        //Create a pointer to a timer as a member variable named MTimer
        private Timer mTimer = null;

        //Create pointer for ULIMS GIS service object
        private IULIMSGISService iULIMSGISService = null;


        //Create pointer for python library object
        private ULIMSGISPython.IPythonLibrary iPythonLibrary = null;

        static ULIMSGISWindowsService uLIMSGISWindowsService = null;

        public ServiceHost serviceHost = null;

        #endregion

        #region Getter and Setter Methods
        public Timer MTimer
        {
            get
            {
                try
                {
                    return mTimer;
                }
                catch (Exception ex)
                {
                    
                    //In case of an error then throws it explicitly up the stack trace and add a message to the re-thrown error
                    throw new Exception("ULIMSGISWindowsService.Timer MTimer get : ", ex); 
                }
            }
            set
            {
                try
                {
                    mTimer = value;
                }
                catch (Exception ex)
                {
                    
                     //In case of an error then throws it explicitly up the stack trace and add a message to the re-thrown error
                    throw new Exception("ULIMSGISWindowsService.Timer MTimer set : ", ex); 
                }
            }
        }
        /// <summary>
        /// Get and Setter Methods
        /// </summary>
        public IULIMSGISService mIULIMSGISService
        {
            get
            {
                try
                {
                    return iULIMSGISService;
                }
                catch (Exception ex)
                {
                    
                    //In case of an error then throws it explicitly up the stack trace and add a message to the re-thrown error
                    throw new Exception("ULIMSGISWindowsService.IULIMSGISService mIULIMSGISService get : ", ex); 
                }
            }
            set
            {
                try
                {
                    iULIMSGISService = value;
                }
                catch (Exception ex)
                {
                    
                    //In case of an error then throws it explicitly up the stack trace and add a message to the re-thrown error
                    throw new Exception("ULIMSGISWindowsService.IULIMSGISService mIULIMSGISService set : ", ex); 
                }
            }
        }

        public ULIMSGISPython.IPythonLibrary IPythonLibrary
        {
            get
            {
                try
                {
                    return iPythonLibrary;
                }
                catch (Exception ex)
                {
                    
                //In case of an error then throws it explicitly up the stack trace and add a message to the re-thrown error
                throw new Exception("ULIMSGISWindowsService.ULIMSGISPython.IPythonLibrary IPythonLibrary get : ", ex);
                }
            }
            set
            {
                try
                {
                    iPythonLibrary = value;
                }
                catch (Exception ex)
                {

                    //In case of an error then throws it explicitly up the stack trace and add a message to the re-thrown error
                    throw new Exception("ULIMSGISWindowsService.ULIMSGISPython.IPythonLibrary IPythonLibrary set : ", ex);
                }
            }
        }

        #endregion

        /// <summary>
        /// Constructor : ULIMSGISWindowsService method
        /// </summary>
        public ULIMSGISWindowsService()
        {
            try
            {
                //Name the Windows Service Name
                ServiceName = "ULIMS WCF GIS Synch Service";

                //Call method to initialze python objects
                initializeGISObjects();
            }
            catch (Exception ex)
            {
                
                //In case of an error then throws it explicitly up the stack trace and add a message to the re-thrown error
                throw new Exception("ULIMSGISWindowsService.ULIMSGISWindowsService() : ", ex);
            }
        }

        /// <summary>
        /// Main method: entry point of this program
        /// </summary>
        public static void Main()
        {
            try
            {
                uLIMSGISWindowsService = new ULIMSGISWindowsService();
                ServiceBase.Run(uLIMSGISWindowsService);
            }
            catch (Exception ex)
            {
                
                Logger.WriteErrorLog("ULIMSGISWindowsService.Main()" + Environment.NewLine + ex.ToString());//Write error to log file
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            try
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
            catch (Exception ex)
            {

                Logger.WriteErrorLog("ULIMSGISWindowsService.OnStart(string[] args)" + Environment.NewLine + ex.ToString());//Write error to log file
            }

            //Apply Timer Settings

            try
            {
                timerSettings();
            }
            catch (Exception ex)
            {

                Logger.WriteErrorLog(ex.ToString());//Write error to log file
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnStop()
        {
            try
            {
                if (serviceHost != null)
                {
                    serviceHost.Close();
                    serviceHost = null;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("ULIMSGISWindowsService.OnStop()" + Environment.NewLine + ex.ToString());//Write error to log file
            }
        }

        private void initializeGISObjects()
        {
            try
            {
                /*
                 * Get instance of IPythonLibrary class
                 * Contains propoerties and methods to help with execution
                 */
                IPythonLibrary = new ULIMSGISPython.PythonLibrary();


                /*
                 * Get instance of GISService class
                 * Contains propoerties and methods to help with execution
                 */
                mIULIMSGISService = new ULIMSGISService();

                mIULIMSGISService.MIPythonLibrary = IPythonLibrary;

                //Code ensures execute path and execute directory are returned successfully
                Logger.ExecutablePath = ""; Logger.ExecutableRootDirectory = "";

                //Get path
                string filePathForSerializedObject = Logger.ExecutableRootDirectory + @"\" + "SavedULIMSObjects.bin";

                //Call save object
                mIULIMSGISService.saveObject(filePathForSerializedObject);

                //Write to logger
                Logger.WriteErrorLog("Executable Path :" + Environment.NewLine + Logger.ExecutablePath);
                Logger.WriteErrorLog("Execute Root Directory" + Environment.NewLine + Logger.ExecutableRootDirectory);
            }
            catch (Exception ex)
            {

                //In case of an error then throws it explicitly up the stack trace and add a message to the re-thrown error
                throw new Exception("ULIMSGISWindowsService.initializeGISObjects() ", ex);
            }

        }

        private void timerSettings()
        {
            try
            {
                //Initialize anew instance of the timer class
                MTimer = new Timer();

                //Get timer interval from app.config
                double timerInterval;
                timerInterval = Convert.ToDouble(ConfigurationManager.AppSettings["timer_interval"].ToString());//Read  timer value from app.config
                MTimer.Interval = timerInterval;
                /*
                 * Wire timer elapsed event to the timer tick handler
                 * Occurs when the interval elapses
                 */
                MTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.mTimer_Tick);

                //Enable the timer to whether to raise the elapsed event
                MTimer.Enabled = true;

                //save object file with false parameter. 
                //This stops the Windows service from running GIS synch process until instructed so by a client
                mIULIMSGISService.saveObject(false);

                //Write to log file indicating GIS service has started successfully.
                Logger.WriteErrorLog("ULIMS GIS Synchonization Service started");

            }
            catch (Exception ex)
            {

                //In case of an error then throws it explicitly up the stack trace and add a message to the re-thrown error
                throw new Exception("ULIMSGISWindowsService.timerSettings() ", ex);
            }
        }

        private void mTimer_Tick(object sender, ElapsedEventArgs e)
        {
            try
            {
                mIULIMSGISService.readObject();

                if (mIULIMSGISService.mULIMSSerializer.HasGISSyncProcessstarted == true)
                {
                    //Check that elapsed event raises this event handler and executes code in it 
                    //  if and only if the previous execution has completed
                    if (mIULIMSGISService.mEexecuting)
                        return;

                    //Set MPythonLibrary is executing as true
                    mIULIMSGISService.mEexecuting = true;

                    try
                    {
                        //Indicate and write to log file shouting that execution of python code is firing from all cylinders
                        Logger.WriteErrorLog(@"Timer ticked and executePythonCode()  method or 
                        job has been done fired");

                        //Call method that executes python code
                        executePythonCode();
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteErrorLog("ULIMSGISWindowsService.executePythonCode()" + Environment.NewLine + ex.ToString());//Write error to log file
                    }
                    finally
                    {
                        //Tell the timer GIS synch process has stopped executing
                        mIULIMSGISService.mEexecuting = false;
                        Logger.WriteErrorLog(@"Tell the timer GIS synch process has stopped executing)");

                        //Tell process to stop GIS Synch Process
                        //save object file with false parameter. 
                        //This stops the Windows service from running GIS synch process until instructed so by a client
                        mIULIMSGISService.saveObject(false);

                        Logger.WriteErrorLog(@"Tell process to stop GIS Synch Process
                        save object file with false parameter. 
                        This stops the Windows service from running GIS synch process until instructed so by a client)");

                        //GIS synch process has completed successfully.//Tell the GIS to Sharepoint client to start shipping erfs to SharePoint
                        mIULIMSGISService.MGISSyncProcess = true;

                        Logger.WriteErrorLog(@"GIS synch process has completed successfully.
                        Tell the GIS to Sharepoint client to start shipping erfs to SharePoint)");

                        Logger.WriteErrorLog(@"Timer ticked and executePythonCode()  method or 
                        job has successfully completed(But with a pinch of salt-There could be errors)");

                    }
                }
            }
            catch (Exception ex)
            {

                Logger.WriteErrorLog("ULIMSGISWindowsService.mTimer_Tick(object sender, ElapsedEventArgs e)" + Environment.NewLine + ex.ToString());//Write error to log file
            }

        }

        /// <summary>
        /// executePythonCode Method: Calls other functions to execute python code
        /// </summary>
        private void executePythonCode()
        {
            try
            {
                //Load config settings from app.config file               
                IPythonLibrary.MPythonCodeFolder = ConfigurationManager.AppSettings["python_folder"];

                //Call function to execute python process for all towns
                IPythonLibrary.executePythonProcess();

                /*
                 * Add code to call .Net Synch Service that performs write to SharePoint Lists
                 * Add new Erfs to SharePoint Lists
                 * Update to SharePoint lists
                 * Flag deleted erfs in SharePoint List
                 */

            }
            catch (Exception ex)
            {
                //In case of an error then throws it explicitly up the stack trace and add a message to the re-thrown error
                throw new Exception("ULIMSGISWindowsService.executePythonCode() ", ex);
            }
        }
    }
}
