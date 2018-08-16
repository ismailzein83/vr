using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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
            string configuredDirectory = ConfigurationManager.AppSettings["BCPTempFilesDirectory"];
            if (!String.IsNullOrEmpty(configuredDirectory))
            {
                string filePath = Path.Combine(configuredDirectory, Guid.NewGuid().ToString());
                using (var stream = File.Create(filePath))
                {
                    stream.Close();
                }
                return filePath;
            }
            else
                return Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        }
    }
}
