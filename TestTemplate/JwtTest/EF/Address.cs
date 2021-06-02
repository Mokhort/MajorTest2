using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtTest.EF
{
    public class Address
    {
        public int Id { get; set; }
        public string Addr { get; set; }
        public string Description { get; set; }
        public virtual Person Owner { get; set; }
        public double Cost { get; set; }
        public int Rooms { get; set; }
    }
}
