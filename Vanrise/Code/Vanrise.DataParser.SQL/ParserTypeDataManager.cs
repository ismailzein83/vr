using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Vanrise.DataParser.Data.SQL
{
    public class ParserTypeDataManager : BaseSQLDataManager, IParserTypeDataManager
    { 
        #region ctor
        public ParserTypeDataManager()
            : base(GetConnectionStringName("DataParserDBConnStringKey", "DataParserDBConnString"))
        {

        }
        #endregion
    }
}
