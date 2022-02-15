using Microsoft.EntityFrameworkCore;
using SimpleJsonDataSource.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebTestProteus.Classes;
using WebTestProteus.Models;

namespace WebTestProteus.Interfaces
{
    public interface IDBContextMemory
    {
        IEnumerable<string> GetOperations();
        Task<IEnumerable<string>> GetOperationsAsync();
        IEnumerable<object> GetTagKeys();
        IEnumerable<object> GetTagValues();
        Task<TimeSeriesGroupModel<object>[]> GetDataSeriesAsync(QueryModel query);

        IDelayer NewDelay(string SessionId, int NumId, string ReportName, string MeasureName);
        //List<ReportMetric> ReportMetrics { get; }

      //  SynchronizedCollection<ReportMetric> ReportMetrics { get;}

         List<ReportMetric> ReportMetrics {get;}
    }
}
