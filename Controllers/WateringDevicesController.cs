using kangla_backend.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kangla_backend.DTO;
using AutoMapper;

namespace kangla_backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WateringDevicesController : ControllerBase
    {
        private readonly ILogger<WateringDevicesController> _logger;
        private readonly WateringContext _context;
        private readonly IMapper _mapper;

        public WateringDevicesController(ILogger<WateringDevicesController> logger, WateringContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WateringDeviceResponseDto>>> GetWateringDevices()
        {
            var wateringDevices = await _context.WateringDevices.ToListAsync();
            var wateringDevicesDto = _mapper.Map<List<WateringDeviceResponseDto>>(wateringDevices);

            return Ok(wateringDevicesDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WateringDevice>> GetWateringDevice(int id)
        {
            var wateringDevice = await _context.WateringDevices.FindAsync(id);

            if (wateringDevice == null)
            {
                return NotFound();
            }

            var wateringDeviceResponseDto = _mapper.Map<WateringDeviceResponseDto>(wateringDevice);

            return Ok(wateringDeviceResponseDto);
        }

        [HttpPost]
        public async Task<ActionResult<WateringDevice>> PostWateringDevice(WateringDeviceCreateRequestDto wateringDevice)
        {
            var wateringDeviceEntity = _mapper.Map<WateringDevice>(wateringDevice);
            _context.WateringDevices.Add(wateringDeviceEntity);
            await _context.SaveChangesAsync();         
            var wateringDeviceResponseDto = _mapper.Map<WateringDeviceResponseDto>(wateringDeviceEntity);

            // Return a 201 Created response with the created entity DTO
            return CreatedAtAction(nameof(GetWateringDevice), new { id = wateringDeviceResponseDto.Id }, wateringDeviceResponseDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutWateringDevice(int id, WateringDeviceUpdateRequestDto wateringDevice)
        {
            var existingDeviceEntity = await _context.WateringDevices.FindAsync(id);

            if (existingDeviceEntity == null)
            {
                return NotFound();
            }

            existingDeviceEntity.Name = wateringDevice.Name;
            existingDeviceEntity.Description = wateringDevice.Description;
            existingDeviceEntity.Location = wateringDevice.Location;
            existingDeviceEntity.Notes = wateringDevice.Notes;
            existingDeviceEntity.WaterNow = wateringDevice.WaterNow;
            existingDeviceEntity.MinimumSoilHumidity = wateringDevice.MinimumSoilHumidity;
            existingDeviceEntity.WateringIntervalSetting = wateringDevice.WateringIntervalSetting;
            existingDeviceEntity.WateringDurationSetting = wateringDevice.WateringDurationSetting;

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

            var wateringDeviceResponseDto = _mapper.Map<WateringDeviceResponseDto>(existingDeviceEntity);

            return Ok(wateringDeviceResponseDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWateringDevice(int id)
        {
            var wateringDeviceEntity = await _context.WateringDevices.FindAsync(id);
            if (wateringDeviceEntity == null)
            {
                return NotFound();
            }

            _context.WateringDevices.Remove(wateringDeviceEntity);
            await _context.SaveChangesAsync();

            return NoContent();                        
        }

        //Todo move to service
        private bool WateringDeviceExists(int id)
        {
            return _context.WateringDevices.Any(e => e.Id == id);
        }       
    }
}
