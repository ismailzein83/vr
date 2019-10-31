using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Retail.NIM.Entities;

namespace Retail.NIM.Business
{
    public class NodePortManager
    {
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();

        public ReservePortOutput ReservePort(ReservePortInput input)
        {

            var filter = new RecordFilterGroup
            {
                Filters = new List<RecordFilter>
                {
                    new ObjectListRecordFilter
                    {
                        FieldName ="Node",
                        Values =  new List<object> {input.NodeId}
                    },
                    new ObjectListRecordFilter
                        {
                              FieldName = "Status",
                              Values = new List<object> { StaticBEDefinitionIDs.FreePortStatusDefinitionId.ToString() }
                        }
                }
            };

            if (input.PartTypeId.HasValue)
            {
                filter.Filters.Add(new ObjectListRecordFilter
                {
                    FieldName = "PartType",
                    Values = new List<object> { input.PartTypeId.Value.ToString() }
                });
            }

            var entities = _genericBusinessEntityManager.GetAllGenericBusinessEntities(StaticBEDefinitionIDs.NodePortBEDefinitionId, null, filter);

            if (entities == null || entities.Count() == 0)
                return null;

            var firstItem = entities.First();

            var portid = (long)firstItem.FieldValues.GetRecord("ID");
            return ReservePort(portid);
        }
        public ReservePortOutput ReservePort(long portId)
        {
            var updatedEntity = _genericBusinessEntityManager.UpdateGenericBusinessEntity(new GenericBusinessEntityToUpdate
            {
                BusinessEntityDefinitionId = StaticBEDefinitionIDs.NodePortBEDefinitionId,
                FieldValues = new Dictionary<string, object> { { "Status", StaticBEDefinitionIDs.ReservedPortStatusDefinitionId } },
                GenericBusinessEntityId = portId,
                FilterGroup =new RecordFilterGroup
                {
                    Filters = new List<RecordFilter>
                    {
                        new ObjectListRecordFilter
                        {
                              FieldName = "Status",
                              Values = new List<object> { StaticBEDefinitionIDs.FreePortStatusDefinitionId }
                        }
                   }
                }
            });

            if (updatedEntity.Result == UpdateOperationResult.Failed)
                return null;

            return new ReservePortOutput
            {
                PortId = portId,
                Number = updatedEntity.UpdatedObject.FieldValues.GetRecord("Number").Description
            };
        }
        
    }

  



    
}
