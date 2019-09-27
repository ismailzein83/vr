using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.GenericData.Business;

namespace Retail.Ogero.Business
{
    public class InterconnectDataSourceManager
    {
        static Guid s_mtcPrefixType = new Guid("1056D52E-DA10-4A15-B3A7-F073792F7D2A");
        static Guid s_alfaPrefixType = new Guid("C27E1884-0E6E-4B66-9C6C-B7CBE32F32CC");
        static Guid s_ogeroPrefixType = new Guid("FF0F735C-3244-4D83-AFA4-8DDACD4DFE8D");

        static List<Guid> s_mobileOperatorNumberPrefixes = new List<Guid>() { s_mtcPrefixType, s_alfaPrefixType };

        static List<string> s_missingZeroNumberPrefixes = new List<string>() { "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "10" };

        static VRNumberPrefixManager s_NumberPrefixManager = new VRNumberPrefixManager();

        public static bool IncludeCDR(Dictionary<TrunkType, string> trunkValues, ref string cgpn, ref string cdpn, ref string inTrunk, ref string outTrunk)
        {
            RemovePrefix(ref cgpn, "961", 10);
            RemovePrefix(ref cdpn, "961", 10);

            Guid? cgpnPrefixType = null;
            Guid? cdpnPrefixType = null;

            if (IsANumberValid(cgpn))
            {
                inTrunk = GetTrunkValue(cgpn, inTrunk, trunkValues, false, out cgpnPrefixType);
                AddPrefix(ref cgpn, "0", cgpnPrefixType);
            }

            bool isBNumberShortNumber;
            if (IsBNumberValid(cdpn, out isBNumberShortNumber))
            {
                outTrunk = GetTrunkValue(cdpn, outTrunk, trunkValues, isBNumberShortNumber, out cdpnPrefixType);
                AddPrefix(ref cdpn, "0", cdpnPrefixType);
            }

            if (cgpnPrefixType.HasValue && cdpnPrefixType.HasValue)
            {
                if ((s_mobileOperatorNumberPrefixes.Contains(cgpnPrefixType.Value) && s_mobileOperatorNumberPrefixes.Contains(cdpnPrefixType.Value))) //Mobile => Mobile
                    return false;

                if (s_ogeroPrefixType == cgpnPrefixType.Value && s_ogeroPrefixType == cdpnPrefixType.Value) //Ogero => Ogero
                    return false;
            }

            if (cdpnPrefixType.HasValue && cdpn.Length > 8 && cdpn.StartsWith("0") && !cdpn.StartsWith("03") && !cdpn.StartsWith("010") && s_mobileOperatorNumberPrefixes.Contains(cdpnPrefixType.Value))
                cdpn = cdpn.Substring(1);

            return true;
        }

        private static bool IsANumberValid(string aNumber)
        {
            if (string.IsNullOrEmpty(aNumber))
                return false;

            if (aNumber.Length > 8 || aNumber.Length < 7)
                return false;

            if (aNumber.Length == 7)
            {
                if (aNumber.StartsWith("0"))
                    return false;

                return true;
            }

            //ANumber Length is 8
            if (aNumber.StartsWith("0"))
                return true;

            if (aNumber.StartsWith("3"))
                return false;

            Guid? aNumberPrefixType = s_NumberPrefixManager.GetNumberPrefixTypeId(aNumber, false);
            if (!aNumberPrefixType.HasValue || !s_mobileOperatorNumberPrefixes.Contains(aNumberPrefixType.Value))
                return false;

            return true;
        }

        private static bool IsBNumberValid(string bNumber, out bool isBNumberShortNumber)
        {
            isBNumberShortNumber = false;

            if (string.IsNullOrEmpty(bNumber))
                return false;

            if (bNumber.Length >= 7)
                return true;

            if (bNumber.Length == 6)
                return false;

            if (bNumber.Length >= 3 && bNumber.Length <= 5)
            {
                isBNumberShortNumber = true;
                return true;
            }

            return false;
        }

        private static string GetTrunkValue(string number, string oldTrunkValue, Dictionary<TrunkType, string> trunkValues, bool isExactMatch, out Guid? numberPrefixType)
        {
            if (!string.IsNullOrEmpty(oldTrunkValue))
            {
                oldTrunkValue = oldTrunkValue.Trim();
                string result = oldTrunkValue.Substring(0, 2);
                if (string.Compare(result, trunkValues[TrunkType.Alfa], true) == 0)
                {
                    numberPrefixType = s_alfaPrefixType;
                }
                else if (string.Compare(result, trunkValues[TrunkType.MTC], true) == 0)
                {
                    numberPrefixType = s_mtcPrefixType;
                }
                else
                {
                    Guid? tempNumberPrefixType = s_NumberPrefixManager.GetNumberPrefixTypeId(number, isExactMatch);
                    if (tempNumberPrefixType.HasValue && tempNumberPrefixType.Value == s_ogeroPrefixType)
                        numberPrefixType = s_ogeroPrefixType;
                    else
                        numberPrefixType = null;
                }

                return oldTrunkValue;
            }

            numberPrefixType = s_NumberPrefixManager.GetNumberPrefixTypeId(number, isExactMatch);
            if (numberPrefixType.HasValue)
            {
                if (numberPrefixType.Value == s_mtcPrefixType)
                    return trunkValues[TrunkType.MTC];

                if (numberPrefixType.Value == s_alfaPrefixType)
                    return trunkValues[TrunkType.Alfa];

                if (numberPrefixType.Value == s_ogeroPrefixType)
                    return trunkValues[TrunkType.Ogero];
            }

            return "Unknown";
        }

        private static void RemovePrefix(ref string number, string prefix, int? minLength)
        {
            if (string.IsNullOrEmpty(number))
                return;

            if (minLength.HasValue && number.Length < minLength.Value)
                return;

            if (number.StartsWith(prefix))
                number = number.Substring(prefix.Length);
        }

        private static void AddPrefix(ref string number, string prefix, Guid? numberPrefix)
        {
            if (string.IsNullOrEmpty(number) || !numberPrefix.HasValue)
                return;

            if (number.Length > 6 && !number.StartsWith(prefix) &&
                (!s_mobileOperatorNumberPrefixes.Contains(numberPrefix.Value) || s_missingZeroNumberPrefixes.Contains(number.Substring(0, 2))))
                number = string.Concat(prefix, number);
        }
    }

    public enum TrunkType
    {
        MTC = 0,
        Alfa = 1,
        Ogero = 2
    }
}