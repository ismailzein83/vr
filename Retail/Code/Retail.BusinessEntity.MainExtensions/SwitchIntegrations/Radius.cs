using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.SwitchIntegrations
{
    public class Radius : SwitchIntegration
    {
        public override Guid ConfigId { get { return new Guid("983c3d4f-5233-4d00-805c-f60d9015ff7c"); } }

        public string ConnectionString { get; set; }

        public string TableName { get; set; }

        public string MappingLogic { get; set; }
    }
}
