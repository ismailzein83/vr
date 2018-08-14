using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
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
        public InvoiceFileSettings InvoiceFileSettings { get; set; }

        public List<InvoiceSubSection> SubSections { get; set; }
        public List<ItemGrouping> ItemGroupings { get; set; }
        public InvoiceTypeSecurity Security { get; set; }
        public List<AutomaticInvoiceAction> AutomaticInvoiceActions { get; set; }
        public List<InvoiceBulkAction> InvoiceBulkActions { get; set; }

        public List<InvoiceSettingPartUISection> InvoiceSettingPartUISections { get; set; }
        public List<InvoiceAttachment> InvoiceAttachments { get; set; }

        public Guid? InvToAccBalanceRelationId { get; set; }
        public string AmountFieldName { get; set; }
        public string CurrencyFieldName { get; set; }
        public Guid ExecutionFlowDefinitionId { get; set; }
        public List<string> StagesToHoldNames { get; set; }
        public List<string> StagesToProcessNames { get; set; }
        public List<ItemSetNameStorageRule> ItemSetNamesStorageRules { get; set; }
        public List<InvoiceMenualBulkAction> InvoiceMenualBulkActions { get; set; }
        public Guid? InvoiceCommentDefinitionId { get; set; }
    }
    public class InvoiceMenualBulkAction
    {
        public Guid InvoiceMenualBulkActionId { get; set; }
        public string Title { get; set; }
        public Guid InvoiceBulkActionId { get; set; }
    }
   
    public class InvoiceFileSettings
    {
        public List<InvoiceFileNamePart> InvoiceFileNameParts { get; set; }
        public List<InvoiceFileAttachment> FilesAttachments { get; set; }
    }
    public class InvoiceFileAttachment
    {
        public string Name { get; set; }
        public Guid AttachmentId { get; set; }

    }
    public class InvoiceFileNamePart
    {
        public string VariableName { get; set; }
        public string Description { get; set; }
        public Vanrise.Entities.VRConcatenatedPartSettings<IInvoiceFileNamePartContext> Settings { get; set; }

    }
    public interface IInvoiceFileNamePartContext
    {
        Invoice Invoice { get; }
        Guid InvoiceTypeId { get; set; }
    }
    public class ItemSetNameStorageRule
    {
        public Guid ItemSetNameStorageRuleId { get; set; }
        public string Name { get; set; }
        public ItemSetNameStorageRuleSettings Settings { get; set; }
    }
    public abstract class ItemSetNameStorageRuleSettings
    {
        public abstract Guid ConfigId { get; }
        public abstract bool IsApplicable(IItemSetNameStorageRuleContext context);
    }
    public interface IItemSetNameStorageRuleContext
    {
        string ItemSetName { get; }
        string StorageConnectionString { set; }
    }
    public class ItemSetNameStorageRuleContext : IItemSetNameStorageRuleContext
    {
        public string ItemSetName { get; set; }
        public string StorageConnectionString { set; get; }
    }
    public class InvoiceAttachment
    {
        public Guid InvoiceAttachmentId { get; set; }
        public string Title { get; set; }
        public InvoiceFileConverter InvoiceFileConverter { get; set; }
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
        public bool IsOverridable { get; set; }
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
        public abstract string RuntimeEditor { get;  }
        public abstract void Execute(IAutomaticInvoiceActionSettingsContext contex);
    }
    public interface IAutomaticInvoiceActionSettingsContext
    {

    }
    public class InvoiceBulkAction
    {
        public Guid InvoiceBulkActionId { get; set; }
        public string Title { get; set; }
        public AutomaticInvoiceActionSettings Settings { get; set; }
    }

    public enum ItemGroupingOrderType { ByAllDimensions = 1, ByAllMeasures = 2 }
    public class ItemGrouping
    {
        public Guid ItemGroupingId { get; set; }
        public string ItemSetName { get; set; }
        public List<DimensionItemField> DimensionItemFields { get; set; }
        public List<AggregateItemField> AggregateItemFields { get; set; }
        public ItemGroupingOrderType? OrderType { get; set; }
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
        public RequiredPermissionSettings ViewSettingsRequiredPermission { get; set; }
        public RequiredPermissionSettings AddSettingsRequiredPermission { get; set; }
        public RequiredPermissionSettings EditSettingsRequiredPermission { get; set; }
        public RequiredPermissionSettings AssignPartnerRequiredPermission { get; set; }

    }


}
