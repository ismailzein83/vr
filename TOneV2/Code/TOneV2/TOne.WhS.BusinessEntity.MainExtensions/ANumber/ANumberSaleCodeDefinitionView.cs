using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
    public class ANumberSaleCodeDefinitionView : GenericBEViewDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("6B3134C3-D543-47B0-840F-25356E31E7DC"); }
        }
        public override string RuntimeDirective
        {
            get { return "whs-be-anumber-salecodegridview-runtime"; }
        }

        public override bool DoesUserHaveAccess(IGenericBEViewDefinitionCheckAccessContext context)
        {
            return true;//base.DoesUserHaveAccess(context);
        }
    }

}
