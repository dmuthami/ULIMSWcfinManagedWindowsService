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
using System.Configuration;

namespace ULIMSWcfClient
{
    class SQLJob2
    {
        //public enum JobExecutionStatus;

        public void Execute()
        {

            string jobName = null;          

            //Pull instance name and server name
            string instanceName = (ConfigurationManager.AppSettings["instance"].ToString());
            string serverName = (ConfigurationManager.AppSettings["servername"].ToString());

            //Server connection string
            string serverConnectionString = @".\" + instanceName;
            string serverConnectionString2 = serverName + "/" + instanceName;

            //server\instance
            ServerConnection serverConnection = new ServerConnection(serverName);
            
      

            Server server = new Server(serverConnection); 
            try
            {
                server.ConnectionContext.LoginSecure = false; //Set to false since we are using datbase authentication

                //Login credentials
                string login = (ConfigurationManager.AppSettings["username"].ToString());
                string password = (ConfigurationManager.AppSettings["password"].ToString());
                
                //server.InstanceName = @".\" + instanceName;
                
                server.ConnectionContext.Login = login;
                server.ConnectionContext.Password = password;
                server.ConnectionContext.Connect();

                //Job name
                jobName = (ConfigurationManager.AppSettings["jobName"].ToString());

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
                Console.WriteLine(String.Format("{0}Job Name : {1} has successfully completed", Environment.NewLine, jobName)); //Write to console saying we are done
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace); //Write to console the stack trace
            }
            finally
            {
                if (server.ConnectionContext.IsOpen)
                {
                    server.ConnectionContext.Disconnect();//Safely disconnect SQL Server
                    Console.WriteLine(String.Format("{0}Job Name : {1} has it's connection closed successfully", 
                        Environment.NewLine, jobName)); //Write to console saying we are done closing the SQL connection       
                }

            }
        }
    }
}
