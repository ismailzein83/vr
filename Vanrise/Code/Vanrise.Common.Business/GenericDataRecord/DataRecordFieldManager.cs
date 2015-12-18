using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities.GenericDataRecord;

namespace Vanrise.Common.Business.GenericDataRecord
{
    public class DataRecordFieldManager
    {
        public Vanrise.Entities.IDataRetrievalResult<DataRecordFieldDetail> GetFilteredDataRecordFields(Vanrise.Entities.DataRetrievalInput<DataRecordFieldQuery> input)
        {
            DataRecordTypeManager manager = new DataRecordTypeManager();

            var dataRecordType = manager.GetDataRecordType(input.Query.DataRecordTypeId);

            Func<DataRecordField, bool> filterExpression = (prod) =>
                (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()))
                &&
                (input.Query.TypeIds == null || input.Query.TypeIds.Contains(prod.Type.ConfigId));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataRecordType.Fields.ToBigResult(input, filterExpression, MapToDetails));
        }
        //public Vanrise.Entities.UpdateOperationOutput<DataRecordFieldDetail> UpdateDataRecordField(DataRecordField dataRecordField)
        //{
        //    Vanrise.Entities.UpdateOperationOutput<DataRecordFieldDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<DataRecordFieldDetail>();
        //    updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
        //    updateOperationOutput.UpdatedObject = null;

        //    var cdrFields = GetChachedCDRFields();
        //    bool updateActionSucc = false;
        //    if (cdrFields.Fields.Any(x => x.ID == dataRecordField.ID))
        //    {
        //        if (cdrFields.Fields.FindRecord(x => x.ID == dataRecordField.ID).FieldName != dataRecordField.Name && cdrFields.Fields.Exists(x => x.FieldName == dataRecordField.Name))
        //        {
        //            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
        //        }
        //        else
        //        {
        //            cdrFields.Fields.FindRecord(x => x.ID == dataRecordField.ID).FieldName = dataRecordField.Name;
        //            cdrFields.Fields.FindRecord(x => x.ID == dataRecordField.ID).Type = dataRecordField.Type;
        //            updateActionSucc = base.UpdateConfiguration(cdrFields);
        //        }

        //    }

        //    if (updateActionSucc)
        //    {
        //        updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
        //        updateOperationOutput.UpdatedObject = MapToDetails(dataRecordField);
        //    }

        //    return updateOperationOutput;

        //}

        //public Vanrise.Entities.DeleteOperationOutput<DataRecordFieldDetail> DeleteCDRField(int dataRecordFieldId)
        //{
        //    Vanrise.Entities.DeleteOperationOutput<DataRecordFieldDetail> deleteOperationOutput = new Vanrise.Entities.DeleteOperationOutput<DataRecordFieldDetail>();
        //    deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Failed;
        //  //  DefineCDRFieldsManager manager = new DefineCDRFieldsManager();
        //    var cdrFields = GetChachedCDRFields();
        //    bool deleteActionSucc = false;
        //    if (cdrFields.Fields.Any(x => x.ID == dataRecordFieldId))
        //    {
        //        cdrFields.Fields.Remove(cdrFields.Fields.FindRecord(x => x.ID == dataRecordFieldId));
        //      //  deleteActionSucc = base.UpdateConfiguration(cdrFields);
        //    }
        //    if (deleteActionSucc)
        //    {
        //        deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Succeeded;
        //    }

        //    return deleteOperationOutput;
        //}

        //public Vanrise.Entities.InsertOperationOutput<DataRecordFieldDetail> AddCDRField(DataRecordField dataRecordField)
        //{

        //    Vanrise.Entities.InsertOperationOutput<DataRecordFieldDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<DataRecordFieldDetail>();

        //    insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
        //    insertOperationOutput.InsertedObject = null;

        //    var cdrFields = GetChachedCDRFields();
        //    bool insertActionSucc = false;

        //    if (!cdrFields.Fields.Exists(x => x.FieldName == dataRecordField.Name))
        //    {
        //        dataRecordField.ID = cdrFields.Fields.Count() + 1;
        //        cdrFields.Fields.Add(dataRecordField);
        //     //   insertActionSucc = base.UpdateConfiguration(dataRecordField);
        //    }
        //    else
        //    {
        //        insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
        //    }


        //    if (insertActionSucc)
        //    {

        //        insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
        //        insertOperationOutput.InsertedObject = MapToDetails(dataRecordField);
        //    }

        //    return insertOperationOutput;
        //}

        //public DataRecordField GetCDRField(int cdrFieldId)
        //{
        //    var cdrFields = GetChachedCDRFields();
        //    return cdrFields.Fields.FindRecord(x => x.ID == cdrFieldId);
        //}
        public List<Vanrise.Entities.TemplateConfig> GetDataRecordFieldTypeTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.CDRFieldConfigType);
        }
        private DataRecordFieldDetail MapToDetails(DataRecordField cdrField)
        {
            var templates = GetDataRecordFieldTypeTemplates();
            var template = templates.FindRecord(x => x.TemplateConfigID == cdrField.Type.ConfigId);
            return new DataRecordFieldDetail
            {
                Entity = cdrField,
                TypeDescription = template != null ? template.Name : null
            };
        }
        //private DataRecordFieldType GetChachedCDRFields()
        //{
        //    var cdrFields = base.GetConfiguration(null);
        //    if (cdrFields.Fields == null)
        //    {
        //        cdrFields.Fields = new List<DataRecordField>();
        //    }
        //    return cdrFields;
        //}
    }
}
