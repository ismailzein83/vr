using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Data.SQL;

namespace TOne.Data.SQL
{
    public class BaseTOneDataManager : BaseSQLDataManager
    {
        public BaseTOneDataManager(string connectionStringKey)
            : base(connectionStringKey)
        {
        }

        public BaseTOneDataManager()
            : base()
        {
        }
    }
}
