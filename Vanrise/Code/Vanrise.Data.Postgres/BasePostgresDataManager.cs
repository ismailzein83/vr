using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.Postgres
{
    public class BasePostgresDataManager : BaseDataManager
    {
        #region ctor

        public BasePostgresDataManager()
            : base()
        {

        }

        public BasePostgresDataManager(string connectionStringKey)
            : base(connectionStringKey)
        {

        }

        #endregion


    }
}
