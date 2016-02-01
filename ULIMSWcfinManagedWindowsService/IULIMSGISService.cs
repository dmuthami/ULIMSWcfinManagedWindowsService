using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace wcf.ulims.com.na
{
    [ServiceContract(Namespace = "http://wcf.ulims.com.na")]
    public interface IULIMSGISService
    {
        [OperationContract]
        string startGISSyncProcess(bool shallStart);

        [OperationContract]
        bool isSuccessGISSyncProcess();

        /// <summary>
        /// Method : executePythonProcess()
        /// Loops through a dictionary object listing the 10 piloting towns
        ///     For each town launches a process to perfom automatic reconcile and post
        /// </summary>
        /// 

        void executePythonProcess();
        /// <summary>
        /// Method : executePythonProcess(String townName)
        /// Creates a python process, passess it parameers and waits for completion. 
        /// Stdout from the python script is read asynchronously and captured into the .net log file
        /// </summary>
        /// <param name="townName"></param>
        /// <param name="pythonFileExecute"></param>  
        [OperationContract]
        void executePythonProcessPerTown(string townName, String pythonFileToExecute);
        /// <summary>
        /// Property : mExecutablePath
        /// Wrapped up in a getter and setter
        /// </summary>       

        bool mEexecuting { get; set; }
        /// <summary>
        /// Property : mExecutablePath
        /// Wrapped up in a getter and setter
        /// </summary>        

        string mExecutablePath { get; set; }
        /// <summary>
        /// Property : mExecutableRootDirectory
        /// Wrapped up in a getter and setter
        /// </summary>        

        string mExecutableRootDirectory { get; set; }
        /// <summary>
        /// Property : mNumOutputLines
        /// Wrapped up in a getter and setter
        /// </summary>        

        int mNumOutputLines { get; set; }
        /// <summary>
        /// Property : mPythonCodeFolder
        /// Wrapped up in a getter and setter
        /// </summary>        string mPythonCodeFolder { get; set; }

        System.Text.StringBuilder mSortOutput { get; set; }
        /// <summary>
        /// Create a log method (WriteErrorLog) to log the exceptions
        /// </summary>
        /// <param name="ex"></param>        

        void WriteErrorLog(Exception ex);
        /// <summary>
        /// Create a log method (WriteErrorLog) to log the custom messages
        /// </summary>
        /// <param name="Message"></param>        
        void WriteErrorLog(string Message);
        /// <summary>
        /// Property : mPythonCodeFolder
        /// Wrapped up in a getter and setter
        /// </summary>   
        string mPythonCodeFolder { get; set; }

        ULIMSGISWindowsService mCalculatorWindowsService { get; set; }

        bool MGISSyncProcess { get; set; }

        bool HasGISSyncProcessstarted { get; set; }

        ULIMSSerializer.ULIMSSerializer mULIMSSerializer { get; set; }

        void readObject();

        void saveObject(bool state1);
    }
}

