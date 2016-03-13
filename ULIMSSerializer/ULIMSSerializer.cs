using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ULIMSSerializer
{
    [Serializable()]
    public class ULIMSSerializer
    {
       public bool  HasGISSyncProcessstarted { get; set; }

       public bool HasGISSyncProcessCompleted { get; set; }

       public string mFileName { get; set; }
    }
}
