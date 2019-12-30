using System;
using System.Linq;

namespace JTS.Business
{
    public class Job
    {
        public static long GetNewJob(string mac)
        {
            using (JTSDataContext dc = new JTSDataContext())
            {
                //dc.JOBs;
                //var n = (from c in dc.JOBs
                //         where c.STATUS == 0 &&
                //         c.ASSIGN_DATE < DateTime.Now.AddHours(-1) &&
                //         !dc.JOBs.Any(k => k.STATUS != 0 && k.N == c.N) &&
                //         !dc.JOBs.Where(k=>k.STATUS==0 &&
                //         k.ASSIGN_DATE<DateTime.Now.AddHours(-1)).Select(k=>k.N).Contains(c.N)
                //         select c.N).FirstOrDefault();
                //if (n > 0) 
                //{
                //    Job.AddJob(dc, mac, n);
                //    return n;
                //}
                //Job.AddJob(dc, mac, n);
                //return 3;
                var hesaplanmisNDegerleri = dc.JOBs.Where(c => c.STATUS != 0).Select(c => c.N);

                var tarih = DateTime.Now.AddHours(-1);

                // Atama işleminden sonra henüz 1 saat geçmemiş N değerleri
                var henuzAtanmisNDegerleri = dc.JOBs.Where(c => c.ASSIGN_DATE > tarih).Select(c => c.N);

                //var N = (from c in dc.JOBs
                //             // Hesaplanmamış ve atamadan sonra 1 saat geçmiş
                //         where c.STATUS == 0 && c.ASSIGN_DATE < DateTime.Now.AddHours(-1)
                //         && !hesaplanmisNDegerleri.Contains(c.N)
                //         && !henuzAtanmisNDegerleri.Contains(c.N)
                //         select c.N).FirstOrDefault();

                var n = (from c in dc.JOBs
                         where c.STATUS == 0 && c.ASSIGN_DATE < DateTime.Now.AddHours(-1)
                            && !dc.JOBs.Any(k => (k.N == c.N && (k.STATUS != 0 || k.ASSIGN_DATE >=
                            tarih)))
                         select c.N).FirstOrDefault();

                if (n > 0)
                {
                    Job.AddJob(dc, mac, n);
                    return n;
                }

                n = (from c in dc.JOBs
                     orderby c.N descending
                     select c.N).FirstOrDefault();

                if (n > 0)
                {
                    Job.AddJob(dc, mac, n + 2);
                    return n + 2;
                }

                Job.AddJob(dc, mac, 3);
                return 3;
            }
                
        }

        public static void AddJob(JTSDataContext dc,string mac,long n)
        {
            dc.JOBs.InsertOnSubmit(new JOB
            {
                JOB_ID=Guid.NewGuid(),
                MAC=mac,
                N=n,
                ASSIGN_DATE=DateTime.Now,
                STATUS=0
            });
            dc.SubmitChanges();
        }
        public static void SaveResult(string mac,long n,bool isPrime)
        {
            using(JTSDataContext dc=new JTSDataContext())
            {
                var item = (from c in dc.JOBs
                            where c.MAC == mac &&
                            c.N == n && c.STATUS == 0
                            orderby c.ASSIGN_DATE descending
                            select c).FirstOrDefault();
                if(item==null)
                {
                    return;
                }
                item.STATUS = isPrime ? (byte)1 : (byte)2;
                item.COMPLETE_DATE = DateTime.Now;
                dc.SubmitChanges();
            }
        }
    }
}
