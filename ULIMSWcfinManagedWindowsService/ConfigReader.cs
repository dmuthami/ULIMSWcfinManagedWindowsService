using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;//Required to read config files
using System.Collections.Specialized; //NameValueCollection
using System.Collections;

namespace wcf.ulims.com.na
{
    class ConfigReader : IConfigReader
    {
        private IULIMSGISService mULIMSGISService;

        internal IULIMSGISService MULIMSGISService
        {
            get { return mULIMSGISService; }
            set { mULIMSGISService = value; }
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="MULIMSGISService"> The python library class</param>
        public ConfigReader(IULIMSGISService pythonLibrary)
        {
            try
            {
                //assign by ref the class level variable from the argument variable
                MULIMSGISService = pythonLibrary;
            }
            catch (Exception)
            {

                throw;//In case of an error then throws it up the stack trace
            }
        }
        /// <summary>
        /// Reads the app.config file. Specifically the section NamibiaLocalAuthorities and resturns a dictionary of the towns
        /// </summary>
        /// <returns>Dictionary: Contains list of towns</returns>
        public Dictionary<String, String> readNamibiaLocalAuthoritiesSection()
        {
            try
            {
                //Create a a dictionary object and intialized to null. This will cntain a list of the local authorities read from the appconfig file
                Dictionary<String, String> dictionary = null;

                //Read the section contain key value pairs of the namibia local authorities
                var NamibiaLocalAuthorities = ConfigurationManager.GetSection("NamibiaLocalAuthorities") as NameValueCollection;

                //Check that the returned name value collection of the Namibia local authorities is not empty
                if (NamibiaLocalAuthorities != null)
                {
                    //Call back method that converts named value collection to a dictionary.dictio
                    dictionary = hashtableToDictionary(NamibiaLocalAuthorities);

                    //Write to Log file indicating 
                    this.writeSectionToLog(dictionary);
                }
                //Pass by reference the dictionary object
                return dictionary;
            }
            catch (Exception)
            {

                throw;//In case of an error then throws it up the stack trace
            }
        }
        /// <summary>
        /// //Loop thru the name value collection and add the key, value pairs to the dictionary
        /// </summary>
        /// <param name="nameValueCollection">This is being read </param>
        /// <returns></returns>
        private Dictionary<String, String> hashtableToDictionary(NameValueCollection nameValueCollection)
        {
            try
            {
                //load items from hashtable to dictionary
                Dictionary<String, String> namibiaTownsDictionary = new Dictionary<String, String>();

                //Loop thru the collection and add the key value pairs to the dictionary
                foreach (string key in nameValueCollection)
                {
                    //Add the key,value pairs after casting to String
                    namibiaTownsDictionary.Add((String)key, (String)nameValueCollection[key]);
                }
                //return dictionary with towns in it
                return namibiaTownsDictionary;
            }
            catch (Exception)
            {

                throw;//In case of an error then throws it up the stack trace
            }
        }

        /// <summary>
        /// Write NamibiaLocalAuthorities section of config path to the log file
        /// </summary>
        /// <param name="dictionary"></param>
        private void writeSectionToLog(Dictionary<String, String> dictionary)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder("");
                stringBuilder.Append(Environment.NewLine + "Local authorities read from config file" + Environment.NewLine);

                //Loop over pairs with foreach loop
                foreach (KeyValuePair<string, string> townpair in dictionary)
                {
                    //Call function to execute python process for each town        
                    stringBuilder.Append(String.Format("Town : {0}", (String)townpair.Value));
                }
            }
            catch (Exception)
            {

                throw;//In case of an error then throws it up the stack trace
            }

        }
    }
}
