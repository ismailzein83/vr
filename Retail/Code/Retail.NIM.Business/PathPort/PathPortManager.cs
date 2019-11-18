using Retail.NIM.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
namespace Retail.NIM.Business
{
    public class PathPortManager
    {
        static Guid s_pathPortBEDefinitionId = new Guid("890b8fbd-6556-4037-8d97-c2421b1b7095");
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();

        static string s_pathPortIdFieldName = "ID";
        static string s_portIdFieldName = "Port";
        static string s_portNodeIdFieldName = "Node";
        static string s_portNodeTypeIdFieldName = "NodeType";
        static string s_portNodePartIdFieldName = "NodePart";
        static string s_portNodePartTypeIdFieldName = "NodePartType";
        static string s_pathIdFieldName = "Path";

        #region Public Methods
        public PathPortOutput AddPortToPath(PathPortInput input)
        {
            var insertedEntity = _genericBusinessEntityManager.AddGenericBusinessEntity(new GenericBusinessEntityToAdd
            {
                BusinessEntityDefinitionId = s_pathPortBEDefinitionId,
                FieldValues = new Dictionary<string, object> { { s_pathIdFieldName, input.PathId }, { s_portIdFieldName, input.PortId } }
            });


            if (insertedEntity.Result == InsertOperationResult.Failed)
                return null;

            return new PathPortOutput
            {
                PathPortId = (long)insertedEntity.InsertedObject.FieldValues.GetRecord(s_pathPortIdFieldName).Value
            };
        }
        public void RemovePathPort(RemovePathPortInput input)
        {
            _genericBusinessEntityManager.DeleteGenericBusinessEntity(new DeleteGenericBusinessEntityInput
            {
                BusinessEntityDefinitionId = s_pathPortBEDefinitionId,
                GenericBusinessEntityIds = new List<object>() { input.PathPortId },

            });
        }
        public List<PathPort> GetPathPorts(long pathId)
        {
            var filter = new RecordFilterGroup
            {
                Filters = new List<RecordFilter>
                {
                    new RecordFilterGroup
                    {
                        LogicalOperator = RecordQueryLogicalOperator.Or,
                        Filters = new List<RecordFilter>
                        {
                             new ObjectListRecordFilter
                             {
                                 FieldName = s_pathIdFieldName,
                                 CompareOperator = ListRecordFilterOperator.In,
                                 Values =new List<object>{ pathId }
                             }
                        }
                    }
                },

            };
            var items = _genericBusinessEntityManager.GetAllGenericBusinessEntities(s_pathPortBEDefinitionId, null, filter);
            if (items == null || items.Count == 0)
                return null;
            return items.MapRecords(PathPortMapper).ToList();
        }

        #endregion

        #region Mapper
        PathPort PathPortMapper(GenericBusinessEntity genericBusinessEntity)
        {
            return new PathPort
            {
                PathPortId = (long)genericBusinessEntity.FieldValues.GetRecord(s_pathPortIdFieldName),
                PortId = (long)genericBusinessEntity.FieldValues.GetRecord(s_portIdFieldName),
                PathId = (long)genericBusinessEntity.FieldValues.GetRecord(s_pathIdFieldName),
                PortNodeId = (long)genericBusinessEntity.FieldValues.GetRecord(s_portNodeIdFieldName),
                PortNodeTypeId = (Guid)genericBusinessEntity.FieldValues.GetRecord(s_portNodeTypeIdFieldName),
                PortNodePartId = (long?)genericBusinessEntity.FieldValues.GetRecord(s_portNodePartIdFieldName),
                PortNodePartTypeId = (Guid?)genericBusinessEntity.FieldValues.GetRecord(s_portNodePartTypeIdFieldName),

            };
        }
        #endregion
    }
}
