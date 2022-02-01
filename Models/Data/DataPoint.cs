//using Newtonsoft.Json;
using System.Text.Json.Serialization;
using System;

namespace SimpleJsonDataSource.Models
{


	public struct DataPoint<T>
    {
		public DataPoint(/*DateTime dateTime*/ long unixTimeStamp, T value)
		{
			//DateTime = dateTime;
			Value = value;
			UnixTimeStamp = unixTimeStamp;
		}

		[JsonPropertyName("value")]
		public T Value { get; }

		//[JsonPropertyName("datetime")]

		//public DateTime DateTime { get; }


		//	[JsonIgnore]
	

		[JsonPropertyName("unixTimeStamp")]
		public long UnixTimeStamp { get; }

	}
}
