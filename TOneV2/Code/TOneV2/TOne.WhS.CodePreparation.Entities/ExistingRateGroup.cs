using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.CodePreparation.Entities.Processing
{
    public class ExistingRateGroup
    {
        public string ZoneName { get; set; }

        private ExistingRatesByOwner _normalRates = new ExistingRatesByOwner();

        public ExistingRatesByOwner NormalRates
        {
            get
            {
                return this._normalRates;
            }
        }
    }
}
