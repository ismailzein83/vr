using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules
{
    public class RuleTree : RuleNode
    {
        List<BaseRuleStructureBehavior> _structureBehaviors;
        public RuleTree(IEnumerable<BaseRule> rules, IEnumerable<BaseRuleStructureBehavior> structureBehaviors)
        {
            _structureBehaviors = structureBehaviors.ToList();

            this.Rules = rules.ToList();
            StructureRules(this, 0, null);
        }

        void StructureRules(RuleNode node, int behaviorIndex, Dictionary<int, List<BaseRule>> priorities)
        {
            if (priorities != null)
                priorities = priorities.OrderBy(itm => itm.Key).ToDictionary(itm => itm.Key, itm => itm.Value);

            if (behaviorIndex >= _structureBehaviors.Count)
                return;

            int nextBehaviorIndex = behaviorIndex + 1;
            node.Behavior = _structureBehaviors[behaviorIndex].CreateNewBehaviorObject();

            List<BaseRule> notMatchedRules;
            IEnumerable<RuleNode> childNodes = node.Behavior.StructureRules(node.Rules, out notMatchedRules);
            if (childNodes != null && childNodes.Count() > 0)
            {
                node.ChildNodes = new List<RuleNode>();
                foreach (var childNode in childNodes)
                {
                    if (childNode.Rules != null && childNode.Rules.Count() > 0)
                    {
                        childNode.ParentNode = node;
                        Dictionary<int, List<BaseRule>> childNodePiorities = BuildPriorities(priorities, childNode.Priorities);
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

        private void OrderRules(RuleNode node, Dictionary<int, List<BaseRule>> priorities)
        {
            if (priorities == null || priorities.Count == 0)
                return;

            if (node.Rules == null || node.Rules.Count == 0)
                return;

            List<BaseRule> rules = new List<BaseRule>();
            foreach (var priority in priorities)
            {
                foreach (BaseRule rule in priority.Value)
                {
                    if (node.Rules.Contains(rule) && !rules.Contains(rule))
                        rules.Add(rule);
                }
            }
            node.Rules = rules;
        }

        private Dictionary<int, List<BaseRule>> BuildPriorities(Dictionary<int, List<BaseRule>> priorities, Dictionary<BaseRule, int> nodePriorities)
        {
            Dictionary<int, List<BaseRule>> result = new Dictionary<int, List<BaseRule>>();
            int priority = 0;
            List<BaseRule> baseRules;

            if (nodePriorities == null || nodePriorities.Count == 0)
                return priorities != null ? new Dictionary<int, List<BaseRule>>(priorities) : null;

            if (priorities == null || priorities.Count == 0)
            {
                foreach (var nodePriority in nodePriorities)
                {
                    if (result.TryGetValue(nodePriority.Value, out baseRules))
                    {
                        baseRules.Add(nodePriority.Key);
                    }
                    else
                    {
                        baseRules = new List<BaseRule>() { nodePriority.Key };
                        result.Add(priority, baseRules);
                        priority++;
                    }
                }
                return result;
            }

            foreach (KeyValuePair<int, List<BaseRule>> item in priorities)
            {
                Dictionary<int, List<BaseRule>> tempDict = new Dictionary<int, List<BaseRule>>();
                foreach (BaseRule baseRule in item.Value)
                {
                    int rulePriority;
                    if (nodePriorities.TryGetValue(baseRule, out rulePriority))
                    {
                        if (tempDict.TryGetValue(rulePriority, out baseRules))
                        {
                            baseRules.Add(baseRule);
                        }
                        else
                        {
                            baseRules = new List<BaseRule>() { baseRule };
                            tempDict.Add(rulePriority, baseRules);
                        }
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

        public BaseRule GetMatchRule(BaseRuleTarget target)
        {
            return GetMatchRule(this, target);
        }

        BaseRule GetMatchRule(RuleNode parentNode, BaseRuleTarget target)
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

        BaseRule GetFirstMatchRuleFromNode(RuleNode node, BaseRuleTarget target)
        {
            foreach (var rule in node.Rules)
            {
                if (!IsRuleMatched(rule, target))
                    continue;
                return rule;
            }
            return null;
        }

        bool IsRuleMatched(BaseRule rule, BaseRuleTarget target)
        {
            DateTime now = DateTime.Now;

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
