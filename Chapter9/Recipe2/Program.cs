using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Console;

namespace Chapter9.Recipe2
{
	class Program
	{
		static void Main(string[] args)
		{
			var server = new AsyncHttpServer(1234);
			var t = Task.Run(() => server.Start());
			WriteLine("Listening on port 1234. Open http://localhost:1234 in your browser.");
			WriteLine("Trying to connect:");
			WriteLine();

			GetResponseAsync("http://localhost:1234").GetAwaiter().GetResult();

			WriteLine();
			WriteLine("Press Enter to stop the server.");
			ReadLine();

			server.Stop().GetAwaiter().GetResult();
		}

        static async Task GetResponseAsync(string url)
        {
	        using (var client = new HttpClient())
	        {
		        HttpResponseMessage responseMessage = await client.GetAsync(url);
		        string responseHeaders = responseMessage.Headers.ToString();
		        string response = await responseMessage.Content.ReadAsStringAsync();

		        WriteLine("Response headers:");
		        WriteLine(responseHeaders);
		        WriteLine("Response body:");
		        WriteLine(response);
	        }
        }

		class AsyncHttpServer
		{
			readonly HttpListener _listener;
			const string RESPONSE_TEMPLATE = 
                "<html><head><title>Test</title></head><body><h2>Test page</h2>" +
                "<h4>Today is: {0}</h4></body></html>";

			public AsyncHttpServer(int portNumber)
			{
				_listener = new HttpListener();
				_listener.Prefixes.Add($"http://localhost:{portNumber}/");
			}

			public async Task Start()
			{
				_listener.Start();

				while (true)
				{
					var ctx = await _listener.GetContextAsync();
					WriteLine("Client connected...");
					var response = string.Format(RESPONSE_TEMPLATE, DateTime.Now);

					using (var sw = new StreamWriter(ctx.Response.OutputStream))
					{
						await sw.WriteAsync(response);
						await sw.FlushAsync();
					}
				}
			}

			public async Task Stop()
			{
				_listener.Abort();
			}
		}
	}
}
