using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Business;

namespace Retail.RA.Business
{
    public class RAOperationDeclarationOnBeforeSaveHandler : GenericBEOnBeforeInsertHandler
    {
        public override Guid ConfigId { get { return new Guid("F7D8C1E5-DF05-4EA1-B43A-5BE5198BE6ED"); } }
        public override void Execute(IGenericBEOnBeforeInsertHandlerContext context)
        {
            //context.ThrowIfNull("context");
            //context.GenericBusinessEntity.ThrowIfNull("context.GenericBusinessEntity");
            //context.GenericBusinessEntity.FieldValues.ThrowIfNull("context.GenericBusinessEntity.FieldValues");
            //OperatorDeclarationManager operatorDeclarationManager = new OperatorDeclarationManager();
            //var operationDeclarations = operatorDeclarationManager.GetAllOperatorDecalarations();
           
            //var periodId = context.GenericBusinessEntity.FieldValues.GetRecord("PeriodId");
            //if (periodId == null)
            //    throw new NullReferenceException("periodId");
            //var operationDeclaration = operationDeclarations.FindRecord(x => x.PeriodId == (int)periodId);

        }
    }
}
