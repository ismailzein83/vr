using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Vanrise.Common.Data.SQL
{
    public class DataEncryptionKeyDataManager : BaseSQLDataManager, IDataEncryptionKeyDataManager
    {
       
        #region ctor
        public DataEncryptionKeyDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }
        #endregion

        public string InsertIfNotExistsAndGetDataEncryptionKey(string keyToInsertIfNotExists)
        {
            return ExecuteScalarSP("[Common].[sp_DataEncryptionKey_InsertIfNotExistsAndGetDataEncryptionKey]", keyToInsertIfNotExists) as string;
        }
    }
}
