using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.SMS.Entities
{
    public class SMSChargingPolicyDefinitionSettings : ChargingPolicyDefinitionSettings
    {

        public override string ChargingPolicyEditor
        {
            get
            {
                return "retail-sms-chargingpolicy-settings";
            }
            set
            {

            }
        }

        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }
    }
}
