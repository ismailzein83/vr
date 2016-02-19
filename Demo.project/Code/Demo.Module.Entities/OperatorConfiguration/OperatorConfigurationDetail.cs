using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class OperatorConfigurationDetail
    {
        public OperatorConfiguration Entity { get; set; }

        public String OperatorName { get; set; }

        public String AmountTypeName { get; set; }

        public string CDRTypeName { get; set; }

        public string CDRDirectionName { get; set; }

        public string UnitTypeName { get; set; }

        public string CurrencyName { get; set; }

    }
}
