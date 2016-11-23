using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.Account360DegreeViews
{
    public class Services : Account360DegreeViewExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("71AB18ED-F2AC-4E71-B4E4-4826D092A201"); }
        }

        public override string RuntimeEditor
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                
            }
        }
    }
}
