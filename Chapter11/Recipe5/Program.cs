using System;
using System.Threading.Tasks;
using static System.Console;

namespace OSXConsoleApplication
{
	class Program
	{
		static void Main(string[] args)
		{
		    WriteLine(".NET Core app on Ubuntu");
			RunCodeAsync().GetAwaiter().GetResult();
		}

		static async Task RunCodeAsync()
		{
			try
			{
				string result = await GetInfoAsync("Async 1");
				WriteLine(result);
				result = await GetInfoAsync("Async 2");
				WriteLine(result);
			}
			catch (Exception ex)
			{
				WriteLine(ex);
			}
		}

		static async Task<string> GetInfoAsync(string name)
		{
			WriteLine($"Task {name} started!");
			await Task.Delay(TimeSpan.FromSeconds(2));
			if(name == "Async 2")
				throw new Exception("Boom!");
			return
			    $"Task {name} completed successfully!"
			  // + $"Thread id {System.Threading.Thread.CurrentThread.ManagedThreadId}."
                ;
		}
	}
}