using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using static System.Console;
using static System.Threading.Thread;

namespace Chapter8.Recipe1
{
	class Program
	{
		static void Main(string[] args)
		{
			foreach (int i in EnumerableEventSequence())
			{
				Write(i);
			}

            WriteLine();
			WriteLine("IEnumerable");

			IObservable<int> o = EnumerableEventSequence().ToObservable();
			using (IDisposable subscription = o.Subscribe(Write))
			{
				WriteLine();
				WriteLine("IObservable");
			}

			o = EnumerableEventSequence().ToObservable()
                .SubscribeOn(TaskPoolScheduler.Default);
			using (IDisposable subscription = o.Subscribe(Write))
			{
				WriteLine();
				WriteLine("IObservable async");
				ReadLine();
			}
		}

		static IEnumerable<int> EnumerableEventSequence()
		{
			for (int i = 0; i < 10; i++)
			{
				Sleep(TimeSpan.FromSeconds(0.5));
				yield return i;
			}
		}
	}
}