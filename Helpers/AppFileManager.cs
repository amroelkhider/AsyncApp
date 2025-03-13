using AsyncApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncApp.Helpers;
public class AppFileManager
{
	private static readonly string baseDire = AppDomain.CurrentDomain.BaseDirectory;

	public static List<string> ReadAllLines(string fileName)
	{
		return File.ReadAllLines(Path.Combine(baseDire, "Data", fileName)).ToList();
	}
	public static Task<List<string>> ReadAllLines(string fileName, CancellationToken token)
	{
		return Task.Run(async () =>
		{
			using var stremReader = new StreamReader(File.OpenRead(Path.Combine(baseDire, "Data", fileName)));

			var lines = new List<string>();
			while (await stremReader.ReadLineAsync() is string line)
			{
				//throw OpertaionCancelExpection if token is canceled
				//token?.ThrowIfCancellationRequested();
				if (token.IsCancellationRequested)
				{
					break;
				}
				lines.Add(line);
			}

			return lines;
		}, token);

	}

	public static List<Customer> GetCustomers(IEnumerable<string> lines)
	{
		return lines.Select(line => GetCustomer(line)).ToList();
	}
	public static Task<List<Customer>> GetCustomers(IEnumerable<string> lines, CancellationToken token)
	{
		return Task.Run(() =>
		{
			var customers = new List<Customer>();
			foreach (var line in lines)
			{
				if (token.IsCancellationRequested)
				{
					break;
				}
				customers.Add(GetCustomer(line));
			}
			return customers;
		}, token);
	}


	private static Customer GetCustomer(string line)
	{
		var parts = line.Split(',');
		return new Customer
		{
			Index = int.Parse(parts[0]),
			CustomerId = parts[1],
			FirstName = parts[2],
			LastName = parts[3],
			Company = parts[4],
			City = parts[5],
			Country = parts[6],
			Phone1 = parts[7],
			Phone2 = parts[8],
			Email = parts[9],
			SubscriptionDate = DateTime.Parse(parts[10]),
			Website = parts[11]
		};
	}
}

