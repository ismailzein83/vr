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

        void StructureRules(RuleNode parentNode, int behaviorIndex)
        {
            if (behaviorIndex >= _structureBehaviors.Count)
                return;
            parentNode.Behavior = _structureBehaviors[behaviorIndex];
            BaseRuleStructureBehavior ruleStructureBehavior = parentNode.Behavior;

            List<BaseRule> notMatchedRules;
            IEnumerable<RuleNode> nodes = ruleStructureBehavior.StructureRules(this.Rules, out notMatchedRules);
            if (nodes != null && nodes.Count() > 0)
            {
                parentNode.ChildNodes = new List<RuleNode>();
                foreach (var node in nodes)
                {
                    if (node.Rules != null && node.Rules.Count() > 0)
                    {
                        node.ParentNode = parentNode;
                        StructureRules(node, (behaviorIndex + 1));
                        parentNode.ChildNodes.Add(node);
                    }
                }
            }
            if (notMatchedRules != null && notMatchedRules.Count > 0)
            {
                parentNode.UnMatchedRulesNode = new RuleNode();
                parentNode.UnMatchedRulesNode.ParentNode = parentNode;
                parentNode.UnMatchedRulesNode.Rules = notMatchedRules;
                StructureRules(parentNode.UnMatchedRulesNode, (behaviorIndex + 1));
            }
        }

        public BaseRule GetMatchRule(object target)
        {
            return GetMatchRule(this, target);
        }

        BaseRule GetMatchRule(RuleNode parentNode, object target)
        {
            if (parentNode.Behavior == null)//last node in the tree
                return GetFirstMatchRuleFromNode(parentNode, target);

            RuleNode matchChildNode = parentNode.Behavior.GetMatchedNode(target);
            if (matchChildNode != null)
                return GetMatchRule(matchChildNode, target);
            else if (parentNode.UnMatchedRulesNode != null)
                return GetMatchRule(parentNode.UnMatchedRulesNode, target);
            else
                return GetFirstMatchRuleFromNode(parentNode, target);
        }

        BaseRule GetFirstMatchRuleFromNode(RuleNode node, object target)
        {    
            RuleNode parentNode = node.ParentNode;
            if(parentNode != null)//not root node
            {
                foreach (var rule in node.Rules)
                {
                    if (!rule.IsAnyCriteriaExcluded(target))
                        return rule;
                }
                if (!node.IsUnMatchedRulesNode && parentNode.UnMatchedRulesNode != null)
                    return GetMatchRule(parentNode, target);
                else
                    return GetFirstMatchRuleFromNode(parentNode, target);
            }
            return null;
        }
    }
}
