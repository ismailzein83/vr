using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.DataRecordFieldFormulas
{
    public class GatewayEvaluatorFieldFormula : DataRecordFieldFormula
    {
        public override Guid ConfigId { get { return new Guid("395B97B4-E304-4040-A53D-16A145B4C42E"); } }

        public string PortFieldName { get; set; }


        public override List<string> GetDependentFields(IDataRecordFieldFormulaGetDependentFieldsContext context)
        {
            return new List<string>() { PortFieldName };
        }

        public override dynamic CalculateValue(IDataRecordFieldFormulaCalculateValueContext context)
        {
            dynamic port = context.GetFieldValue(this.PortFieldName);
            if (!String.IsNullOrEmpty(port))
            {
                var switchConnectivity = (new TOne.WhS.BusinessEntity.Business.SwitchConnectivityManager()).GetMatchConnectivity(port);
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
                List<string> portsToFilter = new List<string>();

                foreach (var switchConnectivityId in objectListFilter.Values)
                {
                    SwitchConnectivity switchConnectivity = switchConnectivityManager.GetSwitchConnectivity(Convert.ToInt32(switchConnectivityId));
                    if (switchConnectivity != null && switchConnectivity.Settings != null && switchConnectivity.Settings.Trunks != null)
                        portsToFilter.AddRange(switchConnectivity.Settings.Trunks.Select(itm => itm.Name));
                }

                if (portsToFilter.Count == 0)
                    return new AlwaysFalseRecordFilter();

                StringListRecordFilter stringListRecordFilter = new StringListRecordFilter();
                stringListRecordFilter.FieldName = this.PortFieldName;
                stringListRecordFilter.CompareOperator = objectListFilter.CompareOperator;
                stringListRecordFilter.Values = portsToFilter;
                return stringListRecordFilter;
            }

            EmptyRecordFilter emptyFilter = context.InitialFilter as EmptyRecordFilter;
            if (emptyFilter != null)
            {
                Dictionary<string, SwitchConnectivity> switchConnectivitiesByPort = switchConnectivityManager.GetSwitchConnectivitiesByPort();
                if (switchConnectivitiesByPort == null || switchConnectivitiesByPort.Count == 0)
                    return null;

                StringListRecordFilter stringListRecordFilter = new StringListRecordFilter();
                stringListRecordFilter.FieldName = this.PortFieldName;
                stringListRecordFilter.CompareOperator = ListRecordFilterOperator.NotIn;
                stringListRecordFilter.Values = switchConnectivitiesByPort.Keys.ToList();

                return new RecordFilterGroup()
                {
                    LogicalOperator = RecordQueryLogicalOperator.Or,
                    Filters = new List<RecordFilter>() { stringListRecordFilter, new EmptyRecordFilter() { FieldName = this.PortFieldName } }
                };
            }

            NonEmptyRecordFilter nonEmptyFilter = context.InitialFilter as NonEmptyRecordFilter;
            if (nonEmptyFilter != null)
            {
                Dictionary<string, SwitchConnectivity> switchConnectivitiesByPort = switchConnectivityManager.GetSwitchConnectivitiesByPort();
                if (switchConnectivitiesByPort == null || switchConnectivitiesByPort.Count == 0)
                    return new AlwaysFalseRecordFilter();

                StringListRecordFilter stringListRecordFilter = new StringListRecordFilter();
                stringListRecordFilter.FieldName = this.PortFieldName;
                stringListRecordFilter.CompareOperator = ListRecordFilterOperator.In;
                stringListRecordFilter.Values = switchConnectivitiesByPort.Keys.ToList();
                return stringListRecordFilter;
            }

            throw new Exception(String.Format("Invalid Record Filter '{0}'", context.InitialFilter.GetType()));
        }
    }
}
