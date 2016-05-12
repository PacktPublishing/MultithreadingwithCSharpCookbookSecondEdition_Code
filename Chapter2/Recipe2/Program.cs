using System;
using System.Threading;
using static System.Console;

namespace Chapter2.Recipe2
{
	class Program
	{
		static void Main(string[] args)
		{
			const string MutexName = "CSharpThreadingCookbook";

			using (var m = new Mutex(false, MutexName))
			{
				if (!m.WaitOne(TimeSpan.FromSeconds(5), false))
				{
					WriteLine("Second instance is running!");
				}
				else
				{
					WriteLine("Running!");
					ReadLine();
					m.ReleaseMutex();
				}
			}
		}
	}
}
