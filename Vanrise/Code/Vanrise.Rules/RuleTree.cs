using System;
using System.Linq;
using System.Collections.Generic;
using Vanrise.Common;

namespace Vanrise.Rules
{
    public class RuleTree : RuleNode
    {
        public static Object s_lockObj = new object();
        List<BaseRuleStructureBehavior> _structureBehaviors;

        public RuleTree(IEnumerable<IVRRule> rules, IEnumerable<BaseRuleStructureBehavior> structureBehaviors)
        {
            _structureBehaviors = structureBehaviors.ToList();

            this.Rules = rules.ToList();
            StructureRules(this, 0, null);
        }

        void StructureRules(RuleNode node, int behaviorIndex, Dictionary<int, List<IVRRule>> priorities)
        {
            if (priorities != null)
                priorities = priorities.OrderBy(itm => itm.Key).ToDictionary(itm => itm.Key, itm => itm.Value);

            if (behaviorIndex >= _structureBehaviors.Count)
                return;

            int nextBehaviorIndex = behaviorIndex + 1;
            node.Behavior = _structureBehaviors[behaviorIndex].CreateNewBehaviorObject();

            List<IVRRule> notMatchedRules;
            IEnumerable<RuleNode> childNodes = node.Behavior.StructureRules(node.Rules, out notMatchedRules);
            if (childNodes != null && childNodes.Count() > 0)
            {
                node.ChildNodes = new List<RuleNode>();
                foreach (var childNode in childNodes)
                {
                    if (childNode.Rules != null && childNode.Rules.Count() > 0)
                    {
                        childNode.ParentNode = node;
                        Dictionary<int, List<IVRRule>> childNodePiorities = BuildPriorities(priorities, childNode.Priorities);
                        StructureRules(childNode, nextBehaviorIndex, childNodePiorities);
                        node.ChildNodes.Add(childNode);
                        if (nextBehaviorIndex >= _structureBehaviors.Count)
                            OrderRules(childNode, childNodePiorities);
                    }
                }
            }
            if (notMatchedRules != null && notMatchedRules.Count > 0)
            {
                node.UnMatchedRulesNode = new RuleNode();
                node.UnMatchedRulesNode.IsUnMatchedRulesNode = true;
                node.UnMatchedRulesNode.ParentNode = node;
                node.UnMatchedRulesNode.Rules = notMatchedRules;
                StructureRules(node.UnMatchedRulesNode, nextBehaviorIndex, priorities);
                if (nextBehaviorIndex >= _structureBehaviors.Count)
                    OrderRules(node, priorities);
            }
        }

        private void OrderRules(RuleNode node, Dictionary<int, List<IVRRule>> priorities)
        {
            if (priorities != null && priorities.Count > 0)
            {
                List<IVRRule> copyOfUnmatchedRules = null;
                List<IVRRule> orderedUnMatchedRules = null;
                bool hasUnMatchedRules = false;
                if (node.UnMatchedRulesNode != null && node.UnMatchedRulesNode.Rules != null && node.UnMatchedRulesNode.Rules.Count > 0)
                {
                    hasUnMatchedRules = true;
                    copyOfUnmatchedRules = new List<IVRRule>(node.UnMatchedRulesNode.Rules);
                    orderedUnMatchedRules = new List<IVRRule>();
                }

                List<IVRRule> copyOfMatchedRules = null;
                List<IVRRule> orderedMatchedRules = null;
                bool hasMatchedRules = false;
                if (node.Rules != null && node.Rules.Count > 0)
                {
                    hasMatchedRules = true;
                    copyOfMatchedRules = new List<IVRRule>(node.Rules);
                    orderedMatchedRules = new List<IVRRule>();
                }

                foreach (var priority in priorities)
                {
                    var orderedRules = OrderRulesByPriorityIfSameCriteria(priority.Value);
                    foreach (IVRRule rule in orderedRules)
                    {
                        if (hasMatchedRules && copyOfMatchedRules.Contains(rule))
                        {
                            orderedMatchedRules.Add(rule);
                            copyOfMatchedRules.Remove(rule);
                        }

                        if (hasUnMatchedRules && copyOfUnmatchedRules.Contains(rule))
                        {
                            orderedUnMatchedRules.Add(rule);
                            copyOfUnmatchedRules.Remove(rule);
                        }
                    }
                }
                if (hasMatchedRules && copyOfMatchedRules.Count > 0)//MatchhedRules without any priority
                    orderedMatchedRules.AddRange(copyOfMatchedRules);

                if (hasUnMatchedRules && copyOfUnmatchedRules.Count > 0)//UnMatchedRules without any priority
                    orderedUnMatchedRules.AddRange(copyOfUnmatchedRules);

                if (orderedMatchedRules != null)
                    node.Rules = orderedMatchedRules;

                if (orderedUnMatchedRules != null)
                    node.UnMatchedRulesNode.Rules = orderedUnMatchedRules;
            }
            else
            {
                if (node.Rules != null && node.Rules.Count > 0)
                    node.Rules = OrderRulesByPriorityIfSameCriteria(node.Rules);

                if (node.UnMatchedRulesNode != null && node.UnMatchedRulesNode.Rules != null && node.UnMatchedRulesNode.Rules.Count > 0)
                    node.UnMatchedRulesNode.Rules = OrderRulesByPriorityIfSameCriteria(node.UnMatchedRulesNode.Rules);
            }
        }

        private List<IVRRule> OrderRulesByPriorityIfSameCriteria(List<IVRRule> rules)
        {
            var context = new RuleGetPriorityContext();
            return rules != null ? rules.OrderByDescending(itm => itm.GetPriorityIfSameCriteria(context)).ToList() : null;
        }

        private Dictionary<int, List<IVRRule>> BuildPriorities(Dictionary<int, List<IVRRule>> priorities, Dictionary<IVRRule, int> nodePriorities)
        {
            Dictionary<int, List<IVRRule>> result = new Dictionary<int, List<IVRRule>>();
            int priority = 0;


            if (nodePriorities == null || nodePriorities.Count == 0)
                return priorities != null ? new Dictionary<int, List<IVRRule>>(priorities) : null;

            if (priorities == null || priorities.Count == 0)
            {
                foreach (var nodePriority in nodePriorities)
                {
                    result.GetOrCreateItem(nodePriority.Value).Add(nodePriority.Key);
                }
                return result;
            }

            foreach (KeyValuePair<int, List<IVRRule>> item in priorities)
            {
                Dictionary<int, List<IVRRule>> tempDict = new Dictionary<int, List<IVRRule>>();
                foreach (var baseRule in item.Value)
                {
                    int rulePriority;
                    if (nodePriorities.TryGetValue(baseRule, out rulePriority))
                    {
                        tempDict.GetOrCreateItem(rulePriority).Add(baseRule);
                    }
                }
                tempDict = tempDict.OrderBy(itm => itm.Key).ToDictionary(itm => itm.Key, itm => itm.Value);
                foreach (var tempItem in tempDict)
                {
                    result.Add(priority, tempItem.Value);
                    priority++;
                }
            }

            return result.Count > 0 ? result : null;
        }

        public IVRRule GetMatchRule(BaseRuleTarget target)
        {
            IVRRule matchRule = GetMatchRule(this, target);
            if (matchRule != null)
            {
                DateTime now = VRClock.Now;
                if (!matchRule.LastRefreshedTime.HasValue || (now - matchRule.LastRefreshedTime.Value) > matchRule.RefreshTimeSpan)
                {
                    lock (s_lockObj)
                    {
                        if (!matchRule.LastRefreshedTime.HasValue || (now - matchRule.LastRefreshedTime.Value) > matchRule.RefreshTimeSpan)
                        {
                            matchRule.RefreshRuleState(new RefreshRuleStateContext() { EffectiveDate = target.EffectiveOn.HasValue ? target.EffectiveOn.Value : now });
                        }
                    }
                }
            }
            return matchRule;
        }

        IVRRule GetMatchRule(RuleNode parentNode, BaseRuleTarget target)
        {
            if (parentNode.Behavior == null)//last node in the tree
            {
                var matchRule = GetFirstMatchRuleFromNode(parentNode, target);
                if (matchRule != null)
                    return matchRule;
            }
            else
            {
                List<RuleNode> matchChildNodes = parentNode.Behavior.GetMatchedNodes(target);
                if (matchChildNodes != null)
                {
                    foreach (RuleNode matchChildNode in matchChildNodes)
                    {
                        var matchRule = GetMatchRule(matchChildNode, target);
                        if (matchRule != null)
                            return matchRule;
                    }
                }

                if (parentNode.UnMatchedRulesNode != null)
                {
                    var matchRule = GetMatchRule(parentNode.UnMatchedRulesNode, target);
                    if (matchRule != null)
                        return matchRule;
                }
            }

            return null;
        }

        IVRRule GetFirstMatchRuleFromNode(RuleNode node, BaseRuleTarget target)
        {
            foreach (var rule in node.Rules)
            {
                if (!IsRuleMatched(rule, target))
                    continue;
                return rule;
            }
            return null;
        }

        bool IsRuleMatched(IVRRule rule, BaseRuleTarget target)
        {
            DateTime now = VRClock.Now;

            if ((target.EffectiveOn.HasValue && (target.EffectiveOn.Value < rule.BeginEffectiveTime
                                                || (rule.EndEffectiveTime.HasValue && target.EffectiveOn.Value >= rule.EndEffectiveTime.Value)))
                || (target.IsEffectiveInFuture && now > rule.BeginEffectiveTime && rule.EndEffectiveTime.HasValue))
                return false;

            if (rule.IsAnyCriteriaExcluded(target))
                return false;

            return true;
        }
    }
}
