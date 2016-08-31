using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class BankDetailsDataSourceSettings : RDLCReportDataSourceSettings
    {
        public override IEnumerable<dynamic> GetDataSourceItems(IRDLCReportDataSourceSettingsContext context)
        {
            Vanrise.Common.Business.ConfigManager configManager = new Vanrise.Common.Business.ConfigManager();
            return configManager.GetBankDetails();
        }
    }
}
