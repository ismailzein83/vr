using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Vanrise.Common.Data;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using System.Drawing;
using System.IO;
using Vanrise.Security.Entities;
using Vanrise.Security.Business;
namespace Vanrise.Common.Business
{
    public class UserActionAuditManager
	{
         ISecurityContext _securityContext;

         public UserActionAuditManager()
        {
            _securityContext = ContextFactory.GetContext();
        }
		#region Public Methods
       
        public void AddUserActionAudit(string ActionUrl, string BaseUrl)
        {
            List<string> Urlparts = ActionUrl.Split('/').ToList();
            int? userId; 
            UserActionAudit UserActionAudit = new UserActionAudit()
            {
                UserId = _securityContext.TryGetLoggedInUserId(out userId) ? userId : null,
                ModuleName = Urlparts[2],
                ControllerName = Urlparts[3],
                ActionName = Urlparts[4],
                BaseUrl = BaseUrl
            };
            IUserActionAuditDataManager dataManager = CommonDataManagerFactory.GetDataManager<IUserActionAuditDataManager>();
            dataManager.Insert(UserActionAudit);

        }

        public Vanrise.Entities.IDataRetrievalResult<UserActionAuditDetail> GetFilteredUserActionAudits(Vanrise.Entities.DataRetrievalInput<UserActionAuditQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new UserActionAuditRequestHandler());

        }
		
		#endregion

		#region Private Classes

        private class UserActionAuditRequestHandler : BigDataRequestHandler<UserActionAuditQuery, UserActionAudit, UserActionAuditDetail>
        {
            public override UserActionAuditDetail EntityDetailMapper(UserActionAudit entity)
            {
                UserActionAuditManager manager = new UserActionAuditManager();
                return manager.UserActionAuditDetailMapper(entity);
            }

            public override IEnumerable<UserActionAudit> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<UserActionAuditQuery> input)
            {
                IUserActionAuditDataManager dataManager = CommonDataManagerFactory.GetDataManager<IUserActionAuditDataManager>();
                return  dataManager.GetAll(input.Query);

                

            }
        }
		
		#endregion


        #region Private Mappers
        private UserActionAuditDetail UserActionAuditDetailMapper(UserActionAudit userActionAudit)
        {
            UserActionAuditDetail userActionAuditDetail = new UserActionAuditDetail();
            userActionAuditDetail.Entity = userActionAudit;
            User user = null;
            if(userActionAudit.UserId.HasValue)
                 user = new UserManager().GetUserbyId(userActionAudit.UserId.Value);
            if (user != null)
                userActionAuditDetail.UserName = user.Name;

            return userActionAuditDetail;
        }

        #endregion
	}
}
