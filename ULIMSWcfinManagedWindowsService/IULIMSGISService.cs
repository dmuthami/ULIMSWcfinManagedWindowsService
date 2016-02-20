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
        Task<bool> executePythonCodeAsync();
        // TODO: Add your service operations here
    }
}

