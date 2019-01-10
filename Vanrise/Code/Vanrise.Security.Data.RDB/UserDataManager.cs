using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Security.Entities;
using Vanrise.Entities;
namespace Vanrise.Security.Data.RDB
{
    public class UserDataManager : IUserDataManager
    {
        #region RDB
        static string TABLE_NAME = "sec_User";
        static string TABLE_ALIAS = "user";
        const string COL_ID = "ID";
        const string COL_SecurityProviderId = "SecurityProviderId";
        internal const string COL_Name = "Name";
        const string COL_Password = "Password";
        const string COL_Email = "Email";
        const string COL_TenantId = "TenantId";
        const string COL_LastLogin = "LastLogin";
        const string COL_Description = "Description";
        const string COL_TempPassword = "TempPassword";
        const string COL_TempPasswordValidTill = "TempPasswordValidTill";
        const string COL_EnabledTill = "EnabledTill";
        const string COL_DisabledTill = "DisabledTill";
        const string COL_Settings = "Settings";
        const string COL_ExtendedSettings = "ExtendedSettings";
        const string COL_IsSystemUser = "IsSystemUser";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_CreatedBy = "CreatedBy";
        const string COL_LastModifiedBy = "LastModifiedBy";
        const string COL_LastModifiedTime = "LastModifiedTime";
        const string COL_PasswordChangeTime = "PasswordChangeTime";


        static UserDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_SecurityProviderId, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_Password, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_Email, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_TenantId, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_LastLogin, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_Description, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 1000 });
            columns.Add(COL_TempPassword, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_TempPasswordValidTill, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_EnabledTill, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_DisabledTill, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_ExtendedSettings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_IsSystemUser, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_CreatedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_LastModifiedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_PasswordChangeTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "sec",
                DBTableName = "User",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime

            });
        }
        #endregion

        #region Mappers
        private User UserMapper(IRDBDataReader reader)
        {
            var user = new User
            {
                UserId = reader.GetInt(COL_ID),
                SecurityProviderId = reader.GetGuid(COL_SecurityProviderId),
                Name = reader.GetString(COL_Name),
                Email = reader.GetString(COL_Email),
                LastLogin = reader.GetNullableDateTime(COL_LastLogin),
                EnabledTill = reader.GetNullableDateTime(COL_EnabledTill),
                DisabledTill = reader.GetNullableDateTime(COL_DisabledTill),
                Description = reader.GetString(COL_Description),
                TenantId = reader.GetInt(COL_TenantId),
                CreatedTime = reader.GetNullableDateTime(COL_CreatedTime),
                CreatedBy = reader.GetNullableInt(COL_CreatedBy),
                LastModifiedBy = reader.GetNullableInt(COL_LastModifiedBy),
                LastModifiedTime = reader.GetNullableDateTime(COL_LastModifiedTime),
                Settings = Common.Serializer.Deserialize<UserSetting>(reader.GetString(COL_Settings)),
                ExtendedSettings = Common.Serializer.Deserialize<Dictionary<string, object>>(reader.GetString(COL_ExtendedSettings))
            };
            var isSystemUser = reader.GetNullableBoolean(COL_IsSystemUser);
            if (isSystemUser.HasValue)
                user.IsSystemUser = isSystemUser.Value;
            var passwordChangeTime = reader.GetNullableDateTime(COL_PasswordChangeTime);
            if (passwordChangeTime.HasValue)
                user.PasswordChangeTime = passwordChangeTime.Value;
            return user;

        }
        #endregion
       
        #region Private Methods
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_Sec", "SecurityDBConnStringKey", "SecurityDBConnString");
        }
        #endregion

        #region IUserDataManager
        public bool ActivatePassword(string email, string password, int lastModifiedBy)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_Password).Value(password);
            updateQuery.Column(COL_TempPassword).Null();
            updateQuery.Column(COL_TempPasswordValidTill).Null();
            updateQuery.Column(COL_LastModifiedBy).Value(lastModifiedBy);
            updateQuery.Column(COL_PasswordChangeTime).DateNow();
            updateQuery.Where().EqualsCondition(COL_Email).Value(email);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool AddUser(User user, string tempPassword, out int insertedId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.AddSelectGeneratedId();
            insertQuery.IfNotExists(TABLE_ALIAS).EqualsCondition(COL_Email).Value(user.Email);
            insertQuery.Column(COL_SecurityProviderId).Value(user.SecurityProviderId);
            insertQuery.Column(COL_Name).Value(user.Name);
            insertQuery.Column(COL_TempPassword).Value(tempPassword);
            insertQuery.Column(COL_Email).Value(user.Email);
            insertQuery.Column(COL_Description).Value(user.Description);
            insertQuery.Column(COL_TenantId).Value(user.TenantId);
            if (user.EnabledTill.HasValue)
                insertQuery.Column(COL_EnabledTill).Value(user.EnabledTill.Value);
            if (user.ExtendedSettings != null)
                insertQuery.Column(COL_ExtendedSettings).Value(Common.Serializer.Serialize(user.ExtendedSettings));
            if (user.Settings != null)
                insertQuery.Column(COL_Settings).Value(Common.Serializer.Serialize(user.Settings));
            if (user.CreatedBy.HasValue)
                insertQuery.Column(COL_CreatedBy).Value(user.CreatedBy.Value);
            if (user.LastModifiedBy.HasValue)
                insertQuery.Column(COL_LastModifiedBy).Value(user.LastModifiedBy.Value);
            var id = queryContext.ExecuteScalar().NullableIntValue;
            if (id.HasValue)
                insertedId = id.Value;
            else
                insertedId = -1;
            return insertedId != -1;
        }

        public bool AreUsersUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public bool ChangePassword(int userId, string newPassword, int lastModifiedBy)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_Password).Value(newPassword);
            updateQuery.Column(COL_LastModifiedBy).Value(lastModifiedBy);
            updateQuery.Column(COL_PasswordChangeTime).DateNow();
            updateQuery.Where().EqualsCondition(COL_ID).Value(userId);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool ChangeUserSecurityProvider(int userId, Guid securityProviderId, string Email, string encryptedPassword, UserSetting userSettings, int lastModifiedBy)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            var ifNotExists = updateQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.NotEqualsCondition(COL_ID).Value(userId);
            ifNotExists.EqualsCondition(COL_Email).Value(Email);
            updateQuery.Column(COL_SecurityProviderId).Value(securityProviderId);
            updateQuery.Column(COL_Email).Value(Email);
            updateQuery.Column(COL_Password).Value(encryptedPassword);
            if (userSettings != null)
                updateQuery.Column(COL_Settings).Value(Common.Serializer.Serialize(userSettings));
            else
                updateQuery.Column(COL_Settings).Null();
            updateQuery.Column(COL_LastModifiedBy).Value(lastModifiedBy);
            updateQuery.Where().EqualsCondition(COL_ID).Value(userId);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool DisableUser(int userID, int lastModifiedBy)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_EnabledTill).Value(DateTime.Now.AddMinutes(-5));
            updateQuery.Column(COL_LastModifiedBy).Value(lastModifiedBy);
            updateQuery.Where().EqualsCondition(COL_ID).Value(userID);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool EditUserProfile(string name, int userId, UserSetting settings, int lastModifiedBy)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_Name).Value(name);
            if (settings != null)
                updateQuery.Column(COL_Settings).Value(Common.Serializer.Serialize(settings));
            else
                updateQuery.Column(COL_Settings).Null();
            updateQuery.Column(COL_LastModifiedBy).Value(lastModifiedBy);
            updateQuery.Where().EqualsCondition(COL_ID).Value(userId);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool EnableUser(int userID, int lastModifiedBy)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_EnabledTill).Null();
            updateQuery.Column(COL_LastModifiedBy).Value(lastModifiedBy);
            updateQuery.Where().EqualsCondition(COL_ID).Value(userID);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public string GetUserPassword(int userId, out DateTime passwordChangeTime)
        {
            string password = null;
            passwordChangeTime = DateTime.MinValue;
            DateTime passwordChangeTime_local = DateTime.MinValue;

            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            selectQuery.Where().EqualsCondition(COL_ID).Value(userId);
            queryContext.ExecuteReader(reader =>
            {
                if (reader.Read())
                {
                    password = reader.GetString(COL_Password);
                    var changeTime = reader.GetNullableDateTime(COL_PasswordChangeTime);
                    if (changeTime.HasValue)
                    {
                        passwordChangeTime_local = changeTime.Value;
                    }
                }
            });
            return password;
        }

        public List<User> GetUsers()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);
            return queryContext.GetItems(UserMapper);
        }

        public string GetUserTempPassword(int userId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().Column(COL_TempPassword);
            var whereQuery = selectQuery.Where();
            whereQuery.EqualsCondition(COL_ID).Value(userId);
            var subCondition = whereQuery.ChildConditionGroup(RDBConditionGroupOperator.OR);
            subCondition.NullCondition(COL_TempPasswordValidTill);
            subCondition.GreaterThanCondition(COL_TempPasswordValidTill).DateNow();
            return queryContext.ExecuteScalar().StringValue;
        }

        public bool ResetPassword(int userId, string password, int lastModifiedBy)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_Password).Value(password);
            updateQuery.Column(COL_LastModifiedBy).Value(lastModifiedBy);
            updateQuery.Column(COL_PasswordChangeTime).DateNow();
            updateQuery.Where().EqualsCondition(COL_ID).Value(userId);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool UnlockUser(int userID, int lastModifiedBy)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_DisabledTill).Null();
            updateQuery.Column(COL_LastModifiedBy).Value(lastModifiedBy);
            updateQuery.Where().EqualsCondition(COL_ID).Value(userID);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool UpdateDisableTill(int userID, DateTime disableTill, int lastModifiedBy)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_DisabledTill).Value(disableTill);
            updateQuery.Column(COL_LastModifiedBy).Value(lastModifiedBy);
            updateQuery.Where().EqualsCondition(COL_ID).Value(userID);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool UpdateLastLogin(int userID, int lastModifiedBy)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_LastLogin).DateNow();
            updateQuery.Column(COL_LastModifiedBy).Value(lastModifiedBy);
            updateQuery.Where().EqualsCondition(COL_ID).Value(userID);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool UpdateTempPasswordByEmail(string email, string password, DateTime? passwordValidTill, int lastModifiedBy)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_TempPassword).Value(password);
            if (passwordValidTill.HasValue)
                updateQuery.Column(COL_TempPasswordValidTill).Value(passwordValidTill.Value);
            else
                updateQuery.Column(COL_TempPasswordValidTill).Null();
            updateQuery.Column(COL_LastModifiedBy).Value(lastModifiedBy);
            updateQuery.Where().EqualsCondition(COL_Email).Value(email);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool UpdateTempPasswordById(int userId, string password, DateTime? passwordValidTill, int lastModifiedBy)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_TempPassword).Value(password);
            if (passwordValidTill.HasValue)
                updateQuery.Column(COL_TempPasswordValidTill).Value(passwordValidTill.Value);
            else
                updateQuery.Column(COL_TempPasswordValidTill).Null();
            updateQuery.Column(COL_LastModifiedBy).Value(lastModifiedBy);
            updateQuery.Where().EqualsCondition(COL_ID).Value(userId);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool UpdateUser(User user)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            var ifNotExists = updateQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.NotEqualsCondition(COL_ID).Value(user.UserId);
            ifNotExists.EqualsCondition(COL_Email).Value(user.Email);
            updateQuery.Column(COL_Name).Value(user.Name);
            updateQuery.Column(COL_Email).Value(user.Email);
            updateQuery.Column(COL_SecurityProviderId).Value(user.SecurityProviderId);
            updateQuery.Column(COL_Description).Value(user.Description);
            updateQuery.Column(COL_TenantId).Value(user.TenantId);
            if (user.EnabledTill.HasValue)
                updateQuery.Column(COL_EnabledTill).Value(user.EnabledTill.Value);
            else
                updateQuery.Column(COL_EnabledTill).Null();
            if (user.Settings != null)
                updateQuery.Column(COL_Settings).Value(Common.Serializer.Serialize(user.Settings));
            else
                updateQuery.Column(COL_Settings).Null();
            if (user.LastModifiedBy.HasValue)
                updateQuery.Column(COL_LastModifiedBy).Value(user.LastModifiedBy.Value);
            else
                updateQuery.Column(COL_LastModifiedBy).Null();
            updateQuery.Where().EqualsCondition(COL_ID).Value(user.UserId);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool UpdateUserSetting(UserSetting userSetting, int userId, int lastModifiedBy)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            if (userSetting != null)
                updateQuery.Column(COL_Settings).Value(Common.Serializer.Serialize(userSetting));
            else
                updateQuery.Column(COL_Settings).Null();
            updateQuery.Column(COL_LastModifiedBy).Value(lastModifiedBy);
            updateQuery.Where().EqualsCondition(COL_ID).Value(userId);
            return queryContext.ExecuteNonQuery() > 0;
        }
        #endregion

        #region PermissionDataManager
        public void SetJoinContext(RDBJoinContext joinContext, string table1Alias, string table2Alias, string column1Name, RDBJoinType joinType)
        {
            joinContext.JoinOnEqualOtherTableColumn(joinType, TABLE_NAME, table2Alias, COL_ID, table1Alias, column1Name);
        }
        #endregion
    }
}
