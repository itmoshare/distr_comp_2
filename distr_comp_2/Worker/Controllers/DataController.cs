using System;
using System.Linq;
using core.Storage;
using Microsoft.AspNetCore.Mvc;

namespace Worker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController
    {
        private readonly Storage _storage;

        public DataController(Storage storage)
        {
            _storage = storage;
        }

        [HttpPut("{key}")]
        public void Put(string key, long data)
        {
            _storage.Insert(key, data);
            Console.WriteLine($"Insert: {key} - {data}");
        }

        [HttpGet("{key}")]
        public ActionResult<long> Get(string key)
        {
            var res = _storage.Select(key);
            Console.WriteLine($"Select: result = {res}");
            return res?.Value;
        }

        [HttpGet]
        public ActionResult<long[]> Get()
        {
            var res = _storage.Select().Select(x => x.Value).ToArray();
            Console.WriteLine($"Select (all): {string.Join(",", res)}");
            return res;
        }
    }
}