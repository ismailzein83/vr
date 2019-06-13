using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.DataRecordTypeExtraFields
{
    class MultipleParentDataRecordTypeExtraFields : DataRecordTypeExtraField
    {
        public override Guid ConfigId { get { return new Guid("466EECEE-F3AC-4880-8818-8FEDD8D92BA8"); } }

        public override List<DataRecordField> GetFields(IDataRecordExtraFieldContext context)
        {
            throw new NotImplementedException();
        }
    }
}
