using System;

namespace Vanrise.Common.MainExtensions
{
    public class NumberToArabicText : NumberToText
    {
        #region Variables

        private static string[] arabicOnes =
           new string[] {
            String.Empty, "واحد", "اثنان", "ثلاثة", "أربعة", "خمسة", "ستة", "سبعة", "ثمانية", "تسعة",
            "عشرة", "أحد عشر", "اثنا عشر", "ثلاثة عشر", "أربعة عشر", "خمسة عشر", "ستة عشر", "سبعة عشر", "ثمانية عشر", "تسعة عشر"
        };

        private static string[] arabicFeminineOnes =
           new string[] {
            String.Empty, "إحدى", "اثنتان", "ثلاث", "أربع", "خمس", "ست", "سبع", "ثمان", "تسع",
            "عشر", "إحدى عشرة", "اثنتا عشرة", "ثلاث عشرة", "أربع عشرة", "خمس عشرة", "ست عشرة", "سبع عشرة", "ثماني عشرة", "تسع عشرة"
        };

        private static string[] arabicTens =
            new string[] {
            "عشرون", "ثلاثون", "أربعون", "خمسون", "ستون", "سبعون", "ثمانون", "تسعون"
        };

        private static string[] arabicHundreds =
            new string[] {
            "", "مائة", "مئتان", "ثلاثمائة", "أربعمائة", "خمسمائة", "ستمائة", "سبعمائة", "ثمانمائة","تسعمائة"
        };

        private static string[] arabicAppendedTwos =
            new string[] {
            "مئتا", "ألفا", "مليونا", "مليارا", "تريليونا", "كوادريليونا", "كوينتليونا", "سكستيليونا"
        };

        private static string[] arabicTwos =
            new string[] {
            "مئتان", "ألفان", "مليونان", "ملياران", "تريليونان", "كوادريليونان", "كوينتليونان", "سكستيليونان"
        };

        private static string[] arabicGroup =
            new string[] {
            "مائة", "ألف", "مليون", "مليار", "تريليون", "كوادريليون", "كوينتليون", "سكستيليون"
        };

        private static string[] arabicAppendedGroup =
            new string[] {
            "", "ألفاً", "مليوناً", "ملياراً", "تريليوناً", "كوادريليوناً", "كوينتليوناً", "سكستيليوناً"
        };

        private static string[] arabicPluralGroups =
            new string[] {
            "", "آلاف", "ملايين", "مليارات", "تريليونات", "كوادريليونات", "كوينتليونات", "سكستيليونات"
        };

        private string ArabicPrefixText = "فقط";

        private string ArabicSuffixText = "لا غير";

        #endregion

        protected override string ConvertNumberToText_Internal(decimal number, int integerValue, int decimalValue, NumberToTextCurrencyInfo currencyInfo)
        {
            return ConvertToArabic(number, integerValue, decimalValue, currencyInfo);
        }

        private string ConvertToArabic(decimal number, int integerValue, int decimalValue, NumberToTextCurrencyInfo currencyInfo)
        {
            Decimal tempNumber = number;

            if (tempNumber == 0)
                return "صفر";

            // Get Text for the decimal part
            string decimalString = ProcessArabicGroup(decimalValue, -1, 0, integerValue, currencyInfo);

            string retVal = String.Empty;
            Byte group = 0;
            while (tempNumber >= 1)
            {
                // seperate number into groups
                int numberToProcess = (int)(tempNumber % 1000);

                tempNumber = tempNumber / 1000;

                // convert group into its text
                string groupDescription = ProcessArabicGroup(numberToProcess, group, Math.Floor(tempNumber), integerValue, currencyInfo);

                if (groupDescription != String.Empty)
                { // here we add the new converted group to the previous concatenated text
                    if (group > 0)
                    {
                        if (retVal != String.Empty)
                            retVal = String.Format("{0} {1}", "و", retVal);

                        if (numberToProcess != 2)
                        {
                            if (numberToProcess % 100 != 1)
                            {
                                if (numberToProcess >= 3 && numberToProcess <= 10) // for numbers between 3 and 9 we use plural name
                                    retVal = String.Format("{0} {1}", arabicPluralGroups[group], retVal);
                                else
                                {
                                    if (retVal != String.Empty) // use appending case
                                        retVal = String.Format("{0} {1}", arabicAppendedGroup[group], retVal);
                                    else
                                        retVal = String.Format("{0} {1}", arabicGroup[group], retVal); // use normal case
                                }
                            }
                            else
                            {
                                retVal = String.Format("{0} {1}", arabicGroup[group], retVal); // use normal case
                            }
                        }
                    }

                    retVal = String.Format("{0} {1}", groupDescription, retVal);
                }

                group++;
            }

            String formattedNumber = String.Empty;
            formattedNumber += (ArabicPrefixText != String.Empty) ? String.Format("{0} ", ArabicPrefixText) : String.Empty;
            formattedNumber += (retVal != String.Empty) ? retVal : String.Empty;
            if (integerValue != 0)
            { // here we add currency name depending on _integerValue : 1 ,2 , 3--->10 , 11--->99
                int remaining100 = (int)(integerValue % 100);

                if (remaining100 == 0)
                    formattedNumber += currencyInfo.Arabic1CurrencyName;
                else
                    if (remaining100 == 1)
                    formattedNumber += currencyInfo.Arabic1CurrencyName;
                else
                        if (remaining100 == 2)
                {
                    if (integerValue == 2)
                        formattedNumber += currencyInfo.Arabic2CurrencyName;
                    else
                        formattedNumber += currencyInfo.Arabic1CurrencyName;
                }
                else
                            if (remaining100 >= 3 && remaining100 <= 10)
                    formattedNumber += currencyInfo.Arabic310CurrencyName;
                else
                                if (remaining100 >= 11 && remaining100 <= 99)
                    formattedNumber += currencyInfo.Arabic1199CurrencyName;

                formattedNumber += (decimalValue != 0) ? " و " : String.Empty;
            }
            formattedNumber += (decimalValue != 0) ? decimalString : String.Empty;
            if (decimalValue != 0)
            { // here we add currency part name depending on _integerValue : 1 ,2 , 3--->10 , 11--->99
                formattedNumber += " ";

                int remaining100 = (int)(decimalValue % 100);

                if (remaining100 == 0)
                    formattedNumber += currencyInfo.Arabic1CurrencyPartName;
                else
                    if (remaining100 == 1)
                    formattedNumber += currencyInfo.Arabic1CurrencyPartName;
                else
                        if (remaining100 == 2)
                    formattedNumber += currencyInfo.Arabic2CurrencyPartName;
                else
                            if (remaining100 >= 3 && remaining100 <= 10)
                    formattedNumber += currencyInfo.Arabic310CurrencyPartName;
                else
                                if (remaining100 >= 11 && remaining100 <= 99)
                    formattedNumber += currencyInfo.Arabic1199CurrencyPartName;
            }
            formattedNumber += (ArabicSuffixText != String.Empty) ? String.Format(" {0}", ArabicSuffixText) : String.Empty;

            return formattedNumber;
        }

        private string GetDigitFeminineStatus(int digit, int groupLevel, NumberToTextCurrencyInfo currencyInfo)
        {
            if (groupLevel == -1)
            { // if it is in the decimal part
                if (currencyInfo.IsCurrencyPartNameFeminine)
                    return arabicFeminineOnes[digit]; // use feminine field
                else
                    return arabicOnes[digit];
            }
            else
                if (groupLevel == 0)
            {
                if (currencyInfo.IsCurrencyNameFeminine)
                    return arabicFeminineOnes[digit];// use feminine field
                else
                    return arabicOnes[digit];
            }
            else
                return arabicOnes[digit];
        }

        private string ProcessArabicGroup(int groupNumber, int groupLevel, Decimal remainingNumber, int integerValue, NumberToTextCurrencyInfo currencyInfo)
        {
            int tens = groupNumber % 100;

            int hundreds = groupNumber / 100;

            string retVal = String.Empty;

            if (hundreds > 0)
            {
                if (tens == 0 && hundreds == 2) // حالة المضاف
                    retVal = String.Format("{0}", arabicAppendedTwos[0]);
                else //  الحالة العادية
                    retVal = String.Format("{0}", arabicHundreds[hundreds]);
            }

            if (tens > 0)
            {
                if (tens < 20)
                { // if we are processing under 20 numbers
                    if (tens == 2 && hundreds == 0 && groupLevel > 0)
                    { // This is special case for number 2 when it comes alone in the group
                        if (integerValue == 2000 || integerValue == 2000000 || integerValue == 2000000000 || integerValue == 2000000000000 || integerValue == 2000000000000000 || integerValue == 2000000000000000000)
                            retVal = String.Format("{0}", arabicAppendedTwos[groupLevel]); // في حالة الاضافة
                        else
                            retVal = String.Format("{0}", arabicTwos[groupLevel]);//  في حالة الافراد
                    }
                    else
                    { // General case
                        if (retVal != String.Empty)
                            retVal += " و ";

                        if (tens == 1 && groupLevel > 0 && hundreds == 0)
                            retVal += " ";
                        else
                            if ((tens == 1 || tens == 2) && (groupLevel == 0 || groupLevel == -1) && hundreds == 0 && remainingNumber == 0)
                            retVal += String.Empty; // Special case for 1 and 2 numbers like: ليرة سورية و ليرتان سوريتان
                        else
                            retVal += GetDigitFeminineStatus(tens, groupLevel, currencyInfo);// Get Feminine status for this digit
                    }
                }
                else
                {
                    int ones = tens % 10;
                    tens = (tens / 10) - 2; // 20's offset

                    if (ones > 0)
                    {
                        if (retVal != String.Empty)
                            retVal += " و ";

                        // Get Feminine status for this digit
                        retVal += GetDigitFeminineStatus(ones, groupLevel, currencyInfo);
                    }

                    if (retVal != String.Empty)
                        retVal += " و ";

                    // Get Tens text
                    retVal += arabicTens[tens];
                }
            }

            return retVal;
        }
    }
}