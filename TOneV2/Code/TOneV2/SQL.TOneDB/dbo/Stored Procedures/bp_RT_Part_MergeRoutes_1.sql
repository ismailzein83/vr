
CREATE PROCEDURE [dbo].[bp_RT_Part_MergeRoutes]
 @BatchID INT = 0,
 @CheckSpecialRequests BIT = 0
AS
BEGIN
	SET NOCOUNT ON;
	  CREATE TABLE [#RouteOptionsToInsert](
			[RouteID] [int] NOT NULL,
			[CustomerID] [varchar](5) NOT NULL,
			[Code] [varchar](15) NOT NULL,
			[SupplierID] [varchar](5) NOT NULL,
			[SupplierZoneID] [int] NULL,
			[SupplierActiveRate] [real] NULL,
			[SupplierServicesFlag] [smallint] NULL,
			[Priority] [tinyint] NOT NULL,
			[NumberOfTries] [tinyint] NULL,
			[State] [tinyint] NOT NULL DEFAULT ((0)),
			[Updated] [datetime] NULL,
			[Percentage] [tinyint] NULL
		) ON [Primary]
			
		CREATE TABLE #RoutesToInsert(
			[RouteID] [int]  NOT NULL ,
			[CustomerID] [varchar](5) NOT NULL,
			[ProfileID] [int] NULL,
			[Code] [varchar](15) NULL,
			[OurZoneID] [int] NULL,
			[OurActiveRate] [real] NULL,
			[OurServicesFlag] [smallint] NULL,
			[State] [tinyint] NOT NULL,
			[Updated] [datetime] NULL,
			[IsToDAffected] tinyint NOT NULL ,
			[IsSpecialRequestAffected] tinyint NOT NULL,
			[IsOverrideAffected] tinyint NOT NULL,
			[IsBlockAffected] tinyint NOT NULL,
			[IsOptionBlock] tinyint NOT NULL,
			[BatchID] INT NOT NULL
		)ON [Primary]
		
	DECLARE @MaxRouteID INT
	select @MaxRouteID =   MAX(r.routeID) FROM [Route] r	
	
	SELECT 'To Delete Routes', r.RouteID FROM [Route] r INNER JOIN #Routes r2 ON r2.CustomerID COLLATE DATABASE_DEFAULT  = r.CustomerID AND r2.Code COLLATE DATABASE_DEFAULT  = r.Code
	
	SELECT 'To Delete Options', ro.* FROM RouteOption ro
	WHERE ro.RouteID IN (SELECT r.RouteID FROM [Route] r INNER JOIN #Routes r2 ON r2.CustomerID COLLATE DATABASE_DEFAULT  = r.CustomerID AND r2.Code COLLATE DATABASE_DEFAULT  = r.Code)
	
	DELETE ro FROM RouteOption ro
	WHERE ro.RouteID IN (SELECT r.RouteID FROM [Route] r INNER JOIN #Routes r2 ON r2.CustomerID COLLATE DATABASE_DEFAULT  = r.CustomerID AND r2.Code COLLATE DATABASE_DEFAULT  = r.Code)
	
	DELETE r FROM [Route] r
	INNER JOIN #Routes r2 ON r2.CustomerID COLLATE DATABASE_DEFAULT  = r.CustomerID AND r2.Code COLLATE DATABASE_DEFAULT  = r.Code
	
	INSERT INTO [ROUTE]
	SELECT convert(int,ROW_NUMBER() OVER (ORDER BY getdate())) + @MaxRouteID AS ROUTEID,rti.CustomerID, rti.ProfileID, rti.Code, rti.OurZoneID,
	       rti.OurActiveRate, rti.OurServicesFlag, rti.[State], rti.Updated,
	       rti.IsToDAffected, rti.IsSpecialRequestAffected, rti.IsOverrideAffected,
	       rti.IsBlockAffected, rti.IsOptionBlock,rti.BatchID
	  FROM #Routes rti
	
	
	--	--Insert Updated Options
	INSERT INTO RouteOption
	SELECT r.RouteID, roti.SupplierID, roti.SupplierZoneID,
	       roti.SupplierActiveRate, roti.SupplierServicesFlag, roti.Priority,
	       roti.NumberOfTries, roti.[State], roti.Updated, roti.Percentage
	  FROM #RouteOptions roti
	  INNER JOIN [Route] r ON r.CustomerID COLLATE DATABASE_DEFAULT = roti.CustomerID AND r.Code COLLATE DATABASE_DEFAULT = roti.Code 
	 WHERE r.BatchID = @BatchID 
	
	--	IF(@CheckSpecialRequests = 1)
	--BEGIN	
		
	--	EXEC bp_RT_Part_BuildRouteAction_SpecialRequestV2;
	--END

	
END