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
        /// <summary>
        /// Property : mExecuting
        /// Checks that 
        /// Wrapped up in a getter and setter
        /// </summary>       
        bool mEexecuting { get; set; }

        [OperationContract]
        string startGISSyncProcess(bool shallStart);

        bool MGISSyncProcess { get; set; }

        [OperationContract]
        bool isSuccessGISSyncProcess();

        bool HasGISSyncProcessstarted { get; set; }

        ULIMSSerializer.ULIMSSerializer mULIMSSerializer { get; set; }

        void readObject();

        void saveObject(bool state1);
    }
}

