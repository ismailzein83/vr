using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Business
{
    public class ServiceTypeOverriddenConfiguration : OverriddenConfigurationExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("4F2A2B2F-CAA6-423A-A08F-39DE8587E3BA"); }
        }

        public Guid ServiceTypeId { get; set; }

        public string OverriddenTitle { get; set; }

        public ServiceTypeSettings OverriddenSettings { get; set; }
    }
}
