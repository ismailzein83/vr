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
        string _nextParentCode;


        string _code;
        int _zoneId;
        RouteRuleMatches _routeRulesMatches;

        public RouteRuleMatchFinder(string code, int zoneId, RouteRuleMatches routeRulesMatches)
        {
            _code = code;
            _zoneId = zoneId;
            _routeRulesMatches = routeRulesMatches;
            _matchRules = new List<RouteRule>();
            _currentSearchStep = FinderStep.MatchCodesList;
            _nextParentCode = _code;
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

        
        private List<RouteRule> GetNextList()
        {
            List<RouteRule> matchRules = null;
            switch (_currentSearchStep)
            {
                case FinderStep.MatchCodesList:
                    _routeRulesMatches.RulesByMatchCodes.TryGetValue(_code, out matchRules);
                _currentSearchStep++;
                    break;
                case FinderStep.MatchCodeAndSubCodesList:
                    while (matchRules == null && _nextParentCode.Length >= Math.Max(_routeRulesMatches.MinSubCodeLength, 1))
                    {
                        _routeRulesMatches.RulesByMatchCodeAndSubCodes.TryGetValue(_nextParentCode, out matchRules);
                        _nextParentCode = _nextParentCode.Substring(0, _nextParentCode.Length - 1);
                    }

                    if (_nextParentCode.Length < Math.Max(_routeRulesMatches.MinSubCodeLength, 1))
                        _currentSearchStep++;
                    break;
                case FinderStep.MatchZonesList:
                    _routeRulesMatches.RulesByMatchZones.TryGetValue(_zoneId, out matchRules);
                _currentSearchStep++;
                    break;
                case FinderStep.MatchingAllZonesList:
                    matchRules = _routeRulesMatches.RulesMatchingAllZones;
                _currentSearchStep++;
                    break;
                case FinderStep.End:
                    return null;
            }

            if (matchRules != null && matchRules.Count > 0)
                return matchRules;
            else
                return GetNextList();
        }
      
        public void GoToStart()
        {
            _nextReturnedIndex = 0;
        }
    }
}