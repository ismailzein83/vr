using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
namespace Vanrise.GenericData.Business
{
    public class GenericEditorTranslationContext : IGenericEditorTranslationContext
    {
        public Guid LanguageId { get; set; }
        public Guid DataRecordTypeId { get; set; }
        public GenericEditorTranslationContext(Guid dataRecordTypeId, Guid languageId)
        {
            DataRecordTypeId = dataRecordTypeId;
            LanguageId = languageId;
        }
        Dictionary<string, DataRecordField> _dataRecordFields;
        public DataRecordFieldType GetFieldType(string fieldName)
        {
            if (_dataRecordFields == null)
            {
                _dataRecordFields = new DataRecordTypeManager().GetDataRecordTypeFields(DataRecordTypeId);
            }
            var field = _dataRecordFields.GetRecord(fieldName);

            return field != null ? field.Type : null;
        }
    }
}
