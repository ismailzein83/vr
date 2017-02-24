using PartnerPortal.Invoice.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Invoice.Entities;
namespace PartnerPortal.Invoice.Business
{
    public class InvoiceViewerTypeManager
    {
        Vanrise.Common.Business.VRComponentTypeManager _vrComponentTypeManager = new Vanrise.Common.Business.VRComponentTypeManager();
        public IEnumerable<InvoiceViewerTypeInfo> GetInvoiceViewerTypeInfo(InvoiceViewerTypeInfoFilter filter)
        {
            Func<InvoiceViewerType, bool> filterExpression = (invoiceViewerType) =>
            {
                if (filter != null)
                {
                    if (filter.Filters != null)
                    {
                        var invoiceViewerTypeInfoFilterContext = new InvoiceViewerTypeInfoFilterContext { InvoiceViewerTypeId = invoiceViewerType.VRComponentTypeId };
                        foreach (IInvoiceViewerTypeInfoFilter invoiceViewerTypeInfoFilter in filter.Filters)
                        {
                            if (invoiceViewerTypeInfoFilter.IsExcluded(invoiceViewerTypeInfoFilterContext))
                                return false;
                        }
                    }
                }
                return true;
            };

            return _vrComponentTypeManager.GetComponentTypes<InvoiceViewerTypeSettings, InvoiceViewerType>().MapRecords(InvoiceViewerTypeInfoMapper, filterExpression);
        }
        public InvoiceViewerType GetInvoiceViewerType(Guid invoiceViewerTypeId)
        {
            return _vrComponentTypeManager.GetComponentType<InvoiceViewerTypeSettings,InvoiceViewerType>(invoiceViewerTypeId);
        }
        public InvoiceViewerTypeRuntime GetInvoiceViewerTypeRuntime(Guid invoiceViewerTypeId)
        {
            InvoiceViewerType invoiceViewerType = _vrComponentTypeManager.GetComponentType<InvoiceViewerTypeSettings, InvoiceViewerType>(invoiceViewerTypeId);
            InvoiceTypeManager invoiceTypeManager = new Business.InvoiceTypeManager();
            IEnumerable<InvoiceUIGridColumnRunTime> invoiceUIGridColumnRunTime = invoiceTypeManager.GetInvoiceTypeGridColumns(invoiceViewerType.Settings.VRConnectionId, invoiceViewerType.Settings.InvoiceTypeId);
            InvoiceViewerTypeRuntime invoiceViewerTypeRuntime = null;


            if (invoiceUIGridColumnRunTime != null && invoiceViewerType.Settings.GridSettings.InvoiceGridFields != null)
            {

                var fields = invoiceTypeManager.GetRemoteInvoiceFieldsInfo(invoiceViewerType.Settings.VRConnectionId, null);
                invoiceViewerTypeRuntime = new InvoiceViewerTypeRuntime
                {
                    RuntimeGridColumns = new List<RuntimeGridColumn>()
                };
                foreach (var item in invoiceViewerType.Settings.GridSettings.InvoiceGridFields)
                {
                    var gridColumn = invoiceUIGridColumnRunTime.FirstOrDefault(x => x.Field == item.Field && x.CustomFieldName == item.CustomFieldName);
                    if (gridColumn != null)
                    {
                        var field = fields.FirstOrDefault(x => x.InvoiceFieldId == item.Field);
                        invoiceViewerTypeRuntime.RuntimeGridColumns.Add(new RuntimeGridColumn
                        {
                            Attribute = gridColumn.Attribute,
                            CustomFieldName = gridColumn.CustomFieldName,
                            Field = gridColumn.Field,
                            FieldName = field != null ?field.Name:null,
                            Header = item.Header,
                        });
                    }
                }
            }
            return invoiceViewerTypeRuntime;
        }
        public IEnumerable<InvoiceQueryInterceptorTemplate> GetInvoiceQueryInterceptorTemplates()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<InvoiceQueryInterceptorTemplate>(InvoiceQueryInterceptorTemplate.EXTENSION_TYPE);
        }
        public IEnumerable<Entities.InvoiceGridActionSettingsConfig> GetInvoiceGridActionSettingsConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<Entities.InvoiceGridActionSettingsConfig>(Entities.InvoiceGridActionSettingsConfig.EXTENSION_TYPE);
        }
        private InvoiceViewerTypeInfo InvoiceViewerTypeInfoMapper(InvoiceViewerType invoiceViewerType)
        {
            return new InvoiceViewerTypeInfo
            {
                InvoiceViewerTypeId = invoiceViewerType.VRComponentTypeId,
                Name = invoiceViewerType.Name,
            };
        }
    }
}
