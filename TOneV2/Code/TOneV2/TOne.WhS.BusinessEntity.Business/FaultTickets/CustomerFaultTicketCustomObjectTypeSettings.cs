using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CustomerFaultTicketCustomObjectTypeSettings : FieldCustomObjectTypeSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("EAD84645-E679-4FDE-8076-33D64EA196F6"); }
        }

        public override string GetDescription(IFieldCustomObjectTypeSettingsContext context)
        {
            //var value = context.FieldValue as List<CustomerFaultTicketDescriptionSetting>;
            //if(value != null)
            //{
            //   return string.Format("{0} {1} {3}",)
            //}
            return null;
        }
    }
}
