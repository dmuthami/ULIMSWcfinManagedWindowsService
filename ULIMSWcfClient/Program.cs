using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ULIMSWcfClient.ULIMSGISServiceRef;

namespace ULIMSWcfClient
{
    public class WorkerPython
    {
        // This method will be called when the thread is started. 
        public void DoWork()
        {
            while (!_shouldStop)
            {
                Console.WriteLine("worker thread: working...");
            }
            Console.WriteLine("worker python thread: terminating gracefully.");
        }
        public void RequestStop()
        {
            _shouldStop = true;
        }
        // Volatile is used as hint to the compiler that this data 
        // member will be accessed by multiple threads. 
        private volatile bool _shouldStop;
    }
    class Program
    {
        //Service Reference
       static ULIMSGISServiceClient client = new ULIMSGISServiceClient();

        static void Main(string[] args)
        {
            try
            {
                //WorkerPython workerPython = new WorkerPython();
                //Thread workerPythonThread = new Thread(workerPython.DoWork);

                //workerPythonThread.Start();
                //Console.WriteLine("main thread: Starting worker thread...");

                //// Loop until worker thread activates. 
                //while (!workerPythonThread.IsAlive) ;

                //// Put the main thread to sleep for 1 millisecond to 
                //// allow the worker thread to do some work:
                //Thread.Sleep(1);

                //// Request that the worker thread stop itself:
                //workerPython.RequestStop();

                //// Use the Join method to block the current thread  
                //// until the object's thread terminates.
                //workerPythonThread.Join();

                client.BeginExecutePythonCodeMethod(GetExecutePythonCodeCallBack, null);
                Console.WriteLine("Waiting for the windows service host async operation.......");

                Console.ReadLine(); //Wait for user input
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.StackTrace); //Write to console the stack trace
            }

        }

        static void GetExecutePythonCodeCallBack(IAsyncResult result)
        {
            

            Console.WriteLine(client.EndExecutePythonCodeMethod(result).ToString());
        }
    }
}

