using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.DataRecordFieldFormulas
{
    public class GatewayEvaluatorFieldFormula : DataRecordFieldFormula
    {
        public override Guid ConfigId { get { return new Guid("395B97B4-E304-4040-A53D-16A145B4C42E"); } }

        public string SwitchFieldName { get; set; }

        public string PortFieldName { get; set; }


        public override List<string> GetDependentFields(IDataRecordFieldFormulaGetDependentFieldsContext context)
        {
            return new List<string>() { PortFieldName, SwitchFieldName };
        }

        public override dynamic CalculateValue(IDataRecordFieldFormulaCalculateValueContext context)
        {
            dynamic port = context.GetFieldValue(this.PortFieldName);
            dynamic switchId = context.GetFieldValue(this.SwitchFieldName);

            if (!String.IsNullOrEmpty(port))
            {
                var switchConnectivity = (new TOne.WhS.BusinessEntity.Business.SwitchConnectivityManager()).GetMatchConnectivity(switchId, port);
                if (switchConnectivity != null)
                    return switchConnectivity.SwitchConnectivityId;
            }
            return null;
        }

        public override RecordFilter ConvertFilter(IDataRecordFieldFormulaConvertFilterContext context)
        {
            if (context.InitialFilter == null)
                throw new ArgumentNullException("context.InitialFilter");

            SwitchConnectivityManager switchConnectivityManager = new SwitchConnectivityManager();

            #region ObjectListRecordFilter

            ObjectListRecordFilter objectListFilter = context.InitialFilter as ObjectListRecordFilter;
            if (objectListFilter != null)
            {
                Dictionary<int, List<string>> portsBySwitchId = new Dictionary<int, List<string>>();
                List<string> ports;

                foreach (var switchConnectivityId in objectListFilter.Values)
                {
                    SwitchConnectivity switchConnectivity = switchConnectivityManager.GetSwitchConnectivity(Convert.ToInt32(switchConnectivityId));
                    if (switchConnectivity != null)
                    {
                        ports = portsBySwitchId.GetOrCreateItem(switchConnectivity.SwitchId);
                        if (switchConnectivity.Settings != null && switchConnectivity.Settings.Trunks != null)
                            ports.AddRange(switchConnectivity.Settings.Trunks.Select(itm => itm.Name));
                    }
                }

                RecordFilter recordFilter = BuildRecordFilter(portsBySwitchId, objectListFilter.CompareOperator);
                if (recordFilter == null)
                    return new AlwaysFalseRecordFilter();

                return recordFilter;
            }

            #endregion

            #region EmptyRecordFilter

            EmptyRecordFilter emptyFilter = context.InitialFilter as EmptyRecordFilter;
            if (emptyFilter != null)
            {
                Dictionary<SwitchPortKey, SwitchConnectivity> switchConnectivitiesBySwitchPortKey = switchConnectivityManager.GetSwitchConnectivitiesBySwitchPortKey();
                if (switchConnectivitiesBySwitchPortKey == null || switchConnectivitiesBySwitchPortKey.Count == 0)
                    return null;

                Dictionary<int, List<string>> portsBySwitchId = new Dictionary<int, List<string>>();
                List<string> ports;

                foreach (SwitchPortKey switchPortKey in switchConnectivitiesBySwitchPortKey.Keys)
                {
                    ports = portsBySwitchId.GetOrCreateItem(switchPortKey.SwitchId);
                    ports.Add(switchPortKey.Port);
                }

                RecordFilter recordFilter = BuildRecordFilter(portsBySwitchId, ListRecordFilterOperator.NotIn);
                if (recordFilter == null)
                    return null;

                RecordFilterGroup recordFilterGroup = new RecordFilterGroup();
                recordFilterGroup.LogicalOperator = RecordQueryLogicalOperator.Or;
                recordFilterGroup.Filters = new List<RecordFilter>() { recordFilter, new EmptyRecordFilter() { FieldName = this.PortFieldName } };
                return recordFilterGroup;
            }

            #endregion

            #region NonEmptyRecordFilter

            NonEmptyRecordFilter nonEmptyFilter = context.InitialFilter as NonEmptyRecordFilter;
            if (nonEmptyFilter != null)
            {
                Dictionary<SwitchPortKey, SwitchConnectivity> switchConnectivitiesBySwitchPortKey = switchConnectivityManager.GetSwitchConnectivitiesBySwitchPortKey();
                if (switchConnectivitiesBySwitchPortKey == null || switchConnectivitiesBySwitchPortKey.Count == 0)
                    return new AlwaysFalseRecordFilter();

                Dictionary<int, List<string>> portsBySwitchId = new Dictionary<int, List<string>>();
                List<string> ports;

                foreach (var switchConnectivity in switchConnectivitiesBySwitchPortKey.Values)
                {
                    ports = portsBySwitchId.GetOrCreateItem(switchConnectivity.SwitchId);
                    if (switchConnectivity.Settings != null && switchConnectivity.Settings.Trunks != null)
                        ports.AddRange(switchConnectivity.Settings.Trunks.Select(itm => itm.Name));
                }

                RecordFilter recordFilter = BuildRecordFilter(portsBySwitchId, ListRecordFilterOperator.In);
                if (recordFilter == null)
                    return new AlwaysFalseRecordFilter();

                return recordFilter;
            }

            #endregion

            throw new Exception(String.Format("Invalid Record Filter '{0}'", context.InitialFilter.GetType()));
        }

        private RecordFilter BuildRecordFilter(Dictionary<int, List<string>> portsBySwitchId, ListRecordFilterOperator compareOperator)
        {
            if (portsBySwitchId == null || portsBySwitchId.Count == 0)
                return null;

            List<RecordFilter> recordFilterGroups = new List<RecordFilter>();

            NumberRecordFilterOperator switchRecordFilterOperator;
            RecordQueryLogicalOperator switchPortQueryLogicalOperator;
            RecordQueryLogicalOperator resultQueryLogicalOperator;

            switch (compareOperator)
            {
                case ListRecordFilterOperator.In:
                    switchRecordFilterOperator = NumberRecordFilterOperator.Equals;
                    switchPortQueryLogicalOperator = RecordQueryLogicalOperator.And;
                    resultQueryLogicalOperator = RecordQueryLogicalOperator.Or;
                    break;

                case ListRecordFilterOperator.NotIn:
                    switchRecordFilterOperator = NumberRecordFilterOperator.NotEquals;
                    switchPortQueryLogicalOperator = RecordQueryLogicalOperator.Or;
                    resultQueryLogicalOperator = RecordQueryLogicalOperator.And;
                    break;

                default: throw new NotSupportedException(string.Format("ListRecordFilte rOperator '{0}'", compareOperator));
            }

            foreach (var kvp in portsBySwitchId)
            {
                if (kvp.Value.Count == 0)
                    continue;

                NumberRecordFilter numberRecordFilter = new NumberRecordFilter();
                numberRecordFilter.FieldName = this.SwitchFieldName;
                numberRecordFilter.CompareOperator = switchRecordFilterOperator;
                numberRecordFilter.Value = kvp.Key;

                StringListRecordFilter stringListRecordFilter = new StringListRecordFilter();
                stringListRecordFilter.FieldName = this.PortFieldName;
                stringListRecordFilter.CompareOperator = compareOperator;
                stringListRecordFilter.Values = kvp.Value;

                recordFilterGroups.Add(new RecordFilterGroup()
                {
                    LogicalOperator = switchPortQueryLogicalOperator,
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
