using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
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
        public IEnumerable<RingoMessageCountEntity> GetRingoMessageCountEntityBySender_LastDay(RingoMessageFilter filter)
        {
            return GetItemsText(string.Format(query_RingoMessage_LastDayWithData, BuildConditionClause(filter)), RingoMessageCountMapper, GetCommandAction(filter));
        }
        public IEnumerable<RingoMessageCountEntity> GetSenderRingoMessageRecords_CTE(RingoMessageFilter filter)
        {
            return GetItemsText(string.Format(query_RingoMessage_GetCountSender_CTE, BuildConditionClause(filter)), RingoMessageCountMapper, GetCommandAction(filter));
        }

        public IEnumerable<RingoMessageCountEntity> GetSenderRingoMessageRecords_EightSheet(RingoMessageFilter firstFilter, RingoMessageFilter secondFilter)
        {
            return GetItemsText(string.Format(query_RingoMessage_EighthReportCTE, BuildConditionClause(firstFilter), BuildConditionClause(secondFilter)), RingoMessageCountMapper, GetCommandAction(firstFilter));
        }

        public IEnumerable<SintesiRingoMessageEntity> GetSintesiRingoMessageEntityByRecipient(TCRRingoReportFilter filter)
        {
            return GetItemsText(string.Format(query_GetSintesi_ByICSIRecipient, BuildTCRRecipientConditionClause(filter)), SintesiRingoMessageMapper, GetCommandAction(filter));
        }

        public IEnumerable<SintesiRingoMessageEntity> GetSintesiRingoMessageEntityBySender(TCRRingoReportFilter filter)
        {
            return GetItemsText(string.Format(query_GetSintesi_ByICSISender, BuildTCRSenderConditionClause(filter)), SintesiRingoMessageMapper, GetCommandAction(filter));
        }

        public IEnumerable<DettaglioRingoMessageEntity> GetDettaglioRingoMessageEntityByRecipient(TCRRingoReportFilter filter)
        {
            return GetItemsText(string.Format(query_GetDettaglio_ByICSIRecipient, BuildTCRRecipientConditionClause(filter)), DettaglioRingoMessageMapper, GetCommandAction(filter));
        }

        public IEnumerable<DettaglioRingoMessageEntity> GetDettaglioRingoMessageEntityBySender(TCRRingoReportFilter filter)
        {
            return GetItemsText(string.Format(query_GetDettaglio_ByICSISender, BuildTCRSenderConditionClause(filter)), DettaglioRingoMessageMapper, GetCommandAction(filter));
        }


        #endregion

        #region Private Methods
        string BuildConditionClause(RingoMessageFilter filter)
        {
            StringBuilder condition = new StringBuilder();
            if (!string.IsNullOrEmpty(filter.RecipientNetwork))
                condition.AppendFormat(" and RecipientNetwork= '{0}' ", filter.RecipientNetwork);
            if (!string.IsNullOrEmpty(filter.Recipient))
                condition.AppendFormat(" and Recipient= '{0}' ", filter.Recipient);
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
        string BuildTCRRecipientConditionClause(TCRRingoReportFilter filter)
        {
            StringBuilder condition = new StringBuilder();
            if (filter.Operator != null && filter.Operator.Count > 0)
            {
                var listOperators = filter.Operator.Select(x => "'" + x + "'").ToList();
                condition.AppendFormat(" and Recipient IN ({0}) ", String.Join(",", listOperators));
            }
            if (filter.From.HasValue)
                condition.AppendFormat(" and MessageDate >= @From");
            if (filter.To.HasValue)
                condition.AppendFormat(" and MessageDate < @To");

            return condition.ToString();
        }
        Action<DbCommand> GetCommandAction(RingoMessageFilter filter)
        {
            return (cmd) =>
            {
                if (filter.From.HasValue)
                    cmd.Parameters.Add(new SqlParameter("@From", filter.From));
                if (filter.To.HasValue)
                    cmd.Parameters.Add(new SqlParameter("@To", filter.To));
            };
        }

        Action<DbCommand> GetCommandAction(TCRRingoReportFilter filter)
        {
            return (cmd) =>
            {
                if (filter.From.HasValue)
                    cmd.Parameters.Add(new SqlParameter("@From", filter.From));
                if (filter.To.HasValue)
                    cmd.Parameters.Add(new SqlParameter("@To", filter.To));
            };
        }

        string BuildTCRSenderConditionClause(TCRRingoReportFilter filter)
        {
            StringBuilder condition = new StringBuilder();
            if (filter.Operator != null && filter.Operator.Count > 0)
            {
                var listOperators = filter.Operator.Select(x => "'" + x + "'").ToList();
                condition.AppendFormat(" and Sender IN ({0}) ", String.Join(",", listOperators));
            }
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

        SintesiRingoMessageEntity SintesiRingoMessageMapper(IDataReader reader)
        {
            var messageDateString = reader["MessageDate"] as string;
            return new SintesiRingoMessageEntity
            {
                MessageDate = DateTime.ParseExact(messageDateString, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
                Network = reader["Network"] as string,
                NumberOfRows = (int)reader["NumberOfRows"],
                Operator = reader["Operator"] as string,
                TotalTransferredCredit = (int)reader["TotalTransferredCredit"]
            };
        }

        DettaglioRingoMessageEntity DettaglioRingoMessageMapper(IDataReader reader)
        {
            return new DettaglioRingoMessageEntity
            {
                Network = reader["Network"] as string,
                RecipientRequestCode = reader["RequestCode"] as string,
                Operator = reader["Operator"] as string,
                TransferredCredit = (int)reader["TransferredCredit"]
            };
        }

        #endregion

        #region Queries

        private const string query_GetDettaglio_ByICSIRecipient = @"
            ;WITH ringo AS (
            SELECT distinct [Sender]
                  ,[Recipient]
                  ,[SenderNetwork]
                  ,[RecipientNetwork]
                  ,[MSISDN]
                  ,[RecipientRequestCode]
                  ,CONVERT(VARCHAR(10),[MessageDate],121) MessageDate
                  ,[StateRequest]
                  ,[FlagCredit]
                  ,[TransferredCredit]
                  ,[FlagRequestCreditTransfer]
                  ,[AccountID]
              FROM [Retail_EDR].[RingoMessage]
               WHERE messagetype IN (10,12) {0} )

                -- Ringo --> other
               SELECT Recipient Operator,RecipientNetwork Network,RecipientRequestCode RequestCode, TransferredCredit
               FROM ringo ";

        private const string query_GetDettaglio_ByICSISender = @"
            ;WITH ringo AS (
            SELECT distinct [Sender]
                  ,[Recipient]
                  ,[SenderNetwork]
                  ,[RecipientNetwork]
                  ,[MSISDN]
                  ,[RecipientRequestCode]
                  ,CONVERT(VARCHAR(10),[MessageDate],121) MessageDate
                  ,[StateRequest]
                  ,[FlagCredit]
                  ,[TransferredCredit]
                  ,[FlagRequestCreditTransfer]
                  ,[AccountID]
              FROM [Retail_EDR].[RingoMessage]
               WHERE messagetype IN (10,12) {0} )

            --Other --> Ringo
                 SELECT Sender Operator,SenderNetwork Network,RecipientRequestCode RequestCode, TransferredCredit
               FROM ringo ";

        private const string query_GetSintesi_ByICSIRecipient = @"
    	    ;WITH ringo AS (
            SELECT distinct [Sender]
                    ,[Recipient]
                    ,[SenderNetwork]
                    ,[RecipientNetwork]
                    ,[MSISDN]
                    ,[RecipientRequestCode]
                    ,CONVERT(VARCHAR(10),[MessageDate],121) MessageDate
                    ,[StateRequest]
                    ,[FlagCredit]
                    ,[TransferredCredit]
                    ,[FlagRequestCreditTransfer]
                    ,[AccountID]
                FROM [Retail_EDR].[RingoMessage]
                WHERE messagetype IN (10,12) {0} )

            -- Ringo --> other
            SELECT Recipient Operator,RecipientNetwork Network,MessageDate,COUNT(*) NumberOfRows,SUM(TransferredCredit)  TotalTransferredCredit
                FROM ringo
                GROUP BY Recipient,RecipientNetwork,MessageDate
                ORDER BY Recipient,RecipientNetwork,MessageDate";

        private const string query_GetSintesi_ByICSISender = @"
              ;WITH ringo AS (
            SELECT distinct [Sender]
                  ,[Recipient]
                  ,[SenderNetwork]
                  ,[RecipientNetwork]
                  ,[MSISDN]
                  ,[RecipientRequestCode]
                  ,CONVERT(VARCHAR(10),[MessageDate],121) MessageDate
                  ,[StateRequest]
                  ,[FlagCredit]
                  ,[TransferredCredit]
                  ,[FlagRequestCreditTransfer]
                  ,[AccountID]
              FROM [Retail_EDR].[RingoMessage]
               WHERE messagetype IN (10,12) {0} )

            --Other --> Ringo
              SELECT  Sender Operator,SenderNetwork Network, MessageDate,COUNT(*) NumberOfRows,SUM(TransferredCredit) TotalTransferredCredit
              FROM ringo
              GROUP BY Sender,SenderNetwork,MessageDate
              ORDER BY Sender,SenderNetwork,MessageDate";

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
        const string query_RingoMessage_GetCountSender_CTE = @"
                                                            WITH cte (sender, msisdn, number)
                                                            AS
                                                            ( 
                                                                SELECT  sender, msisdn, count(*)  
                                                                from    [Retail_EDR].[RingoMessage]
                                                                where   1 = 1 {0}
                                                                group by sender,msisdn
                                                            )
                                                            select distinct(sender) Name, count(*) Total from cte
                                                            group by sender";
        const string query_RingoMessage_LastDayWithData = @"
                                                            ;with maxDate as (
                                                            select  Max(CONVERT(date, MessageDate)) as MaxMessageDate
                                                            from [Retail_EDR].[RingoMessage]
                                                            where 1=1 {0}
                                                            )
                                                            SELECT distinct(Sender) as Name, count(*) Total  from maxDate, [Retail_EDR].[RingoMessage]
                                                            where MessageDate>=MaxMessageDate and MessageDate<dateadd(day,1, MaxMessageDate)
                                                            {0}
                                                            group by (Sender)";

        const string query_RingoMessage_EighthReportCTE = @"
                                            with 
                                            cte1 (sender, Msisdn, Messagedate) 
                                            as (
                                                select sender, msisdn,Messagedate from  [Retail_EDR].[RingoMessage]
                                                where 1=1 {0}
                                            )
                                            ,
                                            cte6 (sender, Msisdn, Messagedate) as (
                                            select sender, msisdn,Messagedate from  [Retail_EDR].[RingoMessage]
                                            where 1=1 {1}),
                                            
                                            ctejoin (sender, msisdn,diff) as (
                                                select cte1.sender, cte1.msisdn, 
                                                datediff (dd,cte1.Messagedate,cte6.Messagedate) - (DATEDIFF(wk,cte1.Messagedate,cte6.Messagedate) *2) -        case 
                                                    when datepart(dw, cte1.Messagedate) = 1 
                                                    then 1 
                                                    else 0 
                                                end + 
                                                case 
                                                    when datepart(dw, cte6.Messagedate) = 1 
                                                    then 1 
                                                    else 0 
                                                end
                                            from cte1 join cte6 on cte1.Msisdn=cte6.msisdn 
                                            and cte1.sender=cte6.sender)

                                            select sender Name, AVG(diff) as Total from ctejoin
                                            group by sender";
        #endregion


    }
}
