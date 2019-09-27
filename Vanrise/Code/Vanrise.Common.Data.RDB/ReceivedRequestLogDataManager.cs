using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
    public class ReceivedRequestLogDataManager : IReceivedRequestLogDataManager
    {
        #region Local Variables
        static string TABLE_NAME = "logging_ActionLogs";
        static string TABLE_ALIAS = "vrLoggingActionLogs";
        const string COL_ID = "ID";
        const string COL_ActionName = "ActionName";
        const string COL_Method = "Method";
        const string COL_ModuleName = "ModuleName";
        const string COL_ControllerName = "ControllerName";
        const string COL_URI = "URI";
        const string COL_Path = "Path";
        const string COL_RequestHeader = "RequestHeader";
        const string COL_Arguments = "Arguments";
        const string COL_RequestBody = "RequestBody";
        const string COL_ResponseHeader = "ResponseHeader";
        const string COL_ResponseStatusCode = "ResponseStatusCode";
        const string COL_IsSucceded = "IsSucceded";
        const string COL_BodyResponse = "BodyResponse";
        const string COL_UserId = "UserId";
        const string COL_StartDateTime = "StartDateTime";
        const string COL_EndDateTime = "EndDateTime";
        #endregion

        #region Contructors
        static ReceivedRequestLogDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_ActionName, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 50 });
            columns.Add(COL_Method, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 50 });
            columns.Add(COL_ModuleName, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 50 });
            columns.Add(COL_ControllerName, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 50 });
            columns.Add(COL_URI, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_Path, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_RequestHeader, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_Arguments, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 900 });
            columns.Add(COL_RequestBody, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_ResponseHeader, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_ResponseStatusCode, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 10 });
            columns.Add(COL_IsSucceded, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add(COL_BodyResponse, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_UserId, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_StartDateTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_EndDateTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "logging",
                DBTableName = "ActionLogs",
                Columns = columns,
                CreatedTimeColumnName = COL_StartDateTime
            });
        }
        #endregion

        #region Public Methods
        public void Insert(string actionName, string method, string moduleName, string controllerName, string uri, string path, string requestHeader, string arguments, string requestBody, string responseHeader, string responseStatusCode, bool isSucceded, string bodyResponse, DateTime startDateTime, int? userId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();

            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.Column(COL_ActionName).Value(actionName);
            insertQuery.Column(COL_Method).Value(method);
            insertQuery.Column(COL_ModuleName).Value(moduleName);
            insertQuery.Column(COL_ControllerName).Value(controllerName);
            insertQuery.Column(COL_URI).Value(uri);
            insertQuery.Column(COL_Path).Value(path);
            insertQuery.Column(COL_RequestHeader).Value(requestHeader);
            insertQuery.Column(COL_Arguments).Value(arguments);
            insertQuery.Column(COL_RequestBody).Value(requestBody);
            insertQuery.Column(COL_ResponseHeader).Value(responseHeader);
            insertQuery.Column(COL_ResponseStatusCode).Value(responseStatusCode);
            insertQuery.Column(COL_IsSucceded).Value(isSucceded);
            insertQuery.Column(COL_BodyResponse).Value(bodyResponse);
            if (userId.HasValue)
                insertQuery.Column(COL_UserId).Value(userId.Value);
            insertQuery.Column(COL_StartDateTime).Value(startDateTime);
            insertQuery.Column(COL_EndDateTime).DateNow();

            queryContext.ExecuteNonQuery();
        }

        #endregion

        #region Private Methods
        private BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_Logging", "LoggingDBConnStringKey", "LogDBConnString");
        }
        #endregion
    }
}
