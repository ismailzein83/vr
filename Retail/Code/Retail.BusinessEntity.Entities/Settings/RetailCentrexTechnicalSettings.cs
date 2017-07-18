using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Entities
{
    public class RetailCentrexTechnicalSettings : SettingData
    { 
        public RetailCentrexImportCDRSettings RetailCentrexImportCDRSettings { get; set; } 
    }

    public class RetailCentrexImportCDRSettings   
    {
        public int? SaleAmountPrecision { get; set; }
    }
}
