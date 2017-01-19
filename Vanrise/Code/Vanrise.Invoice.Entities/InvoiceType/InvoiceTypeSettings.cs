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
        public List<AutoGenerateInvoiceAction> AutoGenerateInvoiceActions { get; set; }

    }


    public class AutoGenerateInvoiceAction
    {
        public Guid AutoGenerateInvoiceActionId { get; set; }
        public string Title { get; set; }
        public AutoGenerateInvoiceActionSettings Settings { get; set; }
    }
    public abstract class AutoGenerateInvoiceActionSettings
    {
        public abstract Guid ConfigId { get; }
        public abstract void Execute(IAutoGenerateInvoiceActionSettingsContext contex);
    }
    public interface IAutoGenerateInvoiceActionSettingsContext
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
