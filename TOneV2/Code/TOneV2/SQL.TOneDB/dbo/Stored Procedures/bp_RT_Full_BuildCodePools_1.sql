
CREATE PROCEDURE [dbo].[bp_RT_Full_BuildCodePools]
WITH RECOMPILE
AS
BEGIN
SET NOCOUNT ON

TRUNCATE TABLE RouteOptionsPool	
TRUNCATE TABLE RoutePool	
-- DROP INDEXES
--DROP INDEX IX_RouteOptionsPool_Code ON RouteOptionsPool
--DROP INDEX IX_RouteOptionsPool_Zone ON RouteOptionsPool
--DROP INDEX IX_RouteOptionsPool_Supplier ON RouteOptionsPool
		
--DROP INDEX IX_RoutePool_Code ON RoutePool
--DROP INDEX IX_RoutePool_Zone ON RoutePool

DROP INDEX [IX_RouteOptionsPool_CodeSupplierID] ON RouteOptionsPool
DROP INDEX [IX_RouteCodePool_SupplierZone] ON RouteOptionsPool
DROP INDEX [IX_RouteCodePool_Multikeys] ON RouteOptionsPool

DROP INDEX IX_RoutePool_ZoneIncCode ON RoutePool
DROP INDEX IX_RoutePool_CodeIncZone ON RoutePool
EXEC bp_RT_Full_PrepareRouteBlockonCodes

---- We should form the excluding code list of blocked zones on a code
;WITH BlockedCodesTemp AS (
SELECT
		CM.Code as	BlockCode
		,cm.SupplierID,
		1 IsBlock
		--,CM.SupplierZoneID ZoneID
FROM
		CodeMatch CM 
		LEFT JOIN RouteBlockConcatinated grb ON cm.SupplierID COLLATE DATABASE_DEFAULT  = grb.SupplierID
WHERE	(grb.SupplierID <> 'Sys' and grb.IncludeSubCodes = 'Y' AND  CM.Code COLLATE DATABASE_DEFAULT  like (grb.Code + '%')
		 AND CM.Code NOT IN (SELECT * FROM ParseArray(grb.ExcludedCodes,','))) 
		or (grb.SupplierID <> 'Sys' and grb.IncludeSubCodes = 'N' AND CM.Code COLLATE DATABASE_DEFAULT  = grb.Code) 	
)			
		
INSERT INTO RouteOptionsPool With(Tablock)
SELECT
		CM.Code,
		CM.SupplierID,
		CM.SupplierZoneID,	
		ZR.ServicesFlag,
		--GETDATE() Updated,
		ZR.ProfileID,
		ZR.ActiveRate,
		ISNULL(bc.IsBlock,zr.IsBlock) IsBlock,
		ZR.IsTOD
		
FROM
		CodeMatch CM INNER JOIN ZoneRates ZR ON CM.SupplierZoneID = ZR.ZoneID 
		AND ZR.SupplierID <> 'SYS'
		LEFT JOIN BlockedCodesTemp bc ON cm.Code COLLATE DATABASE_DEFAULT = bc.BlockCode COLLATE DATABASE_DEFAULT AND bc.SupplierID COLLATE DATABASE_DEFAULT = ZR.SupplierID COLLATE DATABASE_DEFAULT


;WITH BlockedCodesPoolTemp AS (
SELECT
		CM.Code as	BlockCode
		,cm.SupplierID,
		1 IsBlock
		--,CM.SupplierZoneID ZoneID
FROM
		CodeMatch CM 
		LEFT JOIN RouteBlockConcatinated grb ON cm.SupplierID COLLATE DATABASE_DEFAULT  = grb.SupplierID
WHERE	(grb.SupplierID = 'Sys' AND  grb.IncludeSubCodes = 'Y' AND  CM.Code COLLATE DATABASE_DEFAULT like (grb.Code + '%')
		AND CM.Code   NOT IN (SELECT * FROM ParseArray(grb.ExcludedCodes,','))) 
		or (grb.SupplierID = 'Sys' AND grb.IncludeSubCodes = 'N' AND CM.Code COLLATE DATABASE_DEFAULT  = grb.Code) 
)	
	
	
INSERT INTO RoutePool With(Tablock)
SELECT
		CM.Code,
		CM.SupplierZoneID,
		ISNULL(bc.IsBlock,0) IsBlock,
		z.CodeGroup CodeGroup
	
FROM
		CodeMatch CM 
		LEFT JOIN BlockedCodesPoolTemp bc ON cm.Code COLLATE DATABASE_DEFAULT = bc.BlockCode
		LEFT JOIN Zone z ON z.ZoneID = cm.SupplierZoneID
WHERE	CM.SupplierID = 'sys' AND z.IsEffective = 'y' AND z.SupplierID = 'sys'


-- Using temp table for the blocked routes
--CREATE TABLE #BlockedCodesTemp(Code VARCHAR(20),SupplierID VARCHAR(5),IsBlock BIT)
--INSERT INTO #BlockedCodesTemp With(Tablock)
--SELECT
--		CM.Code as	BlockCode
--		,cm.SupplierID,
--		1 IsBlock
--		--,CM.SupplierZoneID ZoneID
--		FROM
--		CodeMatch CM WITH(NOLOCK) 
--			LEFT JOIN RouteBlockConcatinated grb WITH(NOLOCK) ON cm.SupplierID = grb.SupplierID
--WHERE (grb.SupplierID <> 'Sys' and grb.IncludeSubCodes = 'Y' 
--			 AND  CM.Code like (grb.Code + '%')
--			 AND CM.Code NOT IN (SELECT * FROM ParseArray(grb.ExcludedCodes,','))) 
--			or (grb.SupplierID <> 'Sys' and grb.IncludeSubCodes = 'N' AND CM.Code = grb.Code) 	
			
--			CREATE TABLE #BlockedCodesPoolTemp(Code VARCHAR(20),SupplierID VARCHAR(5),IsBlock BIT)

--INSERT INTO #BlockedCodesPoolTemp With(Tablock)
--SELECT
--		CM.Code as	BlockCode
--		,cm.SupplierID,
--		1 IsBlock
--		--,CM.SupplierZoneID ZoneID
--		FROM
--		CodeMatch CM WITH(NOLOCK) 
--			LEFT JOIN RouteBlockConcatinated grb WITH(NOLOCK) ON cm.SupplierID = grb.SupplierID
--WHERE ( grb.SupplierID = 'Sys' AND  grb.IncludeSubCodes = 'Y' 
--			 AND  CM.Code like (grb.Code + '%')
--			 AND CM.Code NOT IN (SELECT * FROM ParseArray(grb.ExcludedCodes,','))) 
--			or (grb.SupplierID = 'Sys' AND grb.IncludeSubCodes = 'N' AND CM.Code = grb.Code) 


--		INSERT INTO RouteOptionsPool With(Tablock)
--		SELECT
--		CM.Code,
--		CM.SupplierID,
--		CM.SupplierZoneID,	
--		ZR.ServicesFlag,
--		--GETDATE() Updated,
--		ZR.ProfileID,
--		ZR.ActiveRate,
--		ISNULL(bc.IsBlock,0) IsBlock,
--		ZR.IsTOD
		
--	FROM
--	CodeMatch CM WITH(NOLOCK) INNER JOIN ZoneRates ZR WITH(NOLOCK) ON CM.SupplierZoneID = ZR.ZoneID 
--			AND ZR.SupplierID <> 'SYS'
--			LEFT JOIN #BlockedCodesTemp bc ON cm.Code COLLATE DATABASE_DEFAULT = bc.Code COLLATE DATABASE_DEFAULT AND bc.SupplierID COLLATE DATABASE_DEFAULT = ZR.SupplierID COLLATE DATABASE_DEFAULT

		
--	INSERT INTO RoutePool With(Tablock)
--		SELECT
--		CM.Code,
--		CM.SupplierZoneID,
--		ISNULL(bc.IsBlock,0) IsBlock,
--		z.CodeGroup CodeGroup
		
--	FROM
--	CodeMatch CM WITH(NOLOCK) 
--			LEFT JOIN #BlockedCodesPoolTemp bc ON cm.Code COLLATE DATABASE_DEFAULT = bc.Code
--			LEFT JOIN Zone z WITH(NOLOCK) ON z.ZoneID = cm.SupplierZoneID
--		WHERE CM.SupplierID = 'sys' AND z.IsEffective = 'y' AND z.SupplierID = 'sys'
	

	
		-- Recreate INDEXES
		--CREATE NONCLUSTERED INDEX IX_RouteOptionsPool_Code ON RouteOptionsPool(Code ASC)
		--CREATE NONCLUSTERED INDEX IX_RouteOptionsPool_Supplier ON RouteOptionsPool(SupplierID ASC)
		--CREATE NONCLUSTERED INDEX IX_RouteOptionsPool_Zone ON RouteOptionsPool(SupplierZoneID ASC)
		
		-- CREATE NONCLUSTERED INDEX [IX_RoutePool_Zone] ON [dbo].[RoutePool]([ZoneID] ASC)
		-- CREATE NONCLUSTERED INDEX [IX_RoutePool_Code] ON [dbo].[RoutePool]([Code] ASC)
		 
		 
		CREATE NONCLUSTERED INDEX [IX_RouteOptionsPool_CodeSupplierID] ON [dbo].[RouteOptionsPool] (	[SupplierID] ASC,	[Code] ASC)
			WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
		CREATE NONCLUSTERED INDEX [IX_RouteCodePool_SupplierZone] ON [dbo].[RouteOptionsPool] (	[SupplierID] ASC,	[SupplierZoneID] ASC)
			WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
		CREATE NONCLUSTERED INDEX [IX_RouteCodePool_Multikeys] ON [dbo].[RouteOptionsPool](
			[SupplierID] ASC,[Code] ASC,[SupplierZoneID] ASC,[ActiveRate] ASC,	[SupplierServicesFlag] ASC)
			WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

		CREATE NONCLUSTERED INDEX [IX_RoutePool_ZoneIncCode] ON [dbo].[RoutePool] (	[ZoneID] ASC)INCLUDE ( [Code]) 
			WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
		CREATE NONCLUSTERED INDEX [IX_RoutePool_CodeIncZone] ON [dbo].[RoutePool] (	[Code] ASC)INCLUDE ( [ZoneID]) 
			WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

END