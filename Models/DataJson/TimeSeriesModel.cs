
using Newtonsoft.Json;
using WebTestProteus.Models.DataJson;

namespace SimpleJsonDataSource.ViewModels
{
    public struct TimeSeriesModel<T>
    {
        public TimeSeriesModel(string target, DataPointByMeasure<T>[] dataPoints)
        {
            Target = target;
            DataPoints = dataPoints;
        }

        [JsonProperty("target")]
        public string Target { get; }

        [JsonProperty("datapoints")]
        public DataPointByMeasure<T>[] DataPoints { get; }
    }

}
