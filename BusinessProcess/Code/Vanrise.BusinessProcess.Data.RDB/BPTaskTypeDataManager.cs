using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;
namespace Vanrise.BusinessProcess.Data.RDB
{
    public class BPTaskTypeDataManager : IBPTaskTypeDataManager
    {
        static string TABLE_NAME = "bp_BPTaskType";
        static string TABLE_ALIAS = "TaskType";

        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_Settings = "Settings";
        const string COL_LastModifiedTime = "LastModifiedTime";


        static BPTaskTypeDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 255 });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "bp",
                DBTableName = "BPTaskType",
                Columns = columns,
                IdColumnName = COL_ID,
                ModifiedTimeColumnName = COL_LastModifiedTime

            });
        }
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("BusinessProcessConfig", "BusinessProcessConfigDBConnStringKey", "BusinessProcessDBConnString");
        }
        #region Public Methods
        public bool AreBPTaskTypesUpdated(ref object lastReceivedDataInfo)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref lastReceivedDataInfo);
        }
        public IEnumerable<BPTaskType> GetBPTaskTypes()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            return queryContext.GetItems(BPTaskTypeMapper);
        }
        #endregion

        #region Mappers
        BPTaskType BPTaskTypeMapper(IRDBDataReader reader)
        {
            var bpTaskType = new BPTaskType
            {
                BPTaskTypeId = reader.GetGuid(COL_ID),
                Name = reader.GetString(COL_Name)
            };
            string settings = reader.GetString(COL_Settings);
            if (!String.IsNullOrWhiteSpace(settings))
                bpTaskType.Settings = Serializer.Deserialize<BPTaskTypeSettings>(settings);

            return bpTaskType;
        }
        #endregion
    }
}
