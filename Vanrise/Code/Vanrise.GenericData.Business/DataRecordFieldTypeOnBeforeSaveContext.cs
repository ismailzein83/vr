using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class DataRecordFieldTypeOnBeforeSaveContext : IDataRecordFieldTypeOnBeforeSaveContext
    {
        public object FieldValue { get; set; }

        public object BusinessEntityId { get; set; }

        public Guid? BusinessEntityDefinitionId { get; set; }
    }
    public class DataRecordFieldTypeOnAfterSaveContext : IDataRecordFieldTypeOnAfterSaveContext
    {
        public object FieldValue { get; set; }

        public object BusinessEntityId { get; set; }

        public Guid? BusinessEntityDefinitionId { get; set; }
    }
}
