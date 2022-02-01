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

namespace WebTestProteus.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly ApiContext _context;
        private List<string> reports = new List<string>() {"RUN_PREPARE_CURRENCY",
                                                            "FIX_SIMPLE_REPORT",
        "REP_SVOD_MANY_CURR",
        "REP_BAGS_RECALS",
        "REP_ABOUT_RECALC_REQUEST",
        "GET_REQ_ASHAN_ATAK",
        "REP_TWO_COUNTRY_TWISE",
        "REP_RECALC_PACK",
        "REP_LIST_RC",
        "REP_CONTROL_VEDOM"};
        public UserController(ApiContext context)
        {
            _context = context;
          //  context.AddTestData();
        }

        private int GetJob(Random _rng)
        {   
            //  OracleConnection  ora 
            int threadid = Thread.CurrentThread.ManagedThreadId;
            var index = _rng.Next(0, 9);
            var ReportName = reports[index];
            var SessionId = Guid.NewGuid().ToString();
            //_context.AddMetric("Session1", "REPORT_NAME", "load");
            using (_context.NewDelay(SessionId, 1, ReportName, "load"))
            {
                  var idelay1 = _rng.Next(80, 200) * 10;

                Thread.Sleep(idelay1);
            }

            using (_context.NewDelay(SessionId, 2, ReportName, "prepare"))
            {
                var idelay1 = _rng.Next(150, 400) * 10;

                Thread.Sleep(idelay1);
            }
            using (_context.NewDelay(SessionId, 3, ReportName, "export"))
            {
                var idelay1 = _rng.Next(50, 300) * 10;
                Thread.Sleep(idelay1);
            }

            using (_context.NewDelay(SessionId, 4, ReportName, "runmacros"))
            {
                var idelay1 = _rng.Next(10, 250) * 10;

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
            return Ok(_context.GetMeasures());
        }



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
        public IActionResult Query([FromBody] QueryModel query)

        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //   var smoothingWindow = TimeSpan.FromTicks(5 * TimeSpan.TicksPerMinute);
            // var halfSmoothingWindow = 0.5 * smoothingWindow;

            //  var dataFrom = query.Range.From - halfSmoothingWindow;
            //   var dataTo = query.Range.To + halfSmoothingWindow;

            var dataFrom = query.range.from;
            var dataTo = query.range.to;

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
            return Ok(query.targets.Select(t => t.target).Select(target => _context.GetMetrics(target)).SelectMany(p=>p)
                               .Select(target => new TimeSeriesGroupModel<object>
                                (
                                   target.Key,
                                  // target.Replace("ots_", "").Replace("pts_", ""),
                                  _context.GetDataPointSource(target, dataFrom, dataTo)//.Select(p => new object[] { p.Value, p.Measure}).ToArray()
                                )).ToArray());
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
        public async Task<IActionResult> Get()
        {

            var rng = new Random();
  
            //return new int[] { 1, 2, 3 };
            var result = Enumerable.Range(1, 1000).AsParallel().AsOrdered().WithDegreeOfParallelism(10).Select(x => GetJob(rng)).ToList();
 
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
