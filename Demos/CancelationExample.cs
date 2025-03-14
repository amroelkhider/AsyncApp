using AsyncApp.Demos.Contract;
using AsyncApp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncApp.Demos;
public class CancelationExample : IExample
{
	CancellationTokenSource? cancellationTokenSource;
	public void Run()
	{
		if (cancellationTokenSource is not null)
		{
			cancellationTokenSource.Cancel();
			cancellationTokenSource.Dispose();
			cancellationTokenSource = null;
			AppLogger.LogSuccess("Task was canceled");
			return;
		}

		try
		{
			cancellationTokenSource = new();

			// use to take action when task is canceled
			cancellationTokenSource.Token.Register(() =>
			{
				AppLogger.LogError("Task was canceled");
			});

			AppLogger.LogSuccess("********* [start reading lines] *********");
			Task<List<string>> loadedLines = AppFileManager.ReadAllLines("customers-100.csv", cancellationTokenSource.Token);

			loadedLines.ContinueWith(task =>
			{
				AppLogger.LogError(task.Exception?.InnerException?.Message);
			}, TaskContinuationOptions.OnlyOnFaulted);


			var customers = loadedLines.ContinueWith((task) =>
			{
				AppLogger.LogSuccess("********* [start processing lines] *********");
				var lines = task.Result.Skip(1);

				return AppFileManager.GetCustomers(lines, cancellationTokenSource.Token).Result;

			}, TaskContinuationOptions.OnlyOnRanToCompletion);

			customers.ContinueWith(task =>
			{
				AppLogger.LogError(task.Exception?.InnerException?.Message);
			}, 
			cancellationTokenSource.Token,
			TaskContinuationOptions.OnlyOnFaulted,
			TaskScheduler.Current);


			customers.ContinueWith((task) =>
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

				cancellationTokenSource.Dispose();
				cancellationTokenSource = null;

			}, TaskContinuationOptions.OnlyOnRanToCompletion);
		}
		catch (Exception ex)
		{//AggregateException

			Console.WriteLine(ex.InnerException.Message);
		}

	}


}

