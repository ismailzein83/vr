using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.Data.SQL;

namespace TOne.LCR.Data.SQL
{
    public class ZoneMatchDataManager : BaseTOneDataManager, IZoneMatchDataManager
    {
        public int UpdateAll(bool isFuture)
        {
            return ExecuteNonQueryText(String.Format(query_UpdateAll, isFuture ? "Future" : "Current"), null);
        }


        public void CreateTempTable(bool isFuture)
        {
            ExecuteNonQueryText(String.Format(query_CreateTempTable, isFuture ? "Future" : "Current"), null);
        }

        public void SwapTableWithTemp(bool isFuture)
        {
            ExecuteNonQueryText(String.Format(query_SwapTableWithTemp, isFuture ? "Future" : "Current"), null);
        }

        public void CreateIndexesOnTable(bool isFuture)
        {
            ExecuteNonQueryText(String.Format(query_CreateIndexesOnTempTable, isFuture ? "Future" : "Current"), null);
        }

        #region Queries

        const string query_UpdateAll = @"WITH newZoneMatch AS (SELECT DISTINCT OC.SupplierZoneID OurZoneID, SC.SupplierZoneID SupplierZoneID, SC.SupplierID
							                                  FROM LCR.CodeMatch{0}_temp OC WITH(NOLOCK), LCR.CodeMatch{0}_temp SC WITH(NOLOCK)
							                                  WHERE 
									                                OC.Code = SC.Code 
									                                AND OC.SupplierID = 'SYS'
									                                AND SC.SupplierID <> 'SYS'
							                                 )
		
		                                INSERT INTO LCR.ZoneMatch{0}_temp WITH (TABLOCK)
			                                   ([OurZoneID]
			                                   ,[SupplierZoneID]
			                                   ,[SupplierID])
		                                SELECT [OurZoneID]
			                                   ,[SupplierZoneID]
			                                   ,[SupplierID]
		                                FROM newZoneMatch ";

        const string query_CreateTempTable = @"IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[LCR].[ZoneMatch{0}_old]') AND type in (N'U'))
		                                        DROP TABLE [LCR].[ZoneMatch{0}_old]

		                                        IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[LCR].[ZoneMatch{0}_temp]') AND type in (N'U'))
		                                        DROP TABLE [LCR].[ZoneMatch{0}_temp]	
		
		                                        CREATE TABLE [LCR].[ZoneMatch{0}_temp](
			                                        [OurZoneID] [int] NOT NULL,
			                                        [SupplierZoneID] [int] NOT NULL,
			                                        [SupplierID] [varchar](5) NOT NULL
		                                        ) ON [PRIMARY]";

        const string query_CreateIndexesOnTempTable = @"ALTER TABLE [LCR].[ZoneMatch{0}_temp] ADD PRIMARY KEY CLUSTERED 
                                                        (
	                                                        [OurZoneID] ASC,
	                                                        [SupplierZoneID] ASC
                                                        )";

        const string query_SwapTableWithTemp = @"BEGIN TRANSACTION
                                                IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[LCR].[ZoneMatch{0}]') AND type in (N'U'))
                                                    EXEC sp_rename 'LCR.ZoneMatch{0}', 'ZoneMatch{0}_Old'
		                                        EXEC sp_rename 'LCR.ZoneMatch{0}_Temp', 'ZoneMatch{0}'
                                                COMMIT TRANSACTION";

       

        #endregion
    }
}
