using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
   public  class SalePLZoneNotification
    {
        private List<SalePLCodeNotification> _codes = new List<SalePLCodeNotification>();

        public string ZoneName { get; set; }

        public List<SalePLCodeNotification> Codes
        {
            get
            {
                return this._codes;
            }
        }

        public SalePLRateNotification Rate { get; set; }    
    }
}
