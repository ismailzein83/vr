using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
    public class VRHttpConnectionLogDataManager : IVRHttpConnectionLogDataManager
    {
        #region Local Variables
        static string TABLE_NAME = "VRHttpConnectionLog";
        static string TABLE_ALIAS = "vrHttpConnectionLog";
        const string COL_ID = "ID";
        const string COL_VRHttpConnectionId = "VRHttpConnectionId";
        const string COL_BaseURL = "BaseURL";
        const string COL_Path = "Path";
        const string COL_Parameters = "Parameters";
        const string COL_RequestHeaders = "RequestHeaders";
        const string COL_RequestBody = "RequestBody";
        const string COL_RequestTime = "RequestTime";
        const string COL_ResponseHeaders = "ResponseHeaders";
        const string COL_Response = "Response";
        const string COL_ResponseTime = "ResponseTime";
        const string COL_ResponseStatusCode = "ResponseStatusCode";
        const string COL_IsSucceded = "IsSucceded";
        const string COL_Exception = "Exception";
        #endregion

        #region Constructors
        static VRHttpConnectionLogDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();

            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_VRHttpConnectionId, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_BaseURL, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_Path, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_Parameters, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_RequestHeaders, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_RequestBody, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_RequestTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_ResponseHeaders, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_Response, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_ResponseTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_ResponseStatusCode, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_IsSucceded, new RDBTableColumnDefinition { DataType = RDBDataType.VarBinary });
            columns.Add(COL_Exception, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "logging",
                DBTableName = "VRHttpConnectionLog",
                Columns = columns,
                IdColumnName = COL_ID
            });
        }
        #endregion

        #region Public Methods

        public bool Insert(Guid VRHttpConnectionId, string BaseURL, string Path, string Parameters, string RequestHeaders, string RequestBody, DateTime RequestTime,
            string ResponseHeaders, string Response, DateTime? ResponseTime, HttpStatusCode? ResponseStatusCode, bool IsSucceded, string Exception)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var inserQuery = queryContext.AddInsertQuery();
            inserQuery.IntoTable(TABLE_NAME);

            inserQuery.Column(COL_VRHttpConnectionId).Value(VRHttpConnectionId);
            inserQuery.Column(COL_BaseURL).Value(BaseURL);
            inserQuery.Column(COL_Path).Value(Path);
            inserQuery.Column(COL_Parameters).Value(Parameters);
            inserQuery.Column(COL_RequestHeaders).Value(RequestHeaders);
            inserQuery.Column(COL_RequestBody).Value(RequestBody);
            inserQuery.Column(COL_RequestTime).Value(RequestTime);
            inserQuery.Column(COL_ResponseHeaders).Value(ResponseHeaders);
            inserQuery.Column(COL_Response).Value(Response);

            if (ResponseTime.HasValue)
                inserQuery.Column(COL_ResponseTime).Value(ResponseTime.Value);
            else
                inserQuery.Column(COL_ResponseTime).Null();

            if (ResponseStatusCode.HasValue)
                inserQuery.Column(COL_ResponseStatusCode).Value((int)ResponseStatusCode.Value);
            else
                inserQuery.Column(COL_ResponseStatusCode).Null();

            inserQuery.Column(COL_IsSucceded).Value(IsSucceded);
            inserQuery.Column(COL_Exception).Value(Exception);

            return queryContext.ExecuteNonQuery() > 0;
        }

        #endregion

        #region Private Methods

        private BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_Common", "LogDBConnStringKey", "LogDBConnString");
        }

        #endregion
    }
}
