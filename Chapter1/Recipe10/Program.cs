using System;
using System.Threading;
using static System.Console;
using static System.Threading.Thread;

namespace Chapter1.Recipe10
{
	class Program
	{
		static void Main(string[] args)
		{
			object lock1 = new object();
			object lock2 = new object();

			new Thread(() => LockTooMuch(lock1, lock2)).Start();

			lock (lock2)
			{
				Thread.Sleep(1000);
				WriteLine("Monitor.TryEnter allows not to get stuck, returning false after a specified timeout is elapsed");
				if (Monitor.TryEnter(lock1, TimeSpan.FromSeconds(5)))
				{
					WriteLine("Acquired a protected resource succesfully");
				}
				else
				{
					WriteLine("Timeout acquiring a resource!");
				}
			}

			new Thread(() => LockTooMuch(lock1, lock2)).Start();

			WriteLine("----------------------------------");
			lock (lock2)
			{
				WriteLine("This will be a deadlock!");
				Sleep(1000);
				lock (lock1)
				{
					WriteLine("Acquired a protected resource succesfully");
				}
			}
		}

		static void LockTooMuch(object lock1, object lock2)
		{
			lock (lock1)
			{
				Sleep(1000);
				lock (lock2);
			}
		}
	}
}
