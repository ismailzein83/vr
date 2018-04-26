using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Security.Entities;
using Vanrise.Common;

namespace Vanrise.Security.Data.SQL
{
    public class UserDataManager : BaseSQLDataManager, IUserDataManager
    {
        #region ctor
        public UserDataManager()
            : base(GetConnectionStringName("SecurityDBConnStringKey", "SecurityDBConnString"))
        {

        }

        #endregion

        #region Public Methods


        public List<User> GetUsers()
        {
            return GetItemsSP("sec.sp_User_GetAll", UserMapper);
        }
        public bool UpdateUserSetting(UserSetting userSetting, int userId, int lastModifiedBy)
        {
            int recordesEffected = ExecuteNonQuerySP("sec.sp_User_UpdateSetting", Serializer.Serialize(userSetting), userId, lastModifiedBy);
            return (recordesEffected > 0);
        }
        public string GetUserPassword(int userId)
        {
            return ExecuteScalarSP("sec.sp_User_GetPassword", userId) as string;
        }

        public string GetUserTempPassword(int userId)
        {
            return ExecuteScalarSP("sec.sp_User_GetTempPassword", userId) as string;
        }

        public bool AddUser(User userObject, string tempPassword, out int insertedId)
        {
            object userID;

            int recordesEffected = ExecuteNonQuerySP("sec.sp_User_Insert", out userID, userObject.Name, tempPassword, userObject.Email, userObject.Description, userObject.TenantId, userObject.EnabledTill, userObject.ExtendedSettings != null ? Vanrise.Common.Serializer.Serialize(userObject.ExtendedSettings) : null, userObject.Settings != null ? Vanrise.Common.Serializer.Serialize(userObject.Settings) : null, userObject.CreatedBy, userObject.LastModifiedBy);
            insertedId = (recordesEffected > 0) ? (int)userID : -1;

            return (recordesEffected > 0);
        }

        public bool UpdateUser(User userObject)
        {
            int recordesEffected = ExecuteNonQuerySP("sec.sp_User_Update", userObject.UserId, userObject.Name, userObject.Email, userObject.Description, userObject.TenantId, userObject.EnabledTill, userObject.Settings != null ? Vanrise.Common.Serializer.Serialize(userObject.Settings) : null, userObject.LastModifiedBy);
            return (recordesEffected > 0);
        }

        public bool DisableUser(int userID, int lastModifiedBy)
        {
            int recordesEffected = ExecuteNonQuerySP("sec.sp_User_SetDisable", userID, lastModifiedBy);
            return (recordesEffected > 0);
        }


        public bool EnableUser(int userID, int lastModifiedBy)
        {
            int recordesEffected = ExecuteNonQuerySP("sec.sp_User_SetEnable", userID, lastModifiedBy);
            return (recordesEffected > 0);
        }

        public bool UnlockUser(int userID, int lastModifiedBy)
        {
            int recordesEffected = ExecuteNonQuerySP("sec.sp_User_Unlock", userID, lastModifiedBy);
            return (recordesEffected > 0);
        }
        public bool UpdateLastLogin(int userID, int lastModifiedBy)
        {
            int recordsAffected = (int)ExecuteNonQuerySP("sec.sp_User_UpdateLastLogin", userID, lastModifiedBy);
            return (recordsAffected > 0);
        }
        public bool UpdateDisableTill(int userId, DateTime disableTill, int lastModifiedBy)
        {
            int recordsAffected = (int)ExecuteNonQuerySP("sec.sp_User_UpdateDisableTill", userId, disableTill, lastModifiedBy);
            return (recordsAffected > 0);
        }
        public bool ResetPassword(int userId, string password, int lastModifiedBy)
        {
            return ExecuteNonQuerySP("sec.sp_User_UpdatePassword", userId, password, lastModifiedBy) > 0;
        }

        public bool ActivatePassword(string email, string password, int lastModifiedBy)
        {
            return ExecuteNonQuerySP("sec.sp_User_ActivatePassword", email, password,  lastModifiedBy) > 0;
        }

        public bool UpdateTempPasswordById(int userId, string password, DateTime? passwordValidTill, int lastModifiedBy)
        {
            return ExecuteNonQuerySP("sec.sp_User_UpdateTempPasswordById", userId, password, passwordValidTill, lastModifiedBy) > 0;
        }

        public bool UpdateTempPasswordByEmail(string email, string password, DateTime? passwordValidTill, int lastModifiedBy)
        {
            return ExecuteNonQuerySP("sec.sp_User_UpdateTempPasswordByEmail", email, password, passwordValidTill, lastModifiedBy) > 0;
        }

        public bool ChangePassword(int userId, string newPassword, int lastModifiedBy)
        {
            int recordsAffected = ExecuteNonQuerySP("sec.sp_User_UpdatePassword", userId, newPassword, lastModifiedBy);
            return (recordsAffected > 0);
        }

        public bool EditUserProfile(string name, int userId ,UserSetting settings, int lastModifiedBy)
        {
            return ExecuteNonQuerySP("sec.sp_User_UpdateProfile", userId, name, settings != null ? Vanrise.Common.Serializer.Serialize(settings) : null, lastModifiedBy) > 0;
        }

        public bool AreUsersUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("sec.[User]", ref updateHandle);
        }

        #endregion

        #region Mappers

        private User UserMapper(IDataReader reader)
        {
            return new Entities.User
            {
                UserId = Convert.ToInt32(reader["Id"]),
                Name = reader["Name"] as string,
                Email = reader["Email"] as string,
                LastLogin = GetReaderValue<DateTime?>(reader, "LastLogin"),
                EnabledTill = GetReaderValue<DateTime?>(reader, "EnabledTill"),
                DisabledTill = GetReaderValue<DateTime?>(reader, "DisabledTill"),
                Description = reader["Description"] as string,
                TenantId = Convert.ToInt32(reader["TenantId"]),
                Settings = Vanrise.Common.Serializer.Deserialize<UserSetting>(reader["Settings"] as string),
                IsSystemUser = GetReaderValue<bool>(reader, "IsSystemUser"),
                ExtendedSettings = reader["ExtendedSettings"] != DBNull.Value ? Vanrise.Common.Serializer.Deserialize<Dictionary<string, object>>(reader["ExtendedSettings"] as string) : null,
                CreatedTime = GetReaderValue<DateTime?>(reader, "CreatedTime"),
                CreatedBy = GetReaderValue<int?>(reader, "CreatedBy"),
                LastModifiedBy = GetReaderValue<int?>(reader, "LastModifiedBy"),
                LastModifiedTime = GetReaderValue<DateTime?>(reader, "LastModifiedTime"),
                PasswordChangeTime = GetReaderValue<DateTime>(reader, "PasswordChangeTime")
            };
        }

        #endregion
    }

    //public enum RDBDataType { Varchar, NVarchar, Int, BigInt, Decimal, DateTime}

    //public class RDBTable
    //{
    //    public string TableName { get; set; }

    //    public Dictionary<string, RDBTableColumn>   Columns {get;set;}
    //}

    //public class RDBTableColumn
    //{
    //    public string ColumnName { get; set; }

    //    public RDBDataType DataType { get; set; }

    //    public int? Size { get; set; }
    //}

    //public class RDBQueryBuilderTest
    //{
    //    RDBTable _userTable = new RDBTable
    //    {
    //        TableName = "sec.User",
    //        Columns = new Dictionary<string, RDBTableColumn>
    //         {
    //              {"ID", new RDBTableColumn { ColumnName = "ID", DataType = RDBDataType.Int}},
    //              {"Name", new RDBTableColumn { ColumnName = "Name", DataType = RDBDataType.NVarchar, Size = 255}}
    //         }
    //    };

    //    static void test()
    //    {
    //        var query = new RDBQueryBuilder().Select().FromTable("User", "u").Columns("ID", "u.Name", "Settings", "g.Name")
    //            .Join("Group","g", RDBJoinType.Inner)
    //            .IntCondition("", RDBConditionOperator.G, 0).And().DecimalCondition("", RDBConditionOperator.G, 3).EndJoin()
    //            .Where()
    //            .TextCondition("Name", RDBConditionOperator.Eq, "Sami")
    //            .And().ConditionGroup().IntCondition("ID", RDBConditionOperator.L, 5).Or().IntCondition("ID", RDBConditionOperator.G, 10).EndConditionGroup()
    //            .And().DecimalCondition("Age", RDBConditionOperator.G, 24).EndWhere();
    //    }
    //}

    //public class RDBQueryBuilder
    //{
    //    StringBuilder _builder = new StringBuilder();
    //    public RDBSelect Select()
    //    {
    //        return new RDBSelect(_builder);
    //    }


    //}

    //public class RDBInsert
    //{
    //    StringBuilder _builder;
    //    public RDBInsert(StringBuilder builder)
    //    {
    //        _builder = builder;
    //    }

    //    public RDBInsertTable IntoTable(string tableName)
    //    {
    //        _builder.AppendFormat("INSERT INTO {0}", tableName);
    //        return new RDBInsertTable(_builder);
    //    }
    //}

    //public class RDBInsertTable
    //{
    //    StringBuilder _builder;

    //    public RDBInsertTable(StringBuilder builder)
    //    {
    //        _builder = builder;
    //    }

    //    public void Value(string columnName, Object value)
    //    {

    //    }

    //    public void DBNow(string columnName)
    //    {

    //    }

    //    public void DBToday(string columnName)
    //    {

    //    }
    //}

    //public class RDBSelect
    //{
    //    StringBuilder _builder;
    //    public RDBSelect(StringBuilder builder)
    //    {
    //        _builder = builder;
    //    }

    //    public RDBSelectTable FromTable(string tableName, string alias)
    //    {
    //        _builder.AppendFormat("{0} {1}", tableName, alias);
    //        return new RDBSelectTable(_builder);
    //    }

    //    public RDBSelectTable FromTable(string tableName)
    //    {
    //        return FromTable(tableName, null);
    //    }
    //}

    //public class RDBSelectTable
    //{
    //    StringBuilder _builder;

    //    public RDBSelectTable(StringBuilder builder)
    //    {
    //        _builder = builder;
    //    }

    //    public RDBSelectTableWithColumns Columns(params string[] columnNames)
    //    {            
    //        for (int i = 0; i < columnNames.Length; i++)
    //        {
    //            _builder.Append(columnNames[i]);
    //            if (i < columnNames.Length - 1)
    //                _builder.Append(", ");
    //        }
    //        return new RDBSelectTableWithColumns(_builder);
    //    }

    //    public RDBSelectTableWithColumns Column(string column, string alias)
    //    {
    //        _builder.Append(String.Format("{0} {1}", column, alias));
    //        return new RDBSelectTableWithColumns(_builder);
    //    }
    //}

    //public class RDBSelectTableWithColumns
    //{
    //    StringBuilder _builder;

    //    public RDBSelectTableWithColumns(StringBuilder builder)
    //    {
    //        _builder = builder;
    //    }

    //    public RDBSelectTableWithColumns Columns(params string[] columnNames)
    //    {
    //        for (int i = 0; i < columnNames.Length; i++)
    //        {
    //            _builder.Append(columnNames[i]);
    //            if (i < columnNames.Length - 1)
    //                _builder.Append(", ");
    //        }
    //        return this;
    //    }

    //    public RDBSelectTableWithColumns Column(string column, string alias)
    //    {
    //        _builder.Append(String.Format("{0} {1}", column, alias));
    //        return this;
    //    }

    //    public RDBJoin Join(string tableName, string alias, RDBJoinType joinType)
    //    {
    //        string joinStatement = joinType == RDBJoinType.Inner ? "JOIN " : "LEFT JOIN ";
    //        _builder.AppendFormat("{0} {1} {2}", joinStatement, tableName, alias);
    //        return new RDBJoin(_builder);
    //    }

    //    public RDBSelectWhere Where()
    //    {
    //        _builder.AppendLine("WHERE ");
    //        return new RDBSelectWhere(_builder);
    //    }

    //}

    //public class RDBSelectTableWithJoin
    //{
    //    StringBuilder _builder;
    //    public RDBSelectTableWithJoin(StringBuilder builder)
    //    {
    //        _builder = builder;
    //    }
    //    public RDBSelectWhere Where()
    //    {
    //        _builder.AppendLine("WHERE ");
    //        return new RDBSelectWhere(_builder);
    //    }
    //}

    //public class RDBSelectTableWithWhere
    //{
    //    StringBuilder _builder;
    //    public RDBSelectTableWithWhere(StringBuilder builder)
    //    {
    //        _builder = builder;
    //    }
    //}

    //public enum RDBJoinType { Inner = 0, Left = 1 }

    //public enum RDBConditionOperator { Eq = 0, NEq = 1, G = 2, GEq = 3, L = 4, LEq = 5, IN = 6, NotIN = 7 }
    //public abstract class BaseRDBConditionGroup<T>
    //{
    //    #region ctor/Fields

    //    protected StringBuilder _builder;
    //    public BaseRDBConditionGroup(StringBuilder builder)
    //    {
    //        _builder = builder;
    //    }

    //    #endregion

    //    #region Public Methods

    //    protected abstract T ReturnThis();

    //    public T TextCondition(string columnName, RDBConditionOperator condOperator, params string[] values)
    //    {
    //        return Condition<string>(columnName, condOperator, (val) => string.Format("'{0}'", val), values);
    //    }

    //    public T IntCondition(string columnName, RDBConditionOperator condOperator, params int[] values)
    //    {
    //        return Condition<int>(columnName, condOperator, (val) => val.ToString(), values);
    //    }

    //    public T LongCondition(string columnName, RDBConditionOperator condOperator, params long[] values)
    //    {
    //        return Condition<long>(columnName, condOperator, (val) => val.ToString(), values);
    //    }

    //    public T DecimalCondition(string columnName, RDBConditionOperator condOperator, params decimal[] values)
    //    {
    //        return Condition<decimal>(columnName, condOperator, (val) => val.ToString(), values);
    //    }

    //    public RDBConditionGroup<T> ConditionGroup()
    //    {
    //        _builder.Append(" (");
    //        return new RDBConditionGroup<T>(_builder, ReturnThis());
    //    }

    //    #endregion

    //    #region Private Methods

    //    T Condition<Q>(string columnName, RDBConditionOperator condOperator, Func<Q, string> valueToQuery, params Q[] values)
    //    {
    //        _builder.AppendFormat("{0} {1} ", columnName, GetConditionOperatorText(condOperator));
    //        if (condOperator == RDBConditionOperator.IN || condOperator == RDBConditionOperator.NotIN)
    //        {
    //            _builder.Append("(");
    //            for (int i = 0; i < values.Length; i++)
    //            {
    //                _builder.Append(valueToQuery(values[i]));
    //                if (i < values.Length - 1)
    //                    _builder.Append(", ");
    //            }
    //            _builder.Append(")");
    //        }
    //        else
    //        {
    //            _builder.AppendFormat(valueToQuery(values[0]));
    //        }
    //        return ReturnThis();
    //    }

    //    string GetConditionOperatorText(RDBConditionOperator condOperator)
    //    {
    //        switch (condOperator)
    //        {
    //            case RDBConditionOperator.Eq: return "=";
    //            case RDBConditionOperator.NEq: return "<>";
    //            case RDBConditionOperator.G: return ">";
    //            case RDBConditionOperator.GEq: return ">=";
    //            case RDBConditionOperator.L: return "<";
    //            case RDBConditionOperator.LEq: return "<=";
    //            case RDBConditionOperator.IN: return "IN";
    //            case RDBConditionOperator.NotIN: return "NOT IN";
    //            default: throw new NotSupportedException(string.Format("condOperator '{0}'", condOperator));
    //        }
    //    }

    //    #endregion
    //}

    //public abstract class RDBCondition<T>
    //{
    //     #region ctor/Fields

    //    protected StringBuilder _builder;
    //    public RDBCondition(StringBuilder builder)
    //    {
    //        _builder = builder;
    //    }

    //    #endregion

    //    protected abstract T ReturnThis();

    //    public RDBLogicalOperator<T> And()
    //    {
    //        return new RDBLogicalOperator<T>(_builder, ReturnThis());
    //    }

    //    public RDBLogicalOperator<T> Or()
    //    {
    //        return new RDBLogicalOperator<T>(_builder, ReturnThis());
    //    }
    //}

    //public class RDBLogicalOperator<T> : BaseRDBConditionGroup<T>
    //{
    //    #region ctor/Fields

    //    protected StringBuilder _builder;
    //    T _originalObj;
    //    public RDBLogicalOperator(StringBuilder builder, T originalObj) : base(builder)
    //    {
    //        _builder = builder;
    //        _originalObj = originalObj;
    //    }

    //    #endregion

    //    protected override T ReturnThis()
    //    {
    //        return _originalObj;
    //    }
    //}

    //public class RDBConditionGroup<Q> : BaseRDBConditionGroup<RDBConditionGroupReady<Q>>
    //{
    //    #region ctor/Fields

    //    Q _originalObject;

    //    public RDBConditionGroup(StringBuilder builder, Q originalObject)
    //        : base(builder)
    //    {
    //        _originalObject = originalObject;
    //    }

    //    #endregion

    //    protected override RDBConditionGroupReady<Q> ReturnThis()
    //    {
    //        return new RDBConditionGroupReady<Q>(_builder, _originalObject);
    //    }

    //}

    //public class RDBConditionGroupReady<Q> : RDBCondition<RDBConditionGroupReady<Q>>
    //{
    //    #region ctor/Fields

    //    Q _originalObject;

    //    public RDBConditionGroupReady(StringBuilder builder, Q originalObject)
    //        : base(builder)
    //    {
    //        _originalObject = originalObject;
    //    }

    //    #endregion

    //    protected override RDBConditionGroupReady<Q> ReturnThis()
    //    {
    //        return this;
    //    }
    //    public Q EndConditionGroup()
    //    {
    //        _builder.Append(" )");
    //        return _originalObject;
    //    }
    //}

    //public class RDBSelectWhere : BaseRDBConditionGroup<RDBSelectWhereReady>
    //{
    //    #region ctor/Fields

    //    public RDBSelectWhere(StringBuilder builder)
    //        : base(builder)
    //    {
    //    }

    //    #endregion

    //    protected override RDBSelectWhereReady ReturnThis()
    //    {
    //        return new RDBSelectWhereReady(_builder);
    //    }
    //}

    //public class RDBSelectWhereReady : RDBCondition<RDBSelectWhereReady>
    //{
    //    #region ctor/Fields
    //    public RDBSelectWhereReady(StringBuilder builder) : base(builder)
    //    {
    //    }

    //    #endregion

    //    protected override RDBSelectWhereReady ReturnThis()
    //    {
    //        return this;
    //    }
    //    public RDBSelectTableWithWhere EndWhere()
    //    {
    //        return new RDBSelectTableWithWhere(_builder);
    //    }
    //}

    //public class RDBJoin : BaseRDBConditionGroup<RDBJoinReady>
    //{
    //    #region ctor/Fields

    //    public RDBJoin(StringBuilder builder)
    //        : base(builder)
    //    {
    //    }

    //    #endregion

    //    protected override RDBJoinReady ReturnThis()
    //    {
    //        return new RDBJoinReady(_builder);
    //    }
    //}

    //public class RDBJoinReady : RDBCondition<RDBJoinReady>
    //{
    //    #region ctor/Fields
    //    public RDBJoinReady(StringBuilder builder)
    //        : base(builder)
    //    {
    //    }

    //    #endregion

    //    protected override RDBJoinReady ReturnThis()
    //    {
    //        return this;
    //    }
    //    public RDBSelectTableWithJoin EndJoin()
    //    {
    //        return new RDBSelectTableWithJoin(_builder);
    //    }
    //}
}