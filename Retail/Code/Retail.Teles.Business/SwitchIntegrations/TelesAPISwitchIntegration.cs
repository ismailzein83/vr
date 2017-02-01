using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Business.SwitchIntegrations
{
    public class TelesAPISwitchIntegration : SwitchIntegration
    {
        public override Guid ConfigId { get { return new Guid("983c3d4f-5233-4d00-805c-f60d9015ff7c"); } }
        public string Token { get; set; }
        public string Authorization { get; set; }
        public string URL { get; set; }
    }
}
