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
        private enum FinderStep { MatchCodesList, MatchCodeAndSubCodesList, MatchZonesList, MatchingAllZonesList, End }

        FinderStep _currentSearchStep;
        List<RouteRule> _matchRules;
        int _nextReturnedIndex;


        string _code;
        int _zoneId;
        RouteRuleMatches _routeRulesMatches;

        public RouteRuleMatchFinder(string code, int zoneId, RouteRuleMatches routeRulesMatches)
        {
            _code = code;
            _zoneId = zoneId;
            _routeRulesMatches = routeRulesMatches;
            _matchRules = new List<RouteRule>();
        }

        public bool GetNext(out RouteRule rule)
        {
            while(_nextReturnedIndex < _matchRules.Count)
            {
                var r = _matchRules[_nextReturnedIndex];
                _nextReturnedIndex++;
                if (!r.CodeSet.IsCodeExcluded(_code) && !r.CodeSet.IsZoneExcluded(_zoneId))
                {
                    rule = r;
                    return true;
                }
            }

            List<RouteRule> nextMatchList = GetNextList();
            if (nextMatchList != null && nextMatchList.Count > 0)
            {
                _matchRules.AddRange(nextMatchList);
                return GetNext(out rule);
            }
            else
            {
                rule = null;
                return false;
            }
        }

        string _nextParentCode;
        private List<RouteRule> GetNextList()
        {
            while(_currentSearchStep < FinderStep.End)
            {
                List<RouteRule> matchRules = null;
                switch (_currentSearchStep)
                {
                    case FinderStep.MatchCodesList:
                        _routeRulesMatches.RulesByMatchCodes.TryGetValue(_code, out matchRules);
                        break;
                    case FinderStep.MatchCodeAndSubCodesList:
                        if (_nextParentCode == null)
                            _nextParentCode = _code;

                        while (matchRules == null && _nextParentCode.Length >= _routeRulesMatches.MinSubCodeLength)
                        {
                            _routeRulesMatches.RulesByMatchCodeAndSubCodes.TryGetValue(_nextParentCode, out matchRules);
                            if (_nextParentCode.Length == 0)
                                break;
                            _nextParentCode = _nextParentCode.Substring(0, _nextParentCode.Length - 1);
                        }
                        break;
                    case FinderStep.MatchZonesList:
                        _routeRulesMatches.RulesByMatchZones.TryGetValue(_zoneId, out matchRules);
                        break;
                    case FinderStep.MatchingAllZonesList:
                        matchRules = _routeRulesMatches.RulesMatchingAllZones;
                        break;
                    case FinderStep.End:
                        return null;
                }
                _currentSearchStep++;
                if (matchRules != null && matchRules.Count > 0)
                    return matchRules;                    
            }
            return null;
        }

      
        public void GoToStart()
        {
            _nextReturnedIndex = 0;
        }
    }
}