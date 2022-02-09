using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebTestProteus.Interfaces;

namespace WebTestProteus.Classes
{
     public class Dalayer : IDelayer, IDisposable
    {
        public delegate void DalayerHandler(ReportMetric rm);
        public event DalayerHandler OnDelay;
        Stopwatch sw;
        ReportMetric _rm;
        public Dalayer(ReportMetric rm)
        {
            _rm = rm;
            sw = Stopwatch.StartNew();

        }
        public void Dispose()
        {
            sw.Stop();
            _rm.Duration = sw.ElapsedMilliseconds;
            OnDelay?.Invoke(_rm);
        }

    }

}
