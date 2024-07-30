﻿using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.DTO;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace kangla_backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HumidityMeasurementsController : ControllerBase
    {
        private readonly ILogger<HumidityMeasurementsController> _logger;
        private readonly IHumidityMeasurementService _humidityMeasurementService;

        public HumidityMeasurementsController(ILogger<HumidityMeasurementsController> logger, IHumidityMeasurementService humidityMeasurementService)
        {
            _logger = logger;
            _humidityMeasurementService = humidityMeasurementService;
        }

        [HttpGet("device/{deviceId}")]
        public async Task<ActionResult<IEnumerable<HumidityMeasurementResponseDto>>> GetHumidityMeasurementsForDevice(int deviceId, int pageNumber, int pageSize)
        {
            var humidityMeasurements = await _humidityMeasurementService.GetHumidityMeasurementsForDeviceAsync(deviceId, pageNumber, pageSize);           
            return Ok(humidityMeasurements);
        }

        [HttpPost]
        public async Task<ActionResult<HumidityMeasurementResponseDto>> PostHumidityMeasurement(HumidityMeasurementCreateRequestDto humidityMeasurement)
        {            
            var createdMeasurement = await _humidityMeasurementService.CreateHumidityMeasurementAsync(humidityMeasurement);
            return CreatedAtAction(nameof(GetHumidityMeasurementsForDevice), new { deviceId = createdMeasurement.WateringDeviceId }, createdMeasurement);
        }
    }
}
