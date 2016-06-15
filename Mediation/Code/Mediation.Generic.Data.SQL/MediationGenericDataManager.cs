using System;
using Vanrise.Data.SQL;

namespace Mediation.Generic.Data.SQL
{
    public class MediationGenericDataManager : BaseSQLDataManager, IMediationGenericDataManager
    {
        public MediationGenericDataManager()
            : base(GetConnectionStringName("MediationDBConnStringKey", "MediationDBConnString"))
        {

        }
    }
}
