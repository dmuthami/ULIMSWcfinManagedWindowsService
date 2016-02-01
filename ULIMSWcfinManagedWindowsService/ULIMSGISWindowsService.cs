﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace wcf.ulims.com.na
{
    public class ULIMSGISWindowsService : ServiceBase
    {
        //Create a pointer to a timer as a member variable named MTimer
        private Timer mTimer = null;

        public Timer MTimer
        {
            get { return mTimer; }
            set { mTimer = value; }
        }

        //Create pointer 
        private IULIMSGISService iULIMSGISService = null;

        /// <summary>
        /// Get and Setter Methods
        /// </summary>
        public IULIMSGISService mIULIMSGISService
        {
            get { return iULIMSGISService; }
            set { iULIMSGISService = value; }
        }

        static ULIMSGISWindowsService uLIMSGISWindowsService = null;

        public ServiceHost serviceHost = null;


        public ULIMSGISWindowsService()
        {
            //Name the Windows Service Name
            ServiceName = "ULIMS WCF GIS Synch Service";

            //Call method to initialze python objects
            initializePythonObjects();
        }

        public static void Main()
        {
            uLIMSGISWindowsService = new ULIMSGISWindowsService();
            ServiceBase.Run(uLIMSGISWindowsService);
        }

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

            //Apply Timer Settings

            try
            {
                timerSettings();
            }
            catch (Exception ex)
            {

                mIULIMSGISService.WriteErrorLog(ex);//Write error to log file
            }
        }

        protected override void OnStop()
        {
            if (serviceHost != null)
            {
                serviceHost.Close();
                serviceHost = null;
            }
        }

        private void initializePythonObjects()
        {
            /*
             * Get instance of IPythonLibrary class
             * Contains propoerties and methods to help with execution
             */
            mIULIMSGISService = new ULIMSGISService();
            mIULIMSGISService.mCalculatorWindowsService = uLIMSGISWindowsService;
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
                mIULIMSGISService.WriteErrorLog("ULIMS GIS Synchonization Service started");

            }
            catch (Exception ex)
            {

                mIULIMSGISService.WriteErrorLog(ex);//Write error to log file
            }
        }

        private void mTimer_Tick(object sender, ElapsedEventArgs e)
        {
            mIULIMSGISService.readObject();


            if (mIULIMSGISService.mULIMSSerializer.HasGISSyncProcessstarted==true)
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
                    mIULIMSGISService.WriteErrorLog(@"Timer ticked and executePythonCode()  method or 
                        job has been done fired");

                    //Call method that executes python code
                    executePythonCode();
                }
                catch (Exception ex)
                {
                    mIULIMSGISService.WriteErrorLog(ex);//Write error to log file
                }
                finally
                {
                    mIULIMSGISService.WriteErrorLog(@"Timer ticked and executePythonCode()  method or 
                        job has successfully completed(But with a pinch of salt-There could be errors)");

                    //GIS synch process has completed successfully.//Tell the GIS to Sharepoint client to start shipping erfs to SharePoint
                    mIULIMSGISService.MGISSyncProcess = true;

                    mIULIMSGISService.WriteErrorLog(@"GIS synch process has completed successfully.
                        Tell the GIS to Sharepoint client to start shipping erfs to SharePoint)");

                    //Tell process to stop GIS Synch Process
                    //save object file with false parameter. 
                    //This stops the Windows service from running GIS synch process until instructed so by a client
                    mIULIMSGISService.saveObject(false);

                    mIULIMSGISService.WriteErrorLog(@"Tell process to stop GIS Synch Process
                        save object file with false parameter. 
                        This stops the Windows service from running GIS synch process until instructed so by a client)");

                    //Tell the timer GIS synch process has stopped executing
                    mIULIMSGISService.mEexecuting = false;

                    mIULIMSGISService.WriteErrorLog(@"Tell the timer GIS synch process has stopped executing)");

                }
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
                mIULIMSGISService.mPythonCodeFolder = ConfigurationManager.AppSettings["python_folder"];

                //Call function to execute python process for all towns
                mIULIMSGISService.executePythonProcess();

                /*
                 * Add code to call .Net Synch Service that performs write to SharePoint Lists
                 * Add new Erfs to SharePoint Lists
                 * Update to SharePoint lists
                 * Flag deleted erfs in SharePoint List
                 */


            }
            catch (Exception)
            {
                throw; //In case of an error then throws it up the stack trace
            }
        }
    }
}