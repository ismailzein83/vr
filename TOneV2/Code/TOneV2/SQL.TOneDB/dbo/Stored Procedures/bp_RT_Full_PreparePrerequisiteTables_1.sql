


CREATE PROCEDURE [dbo].[bp_RT_Full_PreparePrerequisiteTables]
	

WITH RECOMPILE
AS
BEGIN


	DECLARE @Account_Inactive tinyint
	DECLARE @Account_Blocked tinyint
	DECLARE @Account_BlockedInbound tinyint
	DECLARE @Account_BlockedOutbound tinyint
	
	-- Set Enumerations
	SELECT @Account_Inactive = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.ActivationStatus' AND Name = 'Inactive'
	SELECT @Account_Blocked = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'Blocked'
	SELECT @Account_BlockedInbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedInbound'	
	SELECT @Account_BlockedOutbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedOutbound'	


SET NOCOUNT ON

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

--IF NOT EXISTS (SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'dbo.[TempOverrideRules]') AND s.type in (N'U'))
--	BEGIN
--CREATE TABLE TempOverrideRules(
--	RuleID INT,
--	CustomerID VARCHAR(5),
--	Code VARCHAR(30),
--	OurZoneID INT,
--	RouteOptions  VARCHAR(100),
--	IncludeSubCodes CHAR(1),
--	ExcludedCodes VARCHAR(250),
--	ParentID INT,
--	MainExcluded varchar(250),
--	OriginalExcluded varchar(250),
--	HasSubZone BIT,
--	SubCodes VARCHAR(MAX),
--	SubZoneIDs VARCHAR(MAX)
--)
--CREATE NONCLUSTERED INDEX Temp_Customer ON TempOverrideRules(CustomerID) INCLUDE(Code,OurZoneID,RouteOptions,RuleID,IncludeSubCodes)

--	END
--ELSE
--	BEGIN
--		TRUNCATE TABLE TempOverrideRules
--	END	
------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'dbo.[RouteBuildBatch]') AND s.type in (N'U'))
	BEGIN
		CREATE TABLE RouteBuildBatch(
			[BatchID] INT NOT NULL IDENTITY(1,1),
			[CheckSpecialRequests] BIT NOT NULL,
			[CheckRouteBlocks] BIT NOT NULL,
			[CheckTOD] BIT NOT NULL,
			[IncludeBlockedZones] BIT NOT NULL,
			[Type] VARCHAR(2) NULL,
			[Targets] VARCHAR(MAX) NULL,
			[TargetCustomers] VARCHAR(MAX) NULL,
			[IsSynched] BIT NULL,
			[MaxSuppliersPerRoute] INT NOT NULL
			
		)ON [Primary]
	END
ELSE
	BEGIN
		TRUNCATE TABLE RouteBuildBatch
	END	


IF NOT EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'dbo.[ZoneRates]') AND s.type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[ZoneRates](
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

		--CREATE NONCLUSTERED INDEX [IX_ZoneRates_Supplier] ON [dbo].[ZoneRates]([SupplierID] ASC)
		--CREATE NONCLUSTERED INDEX [IX_ZoneRates_Customer] ON [dbo].[ZoneRates]([CustomerID] ASC)
		--CREATE NONCLUSTERED INDEX [IX_ZoneRates_Zone] ON [dbo].[ZoneRates]([ZoneID] ASC)
		CREATE NONCLUSTERED INDEX [IX_ZoneRates_ServicesFlag] ON [dbo].[ZoneRates]([ServicesFlag] ASC)
		CREATE NONCLUSTERED INDEX [IX_ZoneRates_SupplierIsBlock] ON [dbo].[ZoneRates](	[SupplierID] ASC,	[IsBlock] ASC, [ZoneID] ASC)
			INCLUDE ( [CustomerID],[ServicesFlag],[ProfileId],[ActiveRate],[IsTOD]) 
		WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
END





---------Route Pool------------
      
IF NOT EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'dbo.[RoutePool]') AND s.type in (N'U'))
	
	BEGIN
		 CREATE TABLE dbo.[RoutePool] (
		 					--[ID] [int] NOT NULL IDENTITY(1,1) , -- Added By BS, to allow build Route options by Code
							[Code] [VARCHAR] (20) NOT NULL,							
							[ZoneID][int] NOT NULL,
							[IsBlocked]BIT NULL,
							[CodeGroup][varchar](20) NULL
		 )
		 
		 --CREATE NONCLUSTERED INDEX [IX_RoutePool_Zone] ON [dbo].[RoutePool]([ZoneID] ASC)
		 --CREATE NONCLUSTERED INDEX [IX_RoutePool_Code] ON [dbo].[RoutePool]([Code] ASC)
		
		CREATE NONCLUSTERED INDEX [IX_RoutePool_ZoneIncCode] ON [dbo].[RoutePool] (	[ZoneID] ASC)INCLUDE ( [Code]) 
		WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

		CREATE NONCLUSTERED INDEX [IX_RoutePool_CodeIncZone] ON [dbo].[RoutePool] (	[Code] ASC)INCLUDE ( [ZoneID]) 
		WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

	END; 




-----------------Route Option Pool --------------------

      
IF NOT EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'dbo.[RouteOptionsPool]') AND s.type in (N'U'))
	
	BEGIN

CREATE TABLE [dbo].[RouteOptionsPool](
			[Code] [VARCHAR] (20) NOT NULL,
			[SupplierID] VARCHAR(5) NOT NULL,
			[SupplierZoneID] [int] NOT NULL,
			[SupplierServicesFlag] [smallint] NOT NULL,	
			[ProfileId] [int] NULL,
			[ActiveRate] [real] NULL,
			[IsBlock] BIT NULL,
			[IsTOD] BIT NULL
) ON [PRIMARY]

		--CREATE NONCLUSTERED INDEX [IX_RouteOptionsPool_Supplier] ON [dbo].[RouteOptionsPool]([SupplierID] ASC)
		--CREATE NONCLUSTERED INDEX [IX_RouteOptionsPool_Zone] ON [dbo].[RouteOptionsPool]([SupplierZoneID] ASC)
		--CREATE NONCLUSTERED INDEX [IX_RouteOptionsPool_Code] ON [dbo].[RouteOptionsPool]([Code] ASC)
		
		CREATE NONCLUSTERED INDEX [IX_RouteOptionsPool_CodeSupplierID] ON [dbo].[RouteOptionsPool] (	[SupplierID] ASC,	[Code] ASC)
		WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
		
		CREATE NONCLUSTERED INDEX [IX_RouteCodePool_SupplierZone] ON [dbo].[RouteOptionsPool] (	[SupplierID] ASC,	[SupplierZoneID] ASC)
		WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

		CREATE NONCLUSTERED INDEX [IX_RouteCodePool_Multikeys] ON [dbo].[RouteOptionsPool](
		[SupplierID] ASC,[Code] ASC,[SupplierZoneID] ASC,[ActiveRate] ASC,	[SupplierServicesFlag] ASC)
		WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]







		
END


--------------------ACTION TABLES-------------------------------
----------------------------------------------------------------
----------------------------------------------------------------
------------------ Init Block Table ----------------------------
IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'dbo.[Route_Block]') AND s.type in (N'U'))
	BEGIN
		 TRUNCATE TABLE Route_Block
	END
	
ELSE 
  BEGIN
		
			CREATE TABLE [dbo].[Route_Block](
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
				--[Updated] [datetime] NULL,
			-- CONSTRAINT [PK_Route_Block] PRIMARY KEY CLUSTERED 
			--(
			--	[RouteID] ASC
			--)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) 
			)ON [PRIMARY]
CREATE NONCLUSTERED INDEX [IX_Route_Block_multikeys] ON [dbo].[Route_Block](	[CustomerID] ASC,	[Code] ASC,	[OurZoneID] ASC)
INCLUDE ( [IsToDAffected],[IsSpecialRequestAffected],[IsOverrideAffected],[IsBlockAffected],[IsOptionBlock]) 
WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

  END

IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'dbo.[RouteOption_Block]') AND s.type in (N'U'))
	BEGIN
		 TRUNCATE TABLE [RouteOption_Block]
	END
	
ELSE 
  BEGIN
		
					CREATE TABLE [dbo].[RouteOption_Block](
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
				
CREATE NONCLUSTERED INDEX [IX_RouteOption_Block_multikey] ON [dbo].[RouteOption_Block] ([CustomerID] ASC,[SupplierID] ASC,[Code] ASC)
INCLUDE ( [Priority],[NumberOfTries],[State],[Percentage],[ActionType]) 
WITH (STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

  END

IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'dbo.[RouteBlockConcatinated]') AND s.type in (N'U'))
	BEGIN
		 TRUNCATE TABLE [RouteBlockConcatinated]
	END
	
ELSE 
  BEGIN
  CREATE TABLE RouteBlockConcatinated (RouteBlockID INT, CustomerID VARCHAR(5), SupplierID VARCHAR(5), Code VARCHAR(20), ZoneID INT, UpdatedDate SMALLDATETIME, IncludeSubCodes CHAR(1), ExcludedCodes VARCHAR(max), ParentID INT, OriginalExcluded VARCHAR(Max))

CREATE NONCLUSTERED INDEX [IX_RouteBlockConcatinated_multikey] ON [dbo].[RouteBlockConcatinated] ([CustomerID] ASC,[SupplierID] ASC,[Code] ASC)
INCLUDE ( [ZoneID]) 
WITH (STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

END
------------------ Init Override Tables ----------------------------
IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'dbo.[Route_Override]') AND s.type in (N'U'))
	BEGIN
		 TRUNCATE TABLE Route_Override
	END
	
ELSE 
  BEGIN
		
			CREATE TABLE [dbo].[Route_Override](
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
				--[Updated] [datetime] NULL,
			-- CONSTRAINT [PK_Route_Override] PRIMARY KEY CLUSTERED 
			--(
			--	[RouteID] ASC
			--)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) 
			)ON [PRIMARY]
			
			CREATE NONCLUSTERED INDEX [IX_RouteOverride_mutikeys] ON [Route_Override] ([RuleID],[Code]) INCLUDE ([CustomerID])

  END

IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'dbo.[RouteOption_Override]') AND s.type in (N'U'))
	BEGIN
		 TRUNCATE TABLE [RouteOption_Override]
	END
	
ELSE 
  BEGIN
		
					CREATE TABLE [dbo].[RouteOption_Override](
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
				
 CREATE NONCLUSTERED INDEX [IX_RouteOption_Override_multikey] ON [dbo].[RouteOption_Override] ([CustomerID] ASC,[SupplierID] ASC,[Code] ASC)
INCLUDE ( [Priority],[NumberOfTries],[State],[Percentage],[ActionType]) 
WITH (STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

  END


------------------ Init Override Tables ----------------------------
IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'dbo.[Route_Special]') AND s.type in (N'U'))
	BEGIN
		 TRUNCATE TABLE Route_Special
	END
	
ELSE 
  BEGIN
		
			CREATE TABLE [dbo].[Route_Special](
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
				--[Updated] [datetime] NULL,
			-- CONSTRAINT [PK_Route_Special] PRIMARY KEY CLUSTERED 
			--(
			--	[RouteID] ASC
			--)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) 
			)ON [PRIMARY]
			
			CREATE NONCLUSTERED INDEX [IX_RouteSpecial_mutikeys] ON [Route_Special] ([RuleID],[Code]) INCLUDE ([CustomerID])

  END

IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'dbo.[RouteOption_Special]') AND s.type in (N'U'))
	BEGIN
		 TRUNCATE TABLE [RouteOption_Special]
	END
	
ELSE 
  BEGIN
		
					CREATE TABLE [dbo].[RouteOption_Special](
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
				
CREATE NONCLUSTERED INDEX [IX_RouteOption_Special_multikey] ON [dbo].[RouteOption_Special] ([CustomerID] ASC,[SupplierID] ASC,[Code] ASC)
INCLUDE ( [Priority],[NumberOfTries],[State],[Percentage],[ActionType]) 
WITH (STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

  END
--------------------ACTION TABLES-------------------------------
----------------------------------------------------------------
----------------------------------------------------------------



--------------------NON LCR TABLES-------------------------------
----------------------------------------------------------------
----------------------------------------------------------------
IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'dbo.[Route_NonLCR]') AND s.type in (N'U'))
	BEGIN
		 TRUNCATE TABLE [Route_NonLCR]
	END
	
ELSE 
  BEGIN
		
			CREATE TABLE [dbo].[Route_NonLCR](
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
				--[Updated] [datetime] NULL,
			-- CONSTRAINT [PK_Route_NonLCR] PRIMARY KEY CLUSTERED 
			--(
			--	[RouteID] ASC
			--)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) 
			)ON [PRIMARY]
CREATE NONCLUSTERED INDEX [IX_RouteNonLCR_multikeys] ON [dbo].[Route_NonLCR](	[CustomerID] ASC,	[Code] ASC,	[OurZoneID] ASC)
INCLUDE ( [IsToDAffected],[IsSpecialRequestAffected],[IsOverrideAffected],[IsBlockAffected],[IsOptionBlock]) 
WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

  END
  
    IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'dbo.[RouteOption_NonLCR]') AND s.type in (N'U'))
	BEGIN
		 TRUNCATE TABLE [RouteOption_NonLCR]
	END
	
ELSE 
  BEGIN
		
					CREATE TABLE [dbo].[RouteOption_NonLCR](
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
				
CREATE NONCLUSTERED INDEX [IX_RouteOptionNonLCR_multikey] ON [dbo].[RouteOption_NonLCR] ([CustomerID] ASC,[SupplierID] ASC,[Code] ASC)
INCLUDE ( [Priority],[NumberOfTries],[State],[Percentage],[ActionType]) 
WITH (STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

  END

			--------------------NON LCR TABLES-------------------------------
	-----------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------
IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'dbo.[NonMarketPriceOptions]') AND s.type in (N'U'))
	BEGIN
		 TRUNCATE TABLE [NonMarketPriceOptions]
	END
	
ELSE 
  BEGIN    
CREATE TABLE [dbo].[NonMarketPriceOptions](
	[RouteID] [int] NULL,
	[SupplierID] [varchar](5) NULL
) ON [PRIMARY]


end
END