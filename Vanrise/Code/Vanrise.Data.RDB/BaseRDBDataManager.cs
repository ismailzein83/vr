using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class BaseRDBDataManager : BaseDataManager
    {
        #region ctor

        string _moduleName;

        public BaseRDBDataManager(string moduleName)
            : base()
        {
            _moduleName = moduleName;
        }

        public BaseRDBDataManager(string moduleName, string connectionStringKey)
            : base(connectionStringKey)
        {
            _moduleName = moduleName;
        }

        public BaseRDBDataManager(string moduleName, string connectionString, bool getFromConfigSection)
            : base(connectionString, getFromConfigSection)
        {
            _moduleName = moduleName;
        }

        #endregion
    }
}
