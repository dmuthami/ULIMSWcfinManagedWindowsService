using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULIMSWcfClient.ULIMSGISServiceRef;

namespace ULIMSWcfClient
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //Create instance of this class
                Program program = new Program();

                //Call method to start GIS Synch process
                program.GISSynchProcess();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.StackTrace); //Write to console the stack trace
            }

        }


        private void GISSynchProcess()
        {
            //retieve config settings
            try
            {
                int threadInterval = retrieveConfigSettings();

                //Service Reference
                ULIMSGISServiceClient client = new ULIMSGISServiceClient();

                //Enable timer
                string issuccess = client.startGISSyncProcess(true);
                //Write to console

                //Console.WriteLine("Default Timer Started");
                Console.WriteLine(issuccess);

                //Create infinit loop with 5 minutes break

                while (true)
                {
                    if (client.isSuccessGISSyncProcess() == true)
                    {

                        //Todo code to call ULIMS GIS Client Main

                        Console.WriteLine("GIS Sync Process has completed");
                        break;
                    }

                    //Write to console asking for patience
                    Console.WriteLine(String.Format("...Wait for {0} milliseconds GIS Process to Complete", threadInterval));

                    //Let cuurent thread sleep for x minutes
                    System.Threading.Thread.Sleep(threadInterval);
                }

                Console.ReadLine(); //Wait for user input
            }
            catch (Exception)
            {
                
                throw;//In case of an error then throws it up the stack trace
            }
        }

        private int retrieveConfigSettings()
        {
            try
            {
                //Get timer interval from app.config
                int threadInterval;
                threadInterval = Convert.ToInt32(ConfigurationManager.AppSettings["thread_interval"].ToString());//Read  timer value from app.config
                return threadInterval;
            }
            catch (Exception)
            {
                
                throw;//In case of an error then throws it up the stack trace
            }
        }
    }
}

