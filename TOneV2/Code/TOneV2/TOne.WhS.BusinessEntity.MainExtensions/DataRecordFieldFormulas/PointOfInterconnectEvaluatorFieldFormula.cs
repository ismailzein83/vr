using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.DataRecordFieldFormulas
{
    public class PointOfInterconnectEvaluatorFieldFormula : DataRecordFieldFormula
    {
        public override Guid ConfigId { get { return new Guid("33FBAC65-A393-4038-8841-EB02699B9CDE"); } }

        public string SwitchFieldName { get; set; }

        public string TrunkFieldName { get; set; }


        public override List<string> GetDependentFields(IDataRecordFieldFormulaGetDependentFieldsContext context)
        {
            return new List<string>() { TrunkFieldName, SwitchFieldName };
        }

        public override dynamic CalculateValue(IDataRecordFieldFormulaCalculateValueContext context)
        {
            dynamic trunk = context.GetFieldValue(this.TrunkFieldName);
            dynamic switchId = context.GetFieldValue(this.SwitchFieldName);

            if (!String.IsNullOrEmpty(trunk))
                return new PointOfInterconnectManager().GetPointOfInterconnect(switchId, trunk);

            return null;
        }

        public override RecordFilter ConvertFilter(IDataRecordFieldFormulaConvertFilterContext context)
        {
            if (context.InitialFilter == null)
                throw new ArgumentNullException("context.InitialFilter");

            PointOfInterconnectManager pointOfInterconnectManager = new PointOfInterconnectManager();

            #region ObjectListRecordFilter

            ObjectListRecordFilter objectListFilter = context.InitialFilter as ObjectListRecordFilter;
            if (objectListFilter != null)
            {
                Dictionary<int, List<string>> trunksBySwitchId = new Dictionary<int, List<string>>();
                List<string> trunks;

                foreach (var pointOfInterconnectEntityId in objectListFilter.Values)
                {
                    PointOfInterconnectEntity pointOfInterconnectEntity = pointOfInterconnectManager.GetPointOfInterconnect(Convert.ToInt64(pointOfInterconnectEntityId));
                    if (pointOfInterconnectEntity != null)
                    {
                        trunks = trunksBySwitchId.GetOrCreateItem(pointOfInterconnectEntity.SwitchId);
                        if (pointOfInterconnectEntity.Settings != null && pointOfInterconnectEntity.Settings.Trunks != null)
                            trunks.AddRange(pointOfInterconnectEntity.Settings.Trunks.Select(itm => itm.Trunk));
                    }
                }

                RecordFilter recordFilter = BuildRecordFilter(trunksBySwitchId, objectListFilter.CompareOperator);
                if (recordFilter == null)
                    return new AlwaysFalseRecordFilter();

                return recordFilter;
            }

            #endregion

            #region EmptyRecordFilter

            EmptyRecordFilter emptyFilter = context.InitialFilter as EmptyRecordFilter;
            if (emptyFilter != null)
            {
                IEnumerable<PointOfInterconnectEntity> pointOfInterconnectEntities = pointOfInterconnectManager.GetPointOfInterconnects();
                if (pointOfInterconnectEntities == null || pointOfInterconnectEntities.Count() == 0)
                    return null;

                Dictionary<int, List<string>> trunksBySwitchId = new Dictionary<int, List<string>>();
                List<string> trunks;

                foreach (PointOfInterconnectEntity pointOfInterconnectEntity in pointOfInterconnectEntities)
                {
                    trunks = trunksBySwitchId.GetOrCreateItem(pointOfInterconnectEntity.SwitchId);
                    if (pointOfInterconnectEntity.Settings != null && pointOfInterconnectEntity.Settings.Trunks != null)
                        trunks.AddRange(pointOfInterconnectEntity.Settings.Trunks.Select(itm => itm.Trunk));
                }

                RecordFilter recordFilter = BuildRecordFilter(trunksBySwitchId, ListRecordFilterOperator.NotIn);
                if (recordFilter == null)
                    return null;

                RecordFilterGroup recordFilterGroup = new RecordFilterGroup();
                recordFilterGroup.LogicalOperator = RecordQueryLogicalOperator.Or;
                recordFilterGroup.Filters = new List<RecordFilter>() { recordFilter, new EmptyRecordFilter() { FieldName = this.TrunkFieldName } };
                return recordFilterGroup;
            }

            #endregion

            #region NonEmptyRecordFilter

            NonEmptyRecordFilter nonEmptyFilter = context.InitialFilter as NonEmptyRecordFilter;
            if (nonEmptyFilter != null)
            {
                IEnumerable<PointOfInterconnectEntity> pointOfInterconnectEntities = pointOfInterconnectManager.GetPointOfInterconnects();
                if (pointOfInterconnectEntities == null || pointOfInterconnectEntities.Count() == 0)
                    return new AlwaysFalseRecordFilter();

                Dictionary<int, List<string>> trunksBySwitchId = new Dictionary<int, List<string>>();
                List<string> trunks;

                foreach (PointOfInterconnectEntity pointOfInterconnectEntity in pointOfInterconnectEntities)
                {
                    trunks = trunksBySwitchId.GetOrCreateItem(pointOfInterconnectEntity.SwitchId);
                    if (pointOfInterconnectEntity.Settings != null && pointOfInterconnectEntity.Settings.Trunks != null)
                        trunks.AddRange(pointOfInterconnectEntity.Settings.Trunks.Select(itm => itm.Trunk));
                }

                RecordFilter recordFilter = BuildRecordFilter(trunksBySwitchId, ListRecordFilterOperator.In);
                if (recordFilter == null)
                    return new AlwaysFalseRecordFilter();

                return recordFilter;
            }

            #endregion

            throw new Exception(String.Format("Invalid Record Filter '{0}'", context.InitialFilter.GetType()));
        }

        private RecordFilter BuildRecordFilter(Dictionary<int, List<string>> trunksBySwitchId, ListRecordFilterOperator compareOperator)
        {
            if (trunksBySwitchId == null || trunksBySwitchId.Count == 0)
                return null;

            List<RecordFilter> recordFilterGroups = new List<RecordFilter>();

            NumberRecordFilterOperator switchRecordFilterOperator;
            RecordQueryLogicalOperator switchTrunkQueryLogicalOperator;
            RecordQueryLogicalOperator resultQueryLogicalOperator;

            switch (compareOperator)
            {
                case ListRecordFilterOperator.In:
                    switchRecordFilterOperator = NumberRecordFilterOperator.Equals;
                    switchTrunkQueryLogicalOperator = RecordQueryLogicalOperator.And;
                    resultQueryLogicalOperator = RecordQueryLogicalOperator.Or;
                    break;

                case ListRecordFilterOperator.NotIn:
                    switchRecordFilterOperator = NumberRecordFilterOperator.NotEquals;
                    switchTrunkQueryLogicalOperator = RecordQueryLogicalOperator.Or;
                    resultQueryLogicalOperator = RecordQueryLogicalOperator.And;
                    break;

                default: throw new NotSupportedException(string.Format("ListRecordFilte rOperator '{0}'", compareOperator));
            }

            foreach (var kvp in trunksBySwitchId)
            {
                if (kvp.Value.Count == 0)
                    continue;

                List<string> trunks = kvp.Value;

                NumberRecordFilter numberRecordFilter = new NumberRecordFilter();
                numberRecordFilter.FieldName = this.SwitchFieldName;
                numberRecordFilter.CompareOperator = switchRecordFilterOperator;
                numberRecordFilter.Value = kvp.Key;

                StringListRecordFilter stringListRecordFilter = new StringListRecordFilter();
                stringListRecordFilter.FieldName = this.TrunkFieldName;
                stringListRecordFilter.CompareOperator = compareOperator;
                stringListRecordFilter.Values = trunks;

                recordFilterGroups.Add(new RecordFilterGroup()
                {
                    LogicalOperator = switchTrunkQueryLogicalOperator,
                    Filters = new List<RecordFilter>() { numberRecordFilter, stringListRecordFilter }
                });
            }

            if (recordFilterGroups.Count == 0)
            {
                return null;
            }
            else if (recordFilterGroups.Count == 1)
            {
                return recordFilterGroups.First();
            }
            else
            {
                return new RecordFilterGroup()
                {
                    LogicalOperator = resultQueryLogicalOperator,
                    Filters = recordFilterGroups.Select(itm => itm as RecordFilter).ToList()
                };
            }
        }
    }
}