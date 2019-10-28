using System;

namespace Vanrise.Common
{
    public abstract class NumberToText
    {
        #region constructor
        public string ConvertNumberToText(decimal number, NumberToTextCurrency currency)
        {
            int integerValue;
            int decimalValue;

            NumberToTextCurrencyInfo currencyInfo = new NumberToTextCurrencyInfo(currency);
            ExtractIntegerAndDecimalParts(number, currencyInfo, out integerValue, out decimalValue);

            return ConvertNumberToText_Internal(number, integerValue, decimalValue, currencyInfo);
        }

        #endregion

        protected abstract string ConvertNumberToText_Internal(decimal number, int integerValue, int decimalValue, NumberToTextCurrencyInfo currencyInfo);

        #region private methods
        private void ExtractIntegerAndDecimalParts(decimal number, NumberToTextCurrencyInfo currencyInfo, out int integerValue, out int decimalValue)
        {
            decimalValue = 0;
            String[] splits = number.ToString().Split('.');

            integerValue = Convert.ToInt32(splits[0]);

            if (splits.Length > 1)
                decimalValue = Convert.ToInt32(GetDecimalValue(splits[1], currencyInfo));
        }

        private string GetDecimalValue(string decimalPart, NumberToTextCurrencyInfo currencyInfo)
        {
            string result = decimalPart;


            if (currencyInfo.PartPrecision > decimalPart.Length)
            {
                for (int i = 0; i < currencyInfo.PartPrecision - decimalPart.Length; i++)
                    result += "0";
            }
            else
                result = decimalPart.Substring(0, currencyInfo.PartPrecision);

            return result;
        }
        #endregion
    }
}