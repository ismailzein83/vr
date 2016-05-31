using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Analytics.Data;
using TOne.WhS.Analytics.Entities.BillingReport;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.Analytics.Business
{
    public partial class BillingStatisticManager
    {
        public BillingStatisticManager()
        {
        }

        public string FormatNumberDigitRate(Decimal? number)
        {
            int precision = 4;//Digit Rate 
            return String.Format("{0:#0." + "".PadLeft(precision, '0') + "}", number);
        }
        public string FormatNumberDigitRate(Double? number)
        {
            int precision = 4;//Digit Rate 
            return String.Format("{0:#0." + "".PadLeft(precision, '0') + "}", number);
        }

        public string FormatNumber(Decimal? number)
        {
            return String.Format("{0:#,###0.00}", number);
        }

        public string FormatNumber(int? number)
        {
            return String.Format("{0:#,###0}", number);
        }

        public string FormatNumberPercentage(Double? number)
        {
            return String.Format("{0:#,##0.00%}", number);
        }
        public string FormatNumber(Double? number)
        {
            return String.Format("{0:#,###0.00}", number);
        }
    }
}
