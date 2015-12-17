using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Entities;
using Vanrise.Common.Business;
using Vanrise.Common;
namespace TOne.WhS.CDRProcessing.Business
{
    public class DefineCDRFieldsManager : GenericConfigurationManager<CDRFields>
    {
        public Vanrise.Entities.IDataRetrievalResult<CDRFieldDetail> GetFilteredCDRFields(Vanrise.Entities.DataRetrievalInput<CDRFieldsQuery> input)
        {
            var config = GetChachedCDRFields();

            Func<CDRField, bool> filterExpression = (prod) =>
                (input.Query.Name == null || prod.FieldName.ToLower().Contains(input.Query.Name.ToLower()));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, config.Fields.ToBigResult(input, filterExpression, MapToDetails));
        }
        public TOne.Entities.UpdateOperationOutput<CDRFieldDetail> UpdateCDRField(CDRField cdrField)
        {
            TOne.Entities.UpdateOperationOutput<CDRFieldDetail> updateOperationOutput = new TOne.Entities.UpdateOperationOutput<CDRFieldDetail>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            var cdrFields = GetChachedCDRFields();
            bool updateActionSucc;
            if (cdrFields.Fields.Any(x => x.FieldName == cdrField.FieldName))
            {
                cdrFields.Fields.FindRecord(x => x.FieldName == cdrField.FieldName).FieldName = cdrField.FieldName;
                cdrFields.Fields.FindRecord(x => x.FieldName == cdrField.FieldName).Type = cdrField.Type;
                updateActionSucc = base.UpdateConfiguration(cdrFields);
            }
            else
            {
                updateActionSucc = false;
            }

            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = MapToDetails(cdrField);
            }

            return updateOperationOutput;

        }
        public TOne.Entities.InsertOperationOutput<CDRFieldDetail> AddCDRField(CDRField cdrField)
        {

            TOne.Entities.InsertOperationOutput<CDRFieldDetail> insertOperationOutput = new TOne.Entities.InsertOperationOutput<CDRFieldDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            var cdrFields = GetChachedCDRFields();
            cdrField.ID = cdrFields.Fields.Count() + 1;
            if (!cdrFields.Fields.Any(x => x.FieldName == cdrField.FieldName))
                cdrFields.Fields.Add(cdrField);

            bool insertActionSucc = base.UpdateConfiguration(cdrFields);

            if (insertActionSucc)
            {
                
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = MapToDetails(cdrField);
            }

            return insertOperationOutput;
        }

        public CDRField GetCDRField(int cdrFieldId)
        {
            var cdrFields = GetChachedCDRFields();
            return cdrFields.Fields.FindRecord(x=>x.ID==cdrFieldId);
        }
        public List<Vanrise.Entities.TemplateConfig> GetCDRFieldTypeTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.CDRFieldConfigType);
        }
        private CDRFieldDetail MapToDetails(CDRField cdrField)
        {
            var templates = GetCDRFieldTypeTemplates();
            var template = templates.FindRecord(x => x.TemplateConfigID == cdrField.Type.ConfigId);
            return new CDRFieldDetail
            {
                Entity = cdrField,
                TypeDescription = template!=null?template.Name:null
            };
        }
        private CDRFields GetChachedCDRFields()
        {
            var cdrFields = base.GetConfiguration(null);
            if(cdrFields.Fields==null)
            {
                cdrFields.Fields = new List<CDRField>();
            }
            return cdrFields;
        }
    }
}
