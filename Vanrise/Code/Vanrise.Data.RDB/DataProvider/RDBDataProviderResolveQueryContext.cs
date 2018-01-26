using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public abstract class RDBDataProviderResolveQueryContext : IRDBDataProviderResolveQueryContext
    {
        BaseRDBDataProvider _dataProvider;
        public RDBDataProviderResolveQueryContext(BaseRDBDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        int _prmIndex;

        public string GenerateParameterName()
        {
            return String.Concat(_dataProvider.ParameterNamePrefix, "Prm_", _prmIndex++);
        }

        int _tableAliasIndex;

        public string GenerateTableAlias()
        {
            return String.Concat("Table_", _tableAliasIndex++);
        }
    }
}
