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

                Console.WriteLine("Start...");
                //Call method to start GIS Synch process asynchronously
                Task taskGISSynchProcess = new Task(GISSynchProcess);
                taskGISSynchProcess.Start();
                taskGISSynchProcess.Wait();
                Console.ReadLine(); //Wait for user input
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.StackTrace); //Write to console the stack trace
            }

        }


        static async void GISSynchProcess()
        {
            //retieve config settings
            try
            {
                int threadInterval = retrieveConfigSettings();

                //Service Reference
                ULIMSGISServiceClient client = new ULIMSGISServiceClient();

                var task = Task.Factory.StartNew(() => client.executePythonCodeAsync());
                var str = await task;              
                await str.ContinueWith(e =>
                {
                    if (e.IsCompleted)
                    {
                        Console.WriteLine("Execution of Python Code Status = {0}", str.Result);
                    }
                });
                Console.WriteLine("Waiting for the execution of python code asynchronously result");
                
            }
            catch (Exception)
            {
                
                throw;//In case of an error then throws it up the stack trace
            }
        }

        static int retrieveConfigSettings()
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

