using System;
using System.Threading.Tasks;
using static System.Console;
using static System.Threading.Thread;

namespace Chapter4.Recipe1
{
	class Program
	{
		static void Main(string[] args)
		{
			var t1 = new Task(() => TaskMethod("Task 1"));
			var t2 = new Task(() => TaskMethod("Task 2"));
			t2.Start();
			t1.Start();
			Task.Run(() => TaskMethod("Task 3"));
			Task.Factory.StartNew(() => TaskMethod("Task 4"));
			Task.Factory.StartNew(() => TaskMethod("Task 5"), TaskCreationOptions.LongRunning);
			Sleep(TimeSpan.FromSeconds(1));
		}

		static void TaskMethod(string name)
		{
			WriteLine($"Task {name} is running on a thread id " +
			          $"{CurrentThread.ManagedThreadId}. Is thread pool thread: " +
			          $"{CurrentThread.IsThreadPoolThread}");
		}
	}
}
