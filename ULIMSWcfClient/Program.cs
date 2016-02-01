using System;
using System.Collections.Generic;
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
            
            ULIMSGISServiceClient client = new ULIMSGISServiceClient();

            //Enable timer
            string issuccess = client.startGISSyncProcess(true);
            //Write to console

            //Console.WriteLine("Default Timer Started");
            Console.WriteLine(issuccess);

            //Create infinit loop with 5 minutes break
         
            while (true)
            {
                if (client.isSuccessGISSyncProcess()==true)
                {

                    //Todo code to call ULIMS GIS Client Main

                    Console.WriteLine("GIS Sync Process has completed");
                    break;
                }

                Console.WriteLine("Wait for GIS Process to Complete");
                System.Threading.Thread.Sleep(5 * 60 * 1000);
            }



        }
    }
}
