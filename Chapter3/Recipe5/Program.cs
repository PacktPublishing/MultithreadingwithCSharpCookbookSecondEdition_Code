using System;
using System.Threading;
using static System.Console;
using static System.Threading.Thread;


namespace Chapter3.Recipe5
{
	class Program
	{
		static void Main(string[] args)
		{
			RunOperations(TimeSpan.FromSeconds(5));
			RunOperations(TimeSpan.FromSeconds(7));
		}

		static void RunOperations(TimeSpan workerOperationTimeout)
		{
			using (var evt = new ManualResetEvent(false))
			using (var cts = new CancellationTokenSource())
			{
				WriteLine("Registering timeout operation...");
				var worker = ThreadPool.RegisterWaitForSingleObject(evt
                    , (state, isTimedOut) => WorkerOperationWait(cts, isTimedOut)
                    , null
                    , workerOperationTimeout
                    , true);

				WriteLine("Starting long running operation...");
				ThreadPool.QueueUserWorkItem(_ => WorkerOperation(cts.Token, evt));

				Sleep(workerOperationTimeout.Add(TimeSpan.FromSeconds(2)));
				worker.Unregister(evt);
			}
		}

		static void WorkerOperation(CancellationToken token, ManualResetEvent evt)
		{
			for(int i = 0; i < 6; i++)
			{
				if (token.IsCancellationRequested)
				{
					return;
				}
				Sleep(TimeSpan.FromSeconds(1));
			}
			evt.Set();
		}

		static void WorkerOperationWait(CancellationTokenSource cts, bool isTimedOut)
		{
			if (isTimedOut)
			{
				cts.Cancel();
				WriteLine("Worker operation timed out and was canceled.");
			}
			else
			{
				WriteLine("Worker operation succeded.");
			}
		}
	}
}
