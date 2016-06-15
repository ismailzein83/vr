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
        public string ConnectionString { get; set; }

        public string TableName { get; set; }

        public string MappingLogic { get; set; }
    }
}
