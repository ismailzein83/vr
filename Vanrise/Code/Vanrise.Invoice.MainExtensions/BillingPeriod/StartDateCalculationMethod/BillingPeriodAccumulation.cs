using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Business.Context;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class BillingPeriodAccumulation:StartDateCalculationMethod
    {
        public override Guid ConfigId { get { return new Guid("5F9DDA2C-860D-42E6-BDF2-904B1B8FF287"); } }

        public override void CalculateDate(IStartDateCalculationMethodContext context)
        {
            //if(context.InitialStartDate > context.PartnerCreatedDate)
            //    context.StartDate = context.InitialStartDate;
            //else if(context.PartnerCreatedDate >= context.InitialStartDate )
            //{
                 
            //}
            context.StartDate = context.PartnerCreatedDate;
        }
    }
}
