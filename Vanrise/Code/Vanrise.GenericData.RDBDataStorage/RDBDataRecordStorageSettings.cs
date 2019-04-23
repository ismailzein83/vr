using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.GenericData.Entities;
using Vanrise.Entities;
namespace Vanrise.GenericData.RDBDataStorage
{
    public class RDBDataRecordStorageSettings : DataRecordStorageSettings
    {
        public Guid? ParentRecordStorageId { get; set; }

        public string TableName { get; set; }

        public string TableSchema { get; set; }

        public List<RDBDataRecordStorageColumn> Columns { get; set; }

        public List<RDBNullableField> NullableFields { get; set; }

        public List<RDBDataRecordStorageExpressionField> ExpressionFields { get; set; }

        public bool IncludeQueueItemId { get; set; }

        public List<RDBDataRecordStorageJoin> Joins { get; set; }

        public RecordFilterGroup Filter { get; set; }
    }

    public class RDBDataRecordStorageColumn
    {
        public string FieldName { get; set; }
        
        public string ColumnName { get; set; }

        public RDBDataType DataType { get; set; }

        public int? Size { get; set; }

        public int? Precision { get; set; }

        public bool IsUnique { get; set; }

        public bool IsIdentity { get; set; }
    }

    public class RDBDataRecordStorageJoin
    {
        public string RDBRecordStorageJoinName { get; set; }

        public RDBDataRecordStorageJoinSettings Settings { get; set; }
    }

    public abstract class RDBDataRecordStorageJoinSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract string GetCode(IRDBDataRecordStorageJoinExpressionGetCodeContext context);

        public abstract List<string> GetDependentJoins(IRDBDataRecordStorageJoinExpressionGetDependentJoinsContext context);
    }

    public interface IRDBDataRecordStorageJoinExpressionGetCodeContext
    {
    }

    public interface IRDBDataRecordStorageJoinExpressionGetDependentJoinsContext
    {
    }
    
    public class RDBDataRecordStorageExpressionField
    {
        public string FieldName { get; set; }

        public RDBDataRecordStorageExpressionFieldSettings Settings { get; set; }
    }

    public abstract class RDBDataRecordStorageExpressionFieldSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract string GetCode(IRDBDataRecordStorageExpressionFieldGetCodeContext context);

        public abstract List<string> GetDependentJoins(IRDBDataRecordStorageExpressionFieldGetDependentJoinsContext context);
    }

    public interface IRDBDataRecordStorageExpressionFieldGetCodeContext
    {

    }

    public interface IRDBDataRecordStorageExpressionFieldGetDependentJoinsContext
    {

    }
    
    public class RDBNullableField
    {
        public string Name { get; set; }
    }
}
