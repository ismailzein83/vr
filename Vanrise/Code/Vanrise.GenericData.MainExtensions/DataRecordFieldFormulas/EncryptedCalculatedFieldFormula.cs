using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.DataRecordFieldFormulas
{
    public class EncryptedCalculatedFieldFormula : DataRecordFieldFormula
    {
        public override Guid ConfigId { get { return new Guid("B9B7715F-4AFE-402C-AD47-A1C0B2FF8AB1"); } }
        public bool Decrypt { get; set; }
        public string FieldName { get; set; }
        public override List<string> GetDependentFields(IDataRecordFieldFormulaGetDependentFieldsContext context)
        {
            return new List<string>() { FieldName };
        }
        public override dynamic CalculateValue(IDataRecordFieldFormulaCalculateValueContext context)
        {
            var fieldValue = context.GetFieldValue(FieldName);
            string key = DataEncryptionKeyManager.GetLocalTokenDataDecryptionKey();
            if (Decrypt)
            {
                return Cryptography.Decrypt(fieldValue, key);
            }
            else
            {
                return Cryptography.Encrypt(fieldValue, key);
            }
        }

        public override RecordFilter ConvertFilter(IDataRecordFieldFormulaConvertFilterContext context)
        {
            EmptyRecordFilter emptyFilter = context.InitialFilter as EmptyRecordFilter;
            if (emptyFilter != null)
                return new AlwaysFalseRecordFilter() { };

            NonEmptyRecordFilter nonEmptyFilter = context.InitialFilter as NonEmptyRecordFilter;
            if (nonEmptyFilter != null)
                return null;

            throw new Exception(String.Format("Invalid Record Filter '{0}'", context.InitialFilter.GetType()));
        }
    }
}
