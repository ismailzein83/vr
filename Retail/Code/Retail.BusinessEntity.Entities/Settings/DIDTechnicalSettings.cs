using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Entities
{
    public class DIDTechnicalSettings : SettingData
    {
        public const string SETTING_TYPE = "Retail_BE_DIDTechnicalSettings";

        public Guid AccountDIDRelationDefinitionId { get; set; }
    }
}
