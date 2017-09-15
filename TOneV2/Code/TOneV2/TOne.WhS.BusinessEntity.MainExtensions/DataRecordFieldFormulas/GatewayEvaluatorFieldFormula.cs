using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

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

                List<RecordFilterGroup> recordFilterGroups = BuildRecordFilterGroups(portsBySwitchId, objectListFilter.CompareOperator);
                if (recordFilterGroups == null)
                    return new AlwaysFalseRecordFilter();

                if (recordFilterGroups.Count == 1)
                {
                    return recordFilterGroups.First();
                }
                else
                {
                    return new RecordFilterGroup()
                    {
                        LogicalOperator = RecordQueryLogicalOperator.Or,
                        Filters = recordFilterGroups.Select(itm => itm as RecordFilter).ToList()
                    };
                }
            }

            EmptyRecordFilter emptyFilter = context.InitialFilter as EmptyRecordFilter;
            if (emptyFilter != null)
            {
                Dictionary<SwitchPortKey, SwitchConnectivity> switchConnectivitiesBySwitchPortKey = switchConnectivityManager.GetSwitchConnectivitiesBySwitchPortKey();
                if (switchConnectivitiesBySwitchPortKey == null || switchConnectivitiesBySwitchPortKey.Count == 0)
                    return null;

                Dictionary<int, List<string>> portsBySwitchId = new Dictionary<int, List<string>>();
                List<string> ports;

                foreach (var switchConnectivity in switchConnectivitiesBySwitchPortKey.Values)
                {
                    ports = portsBySwitchId.GetOrCreateItem(switchConnectivity.SwitchId);
                    if (switchConnectivity.Settings != null && switchConnectivity.Settings.Trunks != null)
                        ports.AddRange(switchConnectivity.Settings.Trunks.Select(itm => itm.Name));
                }

                List<RecordFilterGroup> recordFilterGroups = BuildRecordFilterGroups(portsBySwitchId, ListRecordFilterOperator.NotIn);
                if (recordFilterGroups == null)
                    return null;

                RecordFilterGroup recordFilterGroup = null;

                if (recordFilterGroups.Count == 1)
                {
                    recordFilterGroup = recordFilterGroups.First();
                }
                else
                {
                    recordFilterGroup = new RecordFilterGroup()
                    {
                        LogicalOperator = RecordQueryLogicalOperator.Or,
                        Filters = recordFilterGroups.Select(itm => itm as RecordFilter).ToList()
                    };
                }

                return new RecordFilterGroup()
                {
                    LogicalOperator = RecordQueryLogicalOperator.Or,
                    Filters = new List<RecordFilter>() { recordFilterGroup, new EmptyRecordFilter() { FieldName = this.PortFieldName } }
                };
            }

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

                List<RecordFilterGroup> recordFilterGroups = BuildRecordFilterGroups(portsBySwitchId, ListRecordFilterOperator.In);
                if (recordFilterGroups == null)
                {
                    return new AlwaysFalseRecordFilter();
                }
                else if (recordFilterGroups.Count == 1)
                {
                    return recordFilterGroups.First();
                }
                else
                {
                    return new RecordFilterGroup()
                    {
                        LogicalOperator = RecordQueryLogicalOperator.Or,
                        Filters = recordFilterGroups.Select(itm => itm as RecordFilter).ToList()
                    };
                }
            }

            throw new Exception(String.Format("Invalid Record Filter '{0}'", context.InitialFilter.GetType()));
        }

        private List<RecordFilterGroup> BuildRecordFilterGroups(Dictionary<int, List<string>> portsBySwitchId, ListRecordFilterOperator compareOperator)
        {
            if (portsBySwitchId == null || portsBySwitchId.Count == 0)
                return null;

            List<RecordFilterGroup> recordFilterGroups = new List<RecordFilterGroup>();

            foreach (var kvp in portsBySwitchId)
            {
                if (kvp.Value.Count == 0)
                    continue;

                NumberRecordFilter numberRecordFilter = new NumberRecordFilter();
                numberRecordFilter.FieldName = this.SwitchFieldName;
                numberRecordFilter.CompareOperator = NumberRecordFilterOperator.Equals;
                numberRecordFilter.Value = kvp.Key;

                StringListRecordFilter stringListRecordFilter = new StringListRecordFilter();
                stringListRecordFilter.FieldName = this.PortFieldName;
                stringListRecordFilter.CompareOperator = compareOperator;
                stringListRecordFilter.Values = kvp.Value;

                recordFilterGroups.Add(new RecordFilterGroup()
                {
                    LogicalOperator = RecordQueryLogicalOperator.And,
                    Filters = new List<RecordFilter>() { numberRecordFilter, stringListRecordFilter }
                });
            }

            return recordFilterGroups.Count > 0 ? recordFilterGroups : null;
        }
    }
}
