using System;
using System.Net;
using core.Storage;
using Microsoft.AspNetCore.Mvc;

namespace PeerWorker.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MonitoringController
	{
		private readonly Storage _storage;
		private readonly Executor _executor;

		public MonitoringController(Storage storage, Executor executor)
		{
			_storage = storage;
			_executor = executor;
		}

		[HttpGet("ping")]
		public ActionResult Ping()
		{
			return _executor.IsInitialized
				? new StatusCodeResult((int)HttpStatusCode.OK)
				: new StatusCodeResult((int)HttpStatusCode.ServiceUnavailable);
		}

		[HttpGet("stats")]
		public string Stats()
		{
			return $"Stored items: {_storage.GetItemsCount()} \n Handled commands: {_storage.GetHandledCount()}";
		}
	}
}