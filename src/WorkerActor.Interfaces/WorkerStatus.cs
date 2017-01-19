using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WorkerActor.Interfaces
{
    [DataContract]
    public class WorkerStatus
    {
        [DataMember]
        public bool IsCompleted { get; set; }

        [DataMember]
        public double Progress { get; set; }

        [DataMember]
        public string Result { get; set; }
    }
}
