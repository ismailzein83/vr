using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace CP.Ringo.Entities
{
    public class RingoRetailSettings : SettingData
    {
        public const string SETTING_TYPE = "Retail_Ringo_TechinicalSettings";
        public Guid RetailVRConnectionId { get; set; }

    }
}
