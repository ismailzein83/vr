using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.DataRecordFields
{
    public class FieldBusinessEntityType : DataRecordFieldType
    {
        public int BusinessEntityDefinitionId { get; set; }

        public override Type GetRuntimeType()
        {
            throw new NotImplementedException();
        }

        public override string GetDescription(Object value)
        {
            BusinessEntityDefinitionManager beDefinitionManager = new BusinessEntityDefinitionManager();
            BusinessEntityDefinition beDefinition = beDefinitionManager.GetBusinessEntityDefinition(BusinessEntityDefinitionId);
            if (beDefinition != null && beDefinition.Settings != null && beDefinition.Settings.ManagerFQTN != null)
            {
                Type beManagerType = Type.GetType(beDefinition.Settings.ManagerFQTN);
                IBusinessEntityManager beManager = Activator.CreateInstance(beManagerType) as IBusinessEntityManager;
                return beManager.GetEntityDescription(new BusinessEntityDescriptionContext() { EntityId = value });
            }
            return null;
        }
    }
}
