﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebTestProteus.Models
{// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Raw
    {
        public string from { get; set; }
        public string to { get; set; }
    }

    public class Range
    {
        public DateTime from { get; set; }
        public DateTime to { get; set; }
        public Raw raw { get; set; }
    }

    public class Target
    {
        public string target { get; set; }
        public string refId { get; set; }
        public string type { get; set; }
    }

    public class Interval
    {
        public string text { get; set; }
        public string value { get; set; }
    }

    public class IntervalMs
    {
        public string text { get; set; }
        public int value { get; set; }
    }

    public class ScopedVars
    {
        public Interval __interval { get; set; }
        public IntervalMs __interval_ms { get; set; }
    }

    public class RangeRaw
    {
        public string from { get; set; }
        public string to { get; set; }
    }

    public class QueryModel
    {
        public string app { get; set; }
        public string requestId { get; set; }
        public string timezone { get; set; }
        public long panelId { get; set; }
        public object dashboardId { get; set; }
        public Range range { get; set; }
        public string timeInfo { get; set; }
        public string interval { get; set; }
        public int intervalMs { get; set; }
        public List<Target> targets { get; set; }
        public int maxDataPoints { get; set; }
        public ScopedVars scopedVars { get; set; }
        public long startTime { get; set; }
        public RangeRaw rangeRaw { get; set; }
        public List<object> adhocFilters { get; set; }
    }


}