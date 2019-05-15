﻿using System;
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

        public bool AddAnalyticTable(Entities.AnalyticTable analyticTable)
        {
            object analyticTableID;

            int recordesEffected = ExecuteNonQuerySP("Analytic.sp_AnalyticTable_Insert", analyticTable.AnalyticTableId, analyticTable.Name, Vanrise.Common.Serializer.Serialize(analyticTable.Settings));
            return (recordesEffected > 0);
        }

        public bool UpdateAnalyticTable(Entities.AnalyticTable analyticTable)
        {
            int recordesEffected = ExecuteNonQuerySP("Analytic.sp_AnalyticTable_Update", analyticTable.AnalyticTableId, analyticTable.Name, Vanrise.Common.Serializer.Serialize(analyticTable.Settings));
            return (recordesEffected > 0);
        }
        public bool SaveAnalyticTableMeasureStyles(AnalyticTableMeasureStyles measureStyles,Guid analyticTableId)
        {
            int recordesEffected = ExecuteNonQuerySP("Analytic.sp_AnalyticTable_SaveMeasureStyles", analyticTableId, Vanrise.Common.Serializer.Serialize(measureStyles));
            return (recordesEffected > 0);
        }
        public bool SaveAnalyticTablePermanentFilter(AnalyticTablePermanentFilter permanentFilter, Guid analyticTableId)
        {
            var serializedPermanentFilter = permanentFilter != null ? Vanrise.Common.Serializer.Serialize(permanentFilter) : null;
            int recordesEffected = ExecuteNonQuerySP("Analytic.sp_AnalyticTable_SavePermanentFilter", analyticTableId, serializedPermanentFilter);
            return (recordesEffected > 0);
        }

        #endregion


        #region Mappers
        private AnalyticTable AnalyticTableReader(IDataReader reader) 
        {
            return new AnalyticTable
            {
                AnalyticTableId = (Guid)reader["ID"],
                DevProjectId = GetReaderValue<Guid?>(reader, "DevProjectID"),
                Name = reader["Name"] as string,
                Settings = Vanrise.Common.Serializer.Deserialize<AnalyticTableSettings>(reader["Settings"] as string),
                MeasureStyles = Vanrise.Common.Serializer.Deserialize<AnalyticTableMeasureStyles>(reader["MeasureStyles"] as string),
                PermanentFilter = Vanrise.Common.Serializer.Deserialize<AnalyticTablePermanentFilter>(reader["PermanentFilter"] as string)
            };
        }
        #endregion

    }
}
