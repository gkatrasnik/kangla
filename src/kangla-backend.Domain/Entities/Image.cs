﻿using Domain.Interfaces;

namespace Domain.Entities
{
    public class Image : IEntity
    {
        public byte[] Data { get; set; } = default!;
    }
}
