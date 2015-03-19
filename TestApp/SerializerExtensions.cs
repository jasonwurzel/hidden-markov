using Newtonsoft.Json;

namespace TestApp
{
	public static class SerializerExtensions
	{
		public static string SerializeToJsonString(this object instance)
		{
			return JsonConvert.SerializeObject(instance);
		}

		public static T DeserializeFromJsonString<T>(this string data)
		{
			return JsonConvert.DeserializeObject<T>(data);
		}
	}
}