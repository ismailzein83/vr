using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace CP.WhS.Business
{
    public class BIDsASKsManager
    {
        public static Guid _bidsDefinitionId = new Guid("0dd77d50-c46b-4bf4-a1d6-be090f91a1df");
        public static Guid _asksDefinitionId = new Guid("fe9167c7-a7fb-47e4-bc45-843403e96968");
        #region Public Methods

        public IDataRetrievalResult<GenericBusinessEntityDetail> GetFilteredBIDs(DataRetrievalInput<Entities.BIDsQuery> input)
        {
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();

            var filterGroup = new RecordFilterGroup
            {
                LogicalOperator = RecordQueryLogicalOperator.And,
                Filters = new List<RecordFilter>
                        {
                            new ObjectListRecordFilter
                            {
                                CompareOperator = ListRecordFilterOperator.NotIn,
                               // Values = new List<object>{null},
                                FieldName= "CarrierAccountId"
                            },
                            new ObjectListRecordFilter
                            {
                                CompareOperator = ListRecordFilterOperator.In,
                                Values = new List<object>{ input.Query.Destination },
                                FieldName= "Destination"
                            }
                        },
            };

            if (input.Query.CLIORNCLI.HasValue)
                filterGroup.Filters.Add(new ObjectListRecordFilter { FieldName = "CLIORNCLI", Values = new List<object> { input.Query.CLIORNCLI.Value } });

            return genericBusinessEntityManager.GetFilteredGenericBusinessEntities(new DataRetrievalInput<GenericBusinessEntityQuery>
            {
                Query = new GenericBusinessEntityQuery
                {
                    BusinessEntityDefinitionId = _bidsDefinitionId,
                    OrderType = OrderType.AdvancedFieldOrder,
                    AdvancedOrderOptions = new AdvancedFieldOrderOptions
                    {
                        Fields = new List<AdvancedFieldOrderItem>
                        {
                            new AdvancedFieldOrderItem { FieldName = "Rate",OrderDirection= OrderDirection.Ascending},
                            new AdvancedFieldOrderItem { FieldName = "Attempts",OrderDirection= OrderDirection.Ascending},
                            new AdvancedFieldOrderItem { FieldName = "Duration",OrderDirection= OrderDirection.Ascending}
                        }
                    },
                    FilterGroup = filterGroup,
                }
            });
        }

        public IDataRetrievalResult<GenericBusinessEntityDetail> GetFilteredASKs(DataRetrievalInput<Entities.ASKsQuery> input)
        {
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();


            var filterGroup = new RecordFilterGroup
            {
                LogicalOperator = RecordQueryLogicalOperator.And,
                Filters = new List<RecordFilter>
                        {
                            new ObjectListRecordFilter
                            {
                                CompareOperator = ListRecordFilterOperator.NotIn,
                             //   Values = new List<object>{ null },
                                FieldName= "CarrierAccountId"
                            },
                            new ObjectListRecordFilter
                            {
                                CompareOperator = ListRecordFilterOperator.In,
                                Values = new List<object>{ input.Query.Destination },
                                FieldName= "Destination"
                            }
                        }
            };

            if (input.Query.CLIORNCLI.HasValue)
                filterGroup.Filters.Add(new ObjectListRecordFilter { FieldName = "CLIORNCLI", Values = new List<object> { input.Query.CLIORNCLI.Value } });

            return genericBusinessEntityManager.GetFilteredGenericBusinessEntities(new DataRetrievalInput<GenericBusinessEntityQuery>
            {
                Query = new GenericBusinessEntityQuery
                {
                    BusinessEntityDefinitionId = _asksDefinitionId,
                    OrderType = OrderType.AdvancedFieldOrder,
                    AdvancedOrderOptions = new AdvancedFieldOrderOptions
                    {
                        Fields = new List<AdvancedFieldOrderItem> {
                            new AdvancedFieldOrderItem { FieldName = "SellingRate", OrderDirection = OrderDirection.Ascending },
                             new AdvancedFieldOrderItem { FieldName = "Attempts", OrderDirection = OrderDirection.Ascending },
                        new AdvancedFieldOrderItem { FieldName = "Duration", OrderDirection = OrderDirection.Ascending }
                        },
                       
                    },
                    FilterGroup = filterGroup
                }
            });
        }
        #endregion


    }
}