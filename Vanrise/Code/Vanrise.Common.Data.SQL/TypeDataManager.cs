using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common.Data.SQL
{
    public class TypeDataManager : Vanrise.Data.SQL.BaseSQLDataManager, ITypeDataManager
    {
        public TypeDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }
        public int GetTypeId(string type)
        {
            return (int)ExecuteScalarSP("[common].[sp_Type_InsertIfNotExistsAndGetID]", type);
        }
    }
}
