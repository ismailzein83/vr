using System.Collections.Generic;
using Vanrise.Entities;


namespace Vanrise.Common.Data
{
    public interface IUserActionAuditDataManager : IDataManager
    {

        void Insert(UserActionAudit userActionAudit);

        List<UserActionAudit> GetAll(UserActionAuditQuery query);
       
    }
}
