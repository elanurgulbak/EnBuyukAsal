using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace JTS.Contracts
{
    [ServiceContract]
    public interface IJobService
    {
        [OperationContract]
        long GetNewJob(string mac);
        [OperationContract]
        void SaveJobResult(string mac, long n, bool isPrime);
    }
}
