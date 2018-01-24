//using BPMExtended.Main.Entities;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.SqlClient;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Vanrise.Data.SQL;

//namespace BPMExtended.Main.Data.SQL
//{
//    public class CustomerRequestDataManager : BaseSQLDataManager, ICustomerRequestDataManager 
//    {
//        #region ctor/Local Variables

//        public CustomerRequestDataManager()
//            : base(GetConnectionStringName("BPMExtended_DBConnStringKey", "BPMExtendedDBConnString"))
//        {

//        }

//        #endregion

//        #region Public Methods

//        public void Insert(Guid requestId, Guid requestTypeId, CustomerObjectType customerObjectType, Guid accountOrContactId, string requestTitle, CustomerRequestStatus requestStatus)
//        {
//            string query = @"INSERT INTO [BPMExtended].[CustomerRequest]
//           ([ID]
//           ,[RequestTypeID]
//           ,[CustomerObjectType]
//           ,[AccountOrContactID]
//           ,[Title]
//           ,[Status]
//           ,[CreatedTime]
//           ,[LastModifiedTime])
//     VALUES
//           (@ID
//           ,@RequestTypeID
//           ,@CustomerObjectType
//           ,@AccountOrContactID
//           ,@Title
//           ,@Status
//           ,GETDATE()
//           ,GETDATE())";
//            ExecuteNonQueryText(query, (cmd) =>
//                {
//                    cmd.Parameters.Add(new SqlParameter("@ID", requestId));
//                    cmd.Parameters.Add(new SqlParameter("@RequestTypeID", requestTypeId));
//                    cmd.Parameters.Add(new SqlParameter("@CustomerObjectType", (int)customerObjectType));
//                    cmd.Parameters.Add(new SqlParameter("@AccountOrContactID", accountOrContactId));
//                    cmd.Parameters.Add(new SqlParameter("@Title", requestTitle));
//                    cmd.Parameters.Add(new SqlParameter("@Status", (int)requestStatus));
//                });
//        }

//        public void UpdateRequestStatus(Guid requestId, Entities.CustomerRequestStatus status)
//        {
//            string query = @"UPDATE [BPMExtended].[CustomerRequest]
//           SET Status = @Status,
//               LastModifiedTime = GETDATE()
//           WHERE ID = @ID";
//            ExecuteNonQueryText(query, (cmd) =>
//            {
//                cmd.Parameters.Add(new SqlParameter("@ID", requestId));
//                cmd.Parameters.Add(new SqlParameter("@Status", (int)status));
//            });
//        }

//        public List<Entities.CustomerRequest> GetRecentCustomerRequests(int nbOfRecords, CustomerObjectType customerObjectType, Guid accountOrContactId, long? lessThanSequenceNb)
//        {
//            StringBuilder queryBuilder = new StringBuilder( @"SELECT TOP (@NbOfRecords) [ID]
//      ,[SequenceNumber]
//      ,[RequestTypeID]
//      ,[CustomerObjectType]
//      ,[AccountOrContactID]
//      ,[Title]
//      ,[Status]
//      ,[CreatedTime]
//      ,[LastModifiedTime]
//  FROM [BPMExtended].[CustomerRequest] WITH(NOLOCK)
//  WHERE CustomerObjectType = @CustomerObjectType AND AccountOrContactID = @AccountOrContactID #SEQUENCEFILTER#
//  ORDER BY SequenceNumber DESC");
//            if (lessThanSequenceNb.HasValue)
//                queryBuilder.Replace("#SEQUENCEFILTER#", " AND SequenceNumber < @SequenceNumber");
//            else
//                queryBuilder.Replace("#SEQUENCEFILTER#", "");
//            return GetItemsText(queryBuilder.ToString(), CustomerRequestMapper, (cmd) =>
//                {
//                    cmd.Parameters.Add(new SqlParameter("@NbOfRecords", nbOfRecords));
//                    cmd.Parameters.Add(new SqlParameter("@CustomerObjectType", (int)customerObjectType));
//                    cmd.Parameters.Add(new SqlParameter("@AccountOrContactID", accountOrContactId));
//                    if (lessThanSequenceNb.HasValue)
//                        cmd.Parameters.Add(new SqlParameter("@SequenceNumber", lessThanSequenceNb.Value));
//                });
//        }

//        #endregion

//        #region Private Mehods
        
//        private CustomerRequest CustomerRequestMapper(IDataReader reader)
//        {
//            return new CustomerRequest
//            {
//                CustomerRequestId = (Guid)reader["ID"],
//                RequestTypeId = (Guid)reader["RequestTypeID"],
//                CustomerObjectType = (CustomerObjectType)reader["CustomerObjectType"],
//                AccountOrContactId = (Guid)reader["AccountOrContactID"],
//                Title = reader["Title"] as string,
//                Status = (CustomerRequestStatus)reader["Status"],
//                SequenceNumber = GetReaderValue<long>(reader, "SequenceNumber"),
//                CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime"),
//                LastModifiedTime = GetReaderValue<DateTime>(reader, "LastModifiedTime")
//            };
//        }

//        #endregion
//    }
//}
