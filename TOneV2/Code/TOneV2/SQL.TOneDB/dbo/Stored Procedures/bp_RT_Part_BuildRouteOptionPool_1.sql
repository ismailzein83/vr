

CREATE PROCEDURE [dbo].[bp_RT_Part_BuildRouteOptionPool]
	 @RebuildRouteOptionPool BIT = 1
AS
BEGIN
	SET NOCOUNT ON;
	PRINT @RebuildRouteOptionPool
	IF(@RebuildRouteOptionPool = 0)
		BEGIN
			INSERT INTO #RouteOptionsPool
			SELECT rop.Code, 
			   rop.SupplierID, 
			   rop.SupplierZoneID,
			   rop.SupplierServicesFlag, 
			   rop.ProfileId, 
			   rop.ActiveRate,
			   rop.IsBlock, 
			   rop.IsTOD
			FROM RouteOptionsPool rop
			INNER JOIN #CostZoneFilter czf ON rop.SupplierZoneID = czf.ZoneID AND rop.SupplierID = czf.SupplierID COLLATE DATABASE_DEFAULT
END			
	ELSE
		BEGIN		
			;WITH BlockedCodesTemp AS (
			SELECT
					CM.Code as	BlockCode,
					cm.SupplierID,
					1 IsBlock
			FROM
					CodeMatch CM 
					LEFT JOIN RouteBlockConcatinated grb ON cm.SupplierID COLLATE DATABASE_DEFAULT  = grb.SupplierID
			WHERE	(grb.SupplierID <> 'Sys' and grb.IncludeSubCodes = 'Y' AND  CM.Code COLLATE DATABASE_DEFAULT  like (grb.Code + '%')
					 AND CM.Code NOT IN (SELECT * FROM ParseArray(grb.ExcludedCodes,','))) 
					or (grb.SupplierID <> 'Sys' and grb.IncludeSubCodes = 'N' AND CM.Code COLLATE DATABASE_DEFAULT  = grb.Code) 	
			)			
					
			INSERT INTO #RouteOptionsPool With(Tablock)
			SELECT
					CM.Code,
					CM.SupplierID,
					CM.SupplierZoneID,	
					czr.ServicesFlag,
					czr.ProfileID,
					czr.ActiveRate,
					ISNULL(bc.IsBlock,czr.IsBlock) IsBlock,
					czr.IsTOD
					
			FROM
					CodeMatch CM 
					INNER JOIN #CostZoneRates czr ON cm.SupplierZoneID = czr.ZoneID AND czr.SupplierID = cm.SupplierID COLLATE DATABASE_DEFAULT
					LEFT JOIN BlockedCodesTemp bc ON cm.Code COLLATE DATABASE_DEFAULT = bc.BlockCode COLLATE DATABASE_DEFAULT
					INNER JOIN #ActiveSuppliers ac ON cm.SupplierID COLLATE DATABASE_DEFAULT = ac.CarrierAccountID 
		END
			
		
		
		CREATE NONCLUSTERED INDEX [IX_#RouteOptionsPool_CodeSupplierID] ON [dbo].[#RouteOptionsPool] (	[SupplierID] ASC,	[Code] ASC)
		WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
		
		CREATE NONCLUSTERED INDEX [IX_#RouteCodePool_SupplierZone] ON [dbo].[#RouteOptionsPool] (	[SupplierID] ASC,	[SupplierZoneID] ASC)
		WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

		CREATE NONCLUSTERED INDEX [IX_#RouteCodePool_Multikeys] ON [dbo].[#RouteOptionsPool](
		[SupplierID] ASC,[Code] ASC,[SupplierZoneID] ASC,[ActiveRate] ASC,	[SupplierServicesFlag] ASC)
		WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

----Write Back
DELETE rop FROM RouteOptionsPool rop
INNER JOIN #RouteOptionsPool ron ON rop.Code COLLATE DATABASE_DEFAULT = ron.Code AND rop.SupplierID COLLATE DATABASE_DEFAULT = ron.SupplierID

INSERT INTO RouteOptionsPool
SELECT rop.Code, 
	   rop.SupplierID,
	   rop.SupplierZoneID, 
	   rop.SupplierServicesFlag,
       rop.ProfileId, 
       rop.ActiveRate, 
       rop.IsBlock, 
       rop.IsTOD
  FROM #RouteOptionsPool rop
		
--Testing
SELECT 'RouteOptionsPool',* FROM #RouteOptionsPool	
	
END