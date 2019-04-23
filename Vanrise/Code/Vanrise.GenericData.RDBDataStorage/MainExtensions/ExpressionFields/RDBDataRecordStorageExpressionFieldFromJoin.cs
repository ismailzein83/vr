using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.RDBDataStorage.MainExtensions.ExpressionFields
{
    public class RDBDataRecordStorageExpressionFieldFromJoin : RDBDataRecordStorageExpressionFieldSettings
    {
        public override Guid ConfigId => new Guid("DEC7A24F-6439-4DEB-A912-1AA3B0290F21");

        public string JoinName { get; set; }

        public string FieldName { get; set; }

        public override string GetCode(IRDBDataRecordStorageExpressionFieldGetCodeContext context)
        {
            return $@"context.RDBExpressionContext.Column(context.GetJoinTableAlias(""{this.JoinName}""), ""{this.FieldName}"");";
        }

        public override List<string> GetDependentJoins(IRDBDataRecordStorageExpressionFieldGetDependentJoinsContext context)
        {
            return new List<string> { this.JoinName };
        }
    }
}
