using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Business;

namespace Retail.RA.Business
{
    public class RAInternationalOperatorDeclarationOnBeforeSaveHandler : GenericBEOnBeforeInsertHandler
    {
        public override Guid ConfigId { get { return new Guid("F7D8C1E5-DF05-4EA1-B43A-5BE5198BE6ED"); } }
        public override void Execute(IGenericBEOnBeforeInsertHandlerContext context)
        {
            context.ThrowIfNull("context");
            context.GenericBusinessEntity.ThrowIfNull("context.GenericBusinessEntity");
            context.GenericBusinessEntity.FieldValues.ThrowIfNull("context.GenericBusinessEntity.FieldValues");
            OperatorDeclarationManager operatorDeclarationManager = new OperatorDeclarationManager();
            var operatorDeclarations = operatorDeclarationManager.GetAllOperatorDecalarations();
            var operatorId = context.GenericBusinessEntity.FieldValues.GetRecord("Operator");
            operatorId.ThrowIfNull("operatorId");
            var periodId = context.GenericBusinessEntity.FieldValues.GetRecord("Period");
            periodId.ThrowIfNull("periodId");
          
            var operatorDeclaration = operatorDeclarations.FindRecord(x => x.PeriodId == Convert.ToInt32(periodId) && x.OperatorId == Convert.ToInt64(operatorId));
            if(operatorDeclaration != null)
            {
                context.OutputResult.Result = false;
                context.OutputResult.Messages.Add("Cannot add two operators with the same period definition");
            }

        }
    }
}
