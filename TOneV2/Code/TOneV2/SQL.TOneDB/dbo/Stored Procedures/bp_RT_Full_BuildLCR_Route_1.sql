
CREATE PROCEDURE [dbo].[bp_RT_Full_BuildLCR_Route]
	 @RoutingTableFileGroup nvarchar(255) = 'PRIMARY'
	,@RoutingIndexesFileGroup nvarchar(255) = 'PRIMARY'
	,@SORT_IN_TEMPDB nvarchar(10) = 'ON'
	,@IncludeBlockedZones CHAR(1) = 'N' 
	,@CheckRouteBlocks CHAR(1) = 'N'
	,@UpdateStamp datetime output
WITH RECOMPILE
AS
BEGIN


	SET NOCOUNT ON

	SET @UpdateStamp = getdate() 
	DECLARE @State_Enabled tinyint
	SET @State_Enabled = 1

	DECLARE @Message varchar(500) 

	DECLARE @RouteTableCreationSQL nvarchar(max) 

	SET @RouteTableCreationSQL = '
		-- Temp Route
		CREATE TABLE [dbo].[TempRoutes](
			[RouteID] [int]  NOT NULL,
			[CustomerID] [varchar](5) NOT NULL,
			[ProfileID] [int] NULL,
			[Code] [varchar](15) NULL,
			[OurZoneID] [int] NULL,
			[OurActiveRate] [real] NULL,
			[OurServicesFlag] [smallint] NULL,
			[State] [tinyint] NOT NULL,
			[Updated] [datetime] NULL,
			[IsToDAffected] bit NOT NULL ,
			[IsSpecialRequestAffected] bit NOT NULL,
			[IsOverrideAffected] bit NOT NULL,
			[IsBlockAffected] bit NOT NULL,
			[IsOptionBlock] bit NOT NULL,
			[BatchID] int not null
		) 
		
		ON [' + @RoutingTableFileGroup + ']
		
	';

	IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TempRoutes]') AND type in (N'U'))
	BEGIN
		DROP TABLE [dbo].[TempRoutes]
	END
	

	EXEC (@RouteTableCreationSQL);

;WITH

    	RoutesToInsert AS (		
			SELECT
			   convert(int,ROW_NUMBER() OVER (ORDER BY getdate())) AS RouteID,
				R.CustomerID CustomerID,
				R.ProfileID ProfileID,
				rcp.Code Code,
				rcp.ZoneID OurZoneID,
				R.ActiveRate OurActiveRate,
				R.ServicesFlag OurServicesFlag,
				@State_Enabled State,
				ISNULL( R.IsTOD, 0) IsTOD,
				r.IsBlock
			FROM RoutePool rcp  WITH (NOLOCK) 
			INNER JOIN ZoneRates R WITH (NOLOCK) ON  rcp.ZoneID = R.ZoneID
			 WHERE  
					r.SupplierID = 'SYS' AND (@IncludeBlockedZones='Y' OR (@IncludeBlockedZones= 'N' AND r.IsBlock=0))
			 

    	)
    	Insert INTO TempRoutes with(TABLOCK) 
			SELECT
			rt.RouteID 
		   ,rt.[CustomerID]
           ,rt.[ProfileID]
           ,rt.[Code]
           ,rt.[OurZoneID]
           ,rt.[OurActiveRate]
           ,rt.[OurServicesFlag]
           ,rt.[State]
           ,DATEADD(MS,CASE WHEN rt.Routeid<1000 THEN -rt.Routeid ELSE -rt.Routeid/1000 End,@UpdateStamp) [Updated]
           ,rt.[IsToD]
           ,isnull(rn.IsSpecialRequestAffected,0) 
		   ,ISNULL(rn.IsOverrideAffected,0) 
		   ,ISNULL(rn.IsBlockAffected,rt.IsBlock) 
		   ,ISNULL(rn.IsOptionBlock,0)
		   ,1 BatchID
			FROM RoutesToInsert rt
			LEFT JOIN Route_NonLCR rn ON rt.CustomerID = rn.CustomerID AND rt.Code = rn.Code AND rt.OurZoneID = rn.OurZoneID
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Routes Inserted', @Message
	
    SELECT @UpdateStamp = MAX(Updated) FROM TempRoutes

END