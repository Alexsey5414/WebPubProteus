//using Newtonsoft.Json;
//using Newtonsoft.Json;
using System.Text.Json.Serialization;
using SimpleJsonDataSource.Models;

namespace SimpleJsonDataSource.ViewModels
{
	//public struct TimeSeriesViewModel<T>
	//   {
	//	public TimeSeriesViewModel(string target, DataPoint<T>[] dataPoints)
	//	{
	//		Target = target;
	//		DataPoints = dataPoints;
	//	}

	//	public string Target { get; }
	//	[JsonPropertyName("datapoints")]
	//	public DataPoint<T>[] DataPoints { get; }
	//   }

	public struct TimeSeriesViewModel<T>
	{
		public TimeSeriesViewModel(string target, double[][] dataPoints)
		{
			Target = target;
			DataPoints = dataPoints;
		}

		public string Target { get; }
		[JsonPropertyName("datapoints")]
		public double[][] DataPoints { get; }
	}

	public struct TimeSeriesGroupModel<T>
	{
		public TimeSeriesGroupModel(string target, T[][] dataPoints)
		{
			Target = target;
			DataPoints = dataPoints;
		}

		public string Target { get; }
		[JsonPropertyName("datapoints")]
		public T[][] DataPoints { get; }
	}
}
