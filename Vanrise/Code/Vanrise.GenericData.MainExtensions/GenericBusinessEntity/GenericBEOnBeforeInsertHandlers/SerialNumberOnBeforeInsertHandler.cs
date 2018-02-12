using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
namespace Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBEOnBeforeInsertHandlers
{
    public class SerialNumberOnBeforeInsertHandler : GenericBEOnBeforeInsertHandler
    {
        public override Guid ConfigId
        {
            get { return new Guid("39D842AB-7AFB-46EC-B43E-E7F19493CB86"); }
        }
        public List<GenericBESerialNumberPart> PartDefinitions { get; set; }
        public string InfoType { get; set; }
        public string FieldName { get; set; }

        public override object TryGetInfoByType(IGenericBEOnBeforeInsertHandlerInfoByTypeContext context)
        {
            if (context.InfoType == null)
                return null;
            switch (context.InfoType)
            {
                case "SerialNumberPartDefinitions":
                    if (PartDefinitions != null)
                    {
                        return PartDefinitions.MapRecords(x =>
                        {
                            return new GenericBESerialNumberPartInfo
                            {
                                VariableDescription = x.VariableDescription,
                                VariableName = x.VariableName
                            };
                        });
                    }
                    return null;
                default: return null;
            }
        }
        public override void Execute(IGenericBEOnBeforeInsertHandlerContext context)
        {

            if (PartDefinitions != null)
            {
                context.DefinitionSettings.ThrowIfNull("context.DefinitionSettings");
                context.DefinitionSettings.ExtendedSettings.ThrowIfNull("context.DefinitionSettings.ExtendedSettings");
                context.GenericBusinessEntity.ThrowIfNull("context.GenericBusinessEntity");
                string serialNumberPattern = new GenericBusinessEntityDefinitionManager().GetExtendedSettingsInfoByType(context.DefinitionSettings, InfoType) as string; 
                if (serialNumberPattern != null)
                {
                    var serialNumberPartContext = new GenericBESerialNumberPartSettingsContext
                    {
                        DefinitionSettings = context.DefinitionSettings,
                        GenericBusinessEntity = context.GenericBusinessEntity,
                        BusinessEntityDefinitionId = context.BusinessEntityDefinitionId
                    };

                    foreach (var part in PartDefinitions)
                    {
                        if (serialNumberPattern != null && serialNumberPattern.Contains(string.Format("#{0}#", part.VariableName)))
                        {
                            serialNumberPattern = serialNumberPattern.Replace(string.Format("#{0}#", part.VariableName), part.Settings.GetPartText(serialNumberPartContext));
                        }
                    }

                    if (context.GenericBusinessEntity.FieldValues == null)
                        context.GenericBusinessEntity.FieldValues = new Dictionary<string, object>();

                    Object serialNumberField = null;
                    if (context.GenericBusinessEntity.FieldValues.TryGetValue(this.FieldName, out serialNumberField))
                    {
                        context.GenericBusinessEntity.FieldValues[this.FieldName] = serialNumberPattern;
                    }
                    else
                    {
                        context.GenericBusinessEntity.FieldValues.Add(this.FieldName, serialNumberPattern);
                    }
                }
            }
        }
    }
 
    public class GenericBESerialNumberPartInfo
    {
        public string VariableName { get; set; }
        public string VariableDescription { get; set; }
    }
    public class GenericBESerialNumberPart
    {
        public Guid GenericBESerialNumberPartId { get; set; }
        public string VariableName { get; set; }
        public string VariableDescription { get; set; }

        public GenericBESerialNumberPartSettings Settings { get; set; }
    }
    public abstract class GenericBESerialNumberPartSettings
    {
        public abstract Guid ConfigId { get; }
        public abstract string RuntimeEditor { get; }
        public abstract string GetPartText(IGenericBESerialNumberPartSettingsContext context);
    }
    public interface IGenericBESerialNumberPartSettingsContext
    {
        Entities.GenericBusinessEntity GenericBusinessEntity { get; set; }
        GenericBEDefinitionSettings DefinitionSettings { get; }
        Guid BusinessEntityDefinitionId { get; }
    }
    public class GenericBESerialNumberPartSettingsContext : IGenericBESerialNumberPartSettingsContext
    {
        public Entities.GenericBusinessEntity GenericBusinessEntity { get; set; }
        public GenericBEDefinitionSettings DefinitionSettings { get; set; }
        public Guid BusinessEntityDefinitionId { get; set; }
    }
    public class GenericFieldSerialNumberPart : GenericBESerialNumberPartSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("E1C87C72-9481-4707-A73A-C511E0F35AEA"); }
        }
        public string FieldName { get; set; }
        public override string GetPartText(IGenericBESerialNumberPartSettingsContext context)
        {
            context.DefinitionSettings.ThrowIfNull("context.DefinitionSettings");
            context.GenericBusinessEntity.ThrowIfNull("context.GenericBusinessEntity");
            context.GenericBusinessEntity.FieldValues.ThrowIfNull("context.GenericBusinessEntity.FieldValues");
           
            var dataRecordType = new DataRecordTypeManager().GetDataRecordType(context.DefinitionSettings.DataRecordTypeId);
            dataRecordType.ThrowIfNull("dataRecordType", context.DefinitionSettings.DataRecordTypeId);
            dataRecordType.Fields.ThrowIfNull("dataRecordType.Fields", context.DefinitionSettings.DataRecordTypeId);
            
            var dataRecordField = dataRecordType.Fields.FindRecord(x => x.Name == FieldName);
            if (dataRecordField == null)
                return null;

            Object fieldValue;
            if(dataRecordField.Formula == null)
                fieldValue = context.GenericBusinessEntity.FieldValues[FieldName];
            else
            {
                var dataRecordFieldTypeDict = dataRecordType.Fields.ToDictionary(itm => itm.Name, itm => itm.Type);
                fieldValue = dataRecordField.Formula.CalculateValue(new DataRecordFieldFormulaCalculateValueContext(dataRecordFieldTypeDict, context.GenericBusinessEntity.FieldValues, dataRecordField.Type));
            }

            return dataRecordField.Type.GetDescription(fieldValue);
        }

        public override string RuntimeEditor
        {
            get { return ""; }
        }
    }
    public enum DateCounterType { Yearly = 0 }
    public class SequenceSerialNumberPart : GenericBESerialNumberPartSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("16197ED3-F3EF-4C50-9106-61A576789726"); }
        }
        public string InfoType { get; set; }
        public bool IncludePartnerId { get; set; }
        public DateCounterType? DateCounterType { get; set; }
        public int PaddingLeft { get; set; }
        public string SequenceKeyFieldName { get; set; }
        public override string GetPartText(IGenericBESerialNumberPartSettingsContext context)
        {
            context.DefinitionSettings.ThrowIfNull("context.DefinitionSettings");
            context.GenericBusinessEntity.ThrowIfNull("context.GenericBusinessEntity");
            context.GenericBusinessEntity.FieldValues.ThrowIfNull("context.GenericBusinessEntity.FieldValues");



            StringBuilder sequenceKey = new StringBuilder();
            StringBuilder sequenceGroup = new StringBuilder();
            sequenceGroup.Append("OVERALL");
            var sequenceKeyValue = context.GenericBusinessEntity.FieldValues.GetRecord(this.SequenceKeyFieldName);

            long initialSequenceValue = new GenericBESerialNumberManager().GetSerialNumberPartInitialSequence(context.DefinitionSettings, InfoType);

            if (this.IncludePartnerId)
            {
                sequenceKey.Append(sequenceKeyValue);
                sequenceGroup.Append("_");
                sequenceGroup.Append(sequenceKeyValue);
            }
            if (this.DateCounterType.HasValue)
            {
                if (sequenceKey.Length > 0)
                    sequenceKey.Append("_");
                sequenceGroup.Append("_");
                sequenceGroup.Append(Common.Utilities.GetEnumDescription(this.DateCounterType.Value));
                switch (this.DateCounterType)
                {
                    case Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBEOnBeforeInsertHandlers.DateCounterType.Yearly:
                        sequenceKey.Append(string.Format("{0}_{1}", DateTime.Today.Year, DateTime.Today.Year + 1));
                        break;
                }
            }
            VRSequenceManager manager = new VRSequenceManager();
            var sequenceNumber = manager.GetNextSequenceValue(sequenceGroup.ToString(), context.BusinessEntityDefinitionId, sequenceKey.ToString(), initialSequenceValue);
            return sequenceNumber.ToString().PadLeft(this.PaddingLeft, '0');
        }

        public override string RuntimeEditor
        {
            get { return ""; }
        }
    }


}
