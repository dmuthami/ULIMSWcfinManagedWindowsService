using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Configuration;//Process

using Utility.ulims.com.na;

namespace ulimsgispython.ulims.com.na
{
    /// <summary>
    /// PythonLibrary Implements IPythonLibrary
    /// Responsible for executing all python scripts
    /// Specifically, compute stand no and Auto Reconcile and Post scripts
    /// </summary>
    public class PythonLibrary : IPythonLibrary
    {

        #region Member Variables

        //Add memeber variables here
        private StringBuilder mSortOutput;

        //Variable serves as a memory pointer to the output lines being written to the console or log file
        private int mNumOutputLines;

        //Varible stores path to the python folder 
        private string mPythonCodeFolder;

        #endregion

        #region Getter and Setters

        /// <summary>
        /// Property : MSortOutput
        /// Wrapped up in a getter and setter
        /// </summary>
        public StringBuilder MSortOutput { get { return mSortOutput; } set { mSortOutput= value; } }

        /// <summary>
        /// Property : MNumOutputLines
        /// Wrapped up in a getter and setter
        /// </summary>
        public int MNumOutputLines { get { return mNumOutputLines; } set { mNumOutputLines = value; } }

        /// <summary>
        /// Property : MPythonCodeFolder
        /// Wrapped up in a getter and setter
        /// </summary>
        public string MPythonCodeFolder { get { return mPythonCodeFolder; } set { mPythonCodeFolder = value; } }

        #endregion

        #region Methods

        /// <summary>
        /// Method : executePythonProcess()
        /// Loops through a dictionary object listing the 10 piloting towns
        ///     For each town launches a process to perfom automatic reconcile and post
        /// </summary>
        public void executePythonProcess()
        {
            try
            {
                //Create a dictionary object with all the 10 piloting sites
                Dictionary<string, string> dictionary = new Dictionary<string, string>();

                //Read towns from config file
                IConfigReader iConfigReader = new ConfigReader();

                //Method returns listof towns as a dictionary
                dictionary = iConfigReader.readNamibiaLocalAuthoritiesSection();

                //Check if we can compute stand number
                Boolean computeStandNo = Convert.ToBoolean(ConfigurationManager.AppSettings["compute_stand_no"].ToString());


                //Check if we can conduct auto reconcile and post
                Boolean autoReconcileAndPost = Convert.ToBoolean(ConfigurationManager.AppSettings["auto_reconcile_and_post"].ToString());

                //Loop over pairs with foreach loop
                foreach (KeyValuePair<string, string> townpair in dictionary)
                {
                    //Check if we can compute stand number
                    if (computeStandNo == true)
                    {
                        //Call function to execute python process for computing stand number for each town    
                        executePythonProcessPerTown((String)townpair.Value, "InvokeComputeStandNo.py");
                    }
                    else
                    {

                        string msg = String.Format("{0}Execution of Compute Stand No : {1} has not been fired/started for {2}.", Environment.NewLine, computeStandNo.ToString(), townpair);
                        Logger.WriteErrorLog("PythonLibrary.executePythonProcess() : "+ msg);

                    }

                    //Check if we can conduct auto reconcile and post
                    if (autoReconcileAndPost == true)
                    {
                        //Call function to execute python process for auto reconcile for each town   
                        executePythonProcessPerTown((String)townpair.Value, "AutoReconcileAndPost.py");
                    }
                    else
                    {

                        string msg = String.Format("{0}Execution of Auto Reconcile and Post : {1} has not been fired/started for {2}.", Environment.NewLine, autoReconcileAndPost.ToString(), townpair);
                        Logger.WriteErrorLog("PythonLibrary.executePythonProcess() : " + msg);

                    }
                }


                //Call code to excute SQL job
                //Check if we can fire the SQL job
                Boolean sqlJob = Convert.ToBoolean(ConfigurationManager.AppSettings["sql_job"].ToString());

                //Check if we can conduct sql job execution
                //Read job name from appconfig file
                string jobName = (ConfigurationManager.AppSettings["jobName"].ToString());
                if (sqlJob == true)
                {
                    //Write error log
                    string msg = String.Format("{0}Execution of Job Name : {1} has been fired/started.", Environment.NewLine, jobName);
                    Logger.WriteErrorLog("PythonLibrary.executePythonProcess() : " + msg);

                    //Instantiate class that runs SQL Job
                    ISQLJob2 iSQLJob2 = new SQLJob2();
                    /*
                     * Set the folowing properties in the SQL Job object
                     */
                    iSQLJob2.JobName = jobName;//pass the job name as an argument
                    iSQLJob2.Login = (ConfigurationManager.AppSettings["username"].ToString());
                    iSQLJob2.Password = (ConfigurationManager.AppSettings["password"].ToString());
                    iSQLJob2.ServerName = (ConfigurationManager.AppSettings["servername"].ToString());
                    //Fire the execute method to run te SQL job
                    iSQLJob2.Execute();
                }
                else
                {

                    string msg = String.Format("{0}Execution of Job Name : {1} has not been fired/started.", Environment.NewLine, jobName);
                    Logger.WriteErrorLog("PythonLibrary.executePythonProcess() : " + msg);

                }

            }
            catch (Exception ex)
            {
                //In case of an error then throws it explicitly up the stack trace and add a message to the re-thrown error
                throw new Exception("PythonLibrary.executePythonProcess() : ", ex);
            }

        }

        /// <summary>
        /// Method : executePythonProcess(String townName)
        /// Creates a python process, passess it parameers and waits for completion. 
        /// Stdout from the python script is read asynchronously and captured into the .net log file
        /// </summary>
        /// <param name="townName"></param>
        /// <param name="pythonFileExecute"></param>  
        public void executePythonProcessPerTown(String townName, String pythonFileToExecute)
        {
            try
            {

                //Get path of config file
                String configFilePath = "\"" + Logger.ExecutableRootDirectory + String.Format("\\local_authorities\\{0}\\Config.ini", townName) + "\"";

                //Get path of main python file
                String pathToPythonMainFile = "\"" + Logger.ExecutableRootDirectory + String.Format("\\local_authorities\\{0}\\{1}", mPythonCodeFolder, pythonFileToExecute) + "\"";

                //Get path of reconcile log file
                String reconcileLogFilePath = "\"" + Logger.ExecutableRootDirectory + String.Format("\\local_authorities\\{0}\\{0}_reconcile.log", townName) + "\"";

                //Set path for current directory or strictly speaking directory of interest that you want to make current
                String currDirPath = "\"" + Logger.ExecutableRootDirectory + String.Format("\\local_authorities", "") + "\"";

                //Create an instance of Python Process class
                Process process = new Process();

                /*
                 * We execute python code
                 * Ensure python path is referenced in the Environment variables
                 */
                process.StartInfo.FileName = "python.exe";

                //make sure we can read output from stdout
                process.StartInfo.UseShellExecute = false;

                // Redirect the standard output of the sort command.  
                // This stream is read asynchronously using an event handler.
                process.StartInfo.RedirectStandardOutput = true;

                //intialize pointer to memory location storing a Stringbuilder object
                MSortOutput = new StringBuilder("");

                // Set our event handler to asynchronously read the sort output.
                process.OutputDataReceived += new DataReceivedEventHandler(sortOutputHandler);

                /*
                 * Start the program with 4 parameters.NB Use of escape characters to escape spaces in file paths
                 * 
                 * First argument: Tells pyhton which python script to execute
                 * Second argument: Supplys the path to the config file
                 * Third argument: Supplys the path to the reconcile log file
                 * Fourth argument: Supplies the current directory. NB. Different from current working directory
                 */
                process.StartInfo.Arguments = pathToPythonMainFile + " " + configFilePath + " " + reconcileLogFilePath + " " + currDirPath;

                //Start the process (i.e the python program)
                process.Start();

                // To avoid deadlocks, use asynchronous read operations on at least one of the streams.
                // Do not perform a synchronous read to the end of both redirected streams.
                process.BeginOutputReadLine();

                //Wait for the python process
                process.WaitForExit();

                Logger.WriteErrorLog("PythonLibrary.executePythonProcessPerTown(String townName, String pythonFileToExecute) : " + MSortOutput.ToString());//Write to dotnet log file

                //Releases all resources by the component
                process.Close();

            }
            catch (Exception ex)
            {

                //In case of an error then throws it explicitly up the stack trace and add a message to the re-thrown error
                throw new Exception("PythonLibrary.executePythonProcessPerTown(String townName, String pythonFileToExecute): ", ex);
            }

        }

        /// <summary>
        /// event Handler : sortOutputHandler
        /// Asynchronously captures ouptput writen to console by python
        /// </summary>
        /// <param name="sendingProcess"></param>
        /// <param name="outLine"></param> 
        private void sortOutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            try
            {
                // Collect the sort command output.
                if (!String.IsNullOrEmpty(outLine.Data))
                {
                    MNumOutputLines++; //concatenate

                    // Add the text to the collected output.
                    MSortOutput.Append(Environment.NewLine +
                        "[" + MNumOutputLines.ToString() + "] - " + outLine.Data);
                }
            }
            catch (Exception ex)
            {

                //In case of an error then throws it explicitly up the stack trace and add a message to the re-thrown error
                throw new Exception("PythonLibrary.sortOutputHandler(object sendingProcess, DataReceivedEventArgs outLine) : ", ex);
            }
        }

        #endregion


    }
}
