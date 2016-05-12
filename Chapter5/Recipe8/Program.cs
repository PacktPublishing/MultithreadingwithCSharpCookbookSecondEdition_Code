using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;
using static System.Threading.Thread;

namespace Chapter5.Recipe8
{
	class Program
	{
		static void Main(string[] args)
		{
			Task t = AsynchronousProcessing();
			t.Wait();
		}

		static async Task AsynchronousProcessing()
		{
			var sync = new CustomAwaitable(true);
			string result = await sync;
			WriteLine(result);

			var async = new CustomAwaitable(false);
			result = await async;

			WriteLine(result);
		}

		class CustomAwaitable
		{
			public CustomAwaitable(bool completeSynchronously)
			{
				_completeSynchronously = completeSynchronously;
			}

			public CustomAwaiter GetAwaiter()
			{
				return new CustomAwaiter(_completeSynchronously);
			}

			private readonly bool _completeSynchronously;
		}

		class CustomAwaiter : INotifyCompletion
		{
			private string _result = "Completed synchronously";
			private readonly bool _completeSynchronously;

			public bool IsCompleted => _completeSynchronously;

		    public CustomAwaiter(bool completeSynchronously)
			{
				_completeSynchronously = completeSynchronously;
			}

			public string GetResult()
			{
				return _result;
			}

			public void OnCompleted(Action continuation)
			{
				ThreadPool.QueueUserWorkItem( state => {
					Sleep(TimeSpan.FromSeconds(1));
					_result = GetInfo();
				    continuation?.Invoke();
				});
			}

			private string GetInfo()
			{
				return
				    $"Task is running on a thread id {CurrentThread.ManagedThreadId}." +
				    $" Is thread pool thread: {CurrentThread.IsThreadPoolThread}";
			}
		}
	}
}
