using System;
namespace ulimsgispython.ulims.com.na
{
    interface IConfigReader
    {
        /// <summary>
        /// Reads the app.config file. Specifically the section NamibiaLocalAuthorities and resturns a dictionary of the towns
        /// </summary>
        /// <returns>Dictionary: Contains list of towns</returns>
        System.Collections.Generic.Dictionary<string, string> readNamibiaLocalAuthoritiesSection();
    }
}
