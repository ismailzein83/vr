using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Retail.Ringo.Entities;
using Vanrise.Data.SQL;

namespace Retail.Ringo.Data.SQL
{
    public class RingoMessageDataManager : BaseSQLDataManager, IRingoMessageDataManager
    {
        #region Constructor
        public RingoMessageDataManager()
            : base(GetConnectionStringName("RetailCDRDBConnStringKey", "RetailCDRDBConnString"))
        {

        }

        #endregion

        #region IRingoMessageDataManager Implementation
        public long GetTotal(RingoMessageFilter filter)
        {
            return Convert.ToInt64(ExecuteScalarText(string.Format(query_GetTotal_ByICSTSender, BuildConditionClause(filter)), GetCommandAction(filter)));
        }
        public IEnumerable<RingoMessageCountEntity> GetRingoMessageCountEntityByRecipient(RingoMessageFilter filter)
        {
            return GetItemsText(string.Format(query_RingoMessage_GetCountByRecipient, BuildConditionClause(filter)), RingoMessageCountMapper, GetCommandAction(filter));
        }
        public IEnumerable<RingoMessageCountEntity> GetRingoMessageCountEntityBySender(RingoMessageFilter filter)
        {
            return GetItemsText(string.Format(query_RingoMessage_GetCountBySender, BuildConditionClause(filter)), RingoMessageCountMapper, GetCommandAction(filter));
        }
        public IEnumerable<RingoMessageCountEntity> GetRingoMessageCountEntityByRecipient_CTE(RingoMessageFilter filter)
        {
            return GetItemsText(string.Format(query_RingoMessage_GetCountRecipient_CTE, BuildConditionClause(filter)), RingoMessageCountMapper, GetCommandAction(filter));
        }

        private static Action<System.Data.Common.DbCommand> GetCommandAction(RingoMessageFilter filter)
        {
            return (cmd) =>
            {
                if (filter.From.HasValue)
                    cmd.Parameters.Add(new SqlParameter("@From", filter.From));
                if (filter.To.HasValue)
                    cmd.Parameters.Add(new SqlParameter("@To", filter.To));
            };
        }

        #endregion

        #region Private Methods
        string BuildConditionClause(RingoMessageFilter filter)
        {
            StringBuilder condition = new StringBuilder();
            if (!string.IsNullOrEmpty(filter.RecipientNetwork))
                condition.AppendFormat(" and RecipientNetwork= '{0}' ", filter.RecipientNetwork);
            if (!string.IsNullOrEmpty(filter.SenderNetwork))
                condition.AppendFormat(" and SenderNetwork= '{0}' ", filter.SenderNetwork);
            if (filter.StateRequests != null && filter.StateRequests.Count > 0)
                condition.AppendFormat(" and StateRequest in({0}) ", BuildInStatement(filter.StateRequests));
            if (filter.MessageTypes != null && filter.MessageTypes.Count > 0)
                condition.AppendFormat(" and MessageType in({0}) ", BuildInStatement(filter.MessageTypes));
            if (!string.IsNullOrEmpty(filter.Sender))
                condition.AppendFormat(" and Sender= '{0}' ", filter.Sender);
            if (!string.IsNullOrEmpty(filter.RecipientNetwork))
                condition.AppendFormat(" and RecipientNetwork= '{0}' ", filter.RecipientNetwork);
            if (filter.From.HasValue)
                condition.AppendFormat(" and MessageDate >= @From");
            if (filter.To.HasValue)
                condition.AppendFormat(" and MessageDate < @To");

            return condition.ToString();
        }
        string BuildInStatement(List<int> lstIds)
        {
            return string.Join(",", lstIds);
        }

        #endregion

        #region Mappers
        RingoMessageCountEntity RingoMessageCountMapper(IDataReader reader)
        {
            return new RingoMessageCountEntity
            {
                Name = reader["Name"] as string,
                Value = (int)reader["Total"]
            };
        }

        #endregion

        #region Queries

        const string query_GetTotal_ByICSTSender = @"
    	                                            SELECT	count(*) 
	                                                from	[Retail_EDR].[RingoMessage] 
	                                                where	1 = 1 {0}";


        const string query_RingoMessage_GetCountByRecipient = @"
                                                            SELECT      Recipient Name, count(*) Total
                                                            From        [Retail_EDR].[RingoMessage] 
                                                            where       1 = 1 {0}
                                                            group by    Recipient";

        const string query_RingoMessage_GetCountBySender = @"
                                                            SELECT      Sender Name, count(*) Total
                                                            From        [Retail_EDR].[RingoMessage] 
                                                            where       1 = 1 {0} 
                                                            group by    Sender";

        const string query_RingoMessage_GetCountRecipient_CTE = @"
                                                            WITH cte (recipient, msisdn, number)
                                                            AS
                                                            ( 
                                                                SELECT  Recipient, msisdn, count(*)  
                                                                from    [Retail_EDR].[RingoMessage]
                                                                where   1 = 1 {0}
                                                                group by Recipient,msisdn
                                                            )
                                                            select distinct(recipient) Name, count(*) Total from cte
                                                            group by recipient";

        #endregion
    }
}
