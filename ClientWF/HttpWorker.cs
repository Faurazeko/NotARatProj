using System.Net;
using System.Text;
using System.Net.Http.Headers;
using System.Text.Json;
using NarLib;

namespace ClientWF
{
	static class HttpWorker
	{
		public static string GetServerBaseUrl() => $"http://{ConfigManager.Config.ServerIp}:{ConfigManager.Config.ServerHttpPort}";

		public static string? Post(string uri, string data, string contentType, NetworkCredential credentials = null, string method = "POST")
		{
			byte[] dataBytes = Encoding.UTF8.GetBytes(data);

			return Post(uri, dataBytes, contentType, credentials, method);
		}

		public static string? Post(string uri, byte[] data, string contentType, NetworkCredential credentials = null, string method = "POST")
		{
			try
			{
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
				request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
				request.ContentLength = data.Length;
				request.ContentType = contentType;
				request.Method = method;


				if (credentials != null)
					request.Headers.Add("Authorization", GetCredentials(credentials.UserName, credentials.Password));

				using (Stream requestBody = request.GetRequestStream())
					requestBody.Write(data, 0, data.Length);

				using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
				using (Stream stream = response.GetResponseStream())
				using (StreamReader reader = new StreamReader(stream))
					return reader.ReadToEnd();
			}
			catch
			{
				return null;
			}
		}

		public static string? Post(string uri, MultipartFormDataContent formData, NetworkCredential credentials = null, string method = "POST")
		{
			using (var client = new HttpClient())
			{
				client.DefaultRequestHeaders.Authorization = GetCredentials("Basic", credentials.UserName, credentials.Password);
				HttpResponseMessage response;

				switch (method.ToUpper())
				{
					case "PUT":
						response = client.PutAsync(uri, formData).GetAwaiter().GetResult();
						break;
					case "POST":
					default:
						response = client.PostAsync(uri, formData).GetAwaiter().GetResult();
						break;
				}

				if (response.IsSuccessStatusCode)
					return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

				return null;
			}
		}

		public static string? Get(string uri, NetworkCredential credentials = null)
		{
			try
			{
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
				request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

				if(credentials != null)
					request.Headers.Add("Authorization", GetCredentials(credentials.UserName, credentials.Password));

				using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
				using (Stream stream = response.GetResponseStream())
				using (StreamReader reader = new StreamReader(stream))
				{
					return reader.ReadToEnd();
				}
			}
			catch
			{
				return null;
			}
		}

		static public List<string>? UploadFilesToServer(IEnumerable<string> filepaths, bool isWallpaper = false)
		{
			var formData = new MultipartFormDataContent();

			foreach (var filepath in filepaths)
			{
				var fileInfo = new FileInfo(filepath);

				if (!fileInfo.Exists)
					continue;

				var byteContent = new ByteArrayContent(File.ReadAllBytes(filepath));

				formData.Add(byteContent, "files", fileInfo.Name);
			}

			try
			{
				var answer = Post
					(
						$"{GetServerBaseUrl()}/api/client/files?isWallpaper={isWallpaper}",
						formData, GetNetCredentials(), "POST"
					);

				var result = JsonSerializer.Deserialize<List<string>>(answer!);

				return result;
			}
			catch { }

			return null;
		}

		static public RegistrationResponse? SendClientData(ClientRegisterDto clientData, bool isUpdate = false)
		{
			var method = "POST";
			NetworkCredential credentials = null;

			if (isUpdate)
			{
				method = "PUT";
				credentials = GetNetCredentials();

				//SendWallpaperToServer();
			}


			var json = JsonSerializer.Serialize(clientData);
			var response = Post($"{GetServerBaseUrl()}/api/client", json, "text/json", credentials!, method);

			if (response == null || isUpdate)
				return null;

			var responseModel = JsonSerializer.Deserialize<RegistrationResponse>(response);

			return responseModel;
		}

		static public NetworkCredential GetNetCredentials() =>
			new NetworkCredential(ConfigManager.Config.ClientId.ToString(), ConfigManager.Config.ClientSecret);

		private static AuthenticationHeaderValue GetCredentials(string scheme, string username, string password)
		{
			return new AuthenticationHeaderValue("Basic",
					Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}")));
		}

		private static string GetCredentials(string username, string password) =>
			$"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"))}";
	}
}
