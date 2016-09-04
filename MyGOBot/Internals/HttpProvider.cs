using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace GO_Bot.Internals {

	internal static class HttpProvider {

		/// <summary>
		/// Performs a GET request.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <returns>A Task object which performs the request.</returns>
		public static Task<HttpResponseMessage> Get(string url) {
			return new HttpClient().GetAsync(url);
		}

		/// <summary>
		/// Performs a POST request.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="keyValuePairs">The data to be posted in key -> value form.</param>
		/// <returns>A Task object which performs the request.</returns>
		public static Task<HttpResponseMessage> Post(string url, Dictionary<string, string> keyValuePairs) {
			return new HttpClient().PostAsync(url, new FormUrlEncodedContent(keyValuePairs));
		}

	}

}
