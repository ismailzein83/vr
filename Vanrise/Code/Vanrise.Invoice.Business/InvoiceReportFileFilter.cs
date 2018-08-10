using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
namespace Vanrise.Invoice.Business
{
    public class InvoiceReportFileFilter : IGenericBusinessEntityFilter
    {
        public Guid InvoiceTypeId { get; set; }

        public bool IsMatch(IGenericBusinessEntityFilterContext context)
        {
            if (context != null) 
            {
                if (context.GenericBusinessEntityDetail != null && context.GenericBusinessEntityDetail.FieldValues != null && context.GenericBusinessEntityDetail.FieldValues.Count > 0)
                {
                    var genericBEValue = context.GenericBusinessEntityDetail.FieldValues.GetRecord("InvoiceTypeId");
                    if (genericBEValue != null)
                    {
                        if ((Guid)genericBEValue.Value != this.InvoiceTypeId)
                        {
                            return false;
                        }
                    }
                   
                }
            }
            return false;
        } 
    }
}
