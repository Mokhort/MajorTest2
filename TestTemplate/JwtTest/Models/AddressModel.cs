using JwtTest.EF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JwtTest.Models
{
    public class AddressModel
    {
        public int Id { get; set; }
        [Required]
        [DisplayName("Адрес")]
        [StringLength(100)]
        public string Addr { get; set; }
        [DisplayName("Описание")]
        [StringLength(100)]
        public string Description { get; set; }
        [Required]
        [DisplayName("Стоимость аренды в сутки")]
        public double Cost { get; set; }
        [Required]
        [DisplayName("Количество комнат")]
        public int Rooms { get; set; }
        
    }
}
