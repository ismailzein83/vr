using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.RDBDataStorage.MainExtensions.ExpressionFields
{
    public class RDBDataRecordStorageCustomCodeExpressionField : RDBDataRecordStorageExpressionFieldSettings
    {
        public override Guid ConfigId => new Guid("2E000BBE-3842-49D5-BCAE-C9B20D778F18");

        public string CustomCode { get; set; }

        public List<string> DependentJoins { get; set; }

        public override string GetCode(IRDBDataRecordStorageExpressionFieldGetCodeContext context)
        {
            return this.CustomCode;
        }

        public override List<string> GetDependentJoins(IRDBDataRecordStorageExpressionFieldGetDependentJoinsContext context)
        {
            return this.DependentJoins;
        }
    }
}
