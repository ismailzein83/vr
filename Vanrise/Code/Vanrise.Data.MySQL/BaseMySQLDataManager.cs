using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.MySQL
{
    public class BaseMySQLDataManager : BaseDataManager
    {
        #region ctor

        public BaseMySQLDataManager()
            : base()
        {
        }

        public BaseMySQLDataManager(string connectionStringKey)
            : base(connectionStringKey)
        {            
        }

        #endregion
        protected StreamForBulkInsert InitializeStreamForBulkInsert()
        {
            string filePath = GetFilePathForBulkInsert();
            return new StreamForBulkInsert(filePath);
        }

        protected string GetFilePathForBulkInsert()
        {
            return System.IO.Path.GetTempFileName();
        }
    }
}
