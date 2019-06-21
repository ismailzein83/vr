using System;
using System.Collections.Generic;
using System.ComponentModel;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Business;

namespace Vanrise.BusinessProcess.MainExtensions.BPTaskTypes
{
    public class BPGenericTaskTypeSettings : BaseBPTaskTypeSettings
    {
        static DataRecordTypeManager s_dataRecordTypeManager = new DataRecordTypeManager();

        public override Guid ConfigId => new Guid("0675F4DE-CB92-4F57-ADF2-00F5BA72E5F5");

        public override string Editor
        {
            get
            {
                return "/Client/Modules/BusinessProcess/Views/BPTask/BPGenericTaskTypeSettingsEditor.html";
            }
            set
            {

            }
        }
        public ModalWidthEnum EditorSize { get; set; }
        public Guid RecordTypeId { get; set; }

        public List<BPGenericTaskTypeAction> TaskTypeActions { get; set; }
        public GenericData.Entities.VRGenericEditorDefinitionSetting EditorSettings { get; set; }
        public bool IncludeTaskLock { get; set; }

        public override string SerializeTaskData(BPTaskData taskData)
        {
            var taskDataSerializable = new BPGenericTaskData();
            BPGenericTaskData genericTaskData = taskData.CastWithValidate<BPGenericTaskData>("genericTaskData");

            Dictionary<string, DataRecordField> recordFields = s_dataRecordTypeManager.GetDataRecordTypeFields(this.RecordTypeId);
            recordFields.ThrowIfNull("recordFields", this.RecordTypeId);

            if (genericTaskData.FieldValues != null)
            {
                taskDataSerializable.FieldValues = new Dictionary<string, dynamic>();

                foreach (var fieldValueEntry in genericTaskData.FieldValues)
                {
                    DataRecordField field = recordFields.GetRecord(fieldValueEntry.Key);
                    field.ThrowIfNull("field", fieldValueEntry.Key);
                    field.Type.ThrowIfNull("field.Type", fieldValueEntry.Key);

                    dynamic valueToStore;
                    if (fieldValueEntry.Value == null)
                    {
                        valueToStore = null;
                    }
                    else
                    {
                        if (field.Type.StoreValueSerialized)
                        {
                            valueToStore = field.Type.SerializeValue(new SerializeDataRecordFieldValueContext { Object = fieldValueEntry.Value });
                        }
                        else
                        {
                            valueToStore = fieldValueEntry.Value;
                        }
                    }
                    taskDataSerializable.FieldValues.Add(fieldValueEntry.Key, valueToStore);
                }
            }

            return Serializer.Serialize(taskDataSerializable);
        }

        public override BPTaskData DeserializeTaskData(string serializedTaskData)
        {
            var taskDataSerializable = Serializer.Deserialize<BPGenericTaskData>(serializedTaskData);
            taskDataSerializable.ThrowIfNull("taskDataSerializable");

            var taskDataToReturn = new BPGenericTaskData();
            
            Dictionary<string, DataRecordField> recordFields = s_dataRecordTypeManager.GetDataRecordTypeFields(this.RecordTypeId);
            recordFields.ThrowIfNull("recordFields", this.RecordTypeId);

            if (taskDataSerializable.FieldValues != null)
            {
                taskDataToReturn.FieldValues = new Dictionary<string, dynamic>();

                foreach (var fieldValueEntry in taskDataSerializable.FieldValues)
                {
                    DataRecordField field = recordFields.GetRecord(fieldValueEntry.Key);

                    if (field != null)//no problem if field is no longer in DataRecordType 
                    {
                        field.Type.ThrowIfNull("field.Type", fieldValueEntry.Key);

                        dynamic originalValue;
                        if (fieldValueEntry.Value == null)
                        {
                            originalValue = null;
                        }
                        else
                        {
                            if (field.Type.StoreValueSerialized)
                            {
                                originalValue = field.Type.DeserializeValue(new DeserializeDataRecordFieldValueContext { Value = fieldValueEntry.Value });
                            }
                            else
                            {
                                originalValue = fieldValueEntry.Value;
                            }
                        }
                        taskDataToReturn.FieldValues.Add(fieldValueEntry.Key, originalValue);
                    }
                }
            }


            return taskDataToReturn;
        }
    }
    public class BPGenericTaskData : BPTaskData
    {
        public Dictionary<string, dynamic> FieldValues { get; set; }
        public T GetFieldValue<T>(String FieldName)
        {
            Type typeParameterType = typeof(T);

            switch (typeParameterType.FullName)
            {
                case "System.Guid":
                    {
                        GuidConverter guidConverter = new GuidConverter();
                        return guidConverter.ConvertFrom(this.FieldValues[FieldName]);
                    }

                default:
                    return (T)Convert.ChangeType(this.FieldValues[FieldName], typeParameterType);
            }
        }
    }
    
    public class BPGenericTaskExecutionInformation : BPTaskExecutionInformation
    {

    }
}
