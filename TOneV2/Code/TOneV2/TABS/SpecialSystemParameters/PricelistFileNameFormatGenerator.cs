using System.Text;

namespace TABS.SpecialSystemParameters
{
    public class PricelistFileNameFormatGenerator
    {

        public static readonly string HelpString = @"
            {0} = Carrier Profile Name, {1} = Pricelist Effective Date, 
            Date can be formatted in standard .NET formats for date, {2} = Carrier Account Suffix";

        public static string GetPricelistNameFormat(PriceList pricelist)
        {
            TABS.CarrierAccount account = pricelist.Customer;

            string fileNameFormat = string.Empty;

            if (account.CustomerMask == "SYS")
                fileNameFormat = TABS.SystemParameter.PricelistNameFormat.TextValue;
            else
            {
                if( account.CustomerMaskAccount.PricelistMaskNameFormat!=null)
                     fileNameFormat = account.CustomerMaskAccount.PricelistMaskNameFormat;
                else
                    fileNameFormat = TABS.SystemParameter.PricelistNameFormat.TextValue;
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendFormat(fileNameFormat, account.CarrierProfile.Name, pricelist.BeginEffectiveDate, account.NameSuffix != null ? account.NameSuffix : string.Empty);

            return sb.ToString();
        }
    }
}
