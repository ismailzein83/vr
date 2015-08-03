

CREATE PROCEDURE [dbo].[bp_RT_Part_BuildRoutePool]
	 @RebuildRoutePool BIT = 1
AS
BEGIN
	SET NOCOUNT ON;
	
	IF(@RebuildRoutePool = 0)
		BEGIN
		
			INSERT INTO #RoutePool
			SELECT 	rp.Code, 
			        rp.ZoneID, 
			        rp.IsBlocked, 
			        rp.CodeGroup
			FROM RoutePool rp
			INNER JOIN #SaleZoneFilter szf ON rp.ZoneID = szf.ZoneID
	
		END
	
	ELSE
	BEGIN
		
		;WITH BlockedCodesPoolTemp AS (
			SELECT
					CM.Code as	BlockCode
					,cm.SupplierID,
					1 IsBlock
			FROM
					CodeMatch CM 
					LEFT JOIN RouteBlockConcatinated grb ON cm.SupplierID COLLATE DATABASE_DEFAULT  = grb.SupplierID
			WHERE	(grb.SupplierID = 'Sys' AND  grb.IncludeSubCodes = 'Y' AND  CM.Code COLLATE DATABASE_DEFAULT like (grb.Code + '%')
					AND CM.Code  NOT IN (SELECT * FROM ParseArray(grb.ExcludedCodes,','))) 
					or (grb.SupplierID = 'Sys' AND grb.IncludeSubCodes = 'N' AND CM.Code COLLATE DATABASE_DEFAULT  = grb.Code) 
			)	
				
				
			INSERT INTO #RoutePool With(Tablock)
			SELECT
					CM.Code,
					CM.SupplierZoneID,
					ISNULL(bc.IsBlock,0) IsBlock,
					szf.CodeGroup
				
			FROM
					CodeMatch CM 
					LEFT JOIN BlockedCodesPoolTemp bc ON cm.Code COLLATE DATABASE_DEFAULT = bc.BlockCode
					INNER JOIN #SaleZoneFilter szf ON szf.ZoneID = cm.SupplierZoneID
					INNER JOIN #CodeFilter cf ON cm.Code COLLATE DATABASE_DEFAULT = cf.Code
			WHERE	CM.SupplierID = 'sys' 
					

		END
			CREATE NONCLUSTERED INDEX [IX_#RoutePool_ZoneIncCode] ON [dbo].[#RoutePool] (	[ZoneID] ASC)INCLUDE ( [Code]) 
		WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

		CREATE NONCLUSTERED INDEX [IX_#RoutePool_CodeIncZone] ON [dbo].[#RoutePool] (	[Code] ASC)INCLUDE ( [ZoneID]) 
		WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]


--- Write Back
DELETE rp FROM RoutePool rp
INNER JOIN #RoutePool rpn ON rpn.Code COLLATE DATABASE_DEFAULT = rp.Code

INSERT INTO RoutePool
SELECT * FROM #RoutePool rp
--Testing
SELECT 'RoutePool',* FROM #RoutePool
	
END