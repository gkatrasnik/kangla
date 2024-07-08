﻿using kangla_backend.Model;
using Microsoft.AspNetCore.Mvc;

namespace kangla_backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WateringEventsController : ControllerBase
    {
        private readonly ILogger<WateringEventsController> _logger;
        private readonly WateringContext _context;

        public WateringEventsController(ILogger<WateringEventsController> logger, WateringContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: api/<ValuesController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ValuesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
