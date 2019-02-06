using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Entities
{
    public class InterconnectSettings : SettingData
    {
        public const string SETTING_TYPE = "Retail_BE_InterconnectSettings";
        public string LocalOperatorName { get; set; }
        private long _localOperatorId = -999999;
        public long LocalOperatorID { get { return _localOperatorId; } set { _localOperatorId = value; } }
    }
}
