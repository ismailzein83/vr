using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using TOne.Data.SQL;

namespace TOne.LCR.Data.SQL
{
    public class ZoneRateDataManager : BaseTOneDataManager, IZoneRateDataManager
    {        
        public void CreateAndFillTable(bool isFuture, bool forSupplier, DateTime effectiveOn)
        {
            ExecuteNonQueryText(String.Format(query_CreateAndFillTable, isFuture ? "Future" : "Current", forSupplier ? "Supplier" : "Customer", forSupplier ? "<>" : "="),
                (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@EffectiveOn", effectiveOn));
                });
        }
        public void SwapTableWithTemp(bool isFuture, bool forSupplier)
        {
            ExecuteNonQueryText(String.Format(query_SwapTableWithTemp, isFuture ? "Future" : "Current", forSupplier ? "Supplier" : "Customer"), null);
        }

        #region Queries       

        const string query_CreateAndFillTable = @"IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[LCR].[ZoneRate{0}{1}_old]') AND type in (N'U'))
		                                        DROP TABLE [LCR].[ZoneRate{0}{1}_old]

		                                        IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[LCR].[ZoneRate{0}{1}_temp]') AND type in (N'U'))
		                                        DROP TABLE [LCR].[ZoneRate{0}{1}_temp]	
		
		                                        DECLARE @Account_Inactive tinyint
	                                            DECLARE @Account_Blocked tinyint
	                                            DECLARE @Account_BlockedInbound tinyint
	                                            DECLARE @Account_BlockedOutbound tinyint
	
	                                            -- Set Enumerations
	                                            SELECT @Account_Inactive = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.ActivationStatus' AND Name = 'Inactive'
	                                            SELECT @Account_Blocked = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'Blocked'
	                                            SELECT @Account_BlockedInbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedInbound'	
	                                            SELECT @Account_BlockedOutbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedOutbound'	;
	
	
	                                            WITH CusCarriers AS  
	                                             (
	                                             SELECT S.*,Pr.Tax2 FROM CarrierAccount S WITH(NOLOCK) Left Join CarrierProfile Pr 
	                                             On S.ProfileID=Pr.ProfileID
	                                             WHERE 	S.ActivationStatus <> @Account_Inactive
			                                            And S.RoutingStatus <> @Account_BlockedInbound 
			                                            AND S.RoutingStatus <> @Account_Blocked 
			                                            AND S.IsDeleted = 'N'-- Needed for bug ID 2822
	                                             ) 
	                                            , SupCarriers AS 
	 
	                                             (
	                                             SELECT S.*,Pr.Tax2 FROM CarrierAccount S WITH(NOLOCK) Left Join CarrierProfile Pr 
	                                             On S.ProfileID=Pr.ProfileID
	                                             WHERE 	S.ActivationStatus <> @Account_Inactive 
			                                            AND S.RoutingStatus <> @Account_Blocked 
			                                            AND S.RoutingStatus <> @Account_BlockedOutbound
			                                            AND S.IsDeleted = 'N'-- Needed for bug ID 2822
	                                             ) 
	
	                                            SELECT
                                                         r.RateID, 
                                                         p.PriceListID, 
                                                         r.ZoneID, 
                                                         p.{1}ID AS {1}ID, 
                                                         NormalRate = (r.Rate/ C.LastRate) +
			                                                          Case When CC.Tax2 is null Then 0 Else CASE WHEN P.SupplierID <> 'SYS' THEN 1 ELSE -1 END * (r.Rate/ C.LastRate) * CC.Tax2 / 100.0 End + 
			                                                          Case When Co.SupplierID is null Then 0 Else isnull(CASE WHEN P.SupplierID <> 'SYS' THEN 1 ELSE -1 END * (CASE WHEN isnull(Co.Amount,0)  <> 0 THEN Amount / C.LastRate ELSE (r.Rate/ C.LastRate) * Percentage / 100.0  END),0) End  , 
                                                         r.ServicesFlag
	                                            
		
	                                            INTO LCR.[ZoneRate{0}{1}_temp]
	                                            FROM Rate r WITH (NOLOCK)
	                                            JOIN PriceList p WITH (NOLOCK) ON r.PriceListID = p.PriceListID
	                                            LEFT JOIN Currency C WITH(NOLOCK) ON P.CurrencyID = C.CurrencyID
	                                            JOIN CusCarriers CS WITH(NOLOCK) ON P.CustomerID = CS.CarrierAccountID
	                                            JOIN SupCarriers CC WITH(NOLOCK) ON P.SupplierID = CC.CarrierAccountID
	                                            LEFT JOIN Commission Co WITH(NOLOCK) ON P.SupplierID = Co.SupplierID	
                                                                                        AND P.CustomerID = Co.CustomerID AND r.ZoneID = Co.ZoneID and r.Rate between co.FromRate and co.ToRate	
                                                                                        AND co.BeginEffectiveDate <= @EffectiveOn AND (co.EndEffectiveDate > @EffectiveOn OR co.EndEffectiveDate IS NULL)
                                                WHERE p.SupplierID {2} 'SYS' AND r.BeginEffectiveDate <= @EffectiveOn AND (r.EndEffectiveDate > @EffectiveOn OR r.EndEffectiveDate IS NULL) ";

       
        const string query_SwapTableWithTemp = @"BEGIN TRANSACTION
                                                IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[LCR].[ZoneRate{0}{1}]') AND type in (N'U'))
                                                    EXEC sp_rename 'LCR.ZoneRate{0}{1}', 'ZoneRate{0}{1}_Old'
		                                        EXEC sp_rename 'LCR.ZoneRate{0}{1}_Temp', 'ZoneRate{0}{1}'
                                                COMMIT TRANSACTION";



        #endregion
    }
}
