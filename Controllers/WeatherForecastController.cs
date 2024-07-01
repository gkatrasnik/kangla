using Microsoft.AspNetCore.Mvc;

namespace kangla_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WateringDevicesController : ControllerBase
    {
        WateringDevice[] wateringDevicesList =
       [
            new WateringDevice
            {
                Id = 1,
                Name = "My device",
                Description = "My device description",
                Location = "My location",
                Notes = "My notes",
                Active = true,
                Deleted = false,
                SoilHumidity = 0.5,
                LastWatering = DateTime.Now,
                WateringInterval = 300,
                WateringDuration = 3,
                WaterNow = false
            },
            new WateringDevice
            {
                Id = 2,
                Name = "My device 2",
                Description = "My device description 2",
                Location = "My location 2",
                Notes = "My notes 2",
                Active = true,
                Deleted = false,
                SoilHumidity = 0.5,
                LastWatering = DateTime.Now,
                WateringInterval = 300,
                WateringDuration = 3,
                WaterNow = false
            },
            new WateringDevice
            {
                Id = 3,
                Name = "My device 3",
                Description = "My device description 3",
                Location = "My location 3",
                Notes = "My notes 3",
                Active = true,
                Deleted = false,
                SoilHumidity = 0.5,
                LastWatering = DateTime.Now,
                WateringInterval = 300,
                WateringDuration = 3,
                WaterNow = false
            }
       ];

        private readonly ILogger<WateringDevicesController> _logger;

        public WateringDevicesController(ILogger<WateringDevicesController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WateringDevice> Get()
        {
            return this.wateringDevicesList;
        }

        [HttpGet("{id}")]
        public WateringDevice? Get(int id)
        {
            var wateringDevice = this.wateringDevicesList.FirstOrDefault(device => device.Id == id);
            if (wateringDevice == null)
            {
                return null;
            }

            return wateringDevice;
        }
    }
}
