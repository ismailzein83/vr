using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Security.Entities;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceTypeSettings
    {
        public StartDateCalculationMethod StartDateCalculationMethod { get; set; }  
        public Guid InvoiceDetailsRecordTypeId { get; set; }
        public List<InvoiceAction> InvoiceActions { get; set; }
        public List<InvoiceGeneratorAction> InvoiceGeneratorActions { get; set; }
        public InvoiceTypeExtendedSettings ExtendedSettings { get; set; }
        public InvoiceGridSettings InvoiceGridSettings { get; set; }
        public InvoiceSerialNumberSettings InvoiceSerialNumberSettings { get; set; }
        public List<InvoiceSubSection> SubSections { get; set; }
        public List<ItemGrouping> ItemGroupings { get; set; }
        public InvoiceTypeSecurity Security { get; set; }
        public List<AutomaticInvoiceAction> AutomaticInvoiceActions { get; set; }
        public List<InvoiceSettingPartUISection> InvoiceSettingPartUISections { get; set; }
    }

    public class InvoiceSettingPartUISection
    {
        public string SectionTitle { get; set; }
        public List<InvoiceSettingPartUIRow> Rows { get; set; }
    }
    public class InvoiceSettingPartUIRow
    {
        public List<InvoiceSettingPartDefinition> Parts { get; set; }
    }
    public class InvoiceSettingPartDefinition
    {
        public Guid PartConfigId { get; set; }
        public Object PartDefinitionSetting { get; set; }
    }

    public class AutomaticInvoiceAction
    {
        public Guid AutomaticInvoiceActionId { get; set; }
        public string Title { get; set; }
        public AutomaticInvoiceActionSettings Settings { get; set; }
    }
    public abstract class AutomaticInvoiceActionSettings
    {
        public abstract Guid ConfigId { get; }
        public abstract void Execute(IAutomaticInvoiceActionSettingsContext contex);
    }
    public interface IAutomaticInvoiceActionSettingsContext
    {

    }

    public class ItemGrouping
    {
        public Guid ItemGroupingId { get; set; }
        public string ItemSetName { get; set; }
        public List<DimensionItemField> DimensionItemFields { get; set; }
        public List<AggregateItemField> AggregateItemFields { get; set; }
    }
    public class DimensionItemField
    {
        public Guid DimensionItemFieldId { get; set; }
        public string FieldName { get; set; }
        public string FieldDescription { get; set; }
        public DataRecordFieldType FieldType { get; set; }
    }
    public enum AggregateType { Count = 1, Sum = 2, Max = 3, Min = 4 }
    public class AggregateItemField
    {
        public Guid AggregateItemFieldId { get; set; }
        public string FieldName { get; set; }
        public string FieldDescription { get; set; }
        public DataRecordFieldType FieldType { get; set; }
        public AggregateType AggregateType { get; set; }
    }
    public class InvoiceTypeSecurity
    {
        public RequiredPermissionSettings ViewRequiredPermission { get; set; }
        public RequiredPermissionSettings GenerateRequiredPermission { get; set; }

    }


}
