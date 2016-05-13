using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities.EntitySynchronization;

namespace Vanrise.Entities
{
    public class Currency 
    {
        public int CurrencyId { get; set; }

        public string Name { get; set; }

        public string Symbol { get; set; }

        public string SourceId { get; set; }
        
    }
}
