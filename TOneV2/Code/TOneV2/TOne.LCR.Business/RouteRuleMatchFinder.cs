using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.LCR.Entities;

namespace TOne.LCR.Business
{
    public class RouteRuleMatchFinder
    {
        #region Private Classes

        private enum FinderStep { MatchCodesList, MatchCodeAndSubCodesList, MatchZonesList, MatchingAllZonesList, End }

        private class MatchList
        {
            public List<BaseRouteRule> Rules { get; set; }

            public int NextReturnedIndex { get; set; }
        }

        #endregion

        FinderStep _currentStep;

        Dictionary<FinderStep, MatchList> _matchLists;

        string _code;
        int _zoneId;
        RouteRuleMatches _routeRulesMatches;

        public RouteRuleMatchFinder(string code, int zoneId, RouteRuleMatches routeRulesMatches)
        {
            _code = code;
            _zoneId = zoneId;
            _routeRulesMatches = routeRulesMatches;
            _matchLists = new Dictionary<FinderStep, MatchList>();
        }

        public bool GetNext(out BaseRouteRule rule)
        {
            if (_currentStep == FinderStep.End)
            {
                rule = null;
                return false;
            }

            MatchList currentList;
            if (!_matchLists.TryGetValue(_currentStep, out currentList))
            {
                currentList = GetMatchList(_currentStep);
                _matchLists.Add(_currentStep, currentList);
            }

            if (currentList.Rules != null)
            {
                while (currentList.NextReturnedIndex < currentList.Rules.Count)
                {
                    rule = currentList.Rules[currentList.NextReturnedIndex];
                    currentList.NextReturnedIndex++;
                    if (!rule.CodeSet.IsCodeExcluded(_code) && !rule.CodeSet.IsZoneExcluded(_zoneId))
                    {                        
                        return true;
                    }
                }
            }

            _currentStep++;
            return GetNext(out rule);
        }

        private MatchList GetMatchList(FinderStep step)
        {
            List<BaseRouteRule> matchRules = null;
            switch (step)
            {
                case FinderStep.MatchCodesList:
                    _routeRulesMatches.RulesByMatchCodes.TryGetValue(_code, out matchRules);
                    break;
                case FinderStep.MatchCodeAndSubCodesList:
                    matchRules = new List<BaseRouteRule>();
                    string parentCode = _code;
                    do
                    {
                        List<BaseRouteRule> parentCodeRules;
                        if (_routeRulesMatches.RulesByMatchCodeAndSubCodes.TryGetValue(parentCode, out parentCodeRules))
                        {
                            if (parentCodeRules != null)
                                foreach (var r in parentCodeRules)
                                {
                                    if (!matchRules.Contains(r))
                                        matchRules.Add(r);
                                }
                        }
                        parentCode = parentCode.Substring(0, parentCode.Length - 1);
                    }
                    while (parentCode.Length >= _routeRulesMatches.MinSubCodeLength);
                    break;
                case FinderStep.MatchZonesList:
                    _routeRulesMatches.RulesByMatchZones.TryGetValue(_zoneId, out matchRules);
                    break;
                case FinderStep.MatchingAllZonesList:
                    matchRules = _routeRulesMatches.RulesMatchingAllZones;
                    break;
            }
            return new MatchList
            {
                Rules = matchRules
            };
        }

        public void GoToStart()
        {
            _currentStep = default(FinderStep);
            foreach (MatchList m in _matchLists.Values)
                m.NextReturnedIndex = 0;
        }
    }
}