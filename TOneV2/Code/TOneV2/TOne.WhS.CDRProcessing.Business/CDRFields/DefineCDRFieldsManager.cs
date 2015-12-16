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
            var config = base.GetConfiguration(null);
            if (config == null)
            {
                config = new CDRFields();
                config.Fields = new List<CDRField>();
            }
               
            Func<CDRField, bool> filterExpression = (prod) =>
                (input.Query.Name == null || prod.FieldName.ToLower().Contains(input.Query.Name.ToLower()));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, config.Fields.ToBigResult(input, filterExpression, MapToDetails));
        }
        public void UpdateCDRField(CDRField cdrField)
        {
            var cdrFields = base.GetConfiguration(null);

            //if (cdrFields.Fields != null)
            //{
            //  cdrFields.Fields.FindRecord(x => x.FieldName == cdrField.FieldName) = cdrField;
            //}

            base.UpdateConfiguration(cdrFields);
        }
        public void AddCDRField(CDRField cdrField)
        {
            var cdrFields = base.GetConfiguration(null);
            if (cdrFields.Fields == null)
                cdrFields.Fields = new List<CDRField>();
            if (cdrFields.Fields != null && !cdrFields.Fields.Any(x => x.FieldName == cdrField.FieldName))
                cdrFields.Fields.Add(cdrField);
            base.UpdateConfiguration(cdrFields);
        }
        private CDRFieldDetail MapToDetails(CDRField cdrField)
        {
            return new CDRFieldDetail
            {
                Entity = cdrField,
                TypeDescription = cdrField.Type.ToString()
            };
        }
    }
}
