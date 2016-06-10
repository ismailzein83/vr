using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Entities
{
    public class ChargingPolicyPartTypeConfig : ExtensionConfiguration
    {
        /// <summary>
        /// this is a link to Extension Configuration name of the Charging Policy Parts of this type
        /// e.g. Retail_BE_ChargingPolicyPart_DurationTariff, Retail_BE_ChargingPolicyPart_RateType
        /// </summary>
        public string PartTypeName { get; set; }
    }
}
