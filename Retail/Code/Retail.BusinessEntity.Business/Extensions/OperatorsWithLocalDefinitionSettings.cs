using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class OperatorsWithLocalDefinitionSettings : Vanrise.GenericData.Entities.BusinessEntityDefinitionSettings
    {
        private static Guid s_configId = new Guid("C6F27953-BDDB-4F0C-BC8C-55A3E5AA0C85");
        public override Guid ConfigId { get { return s_configId; } }

        public Guid AccountBEDefintionId { get; set; }

        public override string SelectorUIControl { get { return "retail-be-icx-operatorswithlocal-selector"; } }
    }
}
