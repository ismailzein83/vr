using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using TOne.WhS.Jazz.Business;

namespace TOne.WhS.Jazz.Business
{
    public class AccountCodeOnBeforeInsertHandler : GenericBEOnBeforeInsertHandler
    {
        public override Guid ConfigId
        {
            get { return new Guid("0D958446-27C2-4685-971E-AAF7816355C0"); }
        }


        public override void Execute(IGenericBEOnBeforeInsertHandlerContext context)
        {
                context.GenericBusinessEntity.ThrowIfNull("context.GenericBusinessEntity");

                AccountCodeManager accountCodeManager = new AccountCodeManager();
                var result = accountCodeManager.ValidateAccountCode(context.GenericBusinessEntity);

                context.OutputResult.Result = result;
                context.OutputResult.Messages.Add(result ? "Account Code inserted Successfully" : "Account Code Is Not Valid");
        }

    }

}

