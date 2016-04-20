using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Data.SQL;

namespace Vanrise.Analytic.Data.SQL
{
    public class AnalyticTableDataManager : BaseSQLDataManager, IAnalyticTableDataManager
    {
        public AnalyticTableDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }


        #region Public Methods
        public bool AreAnalyticTableUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[Analytic].[AnalyticTable]", ref updateHandle);
        }
        public List<AnalyticTable> GetAnalyticTables()
        {
            return GetItemsSP("[Analytic].[sp_AnalyticTable_GetAll]", AnalyticTableReader);
        }

        #endregion


        #region Mappers
        private AnalyticTable AnalyticTableReader(IDataReader reader) 
        {
            return new AnalyticTable
            {
                AnalyticTableId = (int)reader["ID"],
                Name = reader["Name"] as string,
                Settings = Vanrise.Common.Serializer.Deserialize<AnalyticTableSettings>(reader["Settings"] as string),
            };
        }
        #endregion

    }
}
