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

using Utility.ulims.com.na;


namespace ulimsgispython.ulims.com.na
{
    class SQLJob2
    {
        //Name of the job;
        string m_JobName = null;
        string m_InstanceName = null;
        string m_ServerName = null;
        string m_Login = null;
        string m_Password = null;

       IPythonLibrary iPythonLibrary;

        public ulimsgispython.ulims.com.na.IPythonLibrary IPythonLibrary
        {
            get { return iPythonLibrary; }
            set { iPythonLibrary = value; }
        }

        public string Password
        {
            get { return m_Password; }
            set { m_Password = value; }
        }
        public string Login
        {
            get { return m_Login; }
            set { m_Login = value; }
        }
        

        public string InstanceName
        {
            get { return m_InstanceName; }
            set { m_InstanceName = value; }
        }
        
        public string ServerName
        {
            get { return m_ServerName; }
            set { m_ServerName = value; }
        }

        public string JobName
        {
            get { return m_JobName; }
            set { m_JobName = value; }
        }

        public SQLJob2()
        {

        }

        public SQLJob2(string jobName, string instanceName, string serverName, string login, string password, IPythonLibrary iPythonLibrary)
        {
            JobName = jobName;
            InstanceName=instanceName;
            ServerName=serverName;
            Login = login;
            Password= password;
            IPythonLibrary = iPythonLibrary;
        }
        public void Execute()
        {
            ServerConnection serverConnection = new ServerConnection(ServerName);
            Server server = new Server(); //Default instance
            try
            {
                server.ConnectionContext.LoginSecure = false; //Set to false since we are using datbase authentication

                //Login credentials
                server.ConnectionContext.Login = Login; 
                server.ConnectionContext.Password = Password;
                server.ConnectionContext.Connect();


                //Get job instance
                Job job = server.JobServer.Jobs[JobName];

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
                    Logger.WriteErrorLog(String.Format("{0}Job Name : {1} is processing. Please wait for {2} milliseconds", Environment.NewLine, JobName, threadWait));
                    Thread.Sleep(threadWait);
                    job.Refresh();
                }
                //Log message
                Logger.WriteErrorLog(String.Format("{0}Job Name : {1} has successfully completed", Environment.NewLine, JobName)); //Write to console saying we are done
            }
            finally
            {
                if (server.ConnectionContext.IsOpen) //Check if still connection is open
                {
                    server.ConnectionContext.Disconnect();//Safely disconnect SQL Server
                    Logger.WriteErrorLog(String.Format("{0}Job Name : {1} has successfully completed", Environment.NewLine, JobName)); //Write to console saying we are done
                   
                }

            }
        }
    }
}
