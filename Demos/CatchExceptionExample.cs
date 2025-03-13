using AsyncApp.Demos.Contract;
using AsyncApp.Helpers;

namespace AsyncApp.Demoes; 
public class CatchExceptionExample : IExample
{
	public void Run()
	{
		try
		{
			var loadedLines = Task.Run(() =>
			{
				AppLoger.LogSuccess("********* [start reading lines] *********");
				var data = AppFileManager.ReadAllLines("customers-100.csv");
				return data;
			});

			loadedLines.ContinueWith(task =>
			{
				AppLoger.LogError(task.Exception?.InnerException?.Message);
			}, TaskContinuationOptions.OnlyOnFaulted);

			var customers = loadedLines.ContinueWith((task) =>
			{
				AppLoger.LogSuccess("********* [start processing lines] *********");
				var lines = task.Result.Skip(1);

				return AppFileManager.GetCustomers(lines);

			}, TaskContinuationOptions.OnlyOnRanToCompletion);

			loadedLines.ContinueWith(task =>
			{
				AppLoger.LogError(task.Exception?.InnerException?.Message);
			}, TaskContinuationOptions.OnlyOnFaulted);


			_ = customers.ContinueWith((task) =>
			{
				AppLoger.LogSuccess("********* [start displaying customers] *********");

				if (task.Status == TaskStatus.RanToCompletion)
				{
					foreach (var line in task.Result)
					{
						Console.WriteLine(line.FirstName);
					}
				}

				AppLoger.LogSuccess("********* complete *********");
			}, TaskContinuationOptions.OnlyOnRanToCompletion);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}
}
