using System;
using Vanrise.Common;
using Vanrise.Data.RDB;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class CarrierProfileDataManager : ICarrierProfileDataManager
    {
        static string TABLE_NAME = "TOneWhS_BE_CarrierProfile";
        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_Settings = "Settings";
        const string COL_ExtendedSettings = "ExtendedSettings";
        const string COL_SourceID = "SourceID";
        const string COL_IsDeleted = "IsDeleted";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_CreatedBy = "CreatedBy";
        const string COL_LastModifiedBy = "LastModifiedBy";
        const string COL_LastModifiedTime = "LastModifiedTime";
        static string TABLE_ALIAS = "cp";

        static CarrierProfileDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_ExtendedSettings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_SourceID, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add(COL_IsDeleted, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_CreatedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_LastModifiedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "CarrierProfile",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("TOneWhS_BE", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
        }

        #region ICarrierProfileDataManager Members
        public List<CarrierProfile> GetCarrierProfiles()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            return queryContext.GetItems(CarrierProfileMapper);
        }

        public bool Insert(CarrierProfile carrierProfile, out int carrierProfileId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.AddSelectGeneratedId();

            var notExistsCondition = insertQuery.IfNotExists(TABLE_ALIAS);
            notExistsCondition.EqualsCondition(COL_Name).Value(carrierProfile.Name);
            notExistsCondition.ConditionIfColumnNotNull(COL_IsDeleted).EqualsCondition(COL_IsDeleted).Value(false);

            insertQuery.Column(COL_Name).Value(carrierProfile.Name);

            if (carrierProfile.Settings != null)
                insertQuery.Column(COL_Settings).Value(Serializer.Serialize(carrierProfile.Settings));

            if (carrierProfile.CreatedBy.HasValue)
                insertQuery.Column(COL_CreatedBy).Value(carrierProfile.CreatedBy.Value);

            if (carrierProfile.LastModifiedBy.HasValue)
                insertQuery.Column(COL_LastModifiedBy).Value(carrierProfile.LastModifiedBy.Value);

            var returnedValue = queryContext.ExecuteScalar().NullableIntValue;
            if (returnedValue.HasValue)
            {
                carrierProfileId = returnedValue.Value;
                return true;
            }
            carrierProfileId = 0;
            return false;
        }

        public bool Update(CarrierProfileToEdit carrierProfile)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var notExistsCondition = updateQuery.IfNotExists(TABLE_ALIAS);
            notExistsCondition.EqualsCondition(COL_Name).Value(carrierProfile.Name);
            notExistsCondition.NotEqualsCondition(COL_ID).Value(carrierProfile.CarrierProfileId);
            notExistsCondition.ConditionIfColumnNotNull(COL_IsDeleted).EqualsCondition(COL_IsDeleted).Value(false);

            updateQuery.Column(COL_Name).Value(carrierProfile.Name);

            if (carrierProfile.Settings != null)
                updateQuery.Column(COL_Settings).Value(Serializer.Serialize(carrierProfile.Settings));
            else
                updateQuery.Column(COL_Settings).Null();

            if (carrierProfile.LastModifiedBy.HasValue)
                updateQuery.Column(COL_LastModifiedBy).Value(carrierProfile.LastModifiedBy.Value);
            else
                updateQuery.Column(COL_LastModifiedBy).Null();

            updateQuery.Where().EqualsCondition(COL_ID).Value(carrierProfile.CarrierProfileId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool AreCarrierProfilesUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public bool UpdateExtendedSettings(int carrierProfileId, Dictionary<string, object> extendedSettings)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            if (extendedSettings != null)
                updateQuery.Column(COL_ExtendedSettings).Value(Serializer.Serialize(extendedSettings));
            else
                updateQuery.Column(COL_ExtendedSettings).Null();

            updateQuery.Where().EqualsCondition(COL_ID).Value(carrierProfileId);
            return queryContext.ExecuteNonQuery() > 0;
        }

        #endregion
        #region Mapper

        private CarrierProfile CarrierProfileMapper(IRDBDataReader reader)
        {
            return new CarrierProfile
            {
                CarrierProfileId = reader.GetInt(COL_ID),
                Name = reader.GetString(COL_Name),
                Settings = Serializer.Deserialize<CarrierProfileSettings>(reader.GetString(COL_Settings)),
                SourceId = reader.GetString(COL_SourceID),
                CreatedTime = reader.GetDateTime(COL_CreatedTime),
                ExtendedSettings = Serializer.Deserialize<Dictionary<string, Object>>(reader.GetString(COL_ExtendedSettings)),
                IsDeleted = reader.GetBoolean(COL_IsDeleted),
                CreatedBy = reader.GetNullableInt(COL_CreatedBy),
                LastModifiedBy = reader.GetNullableInt(COL_LastModifiedBy),
                LastModifiedTime = reader.GetNullableDateTime(COL_LastModifiedTime)
            };
        }

        #endregion
    }
}
