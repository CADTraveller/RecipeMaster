using Newtonsoft.Json;
using System.Threading.Tasks;

namespace RecipeMaster.Helpers
{
	public static class Json
	{
		#region Public Methods

		public static async Task<string> StringifyAsync(object value)
		{
			return await Task.Run<string>(() =>
			{
				return JsonConvert.SerializeObject(value);
			});
		}

		public static async Task<T> ToObjectAsync<T>(string value)
		{
			return await Task.Run<T>(() =>
			{
				return JsonConvert.DeserializeObject<T>(value);
			});
		}

		#endregion Public Methods
	}
}