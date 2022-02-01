using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace WebTestProteus.Classes
{
    using SimpleJsonDataSource.Models;
    using System.Text.RegularExpressions;
    using WebTestProteus.Interfaces;
    using WebTestProteus.Models.DataJson;

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

    public class ApiContext : DbContext, IApiContext
    {
        static object locker = new object();

        // System.Runtime.Caching.MemoryCache _cache;
        private IMemoryCache _cache;

        //    private string[] Metrics = new string[] { "load", "prepare", "runmacros", "export"};
        public ApiContext(DbContextOptions<ApiContext> options, IMemoryCache cache)

            : base(options)
        {
            _cache = cache;
            //  _cache = new System.Runtime.Caching.MemoryCache(nameof(ApiContext));
        }

        //private DateTime GetPreviousDateTime() => _cache.GetOrCreate<DateTime>(Thread.CurrentThread.ManagedThreadId.ToString(),
        //    entry =>
        //    {
        //        entry.SlidingExpiration =  TimeSpan.FromSeconds(60);
        //        return DateTime.UtcNow;
        //    });
        //_cache.AddOrGetExisting("test", 123, new CacheItemPolicy() { SlidingExpiration = new TimeSpan(0, 0, 60)});

        //private void SetCurrentDateTime(DateTime dt) => _cache.Set(Thread.CurrentThread.ManagedThreadId.ToString(), dt,
        //                                                         new CacheItemPolicy() { SlidingExpiration = new TimeSpan(0, 0, 60) });

        //private void SetCurrentDateTime(DateTime dt) => _cache.Set<DateTime>(Thread.CurrentThread.ManagedThreadId.ToString(), dt, 
        //    new MemoryCacheEntryOptions() { SlidingExpiration  = TimeSpan.FromSeconds(60) });
        //public void Begin()
        //{
        //    string threadid_key = Thread.CurrentThread.ManagedThreadId.ToString();
        //    if (cache.Get(threadid_key) == null)
        //    {
        //        cache.Add(threadid_key, DateTime.UtcNow, new CacheItemPolicy()
        //        { AbsoluteExpiration = DateTime.UtcNow.AddSeconds(600) });
        //    }
        //    else
        //        cache.Set(threadid_key, DateTime.UtcNow, new CacheItemPolicy()
        //        { AbsoluteExpiration = DateTime.UtcNow.AddSeconds(600) });

        //    //    cache.Add(key, value, new CacheItemPolicy()
        //    //    { AbsoluteExpiration = DateTime.UtcNow.AddSeconds(20) });
        //    //}
        //}


        /*
         public IDictionary<T, Y> GetReportMerics<T, Y>(Func<T, T> func, string value) 
         {
            return ReportMetrics.AsEnumerable().
                                 TakeLast(500).
                                  GroupBy(g=>func(g)).
                                  OrderBy(g => g.Key).
                                  ToDictionary(k => k.Key, v => value);
         }      
        */
        public IEnumerable<string> GetThreads() => ReportMetrics.
                                                    AsEnumerable().
                                                    TakeLast(1500).
                                                    GroupBy(g => g.ThreadId).
                                                    OrderBy(g => g.Key).
                                                    Select(s => $"thread{s.Key}");
                                                    
                                                   // ToDictionary(k => k.Key.ToString(), v => "thread");
        //  Select(s => $"thread{s.Key}");
        private IEnumerable<string> GetReports() => ReportMetrics.
                                                AsEnumerable().
                                                TakeLast(1500).
                                                GroupBy(g => g.ReportName).
                                                OrderBy(g => g.Key).
                                                Select(s=> s.Key);
                                               // ToDictionary(k => k.Key, v => "report");
        //  Select(s => s.Key);
        private IEnumerable<string> GetOperations() => new string[] { "load", "prepare", "runmacros", "export" };

        private IEnumerable<string> GetParameters() => new string[] { "load", "prepare", "runmacros", "export" };

        private IEnumerable<string> GetThreadCount() => new string[] { "thread_count" };

        private IEnumerable<string> GetReportCount() => GetReports();//new string[] { "report_count" };

        private IEnumerable<string> GetReportDuration() => GetReports();//new string[] { "report_duration" };

        //new Dictionary<string, string>() { { "thread_count", string.Empty } };
        // "max_thread1", "max_thread2","max_thread3"
        public IEnumerable<string> GetMeasures() => new string[] { "Operations", "Parameters", "ReportCount", "ReportDuration", "ThreadCount", "Threads" };

        private IEnumerable<string> GetEmpty() => new string[] { };
        //{ "tms_load", "tms_export", "tms_runmacros",
        //"msr_load", "msr_export", "msr_runmacros",
        //"thread_count"}
        //.Union(ReportMetrics.AsEnumerable().TakeLast(100)
        //.GroupBy(g=>g.ThreadId)
        //.Select(s=>$"Thread{s.Key}")).ToList();

        private object[][] RunFunc<T, Y>(Func<List<DataPointTimeSeries<T, Y>>> func)
        {
            return func()?.Select(p => new object[] { p.Value, p.Measure }).ToArray();
        }

       public object[][] GetDataPointSource(KeyValuePair<string, string> kvpMeasureName, DateTime startDateTime, DateTime endDateTime)
       {
           object[][] result = null;
          // string _compMeasureName = Regex.Replace(MeasureName, @"\d", "").Trim();
          // string _origMesureName = MeasureName.Replace("ots_", "").Replace("pts_", "");
           switch (kvpMeasureName.Value)
           {
                case "operation":
                    result = RunFunc(() => GetDataByTimeSeries(startDateTime, endDateTime, kvpMeasureName.Key));
                //result = GetDataByTimeSeries(startDateTime, endDateTime, kvpMeasureName.Key)
                //           .Select(p => new object [] { p.Value, p.Measure }).ToArray();
                   break;
                case "parameter":
                    result = RunFunc(() => GetDataByMeasures(startDateTime, endDateTime, kvpMeasureName.Key));
                              //.Select(p => new object[] { p.Value, p.Measure }).ToArray();

                    break;
                case "reportcount":
                    result = RunFunc(() => GetCallReportCount(startDateTime, endDateTime, kvpMeasureName.Key));
                    //.Select(p => new object[] { p.Value, p.Measure }).ToArray();
                    break;

                case "reportduration":
                    result = RunFunc(() => GetCallMaxTimeReport(startDateTime, endDateTime, kvpMeasureName.Key));
                    //.Select(p => new object[] { p.Value, p.Measure }).ToArray();
                    break;


                case "threadcount":
                    result = RunFunc(() => GetDataThreadCount(startDateTime, endDateTime));
                            //.Select(p => new object[] { p.Value, p.Measure }).ToArray();
                   break;
               case "thread":
                    result = RunFunc(() => GetDataThread(startDateTime, endDateTime, kvpMeasureName.Key));
                            //.Select(p => new object[] { p.Value, p.Measure }).ToArray();
                   break;

               default:
                  break;
           }
           return result;
       }

       private IDictionary<string, string> RunFunc(Func<IEnumerable<string>> func, string MeasureName)
        {
            return func()?.ToDictionary(k => k, v => MeasureName);
        }
       public IDictionary<string, string> GetMetrics(string MeasureName)
       {
            IDictionary<string, string> result = null;
           switch (MeasureName)
           {
               case "Operations":
                    result = RunFunc(() => GetOperations(), "operation");//.ToDictionary(k=>k, v=>"operation");//new List<string>() { "ots_load", "ots_export", "ots_runmacros" };
                   break;
                case "Parameters":
                    result = RunFunc(() => GetParameters(), "parameter");//.ToDictionary(k=>k, v=>"operation");//new List<string>() { "ots_load", "ots_export", "ots_runmacros" };
                    break;
                case "ReportCount":
                   result = RunFunc(() => GetReportCount(), "reportcount");  //new List<string>() { "pts_load", "pts_export", "pts_runmacros" };
                   break;

                case "ReportDuration":
                    result = RunFunc(() => GetReportDuration(), "reportduration");  //new List<string>() { "pts_load", "pts_export", "pts_runmacros" };
                    break;


                case "ThreadCount":
                    result = RunFunc(() => GetThreadCount(), "threadcount");// new string[] { "thread_count" };
                   break;
               case "Threads":
                   result = RunFunc(()=> GetThreads(), "thread"); //.ToList();
                   break;
               default:
                   result = RunFunc(() => GetEmpty(), "empty");
                   break;

           }
           return result;
       }
       /*
       public object[][] GetDataPointSource(string MeasureName, DateTime startDateTime, DateTime endDateTime)
       {
           object[][] result = null;
           string _compMeasureName = Regex.Replace(MeasureName, @"\d", "").Trim();
           string _origMesureName = MeasureName.Replace("ots_", "").Replace("pts_", "");
           switch (_compMeasureName)
           {
               case "ots_load": case "ots_export":
               case "ots_runmacros":
                   result = GetDataByTimeSeries(startDateTime, endDateTime, _origMesureName)
                           .Select(p => new object [] { p.Value, p.Measure }).ToArray();
                   break;
               case "pts_load": case "pts_export": case "pts_runmacros":
                   result = GetDataByMeasures(startDateTime, endDateTime, _origMesureName)
                            .Select(p => new object[] { p.Value, p.Measure }).ToArray();

                   break;
                 case "thread_count":
                   result = GetDataCountThreads(startDateTime, endDateTime)
                            .Select(p => new object[] { p.Value, p.Measure }).ToArray();

                   break;
               case "thread":
                  result =  GetDataThread(startDateTime, endDateTime, _origMesureName)
                            .Select(p => new object[] { p.Value, p.Measure }).ToArray();
                   break;

               default:
                  break;
           }
           return result;
       }
       */
        //        => ReportMetrics.Where(p => p.MeasureName.Contains(MeasureName)
        //                              && (p.UtcDateTime >= startDateTime)
        //                             && (p.UtcDateTime <= endDateTime)).Select(p => new DataPoint<double>(p.UtcUnixTime,/* p.UtcUnixTime,*/ p.Duration)).ToList();
        public List<DataPointTimeSeries<double, long>> GetDataByTimeSeries(DateTime startDateTime, DateTime endDateTime, string MeasureName)
              => ReportMetrics.Where(p => (string.IsNullOrEmpty(MeasureName) || MeasureName.Contains(p.MeasureName)) 
                                    && (p.UtcDateTime >= startDateTime)
                                   && (p.UtcDateTime <= endDateTime)).Select(p => new DataPointTimeSeries<double, long>(p.Duration, p.UtcUnixTime)).ToList();

        public List<DataPointTimeSeries<double, string>> GetDataByMeasures(DateTime startDateTime, DateTime endDateTime, string MeasureName)
            => ReportMetrics.Where(p => (string.IsNullOrEmpty(MeasureName) || MeasureName.Contains(p.MeasureName))
                                  && (p.UtcDateTime >= startDateTime)
                                 && (p.UtcDateTime <= endDateTime))
                .Select(p => new DataPointTimeSeries<double, string>(p.Duration, p.MeasureName)).ToList();

        public List<DataPointTimeSeries<int, string>> GetCallReportCount(DateTime startDateTime, DateTime endDateTime, string ReportName = "")
            => ReportMetrics.Where(p => (string.IsNullOrEmpty(ReportName) || ReportName.Contains(p.ReportName))
                                  && (p.UtcDateTime >= startDateTime)
                                  && (p.UtcDateTime <= endDateTime)).
                                  GroupBy(g => new { g.ReportName, g.SessionId }).
                                  Select(g => new { MeasureName = g.Key.ReportName, Count = g.Count()}).
                                  Select(p => new DataPointTimeSeries<int, string>(p.Count, p.MeasureName)).ToList();
        public List<DataPointTimeSeries<double, string>> GetCallMaxTimeReport(DateTime startDateTime, DateTime endDateTime, string ReportName = "")
            => ReportMetrics.Where(p => (string.IsNullOrEmpty(ReportName) || ReportName.Contains(p.ReportName))
                                  && (p.UtcDateTime >= startDateTime)
                                  && (p.UtcDateTime <= endDateTime)).
                                  GroupBy(g => new { g.ReportName, g.SessionId }).
                                  Select(g => new { MeasureName = g.Key.ReportName, SumDuration = g.Sum(m=>m.Duration) }).
                                  GroupBy(g=> g.MeasureName).
                                  Select(g=>new { MeasureName = g.Key, MaxDuration = g.Max(m=>m.SumDuration) })     
                                 .Select(p => new DataPointTimeSeries<double, string>(p.MaxDuration, p.MeasureName)).ToList();


        public List<DataPointTimeSeries<int, string>> GetDataThreadCount(DateTime startDateTime, DateTime endDateTime)
        {
            var cnt_thread = ReportMetrics.Where(p => (p.UtcDateTime >= startDateTime)
                                     && (p.UtcDateTime <= endDateTime)).GroupBy(g => g.ThreadId).Count();
             return  new List<DataPointTimeSeries<int, string>>() { new DataPointTimeSeries<int, string>( cnt_thread, "Count_report_threads") };
        }

        public List<DataPointTimeSeries<double, long>> GetDataThread(DateTime startDateTime, DateTime endDateTime, string MeasureName)
        {
            var ThreadId = Convert.ToInt32(Regex.Replace(MeasureName, @"\D", "").Trim());
            return ReportMetrics.Where(p => (p.UtcDateTime >= startDateTime)
                                                  && (p.UtcDateTime <= endDateTime) && (p.ThreadId == ThreadId))
                                                  .Select(p => new DataPointTimeSeries<double, long>(p.Duration, p.UtcUnixTime)).ToList();

            //var topThreeDataSeries = dataSeriea.GroupBy(g => g.ThreadId)
            //                         .Select(g => new { ThreadId = g.Key, Value = g.Max(v => v.Duration) })
            //                         .OrderByDescending(o => o.Value).Take(3);
            ///     var cnt_thread = ReportMetrics.Where(p => (p.UtcDateTime >= startDateTime)
            //                            && (p.UtcDateTime <= endDateTime)).GroupBy(g => g.ThreadId).Count();
            //return new List<DataPointTimeSeries<int, string>>() { new DataPointTimeSeries<int, string>(cnt_thread, "Count_report_threads") };
        }



        //.GroupBy(g => g.MeasureName)
        // .Select(p => new DataPointByMeasure<string, double>(p.Key, p.Average(m => m.Duration))).ToList();l



        //public List<DataPointMetric<double>> GetDataMetricPoints(string MeasureName, DateTime startDateTime, DateTime endDateTime, int interval)
        //     => ReportMetrics.Where(p => p.MeasureName.Contains(MeasureName)
        //                           && (p.UtcDateTime >= startDateTime)
        //                          && (p.UtcDateTime <= endDateTime)).GroupBy(g => RoundUp(g.UtcDateTime, TimeSpan.FromMilliseconds(interval))).Select(g => new { UtcDateTime = g.Key, Duration = g.Max(x=>x.Duration) })
        //                               .Select(p => new DataPointMetric<double>(p.UtcDateTime, p.Duration)).ToList();



        //   .SkipWhile(p => p.UtcDateTime < startDateTime)
        //  .TakeWhile(p => p.UtcDateTime <= endDateTime).Select(p=>new DataPoint<double>(p.UtcDateTime, p.Duration)).ToList();
        //  public void Start() => SetCurrentDateTime(DateTime.UtcNow);
        private void _AddMetric(string SessionId, int NumId, string ReportName, string MeasureName, double Duration)
        {
            ReportMetrics.RemoveRange(ReportMetrics.Where(x => x.UtcDateTime < DateTime.UtcNow.AddSeconds(-3600)));

            var reportMetric = new ReportMetric
            {
                SessionId = SessionId,
                NumId = NumId,
                ReportName = ReportName,
                MeasureName = MeasureName,
                Duration = Duration
            };

            ReportMetrics.Add(reportMetric);
            
            SaveChanges();

        }

        public IDelayer NewDelay(string SessionId, int NumId,  string ReportName, string MeasureName)
        {
            IDelayer delayer = new Dalayer(new ReportMetric() { SessionId = SessionId, NumId = NumId, ReportName = ReportName, MeasureName = MeasureName });
                    delayer.OnDelay += (ReportMetric rm) => 
                                          AddMetric(rm.SessionId, rm.NumId, rm.ReportName, rm.MeasureName, rm.Duration);
             return delayer;

        }

        //public  void  AddMetric(string SessionId, int NumId, string ReportName, string MeasureName)
        //{
        //    lock (locker)
        //    {
        //        double duration = 0;
        //        DateTime prev_dt = GetPreviousDateTime();
        //      // var lstMetric =  ReportMetrics.LastOrDefault(p=>p.ThreadId == Thread.CurrentThread.ManagedThreadId);
        //        if (prev_dt != null)
        //        {
        //           DateTime curr_dt = DateTime.UtcNow;
        //           duration = Math.Round(curr_dt.Subtract(prev_dt).TotalSeconds, 3);
        //           SetCurrentDateTime(curr_dt);

        //        }
        //        _AddMetric(SessionId, NumId, ReportName, MeasureName, duration);

        //    }
        //}

        public void AddMetric(string SessionId, int NumId, string ReportName, string MeasureName, double Duration)
        {
            lock (locker)
            {
                _AddMetric(SessionId, NumId, ReportName, MeasureName, Duration);
            }
        }

        public void AddMetric(string SessionId, int NumId, string ReportName, string MeasureName, Action _action)
        {
            lock (locker)
            {
                var sw = Stopwatch.StartNew();
                try
                {
                    _action();
                }
                finally
                {
                    sw.Stop();
                }
                _AddMetric(SessionId, NumId, ReportName, MeasureName, sw.ElapsedMilliseconds / 1000);
            }
        }
        
        public DbSet<ReportMetric> ReportMetrics { get; set; }
    }
}
