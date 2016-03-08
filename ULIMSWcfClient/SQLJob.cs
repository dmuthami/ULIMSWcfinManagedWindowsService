using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;


namespace ulimsgispython
{
    class SQLJob
    {

        #region User Variables

        const int timeoutInterval = 60;//Set Timeout in seconds
        static readonly string SqlServer = @"Server112\Dev,20481"; //set SqlServer, may use instance/port too eg: USPLSVUL156\Operations,20481
        static readonly string SqlAgentJobName = "Some_SqlAgentJob_J250"; //set name of the job to fire

        #endregion


        static bool loopContinuity = false;
        static Timer stateTimer;
        static int CurrentRunRetryAttempt = 0;
        static ServerConnection conn;
        static Server server;
        static Job job;

        private void ExecuteSQLJob()
        {
            //Enable Timer
            SetTimer();
            try
            {
                conn = new ServerConnection(SqlServer); //Create SQL server conn, Windows Authentication
                server = new Server(conn); //Connect SQL Server
                job = server.JobServer.Jobs[SqlAgentJobName]; //Get the specified job
                StartJob();
            }
            catch (Exception ex)
            {
                SetTimer(true);
                Console.WriteLine("Failed to start the job :" + ex.Message);
                throw ex;
            }
            finally
            {
                Destroyobjects();
            }
        }

        static void Destroyobjects()
        {
            if (job != null)
                job = null;
            if (server != null)
                server = null;
            if (conn != null)
            {
                conn.Disconnect();
                conn = null;
            }
        }
        private static void SetTimer(bool cancel = false)
        {
            if (!cancel)//Initiate
            {
                stateTimer = new Timer(timeoutInterval * 1000);//Set Timeout interval as timeoutInterval
                stateTimer.Enabled = true;//Enable timer
                stateTimer.Elapsed += new ElapsedEventHandler(Tick); //Set timer elapsed event handler

            }
            else //Disable timer
            {
                if (stateTimer != null)
                    stateTimer.Dispose();

            }
        }

        static public void Tick(object source, ElapsedEventArgs e)
        {

            loopContinuity = true;//normal stop...
            Console.WriteLine(string.Format("Timeout reached at {0){1}CurrentRunRetryAttempt={2}", e.SignalTime, Environment.NewLine, CurrentRunRetryAttempt));
            throw new Exception(string.Format("Timeout reached at {0){1}CurrentRunRetryAttempt={2}", e.SignalTime, Environment.NewLine, CurrentRunRetryAttempt));// comment this line if we do not want an abrupt stop
        }

        static void StartJob()
        {

            try
            {
                while (loopContinuity == false) //Wait till the job is idle
                {
                    job.Refresh();
                    if (job.CurrentRunStatus == JobExecutionStatus.Executing) //Check Job status and find if it’s running now
                    {
                        CurrentRunRetryAttempt++;
                        //We are not ready to fire the job
                        loopContinuity = false;
                        System.Threading.Thread.Sleep(10 * 1000); //Wait 10 secs before we proceed to check it again.
                    }
                    else
                    {
                        //We are ready to fire the job
                        loopContinuity = true; //Set loop exit
                        try
                        {
                            job.Start();//Start the job
                            SetTimer(true);//disable timer if we are able to start the job, i.e. there’s no exception on starting the job.
                        }
                        catch
                        {
                            loopContinuity = false; //Fail to start, continue to loop.
                            System.Threading.Thread.Sleep(10 * 1000); //Fail to start, wait 10 seconds and try again
                        }

                    }
                    string s = "CurrentRunStatus=" + job.CurrentRunStatus.ToString();
                    Console.WriteLine(s); //Print status             

                }
            }
            catch
            {
                throw;
            }
        }

    }
}
