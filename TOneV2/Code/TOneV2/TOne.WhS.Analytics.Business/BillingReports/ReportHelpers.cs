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
        static int GetLongPrecision()
        {
            return GenericParameterManager.Current.GetLongPrecision();
        }

        static int GetNormalPrecision()
        {
            return GenericParameterManager.Current.GetNormalPrecision();
        }
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
            return GetNormalPrecision().ToString();
        }

        public static int GetNormalNumberPrecision()
        {
            return GetNormalPrecision();
        }
        public static string GetLongNumberDigit()
        {
            return GetLongPrecision().ToString();
        }

        public static int GetLongNumberPrecision()
        {
            return GetLongPrecision();
        }
        
        public static string FormatNormalNumberDigit(Decimal? number)
        {
            int normalPrecision = GetNormalPrecision();
            return String.Format("{0:#0." + "".PadLeft(normalPrecision, '0') + "}", Math.Round(number.HasValue ? number.Value : 0, normalPrecision, MidpointRounding.AwayFromZero));
        }
        public static string FormatNormalNumberDigit(Double? number)
        {
            int normalPrecision = GetNormalPrecision();
            return String.Format("{0:#0." + "".PadLeft(normalPrecision, '0') + "}", Math.Round(number.HasValue ? number.Value : 0, normalPrecision, MidpointRounding.AwayFromZero));
        }


        public static string FormatLongNumberDigit(Decimal? number)
        {
            int longPrecision = GetLongPrecision();
            return String.Format("{0:#0." + "".PadLeft(longPrecision, '0') + "}", Math.Round(number.HasValue ? number.Value : 0, longPrecision, MidpointRounding.AwayFromZero));
        }
        public static string FormatLongNumberDigit(Double? number)
        {
            int longPrecision = GetLongPrecision();
            return String.Format("{0:#0." + "".PadLeft(longPrecision, '0') + "}", Math.Round(number.HasValue ? number.Value : 0, longPrecision, MidpointRounding.AwayFromZero));
        }


        public static string FormatNumberDigitRate(Decimal? number)
        {
            return String.Format("{0:#0." + "".PadLeft(GenericParameterManager.Current.GetNormalPrecision(), '0') + "}", (number.HasValue) ? Math.Truncate(number.Value * 10000) / 10000 : number);
        }
        public static string FormatNumberDigitRate(Double? number)
        {
            int normalPrecision = GetNormalPrecision();
            return String.Format("{0:#0." + "".PadLeft(normalPrecision, '0') + "}", Math.Round(number.HasValue ? number.Value : 0, normalPrecision, MidpointRounding.AwayFromZero));
        }

        public static string FormatNumber(Decimal? number)
        {
            return String.Format("{0:#,###0.00}", number);
        }

        public static string FormatNumber(int? number)
        {
            return String.Format("{0:#,###0}", number);
        }
        public static string FormatNumberPercentage(int number)
        {
            return String.Format("{0:#,##0.00%}",  number);
        }
        public static string FormatNumberPercentage(Double? number)
        {
            return String.Format("{0:#,##0.00%}", (number.HasValue) ? Math.Truncate(number.Value *10000)/10000 : number);
        }

        public static string FormatNumberPercentage(Decimal? number)
        {
            return String.Format("{0:#,##0.00%}", (number.HasValue) ? Math.Truncate(number.Value * 10000) / 10000 : number);
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
