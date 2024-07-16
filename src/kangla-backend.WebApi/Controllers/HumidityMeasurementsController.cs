using Microsoft.AspNetCore.Mvc;

namespace kangla_backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HumidityMeasurementsController : Controller
    {
        private readonly ILogger<HumidityMeasurementsController> _logger;

        public HumidityMeasurementsController(ILogger<HumidityMeasurementsController> logger)
        {
            _logger = logger;
        }

        // GET: api/<ValuesController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value11", "value12" };
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
