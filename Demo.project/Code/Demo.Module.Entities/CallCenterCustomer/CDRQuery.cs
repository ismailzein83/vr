using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public enum CDRType{International=1 , National= 2}
    public class CDRQuery
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public List<CDRType> Type { get; set; }
    }
}
