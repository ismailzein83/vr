using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public enum CriteriaCompareOperator {  GreaterThan, LessThan }

    public class CriteriaDefinition
    {
        public int CriteriaId { get; set; }

        public CriteriaCompareOperator CompareOperator { get; set; }
    }
}
