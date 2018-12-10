using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Data.RDB;

namespace Vanrise.Analytic.Data.RDB
{
    public class VRReportGenerationDataManager : IVRReportGenerationDataManager
    {
        static string TABLE_NAME = "VR_Analytic_VRReportGeneration";
        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_Description = "Description";
        const string COL_Settings = "Settings";
        const string COL_AccessLevel = "AccessLevel";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";
        const string COL_CreatedBy = "CreatedBy";
        const string COL_LastModifiedBy = "LastModifiedBy";

        static VRReportGenerationDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_Description, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 1000 });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_AccessLevel, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_CreatedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_LastModifiedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "Analytic",
                DBTableName = "VRReportGeneration",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime

            });
        }


        #region Private Methods
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_Analytic", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
        }
        VRReportGeneration VRReportGenerationMapper(IRDBDataReader reader)
        {
            return new VRReportGeneration
            {

                ReportId = reader.GetLong(COL_ID),
                Name = reader.GetString(COL_Name),
                Description = reader.GetString(COL_Description),
                Settings =  Vanrise.Common.Serializer.Deserialize<VRReportGenerationSettings>(reader.GetString(COL_Settings)),
                AccessLevel = (AccessLevel)reader.GetInt(COL_AccessLevel),
                CreatedBy = reader.GetInt(COL_CreatedBy),
                CreatedTime = reader.GetDateTime(COL_CreatedTime),
                LastModifiedTime = reader.GetDateTime(COL_LastModifiedTime),
                LastModifiedBy = reader.GetInt(COL_LastModifiedBy),
            };
        }
        #endregion


        #region IVRReportGenerationDataManager
        public bool AreVRReportGenerationUpdated(ref object updateHandle)
        {
            throw new NotImplementedException();
        }

        public List<VRReportGeneration> GetVRReportGenerations()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "vrReportGeneration", null, true);
            selectQuery.SelectColumns().AllTableColumns("vrReportGeneration");
            selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);
            return queryContext.GetItems(VRReportGenerationMapper);
        }

        public bool Insert(VRReportGeneration vrReportGeneration, out long reportId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.AddSelectGeneratedId();
            var ifNotExists = insertQuery.IfNotExists("vrReportGeneration");
            ifNotExists.EqualsCondition(COL_Name).Value(vrReportGeneration.Name);

            insertQuery.Column(COL_Description).Value(vrReportGeneration.Description);
            insertQuery.Column(COL_Name).Value(vrReportGeneration.Name);
            insertQuery.Column(COL_AccessLevel).Value((int)vrReportGeneration.AccessLevel);
            insertQuery.Column(COL_CreatedBy).Value(vrReportGeneration.CreatedBy);
            insertQuery.Column(COL_LastModifiedBy).Value(vrReportGeneration.LastModifiedBy);

            if (vrReportGeneration.Settings != null)
                insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(vrReportGeneration.Settings));

            var insertedID = queryContext.ExecuteScalar().NullableLongValue;
            if(insertedID.HasValue)
            {
                reportId = insertedID.Value;
                return true;
            }
            reportId = -1;
            return false;
        }

        public bool Update(VRReportGeneration vrReportGeneration)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var notExistsCondition = updateQuery.IfNotExists("vrReportGeneration");
            notExistsCondition.NotEqualsCondition(COL_ID).Value(vrReportGeneration.ReportId);
            notExistsCondition.EqualsCondition(COL_Name).Value(vrReportGeneration.Name);

            updateQuery.Column(COL_Name).Value(vrReportGeneration.Name);
            updateQuery.Column(COL_Description).Value(vrReportGeneration.Description);
            updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(vrReportGeneration.Settings));
            updateQuery.Column(COL_AccessLevel).Value((int)vrReportGeneration.AccessLevel);
            updateQuery.Column(COL_LastModifiedBy).Value(vrReportGeneration.LastModifiedBy);

            updateQuery.Where().EqualsCondition(COL_ID).Value(vrReportGeneration.ReportId);

            return queryContext.ExecuteNonQuery() > 0;
        }
        #endregion

    }
}
