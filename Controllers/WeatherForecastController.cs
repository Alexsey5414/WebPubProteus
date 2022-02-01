using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Prometheus;
using NLog.Fluent;
using System.Data.SqlClient;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Extensions.Caching.Memory;

namespace WebTestProteus.Controllers
{
    public static class CacheKeys
    {
        public static string Entry { get { return "_Entry"; } }
        public static string CallbackEntry { get { return "_Callback"; } }
        public static string CallbackMessage { get { return "_CallbackMessage"; } }
        public static string Parent { get { return "_Parent"; } }
        public static string Child { get { return "_Child"; } }
        public static string DependentMessage { get { return "_DependentMessage"; } }
        public static string DependentCTS { get { return "_DependentCTS"; } }
        public static string Ticks { get { return "_Ticks"; } }
        public static string CancelMsg { get { return "_CancelMsg"; } }
        public static string CancelTokenSource { get { return "_CancelTokenSource"; } }
    }

    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
       // private IMemoryCache _cache;
        private static readonly Counter ProcessedJobCount = Metrics
    .CreateCounter("myapp_jobs_processed_total", "Number of processed jobs.");

        private static readonly Histogram LoginDuration = Metrics
    .CreateHistogram("myapp_login_duration_seconds", "Histogram of login call processing durations.",
            new HistogramConfiguration
            {
        // Here you specify only the names of the labels.
              LabelNames = new[] { "phazes", "threads" }
           });

        private static readonly Gauge DocumentImportsInProgress = Metrics
    .CreateGauge("myapp_document_imports_in_progress", "Number of import operations ongoing.", 
             new GaugeConfiguration
             {
                 // Here you specify only the names of the labels.
                 LabelNames = new[] { "phazes" }
             });

        private static readonly Histogram OrderValueHistogram = Metrics
    .CreateHistogram("myapp_order_value_usd", "Histogram of received order values (in USD).",
        new HistogramConfiguration
        {
            // We divide measurements in 10 buckets of $100 each, up to $1000.
            Buckets = Histogram.LinearBuckets(start: 500, width: 500, count: 10),
            LabelNames = new[] { "phazes" }

        });

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(/* IMemoryCache cache, */ILogger<WeatherForecastController> logger)
        {
          //  _cache = cache;
            _logger = logger;
        }




        public void testOracleConnection()
        {
            // OracleConnection con = new OracleConnection("DATA SOURCE=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = LAPTOP-KC4P3EL2)(PORT = 1521)) (CONNECT_DATA = (SERVER = DEDICATED) (SERVICE_NAME = XE))); user id=superuser; password=111; Validate Connection=true");
            OracleConnection con = new OracleConnection("DATA SOURCE=XE; user id=superuser; password=111; Validate Connection=true");

            try
            {
                // SqlDbType.DateTime2
                con.Open();
            }
            catch (Exception e)
            {
                string mes = e.Message;
            }
        }

        public void TestConnection()
        {
            // SqlConnection con = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=GraphanaDB;Trusted_Connection=True;");
            SqlConnection con = new SqlConnection("Data Source=localhost\\SQLEXPRESS; Initial Catalog=GraphanaDB; Integrated Security=True;");

            try
            {
               // SqlDbType.DateTime2
                con.Open();
            }
            catch (Exception e)
            {
                string mes = e.Message;
            }
        }

        private int GetJob(Random _rng)
        {
         //  OracleConnection  ora 
            int threadid = Thread.CurrentThread.ManagedThreadId;
            string thId = threadid.ToString();
            ProcessedJobCount.Inc();
            var idelay1 = _rng.Next(5, 200) * 10;
            using (LoginDuration.WithLabels("prepare", thId).NewTimer())
            {
                Thread.Sleep(idelay1);
            };
            _logger.LogTrace(new EventId(idelay1), "prepare");

            var idelay2 = _rng.Next(10, 150) * 10;
            using (LoginDuration.WithLabels("export", thId).NewTimer())
            {
                Thread.Sleep(idelay2);
            };
            _logger.LogTrace(new EventId(idelay2), "export");
            var idelay3 = _rng.Next(10, 300) * 10;
            using (LoginDuration.WithLabels("load", thId).NewTimer())
            {
                Thread.Sleep(idelay3);
            };
            _logger.LogTrace(new EventId(idelay3), "load");
            return threadid;
        }

        //[HttpGet("SetCache")]

        //public IActionResult CacheTryGetValueSet()
        //{
        //    DateTime cacheEntry;

        //    // Look for cache key.
        //    if (!_cache.TryGetValue(CacheKeys.Entry, out cacheEntry))
        //    {
        //        // Key not in cache, so get data.
        //        cacheEntry = DateTime.Now;

        //        // Set cache options.
        //        var cacheEntryOptions = new MemoryCacheEntryOptions()
        //            // Keep in cache for this time, reset time if accessed.
        //            .SetSlidingExpiration(TimeSpan.FromSeconds(600));

        //        // Save data in cache.
        //        _cache.Set(CacheKeys.Entry, cacheEntry, cacheEntryOptions);
        //    }
        //    return Ok($"SetCache = DateTime = {cacheEntry}");
        //    // return Ok(cacheEntry);
        //}
        //[HttpGet("GetCache")]
        //public IActionResult CacheGetOrCreate()
        //{
        //    //var cacheEntry = _cache.GetOrCreate(CacheKeys.Entry, entry =>
        //    //{
        //    //    entry.SlidingExpiration = TimeSpan.FromSeconds(3);
        //    //    return DateTime.Now;
        //    //});
        //    var cacheEntry = _cache.Get<DateTime?>(CacheKeys.Entry);

        //    return Ok($"GetCache = DateTime = {cacheEntry}");
        //}

        [HttpGet]
        public IEnumerable<int> GetX(int cnt = 1000)
        {
            // TestConnection();
            //testOracleConnection();
             var rng = new Random();
          //  return new int[] { 1, 2, 3 };
            var result = Enumerable.Range(1, cnt).AsParallel().WithDegreeOfParallelism(10).Select(x => GetJob(rng)).ToList();
             return result.Distinct();





            //for (int i = 1; i <= cnt; i++)
            //{
            //    ProcessedJobCount.Inc();

            //    var idelay1 = rng.Next(10, 100) * 10;
            //    using (LoginDuration.WithLabels("prepare").NewTimer())
            //    {
            //        Thread.Sleep(idelay1);
            //    };
            //    _logger.LogTrace(new EventId(idelay1), "prepare");

            //    var idelay2 = rng.Next(10, 100) * 10;
            //    using (LoginDuration.WithLabels("export").NewTimer())
            //    {
            //        Thread.Sleep(idelay2);
            //    };
            //    _logger.LogTrace(new EventId(idelay2), "export");
            //    var idelay3 = rng.Next(10, 100) * 10;
            //    using (LoginDuration.WithLabels("load").NewTimer())
            //    {
            //        Thread.Sleep(idelay3);
            //    };
            //    _logger.LogTrace(new EventId(idelay3), "load");

            //}


            // OrderValueHistogram.WithLabels("hello").Observe(2500);

            //using (DocumentImportsInProgress.WithLabels("macros").TrackInProgress())
            //{
            //    Thread.Sleep(1500);
            //}
            //   _logger.LogTrace(new EventId(10), "prepare");
            //return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            //{
            //    Date = DateTime.Now.AddDays(index),
            //    TemperatureC = rng.Next(-20, 55),
            //    Summary = Summaries[rng.Next(Summaries.Length)]
            //})
            //.ToArray();

        }
    }
}
