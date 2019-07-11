using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Transformation.Entities;

namespace Retail.RA.Business
{
    public class PrepaidTaxMappingStep : BaseGenericRuleMappingStep
    {
        public override Guid ConfigId { get { return new Guid("C8AE398C-9B0D-433B-88FD-57B6EBF3482B"); } }

        #region TopUp
        public string TotalAmount { get; set; }
        public string TotalTaxValue { set; get; }
        #endregion

        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            throw new NotImplementedException();
        }
    }
}
