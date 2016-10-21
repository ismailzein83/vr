using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Entities.RecurringPeriod
{
    public class RecurringPeriodConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "Retail_BE_RecurringPeriod";
        public string Editor { get; set; }
    }
}
