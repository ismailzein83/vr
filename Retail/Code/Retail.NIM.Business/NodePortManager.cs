using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Retail.NIM.Entities;

namespace Retail.NIM.Business
{
    public class NodePortManager
    {
        static Guid _connectionDefinitionId = new Guid("e74d9d5d-cc59-4b40-8626-048a755c054c");
        static Guid _nodePortDefinitionId = new Guid("04868fe5-9944-4e2b-b4d2-de9c5f73e2f4");
        static Guid _freeNodeStatus = new Guid("a11d2835-89ed-442c-9646-c1f9b23ff213");

        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();
        public ReserveConnectionOutput ReserveConnection(ReserveConnectionInput input)
        {
            var filter = new Vanrise.GenericData.Entities.RecordFilterGroup
            {
                Filters = new List<Vanrise.GenericData.Entities.RecordFilter>
                {
                    new ObjectListRecordFilter
                    {
                        FieldName ="Port1Node",
                        Values =  new List<object> {input.Port1NodeId}
                    }
                   ,new ObjectListRecordFilter
                    {
                        FieldName ="Port2Node",
                        Values =  new List<object> {input.Port2NodeId}
                    }
                   ,new ObjectListRecordFilter
                   {
                        FieldName = "Port1Status",
                        Values = new List<object> { _freeNodeStatus.ToString() }
                   }
                   ,new ObjectListRecordFilter
                   {
                       FieldName = "Port2Status",
                       Values = new List<object> { _freeNodeStatus.ToString() }
                   }
                }
            };

            if (input.Port1PartTypeId.HasValue)
            {
                filter.Filters.Add(new ObjectListRecordFilter
                {
                    FieldName = "Port1PartType",
                    Values = new List<object> { input.Port1PartTypeId.Value }
                });
            }

            if (input.Port2PartTypeId.HasValue)
            {
                filter.Filters.Add(new ObjectListRecordFilter
                {
                    FieldName = "Port2PartType",
                    Values = new List<object> { input.Port2PartTypeId.Value }
                });
            }

            var entities = _genericBusinessEntityManager.GetAllGenericBusinessEntities(_connectionDefinitionId, null, filter);

            if (entities == null || entities.Count() == 0)
                return null;

            var firstItem = entities.First();

            var port1id = (long)firstItem.FieldValues.GetRecord("Port1");

            var port2id = (long)firstItem.FieldValues.GetRecord("Port2");

            var reservedPort1 = ReservePort(port1id);

            var reservedPort2 = ReservePort(port2id);

            return new ReserveConnectionOutput
            {
                Port1 = reservedPort1,
                Port2 = reservedPort2
            };
        }

        public ReservePortOutput ReservePort(ReservePortInput input)
        {

            var reserveStatus = new Guid("c51bb41b-b31a-45ba-b12e-8f521b0323eb");
            var updatedEntity = _genericBusinessEntityManager.UpdateGenericBusinessEntity(new GenericBusinessEntityToUpdate
            {
                BusinessEntityDefinitionId = _nodePortDefinitionId,
                FieldValues = new Dictionary<string, object> { { "Status", reserveStatus } },
                FilterGroup = new RecordFilterGroup
                {
                    Filters = new List<Vanrise.GenericData.Entities.RecordFilter>
                    {
                        new ObjectListRecordFilter
                        {
                              FieldName = "Node",
                              Values = new List<object> { input.NodeId }
                        },
                        new ObjectListRecordFilter
                        {
                              FieldName = "Status",
                              Values = new List<object> { _freeNodeStatus }
                        }
                   }
                }
            });
            if (updatedEntity.Result == Vanrise.Entities.UpdateOperationResult.Failed)
                return null;

            return new ReservePortOutput
            {
                PortId = (long)updatedEntity.UpdatedObject.FieldValues.GetRecord("ID").Value,
                Number = updatedEntity.UpdatedObject.FieldValues.GetRecord("Number").Description
            };

        }
        public ReservePortOutput ReservePort(long portId)
        {
            var reserveStatus = new Guid("c51bb41b-b31a-45ba-b12e-8f521b0323eb");
            var updatedEntity = _genericBusinessEntityManager.UpdateGenericBusinessEntity(new GenericBusinessEntityToUpdate
            {
                BusinessEntityDefinitionId = _nodePortDefinitionId,
                FieldValues = new Dictionary<string, object> { { "Status", reserveStatus } },
                GenericBusinessEntityId = portId,
                FilterGroup =new RecordFilterGroup
                {
                    Filters = new List<Vanrise.GenericData.Entities.RecordFilter>
                    {
                        new ObjectListRecordFilter
                        {
                              FieldName = "Status",
                              Values = new List<object> { _freeNodeStatus }
                        }
                   }
                }
            });

            if (updatedEntity.Result == Vanrise.Entities.UpdateOperationResult.Failed)
                return null;

            return new ReservePortOutput
            {
                PortId = portId,
                Number = updatedEntity.UpdatedObject.FieldValues.GetRecord("Number").Description
            };
        }
    }

}
