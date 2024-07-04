using kangla_backend.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace kangla_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WateringDevicesController : ControllerBase
    {
        private readonly ILogger<WateringDevicesController> _logger;
        private readonly WateringContext _context;

        public WateringDevicesController(ILogger<WateringDevicesController> logger, WateringContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WateringDevice>>> GetWateringDevices()
        {
            return await _context.WateringDevices.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WateringDevice>> GetWateringDevice(int id)
        {
            var wateringDevice = await _context.WateringDevices.FindAsync(id);

            if (wateringDevice == null)
            {
                return NotFound();
            }

            return wateringDevice;
        }
    }
}
