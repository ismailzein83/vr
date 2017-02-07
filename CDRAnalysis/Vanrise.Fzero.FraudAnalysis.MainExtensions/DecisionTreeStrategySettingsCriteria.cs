using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.MainExtensions
{
    public class DecisionTreeStrategySettingsCriteria : StrategySettingsCriteria
    {
        public override Guid ConfigId { get { return new Guid("7a21f763-6443-4baf-825e-9421a33ce7df"); } }
        public DecisionTree DecisionTree { get; set; }

        public override void PrepareForExecution(IPrepareStrategySettingsCriteriaContext context)
        {
            context.PreparedData = this.DecisionTree;
        }

        public override bool IsNumberSuspicious(IStrategySettingsCriteriaContext context)
        {
            
            Dictionary<int, decimal?> criteriaValuesThresholds = new Dictionary<int, decimal?>();///should know if required or not
            var decisionTree = context.PreparedData as DecisionTree;
            return CheckIsNumberSuspicious(decisionTree.RootNode, context, criteriaValuesThresholds);
        }
        private bool CheckIsNumberSuspicious(DecisionTreeNode decisionTree, IStrategySettingsCriteriaContext context, Dictionary<int, decimal?> criteriaValuesThresholds)
        {

            if (decisionTree.ConditionNode != null)
            {
                if (decisionTree.ConditionNode.Condition != null)
                {
                    var filter = context.GetFilter(decisionTree.ConditionNode.Condition.FilterId);
                    var criteriaValue = context.GetCriteriaValue(decisionTree.ConditionNode.Condition.FilterId);
                    switch (filter.CompareOperator)
                    {
                        case CriteriaCompareOperator.GreaterThanorEqual:
                            if (decisionTree.ConditionNode.Condition.Operator == DecisionTreeConditionOperator.GreaterOrEquals && decisionTree.ConditionNode.Condition.Value >= criteriaValue.Value)
                            {
                                criteriaValuesThresholds.Add(decisionTree.ConditionNode.Condition.FilterId, decisionTree.ConditionNode.Condition.Value);
                                return CheckIsNumberSuspicious(decisionTree.ConditionNode.TrueBranch, context, criteriaValuesThresholds);

                            }
                            else
                            {
                                criteriaValuesThresholds.Add(decisionTree.ConditionNode.Condition.FilterId, decisionTree.ConditionNode.Condition.Value);
                                return CheckIsNumberSuspicious(decisionTree.ConditionNode.FalseBranch, context, criteriaValuesThresholds);
                            }
                        case CriteriaCompareOperator.LessThanorEqual:
                            if (decisionTree.ConditionNode.Condition.Operator == DecisionTreeConditionOperator.LessOrEquals && decisionTree.ConditionNode.Condition.Value <= criteriaValue.Value)
                            {
                                criteriaValuesThresholds.Add(decisionTree.ConditionNode.Condition.FilterId, decisionTree.ConditionNode.Condition.Value);
                                return CheckIsNumberSuspicious(decisionTree.ConditionNode.TrueBranch, context, criteriaValuesThresholds);

                            }
                            else
                            {
                                criteriaValuesThresholds.Add(decisionTree.ConditionNode.Condition.FilterId, decisionTree.ConditionNode.Condition.Value);
                                return CheckIsNumberSuspicious(decisionTree.ConditionNode.FalseBranch, context, criteriaValuesThresholds);
                            }
                    }
                }
            }
            else
            {
                if (decisionTree.SuspicionLevel.HasValue)
                {
                    context.StrategyExecutionItem = new StrategyExecutionItem
                    {
                        AccountNumber = context.NumberProfile.AccountNumber,
                        SuspicionLevelID = (int)decisionTree.SuspicionLevel.Value,
                        FilterValues = criteriaValuesThresholds,
                        AggregateValues = context.NumberProfile.AggregateValues,
                        SuspicionOccuranceStatus = SuspicionOccuranceStatus.Open,
                        StrategyExecutionID = context.NumberProfile.StrategyExecutionID,
                        IMEIs = context.NumberProfile.IMEIs,
                    };
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

    }
}
