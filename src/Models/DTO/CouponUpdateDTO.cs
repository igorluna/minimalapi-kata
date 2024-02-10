using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace minimal_kata.Models.DTO
{
    public class CouponUpdateDTO
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        [Required]
        public int Percent { get; set; }
        public bool IsActive { get; set; }
    }
}