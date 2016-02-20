using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;//Process
using ulimsgispython.ulims.com.na;

namespace wcf.ulims.com.na
{
    public class ULIMSGISService : IULIMSGISService
    {
        #region Member Variables

        IPythonLibrary iPythonLibrary;

        #endregion


        #region Getter and Setter Methods
        public IPythonLibrary IPythonLibrary
        {
            get { return iPythonLibrary; }
            set { iPythonLibrary = value; }
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public ULIMSGISService()
        {
            IPythonLibrary = new PythonLibrary();
        }
        public async Task<bool> executePythonCodeAsync()
        {
            var task = Task.Factory.StartNew(() =>
            {
                return IPythonLibrary.executePythonProcess();
            });
            return await task.ConfigureAwait(false);
        }

    }
}
