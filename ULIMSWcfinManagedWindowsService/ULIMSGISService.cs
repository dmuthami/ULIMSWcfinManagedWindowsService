using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using ulimsgispython.ulims.com.na;//Process

namespace wcf.ulims.com.na
{
    public class ULIMSGISService : IULIMSGISService
    {

        #region Member Variables
        private string mFileName = null;


        private bool mGISSyncProcess;
        private ULIMSSerializer.ULIMSSerializer uLIMSSerializer = new ULIMSSerializer.ULIMSSerializer();

        IPythonLibrary mIPythonLibrary;


        #endregion

        #region Getter and Setters
        public IPythonLibrary MIPythonLibrary
        {
            get { return mIPythonLibrary; }
            set { mIPythonLibrary = value; }
        }
        public string MFileName
        {
            get { return mFileName; }
            set { mFileName = value; }
        }

        #endregion
        /// <summary>
        /// Property : mExecuting
        /// Wrapped up in a getter and setter
        /// </summary>       
        public bool mEexecuting { get; set; }

        public ULIMSGISService() { 

        }
        public ULIMSGISService(IPythonLibrary iPythonLibrary)
        {
            MIPythonLibrary = iPythonLibrary;
            try
            {
                SetFileNamePath();
            }
            catch (Exception  ex)
            {
                
                MIPythonLibrary.WriteErrorLog(ex.ToString());
            }
        }

        public void SetFileNamePath(){

            MFileName = MIPythonLibrary.ExecutableRootDirectory + @"\" + "SavedULIMSObjects.bin";
        }

        private void SetFileNamePath(bool local)
        {


            string executablePath = System.Reflection.Assembly.GetExecutingAssembly().Location;//get path to *.exe
            string eExecutableRootDirectory = System.IO.Path.GetDirectoryName(executablePath); // get directory containing the *.exe
            MFileName = eExecutableRootDirectory + @"\" + "SavedULIMSObjects.bin";
        }
        /// <summary>
        /// startGISSyncProcess Method: Exposed to the wcf clents 
        /// signals start of GIS processing
        /// </summary>
        /// <param name="shallStart"></param>
        /// <returns></returns>
        public string startGISSyncProcess(bool shallStart)
        {
            try
            {
                saveObject(shallStart);

                if (shallStart == true)
                {
                    MGISSyncProcess = false; //Tell the GIS to Sharepoint client to wait
                    return "GIS Synch Service process has started";

                }
                else
                {
                    return "GIS Synch Service process has not started";
                }
            }
            catch (Exception)
            {
                
                throw;//In case of an error then throws it up the stack trace
            }

        }

        /// <summary>
        /// property: MGISSyncProcess
        /// wrapped in a getter and setter
        /// </summary>
        public bool MGISSyncProcess
        {
            get { return mGISSyncProcess; }
            set
            {
                mGISSyncProcess = value;

                uLIMSSerializer.HasGISSyncProcessCompleted = value; // state of GIS Synch process complete      
                //persist to binary file
                Stream TestFileStream = File.Create(MFileName);
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(TestFileStream, mULIMSSerializer);
                TestFileStream.Close();
            }
        }

        public bool isSuccessGISSyncProcess()
        {
            try
            {
                //call method to read persistent object stored in the binary file
                readObject();
                return mULIMSSerializer.HasGISSyncProcessCompleted;
            }
            catch (Exception)
            {
                
                throw;//In case of an error then throws it up the stack trace
            }
        }

        private bool hasGISSyncProcessstarted;

        public bool HasGISSyncProcessstarted
        {
            get { return hasGISSyncProcessstarted; }
            set { hasGISSyncProcessstarted = value; }
        }            

        public ULIMSSerializer.ULIMSSerializer mULIMSSerializer
        {
            get { return uLIMSSerializer; }
            set { uLIMSSerializer = value; }
        }

        public void readObject()
        {
            try
            {
                if (String.IsNullOrEmpty(MFileName) == true) { SetFileNamePath(true); }
                if (File.Exists(MFileName))
                {
                    Stream TestFileStream = File.OpenRead(MFileName);
                    BinaryFormatter deserializer = new BinaryFormatter();
                    mULIMSSerializer = (ULIMSSerializer.ULIMSSerializer)deserializer.Deserialize(TestFileStream);
                    TestFileStream.Close();
                }
            }
            catch (Exception)
            {
                
                throw;//In case of an error then throws it up the stack trace
            }
        }

        public void saveObject(bool state1)
        {
            try
            {
                if (String.IsNullOrEmpty(MFileName) == true) { SetFileNamePath(true); }

                mULIMSSerializer.HasGISSyncProcessstarted = state1; // state of GIS Synch process start      

                Stream TestFileStream = File.Create(MFileName);
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(TestFileStream, mULIMSSerializer);
                TestFileStream.Close();
            }
            catch (Exception)
            {
                
                throw;//In case of an error then throws it up the stack trace
            }
        }

        public void saveObject(string filePathForSerializedObject)
        {
            try
            {
                mULIMSSerializer.mFileName = filePathForSerializedObject; // state of GIS Synch process start      

                Stream TestFileStream = File.Create(MFileName);
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(TestFileStream, mULIMSSerializer);
                TestFileStream.Close();

                MIPythonLibrary.WriteErrorLog(Environment.NewLine + "Serialized Object File Path :" + MFileName);
            }
            catch (Exception)
            {

                throw;//In case of an error then throws it up the stack trace
            }
        }

    }
}
