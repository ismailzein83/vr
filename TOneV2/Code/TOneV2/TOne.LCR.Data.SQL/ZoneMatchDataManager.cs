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
            return ExecuteNonQuerySP("LCR.sp_ZoneMatch_UpdateAll", isFuture);
        }

        public int UpdateByCodeDigit(bool isFuture, char firstDigit)
        {
            return ExecuteNonQueryText(String.Format(query_UpdateByCodeDigit, isFuture ? "Future" : "Current", firstDigit), null);
        }

        public void CreateTempTable(bool isFuture)
        {
            ExecuteNonQuerySP("LCR.sp_ZoneMatch_CreateTempTable", isFuture);
        }

        public void SwapTableWithTemp(bool isFuture)
        {
            ExecuteNonQueryText(String.Format(query_SwapTableWithTemp, isFuture ? "Future" : "Current", null), null);
        }

        public void CreateIndexesOnTable(bool isFuture)
        {
            ExecuteNonQuerySP("LCR.sp_ZoneMatch_CreateIndexesOnTempTable", isFuture);
        }

        #region Queries

        const string query_UpdateByCodeDigit = @"WITH newZoneMatch AS (SELECT DISTINCT OC.SupplierZoneID OurZoneID, SC.SupplierZoneID SupplierZoneID, SC.SupplierID
							                      FROM LCR.CodeMatch{0}{1}_temp OC WITH(NOLOCK), LCR.CodeMatch{0}{1}_temp SC WITH(NOLOCK)
							                      WHERE 
									                    OC.Code = SC.Code 
									                    AND OC.SupplierID = 'SYS'
									                    AND SC.SupplierID <> 'SYS'
							                     )
		
		                                        INSERT INTO LCR.ZoneMatch{0}_temp
			                                           ([OurZoneID]
			                                           ,[SupplierZoneID]
			                                           ,[SupplierID])
		                                        SELECT [OurZoneID]
			                                           ,[SupplierZoneID]
			                                           ,[SupplierID]
		                                        FROM newZoneMatch ";

        const string query_SwapTableWithTemp = @"BEGIN TRANSACTION
                                                IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[LCR].[ZoneMatch{0}{1}]') AND type in (N'U'))
                                                EXEC sp_rename 'LCR.ZoneMatch{0}{1}', 'ZoneMatch{0}{1}_Old'
		                                        EXEC sp_rename 'LCR.ZoneMatch{0}{1}_Temp', 'ZoneMatch{0}{1}'
                                                COMMIT TRANSACTION";

        #endregion
    }
}
