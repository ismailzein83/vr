using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Data
{
    public interface IEncryptionKeyDataManager : IDataManager
    {
        string InsertIfNotExistsAndGetEncryptionKey(string keyToInsertIfNotExists);
    }
}
