using Retail.NIM.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Retail.NIM.Business
{
    public class NodeManager
    {
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();

        public GenericBusinessEntity GetNode(long nodeId, Guid businessEntityDefinitionId)
        {
            return _genericBusinessEntityManager.GetGenericBusinessEntity(nodeId, businessEntityDefinitionId);
        }
        public GenericBusinessEntity GetNode(long nodeId)
        {
            return _genericBusinessEntityManager.GetGenericBusinessEntity(nodeId, StaticBEDefinitionIDs.NodeBEDefinitionId);
        }
        public GenericBusinessEntity GetNodeByNumber(string number, Guid businessEntityDefinitionId)
        {
            var dpItems = _genericBusinessEntityManager.GetAllGenericBusinessEntities(businessEntityDefinitionId, null, new RecordFilterGroup
            {
                Filters = new List<RecordFilter>
                        {
                            new StringRecordFilter
                            {
                                FieldName ="Number",
                                Value = number ,
                                CompareOperator = StringRecordFilterOperator.Equals
                            }
                        }
            });

            if (dpItems == null || dpItems.Count == 0)
                return null;
            return dpItems.First();
        }

        public GenericBusinessEntity GetSwitchBySiteId(long siteId)
        {
            var dpItems = _genericBusinessEntityManager.GetAllGenericBusinessEntities(StaticBEDefinitionIDs.SwitchTypeId, null, new RecordFilterGroup
            {
                Filters = new List<RecordFilter>
                        {
                            new NumberRecordFilter
                            {
                                FieldName ="Site",
                                Value = siteId,
                                CompareOperator = NumberRecordFilterOperator.Equals
                            }
                        }
            });

            if (dpItems == null || dpItems.Count == 0)
                return null;
            return dpItems.First();
        }
        public GenericBusinessEntity GetDslamBySiteId(long siteId)
        {
            var dpItems = _genericBusinessEntityManager.GetAllGenericBusinessEntities(StaticBEDefinitionIDs.DSLAMTypeId, null, new RecordFilterGroup
            {
                Filters = new List<RecordFilter>
                        {
                            new NumberRecordFilter
                            {
                                FieldName ="Site",
                                Value = siteId ,
                                CompareOperator = NumberRecordFilterOperator.Equals
                            }
                        }
            });

            if (dpItems == null || dpItems.Count == 0)
                return null;
            return dpItems.First();
        }
        public GenericBusinessEntity GetNodeByAddress(Guid businessEntityDefinitionId, long areadId, long siteId, int regionId, int cityId, int townId, long streetId)
        {
            var filter = new RecordFilterGroup
            {
                Filters = new List<RecordFilter>
                {
                    new ObjectListRecordFilter
                    {
                        FieldName = "Area",
                        CompareOperator = ListRecordFilterOperator.In,
                        Values =new List<object>{ areadId }
                    },new ObjectListRecordFilter
                    {
                        FieldName = "Site",
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
                        FieldName = "Street",
                        CompareOperator = ListRecordFilterOperator.In,
                        Values =new List<object>{ streetId }
                    }
                }
            };
            var items = _genericBusinessEntityManager.GetAllGenericBusinessEntities(businessEntityDefinitionId, null, filter);
            if (items == null || items.Count == 0)
                return null;
            return items.First();

        }
    }
}
