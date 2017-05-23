using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Invoice.Entities;

namespace Retail.BusinessEntity.MainExtensions.PackageTypes
{
    public class RecurChargePackageSettings : PackageExtendedSettings
    {
        public RecurringChargeEvaluatorSettings Evaluator { get; set; }
    }
}
