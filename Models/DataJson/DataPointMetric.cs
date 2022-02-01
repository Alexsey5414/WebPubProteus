using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebTestProteus.Models.DataJson
{
    public struct DataPointByMeasure<T>
    {

        public DataPointByMeasure(DateTime dateTime, T value)
        {
            DateTime = dateTime;
            Value = value;

        }

        [JsonProperty("value")]
        public T Value { get; }

        [JsonProperty("datetime")]
        public DateTime DateTime { get; }

    }

    public struct DataPointTimeSeries<T, Y>
	{

		public DataPointTimeSeries(T Value, Y Measure)
		{
			this.Measure = Measure;
			this.Value = Value;

		}

		[JsonProperty("value")]
		public T Value { get; }

		[JsonProperty("measure")]
		public Y Measure { get; }

	}

}
