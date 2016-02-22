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
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginExecutePythonCodeMethod( AsyncCallback callback, object asyncState);
        bool EndExecutePythonCodeMethod(IAsyncResult result);
        // TODO: Add your service operations here
    }
}

