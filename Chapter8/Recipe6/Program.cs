using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using System.Timers;
using static System.Console;
using static System.Threading.Thread;

namespace Chapter8.Recipe6
{
	class Program
	{
		delegate string AsyncDelegate(string name);

		static void Main(string[] args)
		{
			IObservable<string> o = LongRunningOperationAsync("Task1");
			using (var sub = OutputToConsole(o))
			{
				Sleep(TimeSpan.FromSeconds(2));
			};
			WriteLine(" ---------------- ");

			Task<string> t = LongRunningOperationTaskAsync("Task2");
			using (var sub = OutputToConsole(t.ToObservable()))
			{
				Sleep(TimeSpan.FromSeconds(2));
			};
			WriteLine(" ---------------- ");

			AsyncDelegate asyncMethod = LongRunningOperation;

			// marked as obsolete, use tasks instead
			Func<string, IObservable<string>> observableFactory = 
				Observable.FromAsyncPattern<string, string>(
                    asyncMethod.BeginInvoke, asyncMethod.EndInvoke);

			o = observableFactory("Task3");
			using (var sub = OutputToConsole(o))
			{
				Sleep(TimeSpan.FromSeconds(2));
			};
			WriteLine(" ---------------- ");

			o = observableFactory("Task4");
			AwaitOnObservable(o).Wait();
			WriteLine(" ---------------- ");

			using (var timer = new Timer(1000))
			{
				var ot = Observable.
                    FromEventPattern<ElapsedEventHandler, ElapsedEventArgs>(
					h => timer.Elapsed += h,
                    h => timer.Elapsed -= h);
				timer.Start();

				using (var sub = OutputToConsole(ot))
				{
					Sleep(TimeSpan.FromSeconds(5));
				}
				WriteLine(" ---------------- ");
				timer.Stop();
			}
		}

		static async Task<T> AwaitOnObservable<T>(IObservable<T> observable)
		{
			T obj = await observable;
			WriteLine($"{obj}" );
			return obj;
		}

		static Task<string> LongRunningOperationTaskAsync(string name)
		{
			return Task.Run(() => LongRunningOperation(name));
		}

		static IObservable<string> LongRunningOperationAsync(string name)
		{
			return Observable.Start(() => LongRunningOperation(name));
		}

		static string LongRunningOperation(string name)
		{
			Sleep(TimeSpan.FromSeconds(1));
			return $"Task {name} is completed. Thread Id {CurrentThread.ManagedThreadId}";
		}

		static IDisposable OutputToConsole(IObservable<EventPattern<ElapsedEventArgs>> sequence)
		{
			return sequence.Subscribe(
				obj => WriteLine($"{obj.EventArgs.SignalTime}")
				, ex => WriteLine($"Error: {ex.Message}")
				, () => WriteLine("Completed")
			);
		}

		static IDisposable OutputToConsole<T>(IObservable<T> sequence)
		{
			return sequence.Subscribe(
				obj => WriteLine("{0}", obj)
				, ex => WriteLine("Error: {0}", ex.Message)
				, () => WriteLine("Completed")
			);
		}
	}
}
