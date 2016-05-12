using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using static System.Text.Encoding;

namespace Chapter9.Recipe1
{
	class Program
	{
		static void Main(string[] args)
		{
			var t = ProcessAsynchronousIO();
			t.GetAwaiter().GetResult();
		}

	    const int BUFFER_SIZE = 4096;

	    static async Task ProcessAsynchronousIO()
	    {
		    using (var stream = new FileStream(
			    "test1.txt", FileMode.Create, FileAccess.ReadWrite, FileShare.None, BUFFER_SIZE))
		    {
			    WriteLine($"1. Uses I/O Threads: {stream.IsAsync}");

			    byte[] buffer = UTF8.GetBytes(CreateFileContent());
			    var writeTask = Task.Factory.FromAsync(
				    stream.BeginWrite, stream.EndWrite, buffer, 0, buffer.Length, null);

			    await writeTask;
		    }

		    using (var stream = new FileStream("test2.txt", FileMode.Create, FileAccess.ReadWrite,
			    FileShare.None, BUFFER_SIZE, FileOptions.Asynchronous))
		    {
			    WriteLine($"2. Uses I/O Threads: {stream.IsAsync}");

			    byte[] buffer = UTF8.GetBytes(CreateFileContent());
			    var writeTask = Task.Factory.FromAsync(
				    stream.BeginWrite, stream.EndWrite, buffer, 0, buffer.Length, null);

			    await writeTask;
		    }

		    using (var stream = File.Create("test3.txt", BUFFER_SIZE, FileOptions.Asynchronous))
		    using (var sw = new StreamWriter(stream))
		    {
			    WriteLine($"3. Uses I/O Threads: {stream.IsAsync}");
			    await sw.WriteAsync(CreateFileContent());
		    }

		    using (var sw = new StreamWriter("test4.txt", true))
		    {
			    WriteLine($"4. Uses I/O Threads: {((FileStream)sw.BaseStream).IsAsync}");
			    await sw.WriteAsync(CreateFileContent());
		    }

		    WriteLine("Starting parsing files in parallel");

		    var readTasks = new Task<long>[4];
		    for (int i = 0; i < 4; i++)
		    {
		        string fileName = $"test{i + 1}.txt";
                readTasks[i] = SumFileContent(fileName);
		    }

		    long[] sums = await Task.WhenAll(readTasks);

		    WriteLine($"Sum in all files: {sums.Sum()}");

		    WriteLine("Deleting files...");

		    Task[] deleteTasks = new Task[4];
		    for (int i = 0; i < 4; i++)
		    {
			    string fileName = $"test{i + 1}.txt";
			    deleteTasks[i] = SimulateAsynchronousDelete(fileName);
		    }

		    await Task.WhenAll(deleteTasks);

		    WriteLine("Deleting complete.");
	    }

	    static string CreateFileContent()
	    {
		    var sb = new StringBuilder();
		    for (int i = 0; i < 100000; i++)
		    {
			    sb.Append($"{new Random(i).Next(0, 99999)}");
			    sb.AppendLine();
		    }
		    return sb.ToString();
	    }

	    static async Task<long> SumFileContent(string fileName)
	    {
		    using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read,
			    FileShare.None, BUFFER_SIZE, FileOptions.Asynchronous))
		    using (var sr = new StreamReader(stream))
		    {
			    long sum = 0;
			    while (sr.Peek() > -1)
			    {
				    string line = await sr.ReadLineAsync();
				    sum += long.Parse(line);
			    }

			    return sum;
		    }
	    }

	    static Task SimulateAsynchronousDelete(string fileName)
	    {
		    return Task.Run(() => File.Delete(fileName));
	    }
	}
}
