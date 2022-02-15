using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebTestProteus.Models
{// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Raw
    {
        public string From { get; set; }
        public string To { get; set; }
    }

    public class Range
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public Raw Raw { get; set; }
    }

    public class Target
    {
        public string target { get; set; }
        public string RefId { get; set; }
        public string Type { get; set; }
    }

    public class Interval
    {
        public string Text { get; set; }
        public string Value { get; set; }
    }

    public class IntervalMs
    {
        public string Text { get; set; }
        public int Value { get; set; }
    }

    public class ScopedVars
    {
        public Interval __interval { get; set; }
        public IntervalMs __interval_ms { get; set; }
    }

    public class RangeRaw
    {
        public string From { get; set; }
        public string To { get; set; }
    }

    public class adhocFilters
    {
        public string Condition { get; set; }
        public string Key { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
    }
    
    public class QueryModel
    {
        public string App { get; set; }
        public string RequestId { get; set; }
        public string Timezone { get; set; }
        public long PanelId { get; set; }
        public object DashboardId { get; set; }
        public Range Range { get; set; }
        public string TimeInfo { get; set; }
        public string Interval { get; set; }
        public int IntervalMs { get; set; }
        public List<Target> Targets { get; set; }
        public int MaxDataPoints { get; set; }
        public ScopedVars ScopedVars { get; set; }
        public long StartTime { get; set; }
        public RangeRaw RangeRaw { get; set; }
        public List<adhocFilters> AdhocFilters { get; set; }
    }

    public class Annotation
    {
        public string Name { get; set; }
        public string Datasource { get; set; }
        public string IconColor { get; set; }
        public bool Enable { get; set; }
        public string Query { get; set; }
    }

    public class AnnotationModel
    {
        public Range Range { get; set; }
        public RangeRaw RangeRaw { get; set; }
        public Annotation Annotation { get; set; }
    }


}
