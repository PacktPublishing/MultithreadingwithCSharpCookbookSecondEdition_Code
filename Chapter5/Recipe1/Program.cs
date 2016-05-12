using System;
using System.Threading.Tasks;
using static System.Console;
using static System.Threading.Thread;

namespace Chapter5.Recipe1
{
	class Program
	{
		static void Main(string[] args)
		{
			Task t = AsynchronyWithTPL();
			t.Wait();

			t = AsynchronyWithAwait();
			t.Wait();
		}

		static Task AsynchronyWithTPL()
		{
			Task<string> t = GetInfoAsync("Task 1");
			Task t2 = t.ContinueWith(task => WriteLine(t.Result),
				TaskContinuationOptions.NotOnFaulted);
			Task t3 = t.ContinueWith(task => WriteLine(t.Exception.InnerException),
				TaskContinuationOptions.OnlyOnFaulted);

			return Task.WhenAny(t2, t3);
		}

		static async Task AsynchronyWithAwait()
		{
			try
			{
				string result = await GetInfoAsync("Task 2");
				WriteLine(result);
			}
			catch (Exception ex)
			{
				WriteLine(ex);
			}
		}

		static async Task<string> GetInfoAsync(string name)
		{
			await Task.Delay(TimeSpan.FromSeconds(2));
			//throw new Exception("Boom!");
			return
			    $"Task {name} is running on a thread id {CurrentThread.ManagedThreadId}." + 
                $" Is thread pool thread: {CurrentThread.IsThreadPoolThread}";
		}
	}
}
