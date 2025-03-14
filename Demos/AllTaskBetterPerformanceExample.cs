using AsyncApp.Helpers;
using AsyncApp.Models;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace AsyncApp.Demoes;

public class AllTaskBetterPerformanceExample
{
	public async Task RunWhenAll()
	{
		try
		{
			CancellationToken cancellationToken = CancellationToken.None;
			var results = new ConcurrentBag<ApiResult>();

			var usersTask = GetUsersAsync(cancellationToken);
			var todosTask = GetTodosAsync(cancellationToken);

			await todosTask.ContinueWith(t =>
			{
				foreach (var item in t.Result)
				{
					results.Add(item);
				}
				AppLogger.LogSuccess(string.Join(",", results.Take(3).Select(r => $"{r.ApiType} | {r.Data}")));

			}).ConfigureAwait(false);

			await usersTask.ContinueWith(t =>
			{
				foreach (var item in t.Result)
				{
					results.Add(item);
				}
				AppLogger.LogSuccess(string.Join(",", results.Take(3).Select(r => $"{r.ApiType} | {r.Data}")));
			}).ConfigureAwait(false);

			
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public async Task RunWhenAny()
	{

		try
		{
			CancellationTokenSource cancellationTokenSource = new();

			var delayTask = Task.Delay(5);
			var allDataTask = Task.WhenAll(GetUsersAsync(cancellationTokenSource.Token), GetTodosAsync(cancellationTokenSource.Token));

			var completedTask = await Task.WhenAny(delayTask, allDataTask);
			if (completedTask == delayTask)
			{
				cancellationTokenSource.Cancel();
				throw new OperationCanceledException("Time out");
			}

			var allResults = allDataTask.Result.SelectMany(data => data);

			foreach (var item in allResults)
			{
				AppLogger.LogSuccess($"Id: {item.Id}, Data: {item.Data}, Type: {item.ApiType}");
			}

		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	private async Task<IEnumerable<ApiResult>> GetUsersAsync(CancellationToken token)
	{
		AppLogger.LogSuccess("********* [start reading users] *********");
		using var client = new HttpClient();
		var response = await client.GetAsync("https://jsonplaceholder.typicode.com/users", token);
		response.EnsureSuccessStatusCode();

		var data = await response.Content.ReadAsStringAsync();
		var users = JsonConvert.DeserializeObject<IEnumerable<User>>(data) ?? Enumerable.Empty<User>();
		return users.Select(user => new ApiResult { Id = user.Id, Data = user.Name, ApiType = "User" });
	}

	private async Task<IEnumerable<ApiResult>> GetTodosAsync(CancellationToken token)
	{
		AppLogger.LogSuccess("********* [start reading todos] *********");
		await Task.Delay(5000);
		using var client = new HttpClient();
		var response = await client.GetAsync("https://jsonplaceholder.typicode.com/todos", token);
		response.EnsureSuccessStatusCode();

		var data = await response.Content.ReadAsStringAsync();
		var todoItems = JsonConvert.DeserializeObject<IEnumerable<TodoItem>>(data)?.Take(10) ?? Enumerable.Empty<TodoItem>();
		return todoItems.Select(todo => new ApiResult { Id = todo.Id, Data = todo.Title, ApiType = "Todo" });
	}

}