using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.OperatorDeclarationServices
{
    public class Topup : OperatorDeclarationServiceSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("B573969D-05A2-4C92-A856-1F846557520C"); }
        }

        public long NumberOfTopups { get; set; }
    }
}
