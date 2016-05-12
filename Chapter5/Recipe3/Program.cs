using System;
using System.Threading.Tasks;
using static System.Console;
using static System.Threading.Thread;

namespace Chapter5.Recipe3
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
			var containerTask = new Task(() => { 
				Task<string> t = GetInfoAsync("TPL 1");
				t.ContinueWith(task => {
					WriteLine(t.Result);
					Task<string> t2 = GetInfoAsync("TPL 2");
					t2.ContinueWith(innerTask => WriteLine(innerTask.Result),
						TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.AttachedToParent);
					t2.ContinueWith(innerTask => WriteLine(innerTask.Exception.InnerException),
						TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.AttachedToParent);
					},
					TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.AttachedToParent);

				t.ContinueWith(task => WriteLine(t.Exception.InnerException),
					TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.AttachedToParent);
			});

			containerTask.Start();
			return containerTask;
		}

		static async Task AsynchronyWithAwait()
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
			if(name == "TPL 2")
				throw new Exception("Boom!");
			return
			    $"Task {name} is running on a thread id {CurrentThread.ManagedThreadId}." +
			    $" Is thread pool thread: {CurrentThread.IsThreadPoolThread}";
		}
	}
}