using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.Data.Entities
{
    public class ICXDataSettings : SettingData 
    {
        public const string SETTING_TYPE = "Retail_Data_ICXDataSettings";

        public decimal SessionOctetsLimit { get; set; }
    }
}
