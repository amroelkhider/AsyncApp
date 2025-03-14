using AsyncApp.Demos.Contract;
using AsyncApp.Helpers;

namespace AsyncApp.Demoes;
public class BasicExample : IExample
{
	public void Run()
	{
		try
		{
			var loadedLines = Task.Run(() =>
			{
				AppLogger.LogSuccess("********* [start reading lines] *********");
				var data = AppFileManager.ReadAllLines("customers-100.csv");
				return data;
			});

			var customers = loadedLines.ContinueWith((task) =>
			{
				AppLogger.LogSuccess("********* [start processing lines] *********");
				var lines = task.Result.Skip(1);

				return AppFileManager.GetCustomers(lines);

			});

			_ = customers.ContinueWith((task) =>
			{
				AppLogger.LogSuccess("********* [start displaying customers] *********");

				if (task.Status == TaskStatus.RanToCompletion)
				{
					foreach (var line in task.Result)
					{
						Console.WriteLine(line.FirstName);
					}
				}

				AppLogger.LogSuccess("********* complete *********");
			});

		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

}