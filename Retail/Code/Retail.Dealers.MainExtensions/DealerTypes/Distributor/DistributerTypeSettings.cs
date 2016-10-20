using Retail.Dealers.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Dealers.MainExtensions.DealerTypes.Distributor
{
    public class DistributerTypeSettings : DealerTypeExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("B7CB3589-CFD7-48A2-A25F-8077EDA9F8F5"); }
        }

        public override string RuntimeEditor
        {
            get { throw new NotImplementedException(); }
        }
    }
}
