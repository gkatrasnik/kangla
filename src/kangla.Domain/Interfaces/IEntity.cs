﻿using System.ComponentModel.DataAnnotations;

namespace kangla.Domain.Interfaces
{
    public class IEntity
    {       
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
