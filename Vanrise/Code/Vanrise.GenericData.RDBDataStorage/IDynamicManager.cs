using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.RDBDataStorage
{
    public interface IDynamicManager
    {
        void WriteFieldsToRecordStream(dynamic record, Vanrise.Data.RDB.RDBBulkInsertQueryWriteRecordContext bulkInsertRecordContext);

        void SetRDBInsertColumnsToTempTableFromRecord(dynamic record, Vanrise.Data.RDB.RDBInsertMultipleRowsQueryRowContext tempTableRowContext);

        dynamic GetDynamicRecordFromReader(Vanrise.Data.RDB.IRDBDataReader reader);

        Vanrise.GenericData.Entities.DataRecord GetDataRecordFromReader(Vanrise.Data.RDB.IRDBDataReader reader, List<string> fieldNames);

        void AddJoin(Vanrise.GenericData.RDBDataStorage.RDBDataStorageAddJoinRDBExpressionContext context);
        
        void SetRDBExpressionFromExpressionField(Vanrise.GenericData.RDBDataStorage.RDBDataStorageSetRDBExpressionFromExpressionFieldRDBExpressionContext context);        
    }

    public abstract class RDBDataStorageRDBExpressionContext
    {
        string _mainTableAlias;
        public RDBDataStorageRDBExpressionContext(string mainTableAlias)
        {
            this._mainTableAlias = mainTableAlias;
        }

        public string GetJoinTableAlias(string joinName)
        {
            return $"{joinName}_{_mainTableAlias}";
        }

        public string MainTableAlias
        {
            get
            {
                return _mainTableAlias;
            }
        }
    }

    public class RDBDataStorageAddJoinRDBExpressionContext : RDBDataStorageRDBExpressionContext
    {
        string _joinName;
        Vanrise.Data.RDB.RDBJoinContext _joinContext;

        public RDBDataStorageAddJoinRDBExpressionContext(string joinName, Vanrise.Data.RDB.RDBJoinContext joinContext, string mainTableAlias)
            : base(mainTableAlias)
        {
            _joinName = joinName;
            _joinContext = joinContext;
        }

        public string JoinName
        {
            get
            {
                return this._joinName;
            }
        }

        public Vanrise.Data.RDB.RDBJoinContext RDBJoinContext
        {
            get
            {
                return _joinContext;
            }
        }

        public string GetTableToJoinTableAlias()
        {
            return GetJoinTableAlias(this.JoinName);
        }
    }

    public class RDBDataStorageSetRDBExpressionFromExpressionFieldRDBExpressionContext : RDBDataStorageRDBExpressionContext
    {
        string _fieldName;
        Vanrise.Data.RDB.RDBExpressionContext _expressionContext;

        public RDBDataStorageSetRDBExpressionFromExpressionFieldRDBExpressionContext(string fieldName, Vanrise.Data.RDB.RDBExpressionContext expressionContext, string mainTableAlias)
            : base(mainTableAlias)
        {
            _fieldName = fieldName;
            _expressionContext = expressionContext;
        }

        public string FieldName
        {
            get
            {
                return this._fieldName;
            }
        }

        public Vanrise.Data.RDB.RDBExpressionContext RDBExpressionContext
        {
            get
            {
                return _expressionContext;
            }
        }
    }    
}
