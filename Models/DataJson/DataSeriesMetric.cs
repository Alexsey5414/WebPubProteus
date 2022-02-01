using Newtonsoft.Json;

namespace WebTestProteus.Models.DataJson
{
    public struct DataSeriesMetric<T>
	{
		public DataSeriesMetric(string name, DataPointByMeasure<T>[] dataPoints) : this()
		{
			Name = name;
			DataPoints = dataPoints;
		}

		public string Name { get; }
		[JsonProperty("datapoints")]
		public DataPointByMeasure<T>[] DataPoints { get; }
	}
}
