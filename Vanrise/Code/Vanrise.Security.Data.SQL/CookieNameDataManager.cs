using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Vanrise.Security.Data.SQL
{
    public class CookieNameDataManager : BaseSQLDataManager, ICookieNameDataManager
        
    {
        #region ctor
        public CookieNameDataManager()
            : base(GetConnectionStringName("SecurityDBConnStringKey", "SecurityDBConnString"))
        {

        }

        #endregion

        public string InsertIfNotExistsAndGetCookieName(string nameToInsertIfNotExists)
        {
            return ExecuteScalarSP("[sec].[sp_CookieName_InsertIfNotExistsAndGetCookieName]", nameToInsertIfNotExists) as string;
        }
    }
}
