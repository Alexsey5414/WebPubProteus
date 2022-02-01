using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using WebTestProteus.Extensions;

namespace WebTestProteus.Classes
{


    public class ReportMetric
    {
        [Key]
        public int id { get; set; }
        public int NumId { get; set; }
        public string SessionId { get; set; }
        public string ReportName { get; set; }
        public int ThreadId { get; set; } = Thread.CurrentThread.ManagedThreadId;
        public string MeasureName {get; set;}
        public double Duration { get; set; }
        public DateTime LocalDataTime { get; set; } = DateTime.Now;
        public DateTime UtcDateTime { get; set; } = DateTime.UtcNow;
        public long LocalUnixTime { get; set; } = DateTime.Now.ToUnixTimestamp();
        public long UtcUnixTime { get; set; } = DateTime.UtcNow.ToUnixTimestamp();


    }
}
