﻿using System.ComponentModel.DataAnnotations;

namespace PhimMoi.Domain.Models
{
    public class Category
    {
        public string Id { get; set; }
        public int IdNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        public string? NormalizeName { get; set; }
        public string? Description { get; set; }
        public List<Movie> Movies { get; set; }
    }
}