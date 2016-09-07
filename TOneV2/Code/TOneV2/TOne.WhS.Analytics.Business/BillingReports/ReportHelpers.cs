using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using TOne.WhS.BusinessEntity.Business;

namespace TOne.WhS.Analytics.Business.BillingReports
{
    public static class ReportHelpers
    {
        private static int _longprecision = GenericParameterManager.Current.GetLongPrecision();
        private static int _normalprecision = GenericParameterManager.Current.GetNormalPrecision();
        public static string GetCarrierName(string id, string carrierAs)
        {
            string name = "multiple " + carrierAs;
            CarrierAccountManager _bemanager = new CarrierAccountManager();
            if (id != null)
            {
                if (id.Split(',').Length > 1)
                {
                    name = "Multiple " + carrierAs;
                }
                else if (id == "")
                {
                    name = "All " + carrierAs;
                }
                else
                {
                    int carrierId = Convert.ToInt32(id);
                    name = _bemanager.GetCarrierAccountName(carrierId);
                }   
            }
            else
            {
                name = "All " + carrierAs;
            }
            return name;
        }
        public static string GetNormalNumberDigit()
        {
            return _normalprecision.ToString();
        }
        public static string GetLongNumberDigit()
        {
            return _longprecision.ToString();
        }
        
        public static string FormatNormalNumberDigit(Decimal? number)
        {
            return String.Format("{0:#0." + "".PadLeft(_normalprecision, '0') + "}", number);
        }
        public static string FormatNormalNumberDigit(Double? number)
        {
            return String.Format("{0:#0." + "".PadLeft(_normalprecision, '0') + "}", number);
        }


        public static string FormatLongNumberDigit(Decimal? number)
        {
            return String.Format("{0:#0." + "".PadLeft(_longprecision, '0') + "}", number);
        }
        public static string FormatLongNumberDigit(Double? number)
        {
            return String.Format("{0:#0." + "".PadLeft(_longprecision, '0') + "}", number);
        }


        public static string FormatNumberDigitRate(Decimal? number)
        {
            int precision = 4;//Digit Rate 
            return String.Format("{0:#0." + "".PadLeft(precision, '0') + "}", number);
        }
        public static string FormatNumberDigitRate(Double? number)
        {
            int precision = 4;//Digit Rate 
            return String.Format("{0:#0." + "".PadLeft(precision, '0') + "}", number);
        }

        public static string FormatNumber(Decimal? number)
        {
            return String.Format("{0:#,###0.00}", number);
        }

        public static string FormatNumber(int? number)
        {
            return String.Format("{0:#,###0}", number);
        }

        public static string FormatNumberPercentage(Double? number)
        {
            return String.Format("{0:#,##0.00%}", (number.HasValue) ? Math.Truncate(number.Value *10000)/10000 : number);
        }
        public static string FormatNumberUnsignedPercentage(Double? number)
        {
            return String.Format("{0:#,##0.00}", number);
        }
        public static string FormatNumber(Double? number)
        {
            return String.Format("{0:#,###0.00}", number);
        }

    }
}
