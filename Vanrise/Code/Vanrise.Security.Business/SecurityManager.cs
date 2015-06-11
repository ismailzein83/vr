using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class SecurityManager
    {
        public AuthenticationToken Authenticate(string email, string password)
        {
            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            User user = dataManager.GetUserbyEmail(email);

            AuthenticationToken result = new AuthenticationToken();
            result.TokenName = "Auth-Token";
            result.Token = null;
            result.UserDisplayName = email;

            if (user.Status != UserStatus.Inactive && user.Password == password)
            {
                SecurityToken userInfo = new SecurityToken
                {
                    UserId = user.UserId
                };
                string encrypted = Common.TempEncryptionHelper.Encrypt(Common.Serializer.Serialize(userInfo));
                result.Token = encrypted;
            }

            return result;
        }
    }
}
