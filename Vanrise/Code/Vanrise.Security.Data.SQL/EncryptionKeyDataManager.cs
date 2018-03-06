using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Vanrise.Security.Data.SQL
{
    public class EncryptionKeyDataManager : BaseSQLDataManager, IEncryptionKeyDataManager
        
    {
        #region ctor
        public EncryptionKeyDataManager()
            : base(GetConnectionStringName("SecurityDBConnStringKey", "SecurityDBConnString"))
        {

        }

        #endregion

        public string InsertIfNotExistsAndGetEncryptionKey(string keyToInsertIfNotExists)
        {
            return ExecuteScalarSP("[sec].[sp_EncryptionKey_InsertIfNotExistsAndGetEncryptionKey]", keyToInsertIfNotExists) as string;
        }
    }
}
