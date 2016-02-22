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
        public IAsyncResult BeginExecutePythonCodeMethod( AsyncCallback callback, object asyncState)
        {
            var task = Task<bool>.Factory.StartNew((res) => MyBeginExecutePythonCodeMethod(asyncState), asyncState);
            return task.ContinueWith(res => callback(task));
        }

        public bool EndExecutePythonCodeMethod(IAsyncResult result)
        {
            return ((Task<bool>)result).Result;
        }

        private bool MyBeginExecutePythonCodeMethod(object asyncState)
        {         
            return IPythonLibrary.executePythonProcess();
        }

    }
}
