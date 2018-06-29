using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;

namespace Vanrise.Common.Business
{
    public class DataEncryptionKeyManager
    {

        static string s_localTokenDecryptionKey;
        static Object s_GetLocalTokenDecryptionKey_LockObj = new object();
        public static string GetLocalTokenDataDecryptionKey()
        {
            if (s_localTokenDecryptionKey == null)
            {
                lock (s_GetLocalTokenDecryptionKey_LockObj)
                {
                    if (s_localTokenDecryptionKey == null)
                    {
                        var dataManager = CommonDataManagerFactory.GetDataManager<IDataEncryptionKeyDataManager>();
                        string keyToInsertIfNotExists = Guid.NewGuid().ToString();
                        s_localTokenDecryptionKey = dataManager.InsertIfNotExistsAndGetDataEncryptionKey(keyToInsertIfNotExists);
                    }
                }
            }
            return s_localTokenDecryptionKey;
        }
    }
}
