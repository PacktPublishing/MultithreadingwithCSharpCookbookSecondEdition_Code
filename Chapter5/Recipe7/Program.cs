using System;
using System.Threading.Tasks;
using static System.Console;
using static System.Threading.Thread;

namespace Chapter5.Recipe7
{
	class Program
	{
		static void Main(string[] args)
		{
			Task t = AsyncTask();
			t.Wait();

			AsyncVoid();
			Sleep(TimeSpan.FromSeconds(3));

			t = AsyncTaskWithErrors();
			while(!t.IsFaulted)
			{
				Sleep(TimeSpan.FromSeconds(1));
			}
			WriteLine(t.Exception);

			//try
			//{
			//	AsyncVoidWithErrors();
			//	Thread.Sleep(TimeSpan.FromSeconds(3));
			//}
			//catch (Exception ex)
			//{
			//	Console.WriteLine(ex);
			//}

			int[] numbers = {1, 2, 3, 4, 5};
			Array.ForEach(numbers, async number => {
				await Task.Delay(TimeSpan.FromSeconds(1));
				if (number == 3) throw new Exception("Boom!");
				WriteLine(number);
			});

			ReadLine();
		}

		static async Task AsyncTaskWithErrors()
		{
			string result = await GetInfoAsync("AsyncTaskException", 2);
			WriteLine(result);
		}

		static async void AsyncVoidWithErrors()
		{
			string result = await GetInfoAsync("AsyncVoidException", 2);
			WriteLine(result);
		}

		static async Task AsyncTask()
		{
			string result = await GetInfoAsync("AsyncTask", 2);
			WriteLine(result);
		}

		static async void AsyncVoid()
		{
			string result = await GetInfoAsync("AsyncVoid", 2);
			WriteLine(result);
		}

		static async Task<string> GetInfoAsync(string name, int seconds)
		{
			await Task.Delay(TimeSpan.FromSeconds(seconds));
			if(name.Contains("Exception"))
				throw new Exception($"Boom from {name}!");
			return
			    $"Task {name} is running on a thread id {CurrentThread.ManagedThreadId}." +
			    $" Is thread pool thread: {CurrentThread.IsThreadPoolThread}";
		}
	}
}
