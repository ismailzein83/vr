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
                if (context.GenericBusinessEntity != null && context.GenericBusinessEntity.FieldValues != null && context.GenericBusinessEntity.FieldValues.Count > 0)
                {
                    var genericBEValue = context.GenericBusinessEntity.FieldValues.GetRecord("InvoiceTypeId");
                    if (genericBEValue != null)
                    {
                        if ((Guid)genericBEValue != this.InvoiceTypeId)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        } 
    }
}
