using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Entities;
using Vanrise.Common;

namespace TOne.WhS.DBSync.Business
{
    public class RulesMigrationHelper
    {
        static Dictionary<string, CodeGroup> _singleCountryCodeGroups;
        public static void BuildSingleCodeGroupContriesCodeGroups(Dictionary<string, CodeGroup> allCodeGroups)
        {
            if (_singleCountryCodeGroups != null)
                return;
            Dictionary<int, List<CodeGroup>> codeGroupsByCountryId = new Dictionary<int, List<CodeGroup>>();

            foreach (string codeGroupKey in allCodeGroups.Keys)
            {
                CodeGroup codeGroup = allCodeGroups[codeGroupKey];
                List<CodeGroup> codeGroupsList = codeGroupsByCountryId.GetOrCreateItem(codeGroup.CountryId);
                codeGroupsList.Add(codeGroup);
            }
            _singleCountryCodeGroups = new Dictionary<string, CodeGroup>();

            foreach (var codeGroupsList in codeGroupsByCountryId.Values)
            {
                if (codeGroupsList.Count == 1)
                {
                    CodeGroup codeGroup = codeGroupsList[0];
                    _singleCountryCodeGroups.GetOrCreateItem(codeGroup.Code, () => { return codeGroup; });
                }
            }
        }

        public static bool IsRuleByCountry(SourceBaseRule sourceRule, out int? countryId)
        {
            countryId = null;

            if (!sourceRule.IncludeSubCode)
                return false;

            CodeGroup codeGroup;
            if (!_singleCountryCodeGroups.TryGetValue(sourceRule.Code, out codeGroup))
                return false;

            countryId = codeGroup.CountryId;
            return true;
        }

        public static void ClearSingleCodeGroupContriesCodeGroups()
        {
            _singleCountryCodeGroups = null;
        }
    }
}
