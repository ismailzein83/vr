using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.GenericData.Business;

namespace Retail.Ogero.Business
{
    public class InterconnectDataSourceManager
    {
        static Guid s_mtcPrefixType = new Guid("");
        static Guid s_alfaPrefixType = new Guid("");
        static Guid s_ogeroPrefixType = new Guid("");

        static List<Guid> s_mobileOperatorNumberPrefixes = new List<Guid>() { s_mtcPrefixType, s_alfaPrefixType };

        static List<string> s_missingZeroNumberPrefixes = new List<string>() { "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "10" };

        static VRNumberPrefixManager s_NumberPrefixManager = new VRNumberPrefixManager();

        public static bool IncludeCDR(IDataReader reader, Dictionary<DSFieldName, string> dsFieldNames, bool getTrunksFromReader, Dictionary<TrunkType, string> trunkValues,
            out string cgpn, out string cdpn, out string inTrunk, out string outTrunk)
        {
            string oldInTrunk = null;
            string oldOutTrunk = null;

            if (getTrunksFromReader)
            {
                oldInTrunk = reader[dsFieldNames[DSFieldName.InTrunk]] as string;
                oldOutTrunk = reader[dsFieldNames[DSFieldName.OutTrunk]] as string;
            }

            cgpn = reader[dsFieldNames[DSFieldName.ANumber]] as string;
            cdpn = reader[dsFieldNames[DSFieldName.BNumber]] as string;

            RemovePrefix(ref cgpn, "961");
            RemovePrefix(ref cdpn, "961");

            Guid? cgpnPrefixType;
            Guid? cdpnPrefixType;

            inTrunk = GetTrunkValue(cgpn, oldInTrunk, trunkValues, out cgpnPrefixType);
            outTrunk = GetTrunkValue(cdpn, oldOutTrunk, trunkValues, out cdpnPrefixType);

            AddPrefix(ref cgpn, "0", cgpnPrefixType);
            AddPrefix(ref cdpn, "0", cdpnPrefixType);

            if (cgpnPrefixType.HasValue && cdpnPrefixType.HasValue)
            {
                if ((s_mobileOperatorNumberPrefixes.Contains(cgpnPrefixType.Value) && s_mobileOperatorNumberPrefixes.Contains(cdpnPrefixType.Value))) //Mobile => Mobile
                    return false;

                if (s_ogeroPrefixType == cgpnPrefixType.Value && s_ogeroPrefixType == cdpnPrefixType.Value) //Ogero => Ogero
                    return false;
            }

            if (cdpnPrefixType.HasValue && cdpn.Length > 8 && cdpn.StartsWith("0") && !cdpn.StartsWith("03") && s_mobileOperatorNumberPrefixes.Contains(cdpnPrefixType.Value))
                cdpn = cdpn.Substring(1);

            return true;
        }

        private static string GetTrunkValue(string number, string oldTrunkValue, Dictionary<TrunkType, string> trunkValues, out Guid? numberPrefixType)
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
                    Guid? tempNumberPrefixType = s_NumberPrefixManager.GetNumberPrefixTypeId(number);
                    if (tempNumberPrefixType.HasValue && tempNumberPrefixType.Value == s_ogeroPrefixType)
                        numberPrefixType = s_ogeroPrefixType;
                    else
                        numberPrefixType = null;
                }

                return oldTrunkValue;
            }

            numberPrefixType = s_NumberPrefixManager.GetNumberPrefixTypeId(number);
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

        private static void RemovePrefix(ref string number, string prefix)
        {
            if (number.StartsWith(prefix))
                number = number.Substring(prefix.Length);
        }

        private static void AddPrefix(ref string number, string prefix, Guid? numberPrefix)
        {
            if (!numberPrefix.HasValue)
                return;

            if (number.Length > 6 && !number.StartsWith(prefix) &&
                (!s_mobileOperatorNumberPrefixes.Contains(numberPrefix.Value) || s_missingZeroNumberPrefixes.Contains(number.Substring(0, 2))))
                number = string.Concat(prefix, number);
        }
    }

    public enum DSFieldName
    {
        ANumber = 0,
        BNumber = 1,
        InTrunk = 2,
        OutTrunk = 3
    }

    public enum TrunkType
    {
        MTC = 0,
        Alfa = 1,
        Ogero = 2
    }
}