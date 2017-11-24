using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountBEActionTypes
{
    public class AssignAccountManagerDefinitionView : AccountViewDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("A5483E0A-A8FB-418C-A04B-E7E58838F829"); }
        }
        public override string RuntimeEditor
        {
            get
            {
                return "";
            }
            set
            {

            }
        }

        public override bool DoesUserHaveAccess(IAccountViewDefinitionCheckAccessContext context)
        {
            return base.DoesUserHaveAccess(context);
        }

    }
}
