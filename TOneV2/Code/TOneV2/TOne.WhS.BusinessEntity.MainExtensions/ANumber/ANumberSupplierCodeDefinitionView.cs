using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
    public class ANumberSupplierCodeDefinitionView : GenericBEViewDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("6821BD9E-318C-49A9-B3B6-08AA3AAA4006"); }
        }
        public override string RuntimeDirective
        {
            get { return "whs-be-anumber-suppliercodegridview-runtime"; }
        }

        public override bool DoesUserHaveAccess(IGenericBEViewDefinitionCheckAccessContext context)
        {
            return true;//base.DoesUserHaveAccess(context);
        }
    }

}
