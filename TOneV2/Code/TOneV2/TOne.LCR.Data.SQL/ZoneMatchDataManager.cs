using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.Data.SQL;

namespace TOne.LCR.Data.SQL
{
    public class ZoneMatchDataManager : RoutingDataManager, IZoneMatchDataManager
    {
        public void UpdateAll()
        {
            ExecuteNonQueryText(query_UpdateAll, null);
        }

        public void CreateIndexesOnTable()
        {
            ExecuteNonQueryText(query_CreateIndexesOnTable, null);
        }

        #region Queries

        const string query_UpdateAll = @" SET NOCOUNT ON;
                                        WITH newZoneMatch AS (SELECT DISTINCT OC.SupplierZoneID OurZoneID, SC.SupplierZoneID SupplierZoneID, SC.SupplierID
							                                  FROM CodeMatch OC WITH(NOLOCK), CodeMatch SC WITH(NOLOCK)
							                                  WHERE 
									                                OC.Code = SC.Code 
									                                AND OC.SupplierID = 'SYS'
									                                AND SC.SupplierID <> 'SYS'
							                                 )
		
		                                INSERT INTO ZoneMatch WITH (TABLOCK)
			                                   ([OurZoneID]
			                                   ,[SupplierZoneID]
			                                   ,[SupplierID])
		                                SELECT [OurZoneID]
			                                   ,[SupplierZoneID]
			                                   ,[SupplierID]
		                                FROM newZoneMatch ";

     
        const string query_CreateIndexesOnTable = @"ALTER TABLE [ZoneMatch] ADD PRIMARY KEY CLUSTERED 
                                                        (
	                                                        [OurZoneID] ASC,
	                                                        [SupplierZoneID] ASC
                                                        )";
               

        #endregion
    }
}
