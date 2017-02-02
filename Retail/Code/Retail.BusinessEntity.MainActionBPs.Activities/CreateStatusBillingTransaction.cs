using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Retail.BusinessEntity.Entities;

namespace Retail.BusinessEntity.MainActionBPs.Activities
{

    public sealed class CreateStatusBillingTransaction : CodeActivity
    {

        //[RequiredArgument]
        public InArgument<long> AccountId { get; set; }

        //[RequiredArgument]
        public InArgument<Guid?> StatusDefinitionId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
        }
    }
}
