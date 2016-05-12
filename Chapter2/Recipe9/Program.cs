using System;
using System.Threading;
using static System.Console;
using static System.Threading.Thread;

namespace Chapter2.Recipe9
{
	class Program
	{
		static void Main(string[] args)
		{
			var t1 = new Thread(UserModeWait);
			var t2 = new Thread(HybridSpinWait);

			WriteLine("Running user mode waiting");
			t1.Start();
			Sleep(20);
			_isCompleted = true;
			Sleep(TimeSpan.FromSeconds(1));
			_isCompleted = false;
			WriteLine("Running hybrid SpinWait construct waiting");
			t2.Start();
			Sleep(5);
			_isCompleted = true;
		}

		static volatile bool _isCompleted = false;

		static void UserModeWait()
		{
			while (!_isCompleted)
			{
				Write(".");
			}
			WriteLine();
			WriteLine("Waiting is complete");
		}

		static void HybridSpinWait()
		{
			var w = new SpinWait();
			while (!_isCompleted)
			{
				w.SpinOnce();
				WriteLine(w.NextSpinWillYield);
			}
			WriteLine("Waiting is complete");
		}
	}
}
