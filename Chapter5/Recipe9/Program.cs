using System;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ImpromptuInterface;
using static System.Console;
using static System.Threading.Thread;

namespace Chapter5.Recipe9
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
			string result = await GetDynamicAwaitableObject(true);
			WriteLine(result);

			result = await GetDynamicAwaitableObject(false);
			WriteLine(result);
		}

		static dynamic GetDynamicAwaitableObject(bool completeSynchronously)
		{
			dynamic result = new ExpandoObject();
			dynamic awaiter = new ExpandoObject();

			awaiter.Message = "Completed synchronously";
			awaiter.IsCompleted = completeSynchronously;
			awaiter.GetResult = (Func<string>)(() => awaiter.Message);

			awaiter.OnCompleted = (Action<Action>) ( callback => 
				ThreadPool.QueueUserWorkItem(state => {
					Sleep(TimeSpan.FromSeconds(1));
					awaiter.Message = GetInfo();
				    callback?.Invoke();
				})
			);

			IAwaiter<string> proxy = Impromptu.ActLike(awaiter);

			result.GetAwaiter = (Func<dynamic>) ( () => proxy );

			return result;
		}

		static string GetInfo()
		{
			return
			    $"Task is running on a thread id {CurrentThread.ManagedThreadId}." +
			    $" Is thread pool thread: {CurrentThread.IsThreadPoolThread}";
		}
    }

    public interface IAwaiter<T> : INotifyCompletion
    {
        bool IsCompleted { get; }

        T GetResult();
    }
}
