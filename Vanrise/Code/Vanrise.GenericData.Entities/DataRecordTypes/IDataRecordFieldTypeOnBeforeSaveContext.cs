using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public interface IDataRecordFieldTypeOnBeforeSaveContext
    {
        Object FieldValue { get; }
        Object BusinessEntityId { get; set; }
        Guid? BusinessEntityDefinitionId { get; }

    }
    public interface IDataRecordFieldTypeOnAfterSaveContext
    {
        Object FieldValue { get; }
        Object BusinessEntityId { get; set; }
        Guid? BusinessEntityDefinitionId { get; }
    }
}
