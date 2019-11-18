using Retail.NIM.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
namespace Retail.NIM.Business
{
    public class NodeManager
    {
        static Guid s_nodeBEDefinitionId = new Guid("c20d34be-9eda-4dcc-8872-41a415f38468");
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();
        static string s_nodeIdFieldName = "ID";
        static string s_modelIdFieldName = "Model";
        static string s_numberFieldName = "Number";
        static string s_statusIdFieldName = "Status";
        static string s_siteIdFieldName = "Site";
        static string s_nodeTypeIdFieldName = "NodeType";
        static string s_areaIdFieldName = "Area";
        static string s_notesFieldName = "Notes";
        static string s_streetIdFieldName = "Street";
        static string s_buildingFieldName = "Building";
        static string s_buildingSizeIdFieldName = "BuildingSize";
        static string s_blockNumberFieldName = "BlockNumber";
        static string s_cityIdFieldName = "Region";
        static string s_regionIdFieldName = "City";
        static string s_townIdFieldName = "Town";

        #region Internal Methods
        internal Node GetNode(long nodeId, Guid businessEntityDefinitionId)
        {
            var node = _genericBusinessEntityManager.GetGenericBusinessEntity(nodeId, businessEntityDefinitionId);
            if (node == null)
                return null;
            return NodeMapper(node);
        }
        internal Node GetNode(long nodeId)
        {
            return GetNode(nodeId, s_nodeBEDefinitionId);
        }
        internal Node GetNodeByNumber(string number, Guid businessEntityDefinitionId)
        {
            var dpItems = _genericBusinessEntityManager.GetAllGenericBusinessEntities(businessEntityDefinitionId, null, new RecordFilterGroup
            {
                Filters = new List<RecordFilter>
                        {
                            new StringRecordFilter
                            {
                                FieldName =s_numberFieldName,
                                Value = number ,
                                CompareOperator = StringRecordFilterOperator.Equals
                            }
                        }
            });

            if (dpItems == null || dpItems.Count == 0)
                return null;
            return NodeMapper(dpItems.First());
        }
        internal Node GetSwitchBySiteId(long siteId)
        {
            var dpItems = _genericBusinessEntityManager.GetAllGenericBusinessEntities(StaticBEDefinitionIDs.SwitchTypeId, null, new RecordFilterGroup
            {
                Filters = new List<RecordFilter>
                        {
                            new ObjectListRecordFilter
                            {
                                FieldName =s_siteIdFieldName,
                                Values = new List<object>{ siteId } ,
                            }
                        }
            });

            if (dpItems == null || dpItems.Count == 0)
                return null;
            return NodeMapper(dpItems.First());
        }
        internal Node GetDslamBySiteId(long siteId)
        {
            var dpItems = _genericBusinessEntityManager.GetAllGenericBusinessEntities(StaticBEDefinitionIDs.DSLAMTypeId, null, new RecordFilterGroup
            {
                Filters = new List<RecordFilter>
                        {
                            new ObjectListRecordFilter
                            {
                                FieldName =s_siteIdFieldName,
                                Values = new List<object>{ siteId } ,
                            }
                        }
            });

            if (dpItems == null || dpItems.Count == 0)
                return null;
            return NodeMapper(dpItems.First());
        }
        internal Node GetNodeByAddress(Guid businessEntityDefinitionId, long areadId, long siteId, int regionId, int cityId, int townId, long streetId, string buildingDetails)
        {
            var filter = new RecordFilterGroup
            {
                Filters = new List<RecordFilter>
                {
                    new ObjectListRecordFilter
                    {
                        FieldName = s_areaIdFieldName,
                        CompareOperator = ListRecordFilterOperator.In,
                        Values =new List<object>{ areadId }
                    },new ObjectListRecordFilter
                    {
                        FieldName = s_siteIdFieldName,
                        CompareOperator = ListRecordFilterOperator.In,
                        Values =new List<object>{ siteId }
                    },new ObjectListRecordFilter
                    {
                        FieldName = "Region",
                        CompareOperator = ListRecordFilterOperator.In,
                        Values =new List<object>{ regionId }
                    },new ObjectListRecordFilter
                    {
                        FieldName = "City",
                        CompareOperator = ListRecordFilterOperator.In,
                        Values =new List<object>{ cityId }
                    },new ObjectListRecordFilter
                    {
                        FieldName = "Town",
                        CompareOperator = ListRecordFilterOperator.In,
                        Values =new List<object>{ townId }
                    },new ObjectListRecordFilter
                    {
                        FieldName = s_streetIdFieldName,
                        CompareOperator = ListRecordFilterOperator.In,
                        Values =new List<object>{ streetId }
                    }
                }
            };
            if(string.IsNullOrEmpty(buildingDetails))
            {
                filter.Filters.Add(new EmptyRecordFilter
                {
                    FieldName = s_buildingFieldName
                });
            }
            else
            {
                filter.Filters.Add(new StringRecordFilter
                {
                    FieldName = s_buildingFieldName,
                    CompareOperator = StringRecordFilterOperator.Equals,
                    Value = buildingDetails
                });
            }
            var items = _genericBusinessEntityManager.GetAllGenericBusinessEntities(businessEntityDefinitionId, null, filter);
            if (items == null || items.Count == 0)
                return null;
            return NodeMapper(items.First());

        }
        #endregion
        #region Mapper
        Node NodeMapper(GenericBusinessEntity genericBusinessEntity)
        {
            return new Node
            {
                NodeId = (long)genericBusinessEntity.FieldValues.GetRecord(s_nodeIdFieldName),
                ModelId = (int)genericBusinessEntity.FieldValues.GetRecord(s_modelIdFieldName),
                Number = genericBusinessEntity.FieldValues.GetRecord(s_numberFieldName) as string,
                StatusId = (Guid)genericBusinessEntity.FieldValues.GetRecord(s_statusIdFieldName),
                SiteId = (long)genericBusinessEntity.FieldValues.GetRecord(s_siteIdFieldName),
                NodeTypeId = (Guid)genericBusinessEntity.FieldValues.GetRecord(s_nodeTypeIdFieldName),
                AreaId = (long)genericBusinessEntity.FieldValues.GetRecord(s_areaIdFieldName),
                Notes = genericBusinessEntity.FieldValues.GetRecord(s_notesFieldName) as string,
                StreetId = (long)genericBusinessEntity.FieldValues.GetRecord(s_streetIdFieldName),
                Building = genericBusinessEntity.FieldValues.GetRecord(s_buildingFieldName) as string,
                BuildingSizeId = (int?)genericBusinessEntity.FieldValues.GetRecord(s_buildingSizeIdFieldName),
                BlockNumber = genericBusinessEntity.FieldValues.GetRecord(s_blockNumberFieldName) as string,
                CityId = (int)genericBusinessEntity.FieldValues.GetRecord(s_cityIdFieldName),
                RegionId = (int)genericBusinessEntity.FieldValues.GetRecord(s_regionIdFieldName),
                TownId = (int)genericBusinessEntity.FieldValues.GetRecord(s_townIdFieldName),

            };
        }
        #endregion

    }
}
