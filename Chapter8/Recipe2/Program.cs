using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using static System.Console;
using static System.Threading.Thread;

namespace Chapter8.Recipe2
{
	class Program
	{
		static void Main(string[] args)
		{
			var observer = new CustomObserver();

			var goodObservable = new CustomSequence(new[] {1, 2, 3, 4, 5});
			var badObservable = new CustomSequence(null);

			using (IDisposable subscription = goodObservable.Subscribe(observer))
			{
			}

			using (IDisposable subscription = goodObservable
                .SubscribeOn(TaskPoolScheduler.Default).Subscribe(observer))
			{
				Sleep(TimeSpan.FromMilliseconds(100));
				WriteLine("Press ENTER to continue");
				ReadLine();
			}

			using (IDisposable subscription = badObservable
                .SubscribeOn(TaskPoolScheduler.Default).Subscribe(observer))
			{
				Sleep(TimeSpan.FromMilliseconds(100));
				WriteLine("Press ENTER to continue");
				ReadLine();
			}
		}

		class CustomObserver : IObserver<int>
		{
			public void OnNext(int value)
			{
				WriteLine($"Next value: {value}; Thread Id: {CurrentThread.ManagedThreadId}");
			}

			public void OnError(Exception error)
			{
				WriteLine($"Error: {error.Message}");
			}

			public void OnCompleted()
			{
				WriteLine("Completed");
			}
		}

		class CustomSequence : IObservable<int>
		{
			private readonly IEnumerable<int> _numbers;
 
			public CustomSequence(IEnumerable<int> numbers)
			{
				_numbers = numbers;
			}
			public IDisposable Subscribe(IObserver<int> observer)
			{
				foreach (var number in _numbers)
				{
					observer.OnNext(number);
				}
				observer.OnCompleted();
				return Disposable.Empty;
			}
		}
	}
}
