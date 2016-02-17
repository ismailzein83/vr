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
            BusinessEntityDefinitionManager beDefinitionManager = new BusinessEntityDefinitionManager();
            BusinessEntityDefinition beDefinition = beDefinitionManager.GetBusinessEntityDefinition(BusinessEntityDefinitionId);
            if (beDefinition == null)
                throw new NullReferenceException(string.Format("beDefinition '{0}'", this.BusinessEntityDefinitionId));
            return Type.GetType(beDefinition.Settings.IdType);
        }

        public override string GetDescription(Object value)
        {
            IBusinessEntityManager beManager = GetBusinessEntityManager();
            return (beManager != null) ? beManager.GetEntityDescription(new BusinessEntityDescriptionContext() { EntityId = value }) : null;
        }

        public override bool IsMatched(object fieldValue, object filterValue)
        {
            IBusinessEntityManager beManager = GetBusinessEntityManager();
            return (beManager != null) ? beManager.IsMatched(new BusinessEntityMatchContext() { FieldValue = fieldValue, FilterValue = filterValue }) : false;
        }

        IBusinessEntityManager GetBusinessEntityManager()
        {
            BusinessEntityDefinitionManager beDefinitionManager = new BusinessEntityDefinitionManager();
            BusinessEntityDefinition beDefinition = beDefinitionManager.GetBusinessEntityDefinition(BusinessEntityDefinitionId);
            if (beDefinition != null && beDefinition.Settings != null && beDefinition.Settings.ManagerFQTN != null)
            {
                Type beManagerType = Type.GetType(beDefinition.Settings.ManagerFQTN);
                return Activator.CreateInstance(beManagerType) as IBusinessEntityManager;
            }
            return null;
        }
    }
}
