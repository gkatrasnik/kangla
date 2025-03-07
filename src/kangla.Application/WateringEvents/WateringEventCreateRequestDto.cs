﻿using System.ComponentModel.DataAnnotations;

namespace kangla.Application.WateringEvents
{
    public class WateringEventCreateRequestDto
    {
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public DateTime End { get; set; }
        [Required]
        public int PlantId { get; set; }
    }
}
