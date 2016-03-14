﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using ulimsgispython.ulims.com.na;//Process

using Utility.ulims.com.na; /*Utility assembly*/

namespace wcf.ulims.com.na
{
    public class ULIMSGISService : IULIMSGISService
    {

        #region Member Variables

        private bool mGISSyncProcess;

        private bool hasGISSyncProcessstarted;

        private ULIMSSerializer.ULIMSSerializer uLIMSSerializer = new ULIMSSerializer.ULIMSSerializer();

        IPythonLibrary mIPythonLibrary;

        #endregion

        #region Getter and Setters
        /// <summary>
        /// Property : MIPythonLibrary
        /// Wrapped up in a getter and setter
        /// </summary>
        public IPythonLibrary MIPythonLibrary
        {
            get
            {
                try
                {
                    return mIPythonLibrary;
                }
                catch (Exception ex)
                {

                    //In case of an error then throws it explicitly up the stack trace and add a message to the re-thrown error
                    throw new Exception("ULIMSGISService.IPythonLibrary MIPythonLibrary : get ", ex);
                }
            }
            set
            {
                try
                {
                    mIPythonLibrary = value;
                }
                catch (Exception ex)
                {

                    //In case of an error then throws it explicitly up the stack trace and add a message to the re-thrown error
                    throw new Exception("ULIMSGISService.IPythonLibrary MIPythonLibrary : set ", ex);
                }
            }
        }

        /// <summary>
        /// Property : mExecuting
        /// Wrapped up in a getter and setter
        /// </summary>       
        public bool mEexecuting { get; set; }

        #endregion

        #region Constructors
        public ULIMSGISService()
        {

        }
        #endregion

        #region Methods

        /// <summary>
        /// Method : SetFileNamePath
        /// Sets the path to the binary file seriliazing the ULIMSSerialize.cs
        /// </summary>
        /// <param name="local"> pass true</param>
        private void SetFileNamePath(bool local)
        {
            try
            {
                Logger.MFileName = Logger.ExecutableRootDirectory + @"\" + "SavedULIMSObjects.bin";
            }
            catch (Exception ex)
            {

                //In case of an error then throws it explicitly up the stack trace and add a message to the re-thrown error
                throw new Exception("ULIMSGISService.SetFileNamePath(bool local) ", ex);
            }
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
            catch (Exception ex)
            {

                //In case of an error then throws it explicitly up the stack trace and add a message to the re-thrown error
                throw new Exception("ULIMSGISService.startGISSynchProcess(bool shallStart) ", ex);
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
                try
                {
                    mGISSyncProcess = value;

                    uLIMSSerializer.HasGISSyncProcessCompleted = value; // state of GIS Synch process complete      
                    //persist to binary file
                    Stream TestFileStream = File.Create(Logger.MFileName);
                    BinaryFormatter serializer = new BinaryFormatter();
                    serializer.Serialize(TestFileStream, mULIMSSerializer);
                    TestFileStream.Close();
                }
                catch (Exception ex)
                {
                    //In case of an error then throws it explicitly up the stack trace and add a message to the re-thrown error
                    throw new Exception("ULIMSGISService.MGISSyncProcess ", ex);
                }
            }
        }

        /// <summary>
        /// Property : isSuccessGISSyncProcess
        /// Exposed to the WCF client
        /// Wrapped up in a getter and setter
        /// </summary>
        public bool isSuccessGISSyncProcess()
        {
            try
            {
                //call method to read persistent object stored in the binary file
                readObject();
                return mULIMSSerializer.HasGISSyncProcessCompleted;
            }
            catch (Exception ex)
            {

                //In case of an error then throws it explicitly up the stack trace and add a message to the re-thrown error
                throw new Exception("ULIMSGISService.isSuccessGISSyncProcess() ", ex);
            }
        }

        /// <summary>
        /// Property : HasGISSyncProcessstarted
        /// Wrapped up in a getter and setter
        /// </summary>
        public bool HasGISSyncProcessstarted
        {
            get
            {
                try
                {
                    return hasGISSyncProcessstarted;
                }
                catch (Exception ex)
                {

                    //In case of an error then throws it explicitly up the stack trace and add a message to the re-thrown error
                    throw new Exception("ULIMSGISService.HasGISSyncProcessstarted : get ", ex);
                }
            }
            set
            {
                try
                {
                    hasGISSyncProcessstarted = value;
                }
                catch (Exception ex)
                {

                    //In case of an error then throws it explicitly up the stack trace and add a message to the re-thrown error
                    throw new Exception("ULIMSGISService.HasGISSyncProcessstarted : set ", ex);
                }
            }
        }

        /// <summary>
        /// Property : ULIMSSerializer.ULIMSSerializer mULIMSSerializer
        /// Wrapped up in a getter and setter
        /// </summary>
        public ULIMSSerializer.ULIMSSerializer mULIMSSerializer
        {
            get
            {
                try
                {
                    return uLIMSSerializer;
                }
                catch (Exception ex)
                {
                    //In case of an error then throws it explicitly up the stack trace and add a message to the re-thrown error
                    throw new Exception("ULIMSGISService.ULIMSSerializer.ULIMSSerializer mULIMSSerializer : get ", ex);
                }
            }
            set
            {
                try
                {
                    uLIMSSerializer = value;
                }
                catch (Exception ex)
                {

                    //In case of an error then throws it explicitly up the stack trace and add a message to the re-thrown error
                    throw new Exception("ULIMSGISService.ULIMSSerializer.ULIMSSerializer mULIMSSerializer : set ", ex);
                }
            }
        }

        /// <summary>
        /// Method : readObject()
        /// </summary>
        public void readObject()
        {
            try
            {
                // Check path to the binary file serializing the objec
                if (String.IsNullOrEmpty(Logger.MFileName) == true) { SetFileNamePath(true); }

                if (File.Exists(Logger.MFileName))//if file does exist 
                {
                    //Create object into RAM from  the binary file
                    Stream TestFileStream = File.OpenRead(Logger.MFileName);
                    BinaryFormatter deserializer = new BinaryFormatter();
                    mULIMSSerializer = (ULIMSSerializer.ULIMSSerializer)deserializer.Deserialize(TestFileStream);
                    TestFileStream.Close();
                }
            }
            catch (Exception ex)
            {

                //In case of an error then throws it explicitly up the stack trace and add a message to the re-thrown error
                throw new Exception("ULIMSGISService.readObject() : ", ex);
            }
        }

        /// <summary>
        /// Method saveObject
        /// Pass appropriate state as an argument 
        /// </summary>
        /// <param name="filePathForSerializedObject"> path to the binary file serializing the object</param>
        public void saveObject(bool state1)
        {
            try
            {
                if (String.IsNullOrEmpty(Logger.MFileName) == true) { SetFileNamePath(true); }

                mULIMSSerializer.HasGISSyncProcessstarted = state1; // state of GIS Synch process start   is set    

                //Save the state in the binary file
                Stream TestFileStream = File.Create(Logger.MFileName);
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(TestFileStream, mULIMSSerializer);
                TestFileStream.Close();
            }
            catch (Exception ex)
            {

                //In case of an error then throws it explicitly up the stack trace and add a message to the re-thrown error
                throw new Exception("ULIMSGISService.saveObject(bool state1) : ", ex);
            }
        }

        /// <summary>
        /// Method saveObject
        /// Pass appropriate state as an argument 
        /// </summary>
        /// <param name="filePathForSerializedObject"> path to the binary file serializing the object</param>
        public void saveObject(string filePathForSerializedObject)
        {
            try
            {

                Logger.MFileName = filePathForSerializedObject; // state of GIS Synch process start      

                //Save the state in the binary file
                Stream TestFileStream = File.Create(Logger.MFileName);
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(TestFileStream, mULIMSSerializer);
                TestFileStream.Close(); 

                Logger.WriteErrorLog(Environment.NewLine + "Serialized Object File Path :" + Logger.MFileName);
            }
            catch (Exception ex)
            {

                //In case of an error then throws it explicitly up the stack trace and add a message to the re-thrown error
                throw new Exception("ULIMSGISService.saveObject(string filePathForSerializedObject) : ", ex);
            }
        }
        
        #endregion

    }
}
