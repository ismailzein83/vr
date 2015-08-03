
CREATE PROCEDURE [dbo].[bp_RT_Part_BuildLCR_Route]
@IncludeBlockedZones BIT = 1,
@BatchID INT 
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @State_Enabled tinyint
	SET @State_Enabled = 1
	DECLARE @Message varchar(500) 
	

;WITH
   	RoutesToInsert AS (		
			SELECT
				R.CustomerID CustomerID,
				R.ProfileID ProfileID,
				rcp.Code Code,
				rcp.ZoneID OurZoneID,
				R.ActiveRate OurActiveRate,
				R.ServicesFlag OurServicesFlag,
				@State_Enabled State,
				ISNULL( R.IsTOD, 0) IsTOD,
				r.IsBlock
			FROM #RoutePool rcp  WITH (NOLOCK) 
			INNER JOIN #SaleZoneRates R WITH (NOLOCK) ON  rcp.ZoneID = R.ZoneID
			 WHERE  
					@IncludeBlockedZones= 1 OR (@IncludeBlockedZones= 0 AND r.IsBlock=0)
			 

    	)
Insert INTO #Routes with(TABLOCK) 
			SELECT 
			rt.[CustomerID]
           ,rt.[ProfileID]
           ,rt.[Code]
           ,rt.[OurZoneID]
           ,rt.[OurActiveRate]
           ,rt.[OurServicesFlag]
           ,rt.[State]
           ,GETDATE() [Updated]
           ,rt.[IsToD]
           ,isnull(rn.IsSpecialRequestAffected,0) 
			,ISNULL(rn.IsOverrideAffected,0) 
			,ISNULL(rn.IsBlockAffected,rt.IsBlock) 
			,ISNULL(rn.IsOptionBlock,0)
			,@BatchID BatchID
			
			FROM RoutesToInsert rt
			LEFT JOIN #Route_NonLCR rn ON rt.CustomerID COLLATE DATABASE_DEFAULT = rn.CustomerID AND rt.Code COLLATE DATABASE_DEFAULT = rn.Code --AND rt.OurZoneID = rn.OurZoneID
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutesPartial: Routes Inserted', @Message
	
--Testing
SELECT 'LCR Route', * FROM #Routes r	
END