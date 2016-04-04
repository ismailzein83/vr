using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace CDRComparison.Data.SQL
{
    public abstract class BaseCDRDataManager : BaseSQLDataManager,IBaseCDRDataManager
    {
        public BaseCDRDataManager()
            : base(GetConnectionStringName("CDRComparisonDBConnStringKey", "CDRComparisonDBConnString"))
        {

        }
        protected string _tableNameKey;
        public string TableNameKey
        {
            set { _tableNameKey = value; }
        }
        protected string TableName
        {
            get
            {
                if (this._tableNameKey == null)
                    throw new NullReferenceException("_tableNameKey");
                return string.Format("{0}_{1}", this.TableNamePrefix, this._tableNameKey);
            }
        }
        protected abstract string TableNamePrefix { get; }
    }
}
