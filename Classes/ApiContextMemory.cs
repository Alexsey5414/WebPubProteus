using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace WebTestProteus.Classes
{
    using SimpleJsonDataSource.ViewModels;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using WebTestProteus.Consts;
    using WebTestProteus.Interfaces;
    using WebTestProteus.Models;
    using WebTestProteus.Models.DataJson;

    public class DBContextMemory : DbContext, IDBContextMemory

    {
        static object locker = new object();
        public DBContextMemory(DbContextOptions<DBContextMemory> options) : base(options)
        {
       
        }
        //Получение метрик мониторинга потоков
        private IEnumerable<string> GetTargetThreads(DateTime startDateTime, DateTime endDateTime) => ReportMetrics.
                                                    Where(p => (p.UtcDateTime >= startDateTime) && (p.UtcDateTime <= endDateTime)).
                                                    GroupBy(g => new { g.SessionId, g.ThreadId }).
                                                    Select(g => new { g.Key.ThreadId, Count = g.Count() }).
                                                    Where(g => g.Count < ConstParameters.CountMonitorOperation).
                                                    GroupBy(g => g.ThreadId).
                                                    OrderBy(g => g.Key).
                                                    Select(s => $"thread{s.Key}").
                                                    Take(10);
        //Получение метрик мониторинга отчетов
        private IEnumerable<string> GetTargetReports(DateTime startDateTime, DateTime endDateTime) => ReportMetrics.
                                                Where(p => (p.UtcDateTime >= startDateTime) && (p.UtcDateTime <= endDateTime)).
                                                GroupBy(g => g.ReportName).
                                                OrderBy(g => g.Key).
                                                Select(s => s.Key);

        //Получение метрик мониторинга операций отчетов
        private IEnumerable<string> GetTargetOperations() => new string[] { ConstMetrics.load, ConstMetrics.prepare, ConstMetrics.runmacros, ConstMetrics.export };
        //Получение метрик мониторинга последних операций отчетов
        private IEnumerable<string> GetTargetLastOperation() => new string[] { ConstMetrics.load, ConstMetrics.prepare, ConstMetrics.runmacros, ConstMetrics.export };
        //Получение метрик мониторинга активных управляемых потоков
        private IEnumerable<string> GetTargetActiveThreadCount() => new string[] { ConstMetrics.threadcount };
        //Получение метрик мониторинга максимальных топ n количества выполненных отчетов
        private IEnumerable<string> GetTargetDoneReportCount(DateTime startDateTime, DateTime endDateTime, int topvalue) =>
              ReportMetrics.Where(p => (p.UtcDateTime >= startDateTime)
                                       && (p.UtcDateTime <= endDateTime)).
                                              GroupBy(g => new { g.SessionId, g.ReportName }).
                                              Select(g => new { MeasureName = g.Key.ReportName, Count = g.Count() }).
                                              Where(g => g.Count >= ConstParameters.CountMonitorOperation).
                                              GroupBy(g => g.MeasureName).
                                              Select(g => new { MeasureName = g.Key, Count = g.Count() }).
                                              OrderByDescending(g => g.Count).
                                              Select(g => g.MeasureName).
                                              Take(topvalue > 0 ? topvalue : 10);
        
        //Получение метрик мониторинга максимальных топ n времени выполненных отчетов
        private IEnumerable<string> GetTargetDoneReportDuration(DateTime startDateTime, DateTime endDateTime, int topvalue) =>
                                                ReportMetrics.
                                                Where(p => (p.UtcDateTime >= startDateTime) && (p.UtcDateTime <= endDateTime)).
                                                GroupBy(g => new { g.SessionId, g.ReportName }).
                                                Select(g => new { MeasureName = g.Key.ReportName,
                                                    SumDuration = g.Sum(m => m.Duration),
                                                    Count = g.Count() }).
                                                Where(g => g.Count >= ConstParameters.CountMonitorOperation).
                                                GroupBy(g => g.MeasureName).
                                                Select(g => new { MeasureName = g.Key, MaxDuration = g.Max(m => m.SumDuration) }).
                                                OrderByDescending(g => g.MaxDuration).
                                                Select(g => g.MeasureName).
                                                Take(topvalue > 0 ? topvalue : 10);

        //Получение метрик мониторинга максимальных топ n времени выполняемых отчетов
        private IEnumerable<string> GetTargetCurrentReportDuration(DateTime startDateTime, DateTime endDateTime) => //GetReports(startDateTime, endDateTime);
             ReportMetrics.Where(p => (p.UtcDateTime >= startDateTime)
                                 && (p.UtcDateTime <= endDateTime)).
                                 GroupBy(g => new { g.SessionId, g.ReportName }).
                                 Select(g => new { MeasureName = g.Key.ReportName, SumDuration = g.Sum(m => m.Duration), Count = g.Count() }).
                                 Where(g => g.Count < ConstParameters.CountMonitorOperation).
                                 GroupBy(g => g.MeasureName).
                                 Select(g => new { MeasureName = g.Key, MaxDuration = g.Max(m => m.SumDuration) }).
                                 OrderByDescending(g => g.MaxDuration).
                                 Select(g => g.MeasureName).
                                 Take(10);
        //Список операций
        public IEnumerable<string> GetOperations() => new string[] { ConstParameters.Operations, ConstParameters.LastOperations, ConstParameters.DoneReportCount, 
                                                                      ConstParameters.DoneReportDuration, ConstParameters.ThreadCount, ConstParameters.Threads, 
                                                                      ConstParameters.CurrentReportDuration };

        //Наименование параметра для adHocFilter фильтра
        public IEnumerable<object> GetTagKeys() => new object[] { new { type = "string", text = ConstParameters.firstTopTagKeys } };
        //Значение adHocFilter фильтра
        public IEnumerable<object> GetTagValues() => Enumerable.Range(1, 5).Select(p => new { text = $"{p * 10}" });
        private IEnumerable<string> GetEmpty() => new string[] { };

        private object[][] RunFunc<T, Y>(Func<List<DataPointTimeSeries<T, Y>>> func)
        {
            return func()?.Select(p => new object[] { p.Value, p.Measure }).ToArray();
        }

       public object[][] GetDataPointSource(KeyValuePair<string, Measures> kvpMeasureName, DateTime startDateTime, DateTime endDateTime)
       {
           object[][] result = null;

           switch (kvpMeasureName.Value)
           {
                case Measures.Operation:
                         result = RunFunc(() => GetDataTimeSeries(startDateTime, endDateTime, kvpMeasureName.Key));
  
                    break;
                case Measures.LastOper:
                    result = RunFunc(() => GetDataLastOperation(startDateTime, endDateTime, kvpMeasureName.Key));

                    break;
                case Measures.ReportCount:
                    result = RunFunc(() => GetMaxCountDoneReport(startDateTime, endDateTime, kvpMeasureName.Key));
                    break;

                case Measures.ReportDuration:
                    result = RunFunc(() => GetMaxTimeDoneReport(startDateTime, endDateTime, kvpMeasureName.Key));
                    break;

                case Measures.ThreadCount:
                    result = RunFunc(() => GetDataThreadCount(startDateTime, endDateTime));
                        
                   break;
                case Measures.ThreadInfo:
                    result = RunFunc(() => GetDataThread(startDateTime, endDateTime, kvpMeasureName.Key));

                   break;
                case Measures.CurrentReport:
                    result = RunFunc(() => GetCurrentReportDuration(startDateTime, endDateTime, kvpMeasureName.Key));
                    break;
                default:
                  break;
           }
           return result;
       }

       private IDictionary<string, Measures> RunFunc(Func<IEnumerable<string>> func, Measures measure)
        {
            return func()?.ToDictionary(k => k, v => measure);
        }
       public IDictionary<string, Measures> GetTargetMetrics(string targetName, DateTime startDateTime, 
                                                     DateTime endDateTime, List<adhocFilters> hocFilters)
        {
            IDictionary<string, Measures> result = null;
            var topValue = Convert.ToInt32(hocFilters.FirstOrDefault(af => ConstParameters.firstTopTagKeys.Equals(af.key, StringComparison.OrdinalIgnoreCase))?.value);
           switch (targetName)
           {
                case ConstParameters.Operations: 
                    result = RunFunc(() => GetTargetOperations(), Measures.Operation);
                   break;

                case ConstParameters.LastOperations:
                    result = RunFunc(() => GetTargetLastOperation(), Measures.LastOper); 
                    break;

                case ConstParameters.DoneReportCount:
                    result = RunFunc(() => GetTargetDoneReportCount(startDateTime, endDateTime, topValue), Measures.ReportCount);
                   break;

                case ConstParameters.DoneReportDuration: 
                    result = RunFunc(() => GetTargetDoneReportDuration(startDateTime, endDateTime, topValue), Measures.ReportDuration);
                    break;

                case ConstParameters.ThreadCount: 
                    result = RunFunc(() => GetTargetActiveThreadCount(), Measures.ThreadCount); 
                   break;

                case ConstParameters.Threads:
                    result = RunFunc(() => GetTargetThreads(startDateTime, endDateTime), Measures.ThreadInfo);
                   break;

                case ConstParameters.CurrentReportDuration: 
                    result = RunFunc(() => GetTargetCurrentReportDuration(startDateTime, endDateTime), Measures.CurrentReport);
                    break;


                default:
                   result = RunFunc(() => GetEmpty(), Measures.EmptyOper);
                   break;

           }
           return result;
       }
       
        public async Task<TimeSeriesGroupModel<object>[]> GetDataSeriesAsync(QueryModel query) =>
           await Task.Run(()=>query.targets.Select(t => t.target).
                                               Select(target => GetTargetMetrics(target, query.range.from, query.range.to, 
                                                                           query.adhocFilters)).
                                               SelectMany(p => p)
                               .Select(target => new TimeSeriesGroupModel<object>
                                (
                                   target.Key,
                                   GetDataPointSource(target, query.range.from, query.range.to)
                                )).Where(target => target.DataPoints.Any()).ToArray());
        private List<DataPointTimeSeries<double, long>> GetDataTimeSeries(DateTime startDateTime, DateTime endDateTime, string MeasureName)
              => ReportMetrics.Where(p => (string.IsNullOrEmpty(MeasureName) || MeasureName.Contains(p.MeasureName)) 
                                    && (p.UtcDateTime >= startDateTime)
                                   && (p.UtcDateTime <= endDateTime)).Select(p => new DataPointTimeSeries<double, long>(p.Duration, p.UtcUnixTime)).ToList();

        private List<DataPointTimeSeries<double, string>> GetDataLastOperation(DateTime startDateTime, DateTime endDateTime, string MeasureName)
            => ReportMetrics.Where(p => (string.IsNullOrEmpty(MeasureName) || MeasureName.Contains(p.MeasureName))
                                  && (p.UtcDateTime >= startDateTime)
                                 && (p.UtcDateTime <= endDateTime)).
                                 OrderByDescending(p=>p.UtcDateTime).
                                 Take(1).
                                 Select(p => new DataPointTimeSeries<double, string>(p.Duration, p.MeasureName)).ToList();

        private List<DataPointTimeSeries<int, string>> GetMaxCountDoneReport(DateTime startDateTime, DateTime endDateTime, string ReportName = "")
            => ReportMetrics.Where(p => (string.IsNullOrEmpty(ReportName) || ReportName.Contains(p.ReportName))
                                  && (p.UtcDateTime >= startDateTime)
                                  && (p.UtcDateTime <= endDateTime)).
                                  GroupBy(g => new { g.SessionId , g.ReportName}).
                                  Select(g => new { MeasureName = g.Key.ReportName, Count = g.Count()}).
                                  Where(g => g.Count >= ConstParameters.CountMonitorOperation).
                                  GroupBy(g => g.MeasureName).
                                  Select(g => new { MeasureName = g.Key, Count = g.Count() }).
                                  OrderByDescending(g => g.Count).
                                  Select(p => new DataPointTimeSeries<int, string>(p.Count, p.MeasureName)).ToList();
        private List<DataPointTimeSeries<double, string>> GetCurrentReportDuration(DateTime startDateTime, DateTime endDateTime, string ReportName = "")=>
                 ReportMetrics.Where(p => (string.IsNullOrEmpty(ReportName) || ReportName.Contains(p.ReportName))
                                 && (p.UtcDateTime >= startDateTime)
                                 && (p.UtcDateTime <= endDateTime)).
                                 GroupBy(g => new { g.SessionId, g.ReportName }).
                                 Select(g => new { MeasureName = g.Key.ReportName, SumDuration = g.Sum(m => m.Duration), Count = g.Count() }).
                                 Where(g => g.Count < ConstParameters.CountMonitorOperation).
                                 GroupBy(g => g.MeasureName).
                                 Select(g => new { MeasureName = g.Key, MaxDuration = g.Max(m => m.SumDuration) }).
                                 OrderByDescending(g => g.MaxDuration).
                                 Select(p => new DataPointTimeSeries<double, string>(p.MaxDuration, p.MeasureName)).ToList();

        private List<DataPointTimeSeries<double, string>> GetMaxTimeDoneReport(DateTime startDateTime, DateTime endDateTime, string ReportName = "")
           => ReportMetrics.Where(p => (string.IsNullOrEmpty(ReportName) || ReportName.Contains(p.ReportName))
                                 && (p.UtcDateTime >= startDateTime)
                                 && (p.UtcDateTime <= endDateTime)).
                                 GroupBy(g => new { g.SessionId, g.ReportName }).
                                 Select(g => new { MeasureName = g.Key.ReportName, SumDuration = g.Sum(m => m.Duration), Count = g.Count() }).
                                 Where (g => g.Count >= ConstParameters.CountMonitorOperation).  
                                 GroupBy(g => g.MeasureName).
                                 Select(g => new { MeasureName = g.Key, MaxDuration = g.Max(m => m.SumDuration) }).
                                OrderByDescending(g=>g.MaxDuration).      
                                   Select(p => new DataPointTimeSeries<double, string>(p.MaxDuration, p.MeasureName)).ToList();

        public List<DataPointTimeSeries<int, string>> GetDataThreadCount(DateTime startDateTime, DateTime endDateTime)
        {
            var cnt_thread = ReportMetrics.Where(p => (p.UtcDateTime >= startDateTime)
                                     && (p.UtcDateTime <= endDateTime)).
                                      GroupBy(g =>  new { g.SessionId, g.ThreadId } ).
                                 Select(g => new { g.Key.ThreadId, Count = g.Count() }).
                                 Where(g => g.Count < ConstParameters.CountMonitorOperation).
                                 GroupBy(g => g.ThreadId).Take(10).Count();
            return new List<DataPointTimeSeries<int, string>>() { new DataPointTimeSeries<int, string>( cnt_thread, ConstMetrics.threadcount) };
        }

        public List<DataPointTimeSeries<double, long>> GetDataThread(DateTime startDateTime, DateTime endDateTime, string MeasureName)
        {
            var ThreadId = Convert.ToInt32(Regex.Replace(MeasureName, @"\D", "").Trim());
            return ReportMetrics.Where(p => (p.UtcDateTime >= startDateTime)
                                                  && (p.UtcDateTime <= endDateTime) && (p.ThreadId == ThreadId))
                                                  .Select(p => new DataPointTimeSeries<double, long>(p.Duration, p.UtcUnixTime)).ToList();
        }

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
