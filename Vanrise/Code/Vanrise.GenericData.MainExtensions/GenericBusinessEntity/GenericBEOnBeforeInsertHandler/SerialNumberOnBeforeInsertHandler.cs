using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
namespace Vanrise.GenericData.MainExtensions
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
                        GenericBusinessEntity = context.GenericBusinessEntity
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
        GenericBusinessEntity GenericBusinessEntity { get; set; }
        GenericBEDefinitionSettings DefinitionSettings { get; }
    }
    public class GenericBESerialNumberPartSettingsContext : IGenericBESerialNumberPartSettingsContext
    {
        public GenericBusinessEntity GenericBusinessEntity { get; set; }
        public GenericBEDefinitionSettings DefinitionSettings { get; set; }
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

            var dataRecordField = new DataRecordTypeManager().GetDataRecordField(context.DefinitionSettings.DataRecordTypeId, FieldName);
            if (dataRecordField == null)
                return null;
            var fieldValue = context.GenericBusinessEntity.FieldValues[FieldName];

            return dataRecordField.Type.GetDescription(fieldValue);
        }

        public override string RuntimeEditor
        {
            get { return ""; }
        }
    }

}
