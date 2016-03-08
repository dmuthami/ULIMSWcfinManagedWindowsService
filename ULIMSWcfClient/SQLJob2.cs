using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using System.Data;
using System.Threading;

namespace ULIMSWcfClient
{
    class SQLJob2
    {
        //public enum JobExecutionStatus;

        public void Execute()
        {
            string jobName = null;
            Server server = new Server(); //Default instance
            try
            {
                server.ConnectionContext.LoginSecure = false; //Set to false since we are using datbase authentication

                //Login credentials
                server.ConnectionContext.Login = "sa"; 
                server.ConnectionContext.Password = "gisadmin";
                server.ConnectionContext.Connect();

                //Job name
                jobName = "temp_h_job";
                //Get job instance
                Job job = server.JobServer.Jobs[jobName];

                //Start the job
                job.Start();

                /*
                 * Job execution is synchronous so lets use a thread to know when it completed
                 */

                //Thread wait in miliseconds
                int threadWait = 5000; 

                Thread.Sleep(threadWait); //Sleep or some time

                job.Refresh();//Refresh the job thread.
                
                //Check if the job is complete 
                while (job.CurrentRunStatus != JobExecutionStatus.Idle /*4*/)
                {
                    //Write to console asking for patience as the job executes
                    Console.WriteLine(String.Format("{0}Job Name : {1} is processing. Please wait for {2} milliseconds", Environment.NewLine, jobName, threadWait));
                    Thread.Sleep(threadWait);
                    job.Refresh();
                }

            }
            finally
            {
                if (server.ConnectionContext.IsOpen)
                {
                    server.ConnectionContext.Disconnect();//Safely disconnect SQL Server
                    Console.WriteLine(String.Format("{0}Job Name : {1} has successfully completed", Environment.NewLine, jobName)); //Write to console saying we are done
                   
                }

            }
        }
    }
}
