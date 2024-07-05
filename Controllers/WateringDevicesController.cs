using kangla_backend.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace kangla_backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
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

        [HttpPost]
        public async Task<ActionResult<WateringDevice>> PostWateringDevice(WateringDevice wateringDevice)
        {
            _context.WateringDevices.Add(wateringDevice);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetWateringDevice), new { id = wateringDevice.Id }, wateringDevice);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutWateringDevice(int id, WateringDevice wateringDevice)
        {
            if (id != wateringDevice.Id)
            {
                return BadRequest();
            }

            _context.Entry(wateringDevice).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WateringDeviceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWateringDevice(int id)
        {
            var wateringDevice = await _context.WateringDevices.FindAsync(id);
            if (wateringDevice == null)
            {
                return NotFound();
            }

            _context.WateringDevices.Remove(wateringDevice);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool WateringDeviceExists(int id)
        {
            return _context.WateringDevices.Any(e => e.Id == id);
        }
    }

}
