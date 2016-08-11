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
            StructureRules(this, 0);
        }

        void StructureRules(RuleNode node, int behaviorIndex)
        {
            if (behaviorIndex >= _structureBehaviors.Count)
                return;
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
                        StructureRules(childNode, (behaviorIndex + 1));
                        node.ChildNodes.Add(childNode);
                    }
                }
            }
            if (notMatchedRules != null && notMatchedRules.Count > 0)
            {
                node.UnMatchedRulesNode = new RuleNode();
                node.UnMatchedRulesNode.IsUnMatchedRulesNode = true;
                node.UnMatchedRulesNode.ParentNode = node;
                node.UnMatchedRulesNode.Rules = notMatchedRules;
                StructureRules(node.UnMatchedRulesNode, (behaviorIndex + 1));
            }
        }

        public BaseRule GetMatchRule(BaseRuleTarget target)
        {
            return GetMatchRule(this, target);
        }

        BaseRule GetMatchRule(RuleNode parentNode, BaseRuleTarget target)
        {
            if (parentNode.Behavior == null)//last node in the tree
                return GetFirstMatchRuleFromNode(parentNode, target);

            RuleNode matchChildNode = parentNode.Behavior.GetMatchedNode(target);
            if (matchChildNode != null)
                return GetMatchRule(matchChildNode, target);
            else if (parentNode.UnMatchedRulesNode != null)
                return GetMatchRule(parentNode.UnMatchedRulesNode, target);
            var node = parentNode;
            while (node.ParentNode != null)//not root node
            {
                if (!node.IsUnMatchedRulesNode && node.ParentNode.UnMatchedRulesNode != null)
                    return GetMatchRule(node.ParentNode.UnMatchedRulesNode, target);
                node = node.ParentNode;
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
            while (node.ParentNode != null)//not root node
            {
                if (!node.IsUnMatchedRulesNode && node.ParentNode.UnMatchedRulesNode != null)
                    return GetMatchRule(node.ParentNode.UnMatchedRulesNode, target);
                node = node.ParentNode;
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
