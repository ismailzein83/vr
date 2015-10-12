using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class CurrencyFactor
    {
        public string CurrencyId { get; set; }

        public double Factor { get; set; }

        public string Name { get; set; }

        public bool IsMain { get; set; }
    }
}
