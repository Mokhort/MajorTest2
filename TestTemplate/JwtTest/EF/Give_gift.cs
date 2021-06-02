using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtTest.EF
{
    public class Give_gift
    {
        public int Id { get; set; }   
        public virtual List<Person> From { get; set; }
        public virtual List<Person> To { get; set; }
        public virtual List<Gift> Gift { get; set; }

    }
}
