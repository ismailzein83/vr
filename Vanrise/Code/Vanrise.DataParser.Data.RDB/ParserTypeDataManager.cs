using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.DataParser.Entities;
using Vanrise.Entities;

namespace Vanrise.DataParser.Data.RDB
{
    public class ParserTypeDataManager : IParserTypeDataManager
    {
        #region Fields/Properties/Ctor

        static string TABLE_NAME = "dataparser_ParserType";
        static string TABLE_ALIAS = "pt";

        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_Settings = "Settings";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";

        static ParserTypeDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "dataparser",
                DBTableName = "ParserType",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }

        #endregion

        #region Public Methods

        public List<ParserType> GetParserTypes()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS); 
            return queryContext.GetItems<ParserType>(ParserTypeMapper);
        }

        public bool AreParserTypesUpdated(ref object lastReceivedDataInfo)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref lastReceivedDataInfo);
        }

        public bool Insert(ParserType parserType)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);

            var notExistsCondition = insertQuery.IfNotExists(TABLE_ALIAS);
            notExistsCondition.EqualsCondition(COL_Name).Value(parserType.Name);

            insertQuery.Column(COL_ID).Value(parserType.ParserTypeId);
            insertQuery.Column(COL_Name).Value(parserType.Name);

            if (parserType.Settings != null)
                insertQuery.Column(COL_Settings).Value(Serializer.Serialize(parserType.Settings));

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool Update(ParserType parserType)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var notExistsCondition = updateQuery.IfNotExists(TABLE_ALIAS);
            notExistsCondition.NotEqualsCondition(COL_ID).Value(parserType.ParserTypeId);
            notExistsCondition.EqualsCondition(COL_Name).Value(parserType.Name);

            updateQuery.Column(COL_Name).Value(parserType.Name);

            if (parserType.Settings != null)
                updateQuery.Column(COL_Settings).Value(Serializer.Serialize(parserType.Settings));
            else
                updateQuery.Column(COL_Settings).Null();

            var where = updateQuery.Where();
            where.EqualsCondition(COL_ID).Value(parserType.ParserTypeId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        #endregion

        #region Private Methods

        private BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("DataParser", "DataParserDBConnStringKey", "DataParserDBConnString");
        }

        #endregion

        #region Mappers

        private ParserType ParserTypeMapper(IRDBDataReader reader)
        {
            ParserType parserType = new ParserType
            {
                ParserTypeId = reader.GetGuid(COL_ID),
                Name = reader.GetString(COL_Name)
            };

            string serializedSettings = reader.GetString(COL_Settings);
            if (!string.IsNullOrEmpty(serializedSettings))
                parserType.Settings = Vanrise.Common.Serializer.Deserialize<ParserTypeSettings>(serializedSettings);

            return parserType;
        }

        #endregion
    }
}