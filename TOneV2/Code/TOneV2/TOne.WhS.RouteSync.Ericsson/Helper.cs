using System;
using System.Linq;
using System.Collections.Generic;
using TOne.WhS.RouteSync.Ericsson.Entities;

namespace TOne.WhS.RouteSync.Ericsson
{
    public static class Helper
    {
        public const char OptionsSeparator = '@';
        public const string OptionsSeparatorAsString = "@";

        public const char OptionFieldsSeparator = '$';
        public const string OptionFieldsSeparatorAsString = "$";
        public static string SerializeRouteCaseOptions(List<RouteCaseOption> routeCaseOptions)
        {
            if (routeCaseOptions == null)
                return null;

            List<string> serializedOptions = new List<string>();
            foreach (RouteCaseOption routeCaseOption in routeCaseOptions)
            {
                string serializedRouteCaseOption = SerializeRouteCaseOption(routeCaseOption);
                if (!string.IsNullOrEmpty(serializedRouteCaseOption))
                    serializedOptions.Add(serializedRouteCaseOption);
            }
            return String.Join<string>(OptionsSeparatorAsString, serializedOptions);
        }

        public static string SerializeRouteCaseOption(RouteCaseOption routeCaseOption)
        {
            if (routeCaseOption == null)
                return null;

            string percentage = routeCaseOption.Percentage.HasValue ? routeCaseOption.Percentage.Value.ToString() : string.Empty;
            string trunkPercentage = routeCaseOption.TrunkPercentage.HasValue ? routeCaseOption.TrunkPercentage.Value.ToString() : string.Empty;

            return string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}", percentage, routeCaseOption.Priority, routeCaseOption.OutTrunk, (int)routeCaseOption.Type,
                routeCaseOption.BNT, routeCaseOption.SP, trunkPercentage, routeCaseOption.IsBackup ? 1 : 0, routeCaseOption.GroupID);
        }

        public static List<RouteCaseOption> DeserializeRouteCaseOptions(string serializedRouteCaseOptions)
        {
            if (string.IsNullOrEmpty(serializedRouteCaseOptions))
                return null;

            string[] routeCaseOptionsAsString = serializedRouteCaseOptions.Split(OptionsSeparator);
            if (routeCaseOptionsAsString == null)
                return null;

            List<RouteCaseOption> routeCaseOptions = new List<RouteCaseOption>();
            foreach (string serializedRouteCase in routeCaseOptionsAsString)
            {
                RouteCaseOption routeCaseOption = DeserializeRouteCaseOption(serializedRouteCase);
                if (routeCaseOption != null)
                    routeCaseOptions.Add(routeCaseOption);
            }

            return routeCaseOptions;
        }

        public static RouteCaseOption DeserializeRouteCaseOption(string serializedRouteCaseOption)
        {
            if (string.IsNullOrEmpty(serializedRouteCaseOption))
                return null;

            string[] routeCaseOptionAsString = serializedRouteCaseOption.Split(OptionFieldsSeparator);
            if (routeCaseOptionAsString == null || routeCaseOptionAsString.Count() != 9)
                return null;

            RouteCaseOption routeCaseOption = new RouteCaseOption();

            string percentage = routeCaseOptionAsString[0];
            if (!string.IsNullOrEmpty(percentage))
                routeCaseOption.Percentage = int.Parse(percentage);

            string priority = routeCaseOptionAsString[1];
            if (!string.IsNullOrEmpty(priority))
                routeCaseOption.Priority = int.Parse(priority);

            routeCaseOption.OutTrunk = routeCaseOptionAsString[2];

            string type = routeCaseOptionAsString[3];
            if (!string.IsNullOrEmpty(type))
                routeCaseOption.Type = (TrunkType)int.Parse(type);

            string bnt = routeCaseOptionAsString[4];
            if (!string.IsNullOrEmpty(bnt))
                routeCaseOption.BNT = int.Parse(bnt);

            string sp = routeCaseOptionAsString[5];
            if (!string.IsNullOrEmpty(sp))
                routeCaseOption.SP = short.Parse(sp);

            string trunkPercentage = routeCaseOptionAsString[6];
            if (!string.IsNullOrEmpty(trunkPercentage))
                routeCaseOption.TrunkPercentage = int.Parse(trunkPercentage);

            string isBackup = routeCaseOptionAsString[7];
            if (!string.IsNullOrEmpty(isBackup))
                routeCaseOption.IsBackup = int.Parse(isBackup) > 0;

            string groupID = routeCaseOptionAsString[8];
            if (!string.IsNullOrEmpty(groupID))
                routeCaseOption.GroupID = int.Parse(groupID);

            return routeCaseOption;
        }

		public static TrunkGroup GetMatchedTrunkGroup(IEnumerable<TrunkGroup> trunkGroups, string code)
		{
			if (trunkGroups == null || !trunkGroups.Any() || string.IsNullOrEmpty(code))
				return null;
			int maxMatchLength = 0;
			TrunkGroup matchedTrunkGroup = null;

			foreach (var trunkGroup in trunkGroups)
			{
				if (trunkGroup.CodeGroupTrunkGroups == null || trunkGroup.CodeGroupTrunkGroups.Count == 0)
					throw new Exception();
				foreach (var codeGroup in trunkGroup.CodeGroupTrunkGroups)
				{
					if (codeGroup.CodeGroup.Length <= maxMatchLength)
						continue;
					var matchLength = GetCodeGroupAndCodeMatchLength(codeGroup.CodeGroup, code);
					if (matchLength > maxMatchLength)
					{
						maxMatchLength = matchLength;
						matchedTrunkGroup = trunkGroup;
					}
				}
			}
			return matchedTrunkGroup;
		}

		public static int GetCodeGroupAndCodeMatchLength(string codeGroup, string code)
		{
			int matchLength = 0;
			for (int i = 0; i < codeGroup.Length && i < code.Length; i++)
			{
				if (codeGroup.ElementAt(i) == code.ElementAt(i))
					matchLength++;
				else break;
			}
			return matchLength;
		}
	}
}
