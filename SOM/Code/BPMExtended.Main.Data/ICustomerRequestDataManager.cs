//using BPMExtended.Main.Entities;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace BPMExtended.Main.Data
//{
//    public interface ICustomerRequestDataManager : IDataManager
//    {
//        void Insert(Guid requestId, Guid requestTypeId, CustomerObjectType customerObjectType, Guid accountOrContactId, string requestTitle, CustomerRequestStatus requestStatus);

//        void UpdateRequestStatus(Guid requestId, CustomerRequestStatus status);

//        List<CustomerRequest> GetRecentCustomerRequests(int nbOfRecords, CustomerObjectType customerObjectType, Guid accountOrContactId, long? lessThanSequenceNb);
//    }
//}
