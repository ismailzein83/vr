
CREATE PROCEDURE [dbo].[bp_RT_Part_BuildRoutePartial]
     @RebuildOptionPool BIT = 1
    ,@RebuildRoutePool BIT = 1
    ,@RebuildZoneRates BIT = 1
	,@CheckToD BIT = 1 
	,@CheckSpecialRequests BIT = 1 
	,@CheckRouteBlocks BIT = 1
	,@MaxSuppliersPerRoute INT = 10
	,@IncludeBlockedZones BIT = 1
	,@Targets VARCHAR(MAX)
	,@TargetsType VARCHAR(5)
	,@TargetCustomers VARCHAR(MAX)
	,@ApplySaleMarketPrice BIT = 0
WITH RECOMPILE
AS
BEGIN

SET NOCOUNT ON


	DECLARE @Account_Inactive tinyint
	DECLARE @Account_Blocked tinyint
	DECLARE @Account_BlockedInbound tinyint
	DECLARE @Account_BlockedOutbound tinyint
	
	-- Set Enumerations
	SELECT @Account_Inactive = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.ActivationStatus' AND Name = 'Inactive'
	SELECT @Account_Blocked = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'Blocked'
	SELECT @Account_BlockedInbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedInbound'	
	SELECT @Account_BlockedOutbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedOutbound'	
	
	
	set @CheckRouteBlocks = CASE WHEN EXISTS (SELECT CheckRouteBlocks FROM RouteBuildBatch WHERE BatchID = 1) THEN  (SELECT CheckRouteBlocks FROM RouteBuildBatch WHERE BatchID = 1) ELSE 1 END
    set @IncludeBlockedZones = CASE WHEN EXISTS  (SELECT IncludeBlockedZones FROM RouteBuildBatch WHERE BatchID = 1) THEN   (SELECT IncludeBlockedZones FROM RouteBuildBatch WHERE BatchID = 1) ELSE 1 END 
    set @CheckSpecialRequests = CASE WHEN EXISTS  (SELECT CheckSpecialRequests FROM RouteBuildBatch WHERE BatchID = 1) THEN   (SELECT CheckSpecialRequests FROM RouteBuildBatch WHERE BatchID = 1) ELSE 1 END 
    set @CheckToD = CASE WHEN EXISTS  (SELECT CheckToD FROM RouteBuildBatch WHERE BatchID = 1) THEN   (SELECT CheckToD FROM RouteBuildBatch WHERE BatchID = 1) ELSE 1 END 
    SET @MaxSuppliersPerRoute = CASE WHEN EXISTS  (SELECT MaxSuppliersPerRoute FROM RouteBuildBatch WHERE BatchID = 1) THEN   (SELECT MaxSuppliersPerRoute FROM RouteBuildBatch WHERE BatchID = 1) ELSE 6 END
----------------------------Check Missing Tables-----------------------------------
IF NOT EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'dbo.[CodeMatch]') AND s.type in (N'U'))
BEGIN
	RAISERROR (N'CodeMatch Table does not exists', 15, 1);
	RETURN 
END

IF NOT EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'dbo.[RouteBlock]') AND s.type in (N'U'))
BEGIN
	RAISERROR (N'RouteBlock Table does not exists', 15, 1);
	RETURN 
END

IF NOT EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'dbo.[SpecialRequest]') AND s.type in (N'U'))
BEGIN
	RAISERROR (N'SpecialRequest Table does not exists', 15, 1);
	RETURN 
END

IF NOT EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'dbo.[RouteOverride]') AND s.type in (N'U'))
BEGIN
	RAISERROR (N'RouteOverride Table does not exists', 15, 1);
	RETURN 
END


----------------Valid Suppliers -------------------------
IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'dbo.[#ActiveSuppliers]') AND s.type in (N'U'))
	BEGIN
		 TRUNCATE TABLE #ActiveSuppliers
	END
	
ELSE
	BEGIN
		CREATE TABLE #ActiveSuppliers(
			[CarrierAccountID] [varchar](5) NOT NULL,
			[ProfileID] [INT] NOT NULL,
			[GMTTime] [smallint] NULL,
			[Tax2] [numeric](6,2) NULL
			)	
	END
	
	
IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'dbo.[#ActiveCustomers]') AND s.type in (N'U'))
	BEGIN
		 TRUNCATE TABLE #ActiveCustomers
	END
	
ELSE
	BEGIN
		CREATE TABLE #ActiveCustomers(
			[CarrierAccountID] [varchar](5) NOT NULL,
			[ProfileID] [INT] NOT NULL
			)	
	END	
	
	INSERT INTO #ActiveSuppliers
	SELECT ca.CarrierAccountID,ca.ProfileID,ca.GMTTime,pr.Tax2
	  FROM CarrierAccount ca
	Left Join CarrierProfile Pr 
 On ca.ProfileID=Pr.ProfileID
	 WHERE 	ca.ActivationStatus <> @Account_Inactive 
	      AND ca.RoutingStatus <> @Account_Blocked 
		  AND ca.RoutingStatus <> @Account_BlockedOutbound
		  AND ca.isdeleted='N'

	INSERT INTO #ActiveCustomers
	SELECT ca.CarrierAccountID,ca.ProfileID FROM CarrierAccount ca
	WHERE ca.ActivationStatus <> @Account_Inactive
		  And ca.RoutingStatus <> @Account_BlockedInbound 
		  AND ca.RoutingStatus <> @Account_Blocked 
		  AND ca.isdeleted='N'

-----Filter Tables---------
IF  EXISTS ( select  * from tempdb.dbo.sysobjects o where o.xtype in ('U') and o.id = object_id(N'tempdb..#SaleZoneFilter'))
BEGIN
	DROP TABLE #SaleZoneFilter
END
ELSE 
	BEGIN
		CREATE TABLE #SaleZoneFilter(
			[ZoneID] [int] NOT NULL,
			[CodeGroup] [varchar](15) NOT NULL,
			[IsActive] BIT NOT NULL,
			[SupplierID] VARCHAR(5) NULL
		)ON [PRIMARY]
	END


IF  EXISTS ( select  * from tempdb.dbo.sysobjects o where o.xtype in ('U') and o.id = object_id(N'tempdb..#CostZoneFilter'))
BEGIN
	DROP TABLE #CostZoneFilter
END
ELSE 
	BEGIN
		CREATE TABLE #CostZoneFilter(
			[ZoneID] [int] NOT NULL,
			[CodeGroup] [varchar](15) NOT NULL,
			[IsActive] BIT NOT NULL, 
			[SupplierID] VARCHAR(5) NULL
		)ON [PRIMARY]
	END


IF  EXISTS ( select  * from tempdb.dbo.sysobjects o where o.xtype in ('U') and o.id = object_id(N'tempdb..#CustomerFilter'))
BEGIN
	DROP TABLE #CustomerFilter
END
ELSE 
	BEGIN
		CREATE TABLE #CustomerFilter(
			[CustomerID] [varchar](5) NOT NULL,
			[ProfileID] INT NULL,
			[IsActive] BIT NOT NULL
		)ON [PRIMARY]
	END

IF  EXISTS ( select  * from tempdb.dbo.sysobjects o where o.xtype in ('U') and o.id = object_id(N'tempdb..#CodeFilter'))
BEGIN
	DROP TABLE #CodeFilter
END
ELSE 
	BEGIN
		CREATE TABLE #CodeFilter(
			[Code] [varchar](15) NOT NULL,
			[IsActive] BIT NOT NULL
		)ON [PRIMARY]
	END
	
	
	
IF  EXISTS ( select  * from tempdb.dbo.sysobjects o where o.xtype in ('U') and o.id = object_id(N'tempdb..#SaleZoneRates'))
BEGIN
	DROP TABLE #SaleZoneRates
END
ELSE 
	BEGIN
	CREATE TABLE [dbo].[#SaleZoneRates](
	[ZoneID] [int] NOT NULL,
	[SupplierID] [varchar](5) NOT NULL,
	[CustomerID] [varchar](5) NOT NULL,	
	[ServicesFlag] [smallint] NULL,
	[ProfileId] [int] NULL,
	[ActiveRate] [real] NULL,
	[IsTOD] BIT NULL,
	[IsBlock] BIT NULL,
	[CodeGroup] [varchar](20) NULL
) ON [Primary]

	END


IF  EXISTS ( select  * from tempdb.dbo.sysobjects o where o.xtype in ('U') and o.id = object_id(N'tempdb..#CostZoneRates'))
BEGIN
	DROP TABLE #CostZoneRates
END
ELSE 
	BEGIN
	CREATE TABLE [dbo].[#CostZoneRates](
	[ZoneID] [int] NOT NULL,
	[SupplierID] [varchar](5) NOT NULL,
	[CustomerID] [varchar](5) NOT NULL,	
	[ServicesFlag] [smallint] NULL,
	[ProfileId] [int] NULL,
	[ActiveRate] [real] NULL,
	[IsTOD] BIT NULL,
	[IsBlock] BIT NULL,
	[CodeGroup] [varchar](20) NULL
) ON [Primary]


	END

---------Route Pool------------
      
IF  EXISTS(select * from tempdb.dbo.sysobjects o where o.xtype in ('U') and o.id = object_id(N'tempdb..#RoutePool'))
	
	BEGIN
     DROP TABLE #RoutePool
	END
	ELSE 
		BEGIN
					 CREATE TABLE #RoutePool (
							[Code] [VARCHAR] (20) NOT NULL,							
							[ZoneID][int] NOT NULL,
							[IsBlocked]BIT NULL,
							[CodeGroup][varchar](20) NULL
		 )

		END 


-----------------Route Option Pool --------------------

      
IF EXISTS( select * from tempdb.dbo.sysobjects o where o.xtype in ('U') and o.id = object_id(N'tempdb..#RouteOptionsPool'))
	
	BEGIN
		DROP TABLE #RouteOptionsPool
	END
	ELSE 
		BEGIN			
			CREATE TABLE [dbo].[#RouteOptionsPool](
			[Code] [VARCHAR] (20) NOT NULL,
			[SupplierID] VARCHAR(5) NOT NULL,
			[SupplierZoneID] [int] NOT NULL,
			[SupplierServicesFlag] [smallint] NOT NULL,	
			[ProfileId] [int] NULL,
			[ActiveRate] [real] NULL,
			[IsBlock] BIT NULL,
			[IsTOD] BIT NULL
) ON [PRIMARY]
			
		END


--------------------ACTION TABLES-------------------------------
----------------------------------------------------------------
----------------------------------------------------------------
------------------ Init Block Table ----------------------------
IF EXISTS(  select * from tempdb.dbo.sysobjects o where o.xtype in ('U') and o.id = object_id(N'tempdb..#Route_Block'))
	BEGIN
		 DROP TABLE #Route_Block
	END
	
ELSE 
  BEGIN
		
			CREATE TABLE [dbo].[#Route_Block](
				[RouteID] [int] NOT NULL IDENTITY(1,1),
				[CustomerID] VARCHAR(5) NULL,
				[ProfileID] [smallint] NULL,
				[Code] [VARCHAR] (20) NULL,
				[OurZoneID] [int] NULL,
				[OurActiveRate] [real] NULL,
				[OurServicesFlag] [smallint] NULL,
				[State] [bit] NOT NULL,				
				[IsToDAffected] BIT NOT NULL,
				[IsSpecialRequestAffected] BIT NOT NULL,
				[IsOverrideAffected] BIT NOT NULL,
				[IsBlockAffected] BIT NOT NULL,
				[IsOptionBlock] BIT NULL,
				[RuleID] [INT] NULL
			)ON [PRIMARY]

  END

IF EXISTS( select * from tempdb.dbo.sysobjects o where o.xtype in ('U') and o.id = object_id(N'tempdb..#RouteOption_Block'))
	BEGIN
		 DROP TABLE [#RouteOption_Block]
	END
	
ELSE 
  BEGIN
		
					CREATE TABLE [dbo].[#RouteOption_Block](
					[CustomerID] VARCHAR(5) NOT NULL,
					[SupplierID] VARCHAR(5) NOT NULL,
					[Code] [VARCHAR] (20) NOT NULL,
					[SupplierZoneID] [int] NULL,
					[SupplierActiveRate] [real] NULL,
					[SupplierServicesFlag] [smallint] NULL,
					[Priority] [tinyint] NOT NULL,
					[NumberOfTries] [tinyint] NULL,
					[State] BIT NOT NULL,					
					[Percentage] [tinyint] NULL,
					[ActionType] [tinyint] NULL
				) ON [PRIMARY]
				


  END


------------------ Init Override Tables ----------------------------
IF EXISTS( select * from tempdb.dbo.sysobjects o where o.xtype in ('U') and o.id = object_id(N'tempdb..#Route_Override'))
	BEGIN
		 DROP TABLE #Route_Override
	END
	
ELSE 
  BEGIN
		
			CREATE TABLE [dbo].[#Route_Override](
				[RouteID] [int] NOT NULL IDENTITY(1,1) ,
				[CustomerID] VARCHAR(5) NOT NULL,
				[ProfileID] [int] NULL,
				[Code] [VARCHAR] (20) NULL,
				[OurZoneID] [int] NULL,
				[OurActiveRate] [real] NULL,
				[OurServicesFlag] [smallint] NULL,
				[State] BIT NOT NULL,				
				[IsToDAffected] BIT NOT NULL,
				[IsSpecialRequestAffected] BIT NOT NULL,
				[IsOverrideAffected] BIT NOT NULL,
				[IsBlockAffected] BIT NOT NULL,
				[IsOptionBlock] BIT NULL,
				[RuleID] INT NOT NULL
			)ON [PRIMARY]
			

  END

IF EXISTS(select * from tempdb.dbo.sysobjects o where o.xtype in ('U') and o.id = object_id(N'tempdb..#RouteOption_Override'))
	BEGIN
		 DROP TABLE [#RouteOption_Override]
	END
	
ELSE 
  BEGIN
		
					CREATE TABLE [dbo].[#RouteOption_Override](
					[CustomerID] VARCHAR(5) NOT NULL,
					[SupplierID] VARCHAR(5) NOT NULL,
					[Code] [VARCHAR] (20) NULL,
					[SupplierZoneID] [int] NULL,
					[SupplierActiveRate] [real] NULL,
					[SupplierServicesFlag] [smallint] NULL,
					[Priority] [tinyint] NOT NULL,
					[NumberOfTries] [tinyint] NULL,
					[State] BIT NOT NULL,					
					[Percentage] [tinyint] NULL,
					[ActionType] [tinyint] NULL
					--[Updated] [datetime] NULL,
				) ON [PRIMARY]
				

  END


------------------ Init Override Tables ----------------------------
IF EXISTS( select * from tempdb.dbo.sysobjects o where o.xtype in ('U') and o.id = object_id(N'tempdb..#Route_Special'))
	BEGIN
		 DROP TABLE #Route_Special
	END
	
ELSE 
  BEGIN
		
			CREATE TABLE [dbo].[#Route_Special](
				[RouteID] [int] NOT NULL IDENTITY ,
				[CustomerID] VARCHAR(5) NOT NULL,
				[ProfileID] [int] NULL,
				[Code] [VARCHAR] (20) NULL,
				[OurZoneID] [int] NULL,
				[OurActiveRate] [real] NULL,
				[OurServicesFlag] [smallint] NULL,
				[State] BIT NOT NULL,				
				[IsToDAffected] BIT NOT NULL,
				[IsSpecialRequestAffected] BIT NOT NULL,
				[IsOverrideAffected] BIT NOT NULL,
				[IsBlockAffected] BIT NOT NULL,
				[IsOptionBlock] BIT NULL,
				[RuleID] INT NOT NULL
			)ON [PRIMARY]
		

  END

IF EXISTS( select * from tempdb.dbo.sysobjects o where o.xtype in ('U') and o.id = object_id(N'tempdb..#RouteOption_Special'))
	BEGIN
		 DROP TABLE [#RouteOption_Special]
	END
	
ELSE 
  BEGIN
		
					CREATE TABLE [dbo].[#RouteOption_Special](
					[CustomerID] VARCHAR(5) NOT NULL,
					[SupplierID] VARCHAR(5) NOT NULL,
					[Code] [VARCHAR] (20) NOT NULL,
					[SupplierZoneID] [int] NULL,
					[SupplierActiveRate] [real] NULL,
					[SupplierServicesFlag] [smallint] NULL,
					[Priority] [tinyint] NOT NULL,
					[NumberOfTries] [tinyint] NULL,
					[State] BIT NOT NULL,					
					[Percentage] [tinyint] NULL,
					[ActionType] [tinyint] NULL
				) ON [PRIMARY]
				

  END
--------------------ACTION TABLES-------------------------------
----------------------------------------------------------------
----------------------------------------------------------------



--------------------NON LCR TABLES-------------------------------
----------------------------------------------------------------
----------------------------------------------------------------
IF EXISTS( select * from tempdb.dbo.sysobjects o where o.xtype in ('U') and o.id = object_id(N'tempdb..#Route_NonLCR'))
	BEGIN
		 DROP TABLE [#Route_NonLCR]
	END
	
ELSE 
  BEGIN
		
			CREATE TABLE [dbo].[#Route_NonLCR](
				[RouteID] [int] NOT NULL IDENTITY ,
				[CustomerID] VARCHAR(5) NOT NULL,
				[ProfileID] [int] NULL,
				[Code] [VARCHAR] (20) NULL,
				[OurZoneID] [int] NULL,
				[OurActiveRate] [real] NULL,
				[OurServicesFlag] [smallint] NULL,
				[State] BIT NOT NULL,				
				[IsToDAffected] BIT NOT NULL,
				[IsSpecialRequestAffected] BIT NOT NULL,
				[IsOverrideAffected] BIT NOT NULL,
				[IsBlockAffected] BIT NOT NULL,
				[IsOptionBlock] BIT NULL,
				[RuleID] INT NOT NULL
			)ON [PRIMARY]

  END
  
    IF EXISTS( select * from tempdb.dbo.sysobjects o where o.xtype in ('U') and o.id = object_id(N'tempdb..#RouteOption_NonLCR'))
	BEGIN
		 DROP TABLE [#RouteOption_NonLCR]
	END
	
ELSE 
  BEGIN
		
					CREATE TABLE [dbo].[#RouteOption_NonLCR](
					[CustomerID] VARCHAR(5) NOT NULL,
					[SupplierID] VARCHAR(5) NOT NULL,
					[Code] [VARCHAR] (20) NOT NULL,
					[SupplierZoneID] [int] NULL,
					[SupplierActiveRate] [real] NULL,
					[SupplierServicesFlag] [smallint] NULL,
					[Priority] [tinyint] NOT NULL,
					[NumberOfTries] [tinyint] NULL,
					[State] BIT NOT NULL,					
					[Percentage] [tinyint] NULL,
					[ActionType] [tinyint] NULL
					--[Updated] [datetime] NULL,
				) ON [PRIMARY]
				

  END

			--------------------NON LCR TABLES-------------------------------
	-----------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------



-----------Result Tables------------------------
    IF EXISTS( select * from tempdb.dbo.sysobjects o where o.xtype in ('U') and o.id = object_id(N'tempdb..#Routes'))
	BEGIN
		 DROP TABLE #Routes
	END
	
ELSE 
  BEGIN
CREATE TABLE #Routes(
			[RouteID] [int]  NOT NULL Identity,
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
		) 
		
		ON [Primary]
  END
  
  
      IF EXISTS( select * from tempdb.dbo.sysobjects o where o.xtype in ('U') and o.id = object_id(N'tempdb..#RouteOptions'))
	BEGIN
		 DROP TABLE #RouteOptions
	END
	
ELSE 
  BEGIN
  
  CREATE TABLE [dbo].[#RouteOptions](
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
    
  END
  
  









IF NOT EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'dbo.[SystemMessages]') AND s.type in (N'U'))
BEGIN;
	
		CREATE TABLE [dbo].[SystemMessages](
	[MessageID] [varchar](100) NOT NULL,
	[Description] [varchar](max) NOT NULL,
	[Message] [varchar](max) NULL,
	[Updated] [datetime] NOT NULL,
	[timestamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_SystemMessages] PRIMARY KEY CLUSTERED 
(
	[MessageID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY];


ALTER TABLE [dbo].[SystemMessages] ADD  CONSTRAINT [DF_SystemMessages_Updated]  DEFAULT (getdate()) FOR [Updated];
END;

	-- If Rebuild Routes is already running error and return
	--DECLARE @IsRunning char(1);
	--SELECT @IsRunning = 'Y' FROM SystemMessages WHERE MessageID = 'PartialBuildRoutes: Status' AND [Message] IS NOT NULL;
	--IF @IsRunning = 'Y' 
	--BEGIN
	--	RAISERROR (N'Build Partial Routes is already Runnning', 15, 1); 
	--	RETURN 
	--END 

	DECLARE @MessageID varchar(50);
	DECLARE @Description varchar(450);
	DECLARE @Message varchar(500); 

	DELETE FROM SystemMessages WHERE MessageID LIKE 'PartialBuildRoutes: %';
	
	SET @Message = CONVERT(varchar, getdate(), 121); 
	EXEC bp_RT_Full_SetSystemMessages 'PartialBuildRoutes: Start', @Message; 	

	SET @Message = ('Started'); 
	EXEC bp_RT_Full_SetSystemMessages 'PartialBuildRoutes: Status ', @Message; 
	
	 
	DECLARE @TruncateLogSQL varchar(max); 
	SELECT @TruncateLogSQL = 'BACKUP LOG ' + db_name() + ' WITH TRUNCATE_ONLY';

   	--------------------------------------------------
	SET @Message = CONVERT(varchar, getdate(), 121);
	EXEC bp_RT_Full_SetSystemMessages 'PartialBuildRoutes: Create Tables', @Message;
    
    --EXEC bp_RT_Part_CreateTempTables
    --    @Targets = @Targets,
    --	@TargetsType = @TargetsType,
    --	@TargetCustomers = @TargetCustomers;
    -------------------------------------------------
    SET @Message = CONVERT(varchar, getdate(), 121);
	EXEC bp_RT_Full_SetSystemMessages 'PartialBuildRoutes: Fill Filtration Tables', @Message;
    
    EXEC bp_RT_Part_FillFiltrationTables
    	@Targets = @Targets,
    	@TargetsType = @TargetsType,
    	@TargetCustomers = @TargetCustomers;
    	
    --------------------------------------------------	
    SET @Message = CONVERT(varchar, getdate(), 121);
	EXEC bp_RT_Full_SetSystemMessages 'PartialBuildRoutes: Build Base Tables', @Message;
    
	EXEC bp_RT_Part_BuildBaseTables
		@RebuildOptionPool = @RebuildOptionPool,
		@RebuildRoutePool = @RebuildRoutePool,
		@RebuildZoneRates = @RebuildZoneRates,
		@CheckToD = @CheckToD,
		@IncludeBlockedZones = @IncludeBlockedZones;
    
    -----------------------------------------------------
    SET @Message = CONVERT(varchar, getdate(), 121);
	EXEC bp_RT_Full_SetSystemMessages 'PartialBuildRoutes: Build Action Tables', @Message;
    
    EXEC bp_RT_Part_BuildActionRoutes
    	@CheckSpecialRequests = @CheckSpecialRequests,
    	@CheckRouteBlocks = @CheckRouteBlocks;
    	
    ------------------------------------------------------
    SET @Message = CONVERT(varchar, getdate(), 121);
	EXEC bp_RT_Full_SetSystemMessages 'PartialBuildRoutes: Build NonLCR', @Message;	
	
	EXEC bp_RT_Part_BuildNonLCR;
	
	-------------------------------------------------------------
	 DECLARE	@ReturnBatchID int
	 EXEC bp_RT_Part_InsertRouteBatch
			 @CheckToD = @CheckToD
			,@CheckSpecialRequests = @CheckSpecialRequests
			,@CheckRouteBlocks = @CheckRouteBlocks
			,@IncludeBlockedZones = @IncludeBlockedZones
			,@Targets = @Targets
			,@TargetsType = @TargetsType
			,@TargetCustomers = @TargetCustomers
			,@BatchID = @ReturnBatchID OUTPUT
			,@MaxSuppliersPerRoute = @MaxSuppliersPerRoute;
	
	-------------------------------------------------------
	SET @Message = CONVERT(varchar, getdate(), 121);
	EXEC bp_RT_Full_SetSystemMessages 'PartialBuildRoutes: Build LCR', @Message;
    
    EXEC bp_RT_Part_BuildLCR_Route
		 @IncludeBlockedZones = @IncludeBlockedZones,
		 @BatchID = @ReturnBatchID;
        
    -------------------------------------------------------
	SET @Message = CONVERT(varchar, getdate(), 121);
	EXEC bp_RT_Full_SetSystemMessages 'PartialBuildRoutes: Build LCR', @Message;
    
    EXEC bp_RT_Part_BuildLCR_Option
		  @CheckRouteBlocks = @CheckRouteBlocks,
		  @MaxSuppliersPerRoute = @MaxSuppliersPerRoute,
		  @ApplySaleMarketPrice = @ApplySaleMarketPrice;
    
    --------------------------------------------------------
    SET @Message = CONVERT(varchar, getdate(), 121);
	EXEC bp_RT_Full_SetSystemMessages 'PartialBuildRoutes: Merge Routes', @Message;
	
	EXEC bp_RT_Part_MergeRoutes
		@BatchID = @ReturnBatchID,
		@CheckSpecialRequests = @CheckSpecialRequests;
	------------------------------------------------------------
	
    
    
	SET @Message = CONVERT(varchar, getdate(), 121);
	EXEC bp_RT_Full_SetSystemMessages 'PartialBuildRoutes: End', @Message;


	-- Set Status to NULL (Not Running)
	EXEC bp_RT_Full_SetSystemMessages 'PartialBuildRoutes: Status', @message = NULL;
    
END