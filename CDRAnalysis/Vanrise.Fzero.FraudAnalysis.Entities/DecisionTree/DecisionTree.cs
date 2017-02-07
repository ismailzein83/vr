using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public enum DecisionTreeConditionOperator
    {
        [Description(" = ")]
        Equals = 0,
        [Description(" <> ")]
        NotEquals = 1,
        [Description(" > ")]
        Greater = 2,
        [Description(" >= ")]
        GreaterOrEquals = 3,
        [Description(" < ")]
        Less = 4,
        [Description(" <= ")]
        LessOrEquals = 5
    }
    public class DecisionTree
    {
        public DecisionTreeNode RootNode { get; set; }
    }
    public class DecisionTreeNode
    {
        public DecisionTreeConditionNode ConditionNode { get; set; }
        public SuspicionLevel? SuspicionLevel { get; set; }
    }
    public class DecisionTreeConditionNode
    {
        public DecisionTreeCondition Condition { get; set; }
        public DecisionTreeNode TrueBranch { get; set; }
        public DecisionTreeNode FalseBranch { get; set; }
    }
    public class DecisionTreeCondition
    {
        public int FilterId { get; set; }
        public decimal Value { get; set; }
        public DecisionTreeConditionOperator Operator { get; set; }
    }
}
