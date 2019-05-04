using System;
using core.Storage;
using Microsoft.AspNetCore.Mvc;

namespace Worker.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MonitoringController
	{
		private readonly Storage _storage;

		public MonitoringController(Storage storage)
		{
			_storage = storage;
		}

		[HttpGet("ping")]
		public ActionResult<bool> Ping()
		{
			return true;
		}

		[HttpGet("stats")]
		public void Stats()
		{
			Console.WriteLine("Getting stats..");
			Console.WriteLine($"Stored items: {_storage.GetItemsCount()}");
			Console.WriteLine($"Handled commands: {_storage.GetHandledCount()}");
		}
	}
}