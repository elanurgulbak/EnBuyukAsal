using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using JTS.Contracts;
using JTS.Business;

namespace JTS.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "JobService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select JobService.svc or JobService.svc.cs at the Solution Explorer and start debugging.
    public class JobService : IJobService
    {
        public long GetNewJob(string mac)
        {
            return Job.GetNewJob(mac);
        }
        public void SaveJobResult(string mac,long n,bool isPrime)
        {
            Job.SaveResult(mac, n, isPrime);
        }
    }
}
