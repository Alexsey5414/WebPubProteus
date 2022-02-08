using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SimpleJsonDataSource.ViewModels;
//using SimpleJsonDataSource.ViewModels.Converters;
using WebTestProteus.Classes;
using WebTestProteus.Models;
using Newtonsoft.Json.Serialization;
using SimpleJsonDataSource;
using Microsoft.Extensions.Logging;
using NLog;

namespace WebTestProteus.Controllers
{
  
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly DBContextMemory _context;
        private readonly ILogger<UserController> _logger;
        private readonly Logger logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
        private List<string> reports = new List<string>() {"RUN_PREPARE_CURRENCY",
                                                            "FIX_SIMPLE_REPORT",
        "REP_SVOD_MANY_CURR",
        "REP_BAGS_RECALS",
        "REP_ABOUT_RECALC_REQUEST",
        "GET_REQ_ASHAN_ATAK",
        "REP_TWO_COUNTRY_TWISE",
        "REP_RECALC_PACK",
        "REP_LIST_RC",
        "REP_CONTROL_VEDOM",
        "REP_SVOD_MANY_CURR1",
        "REP_BAGS_RECALS2",
        "REP_ABOUT_RECALC_REQUEST3",
        "GET_REQ_ASHAN_ATAK4",
        "REP_TWO_COUNTRY_TWISE5",
        "REP_RECALC_PACK6",
        "REP_LIST_RC7",
        "REP_CONTROL_VEDOM8",

        "RUN_PREPARE_CURRENCY_NOW",
        "FIX_SIMPLE_REPORT_NOW",
        "REP_SVOD_MANY_CURR_NOW",
        "REP_BAGS_RECALS_NOW",
        "REP_ABOUT_RECALC_REQUEST_NOW",
        "GET_REQ_ASHAN_ATAK_NOW",
        "REP_TWO_COUNTRY_TWISE_NOW",
        "REP_RECALC_PACK_NOW",
        "REP_LIST_RC_NOW",
        "REP_CONTROL_VEDOM_NOW",
        "REP_SVOD_MANY_CURR1_NOW",
        "REP_BAGS_RECALS2_NOW",
        "REP_ABOUT_RECALC_REQUEST3_NOW",
        "GET_REQ_ASHAN_ATAK4_NOW",
        "REP_TWO_COUNTRY_TWISE5_NOW",
        "REP_RECALC_PACK6_NOW",
        "REP_LIST_RC7_NOW",
        "REP_CONTROL_VEDOM8_NOW"};
        public UserController(DBContextMemory context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;

            //  context.AddTestData();
        }

        private void LogMsg(string SessionId, int NumId, string ReportName, string MeasureName, int duration)
        {
            // NLog: setup the logger first to catch all errors
           // var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            LogEventInfo theEvent = new LogEventInfo(NLog.LogLevel.Trace, "database", MeasureName);
            theEvent.Properties["duration"] = duration;
            theEvent.Properties["sessionid"] = SessionId;
            theEvent.Properties["numid"] = NumId;
            theEvent.Properties["reportname"] = ReportName;
    
            // theEvent.Properties["MyDateTimeValueWithCultureAndFormat"] = new DateTime(2015, 08, 30, 11, 26, 50);
            logger.Log(theEvent);

        }

        private int GetJob(Random _rng)
        {   
            //  OracleConnection  ora 
            int threadid = Thread.CurrentThread.ManagedThreadId;
            var index = _rng.Next(0, 32);
            var ReportName = reports[index];
            var SessionId = Guid.NewGuid().ToString();
            //_context.AddMetric("Session1", "REPORT_NAME", "load");
            using (_context.NewDelay(SessionId, 1, ReportName, "load"))
            {
                  var idelay1 = _rng.Next(80, 200) * 10;
                LogMsg(SessionId, 1, ReportName, "load", idelay1);
                // _logger.LogTrace(new EventId(idelay1), "load");
                Thread.Sleep(idelay1);
            }
           

            using (_context.NewDelay(SessionId, 2, ReportName, "prepare"))
            {
                var idelay1 = _rng.Next(200, 300) * 10;
                //  _logger.LogTrace(new EventId(idelay1), "prepare");
                LogMsg(SessionId, 2, ReportName, "prepare", idelay1);
                Thread.Sleep(idelay1);
            }
            using (_context.NewDelay(SessionId, 3, ReportName, "export"))
            {
                var idelay1 = _rng.Next(100, 300) * 10;
                //  _logger.LogTrace(new EventId(idelay1), "export");
                LogMsg(SessionId, 3, ReportName, "export", idelay1);
                Thread.Sleep(idelay1);
            }

            using (_context.NewDelay(SessionId, 4, ReportName, "runmacros"))
            {
                var idelay1 = _rng.Next(10, 250) * 10;
                LogMsg(SessionId, 4, ReportName, "runmacros", idelay1);
                //  _logger.LogTrace(new EventId(idelay1), "runmacros");
                Thread.Sleep(idelay1);
            }

            // _context.Start();

            //Thread.Sleep(1000);
            ////   _context.AddMetric("Session1", "REPORT_NAME", "load", ()=> { Thread.Sleep(0); });
            ////   var idelay1 = 1000; //_rng.Next(5, 200) * 10;
            ////  Thread.Sleep(idelay1);
            ////   _context.AddMetric("Session1", "REPORT_NAME", "load", () => { Thread.Sleep(1000); });
            //_context.AddMetric("Session1", "REPORT_NAME", "load");
            ////  var idelay2 = 2000;//_rng.Next(10, 150) * 10;
            //// Thread.Sleep(idelay2);
            //Thread.Sleep(3000);
            //_context.AddMetric("Session1", "REPORT_NAME", "export");
            //// var idelay3 = 1500;//_rng.Next(10, 300) * 10;
            ////   Thread.Sleep(idelay3);
            //Thread.Sleep(1500);
            //_context.AddMetric("Session1", "REPORT_NAME", "runmacros");

            return threadid;
        }

        [HttpPost("search")]
        public IActionResult Search([FromBody]string value)
        {
            return Ok(_context.GetOperations());
        }


        [HttpPost("tag-keys")]
        public IActionResult TagKeys([FromBody] string value)
        {
            return Ok(_context.GetTagKeys());
        }

        [HttpPost("tag-values")]
        public IActionResult TagValues([FromBody] string value)
        {
            return Ok(_context.GetTagValues());
        }


        //[HttpPost("annotations")]
        //public IActionResult TagValues([FromBody] AnnotationModel value)
        //{
        //    return Ok(_context.GetAnnotations(value));
        //}

        

        [HttpPost("help")]
        public IActionResult Help([FromBody] QueryModel query)
        // public IActionResult Query([FromBody]int value)

        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //   var smoothingWindow = TimeSpan.FromTicks(5 * TimeSpan.TicksPerMinute);
            // var halfSmoothingWindow = 0.5 * smoothingWindow;

            //  var dataFrom = query.Range.From - halfSmoothingWindow;
            //   var dataTo = query.Range.To + halfSmoothingWindow;

         //   var dataFrom = query.range.from;
           // var dataTo = query.range.to;

            var t = _context.ReportMetrics.Where(r=>r.MeasureName.Contains("load")).Take(5);
            query.range.from = t.First().UtcDateTime;
            query.range.to = t.Last().UtcDateTime;
            query.targets.Clear();
            query.targets.Add(new Target() { target = "load", refId = "A", type = "timeserie" });
            //var samplingInterval = new TimeSpan(query.IntervalMs * TimeSpan.TicksPerMillisecond);
            //  return Ok(query);
            return Ok(query);
        }


        //[HttpPost("query")]
        //public JsonResult Query([FromBody] JsonElement json)
        //{
        //    var str = json.GetRawText();

        //    return Json(json);
        //}


        [HttpPost("query")]
        public async Task<IActionResult> QueryAsync([FromBody] QueryModel query)

        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //   var smoothingWindow = TimeSpan.FromTicks(5 * TimeSpan.TicksPerMinute);
            // var halfSmoothingWindow = 0.5 * smoothingWindow;

            //  var dataFrom = query.Range.From - halfSmoothingWindow;
            //   var dataTo = query.Range.To + halfSmoothingWindow;

          //  var dataFrom = query.range.from;
          //  var dataTo = query.range.to;

            //    //var samplingInterval = new TimeSpan(query.IntervalMs * TimeSpan.TicksPerMillisecond);
            //    //  return Ok(query);
            //return Ok(query.targets.Select(t => t.target).Select(target =>  new TimeSeriesViewModel<double>
            //                   (
            //                     target, 
            //                     _context.GetDataPoints(target, dataFrom, dataTo).ToArray()
            //                   )).ToArray());
            //    return Ok(query.targets.Select(t => t.target).Select(target => new TimeSeriesViewModel<double>
            //                        (
            //                          target,
            //                          _context.GetDataPoints(target, dataFrom, dataTo).Select(p => new double[] {p.Value, p.UnixTimeStamp }).ToArray()
            //                        )).ToArray()) ;
            //return Ok(query.targets.Select(t => t.target).Select(target => _context.GetMetrics(target, dataFrom, dataTo)).SelectMany(p=>p)
            //                   .Select(target => new TimeSeriesGroupModel<object>
            //                    (
            //                       target.Key,
            //                      // target.Replace("ots_", "").Replace("pts_", ""),
            //                      _context.GetDataPointSource(target, dataFrom, dataTo)//.Select(p => new object[] { p.Value, p.Measure}).ToArray()
            //                    )).Where(target=> target.DataPoints.Count() > 0).ToArray());

            return Ok(await _context.GetDataSeriesAsync(query));
        }

        //[HttpPost("query1")]
        //public IActionResult Query1([FromBody] JsonElement json11111111111111111111)
        //{
        //    var str_json = json.GetRawText();

        //    JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        //    {
        //        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        //        Converters = new List<JsonConverter>()
        //        {
        //            new StringEnumConverter(),
        //            // Add serialization support for specific data point types.
        //            DataPointConverter<byte>.Instance,
        //            DataPointConverter<sbyte>.Instance,
        //            DataPointConverter<short>.Instance,
        //            DataPointConverter<ushort>.Instance,
        //            DataPointConverter<int>.Instance,
        //            DataPointConverter<uint>.Instance,
        //            DataPointConverter<long>.Instance,
        //            DataPointConverter<ulong>.Instance,
        //            DataPointConverter<float>.Instance,
        //            DataPointConverter<double>.Instance
        //        }
        //     };
        //    //  JsonConvert.
        //    QueryModel query = JsonConvert.DeserializeObject<QueryModel>(str_json, serializerSettings);


        //    var smoothingWindow = TimeSpan.FromTicks(1 * TimeSpan.TicksPerSecond);
        //    var halfSmoothingWindow = 0.1;// 0.5 * smoothingWindow;

        //    var dataFrom = query.range.from; //- halfSmoothingWindow;
        //    var dataTo = query.range.to; //+ halfSmoothingWindow;
        //    //	var dataFrom = query.Range.From;
        //    //	var dataTo = query.Range.To;


        //    var samplingInterval = new TimeSpan(query.intervalMs * TimeSpan.TicksPerMillisecond);

        //  //  var dataFrom = query.range.from;
        // //   var dataTo = query.range.to;

        //    //    //var samplingInterval = new TimeSpan(query.IntervalMs * TimeSpan.TicksPerMillisecond);
        //    //    //  return Ok(query);
        //    //return Ok(query.targets.Select(t => t.target).Select(target =>  new TimeSeriesViewModel<double>
        //    //                   (
        //    //                     target,
        //    //                     _context.GetDataPoints(target, dataFrom, dataTo).ToArray()
        //    //                   )).ToArray());
        //    //    return Ok(query.targets.Select(t => t.target).Select(target => new TimeSeriesViewModel<double>
        //    //                        (
        //    //                          target,
        //    //                          _context.GetDataPoints(target, dataFrom, dataTo).Select(p => new double[] { p.Value, p.UnixTimeStamp }).ToArray()
        //    //                        )).ToArray());

        //    TimeSeriesModel<double>[] lstTimeSeries = query.targets.Select(t => t.target).Select(target => new TimeSeriesModel<double>
        //                        (
        //                          target,
        //                         DataSeriesFilter.FilterDataPoints(_context.GetDataMetricPoints(dataFrom, dataTo, target), query.range.from, query.range.to, samplingInterval, smoothingWindow).ToArray()

        //                        // _context.GetDataMetricPoints(target, dataFrom, dataTo, query.intervalMs).ToArray()
        //                        )).ToArray();

        //   string  result  = JsonConvert.SerializeObject(lstTimeSeries, serializerSettings);
        //   return Ok(result);

        //}

        [HttpGet]
        public async Task<IActionResult> Get(int id = 1000)
        {

            var rng = new Random();
  
            //return new int[] { 1, 2, 3 };
            var result = Enumerable.Range(1, id).AsParallel().AsOrdered().WithDegreeOfParallelism(10).Select(x => GetJob(rng)).ToList();
 
            var metrics = await _context.ReportMetrics.OrderBy(r=>r.ThreadId).ThenBy(r=>r.NumId).ToArrayAsync();

            //var response = users.Select(u => new
            //{
            //    firstName = u.FirstName,
            //    lastName = u.LastName,
            //    posts = u.Posts.Select(p => p.Content)
            //});

            return Ok(metrics);
        }
    }
}
