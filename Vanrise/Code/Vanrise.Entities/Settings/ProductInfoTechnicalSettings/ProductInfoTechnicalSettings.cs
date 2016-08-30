using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class ProductInfoTechnicalSettings : SettingData
    {
        public const string SETTING_TYPE = "VR_Common_ProductInfoTechnicalSettings";

        public ProductInfo ProductInfo { get; set; } 
    }
}
 