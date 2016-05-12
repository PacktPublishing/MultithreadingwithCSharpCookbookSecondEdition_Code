using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using static System.Console;
using static System.Threading.Thread;

namespace Chapter8.Recipe4
{
	class Program
	{
		static void Main(string[] args)
		{
			IObservable<int> o = Observable.Return(0);
			using (var sub = OutputToConsole(o));
			WriteLine(" ---------------- ");
	
			o = Observable.Empty<int>();
			using (var sub = OutputToConsole(o));
			WriteLine(" ---------------- ");

			o = Observable.Throw<int>(new Exception());
			using (var sub = OutputToConsole(o));
			WriteLine(" ---------------- ");

			o = Observable.Repeat(42);
			using (var sub = OutputToConsole(o.Take(5)));
			WriteLine(" ---------------- ");

			o = Observable.Range(0, 10);
			using (var sub = OutputToConsole(o));
			WriteLine(" ---------------- ");

			o = Observable.Create<int>(ob => {
				for (int i = 0; i < 10; i++)
				{
					ob.OnNext(i);
				}
				return Disposable.Empty;
			});
			using (var sub = OutputToConsole(o)) ;
			WriteLine(" ---------------- ");

			o = Observable.Generate(
				0 // initial state
				, i => i < 5 // while this is true we continue the sequence
				, i => ++i // iteration
				, i => i*2 // selecting result
			);
			using (var sub = OutputToConsole(o));
			WriteLine(" ---------------- ");

			IObservable<long> ol = Observable.Interval(TimeSpan.FromSeconds(1));
			using (var sub = OutputToConsole(ol))
			{
				Sleep(TimeSpan.FromSeconds(3));
			};
			WriteLine(" ---------------- ");

			ol = Observable.Timer(DateTimeOffset.Now.AddSeconds(2));
			using (var sub = OutputToConsole(ol))
			{
				Sleep(TimeSpan.FromSeconds(3));
			};
			WriteLine(" ---------------- ");
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
