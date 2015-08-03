-- =============================================================================
-- Author:		Fadi Chamieh
-- Create date: 2011-02-17
-- Description:	Clean Up Entities that may become invalid after
--              a state backup restore
-- =============================================================================
CREATE PROCEDURE [dbo].[bp_CleanUpAfterStateBackupRestore]
AS
BEGIN
	SET NOCOUNT ON;
	 
	SELECT ZoneID,SupplierID INTO [#TempCleanup] FROM Zone
	DELETE FROM RouteBlock WHERE ZoneID IS NOT NULL AND ZoneID NOT IN (SELECT ZoneID From [#TempCleanup] WHERE SupplierID <> 'SYS')
	DELETE FROM RouteOverride WHERE OurZoneID IS NOT NULL AND OurZoneID NOT IN (SELECT ZoneID From [#TempCleanup] WHERE SupplierID = 'SYS')
	DELETE FROM PricingTemplatePlan WHERE ZoneID IS NOT NULL AND ZoneID NOT IN (SELECT ZoneID From [#TempCleanup] WHERE SupplierID = 'SYS')
	DELETE FROM BQR_CriterionOnZone WHERE ZoneID IS NOT NULL AND ZoneID NOT IN (SELECT ZoneID From [#TempCleanup])
	DELETE FROM BQR_Action WHERE TargetSupplierZoneID NOT IN (SELECT ZoneID From [#TempCleanup])
	DELETE FROM Pricelistdata where Pricelistid  not in (select pricelistid from pricelist)
	DELETE FROM Commission WHERE ZoneID IS NOT NULL AND ZoneID NOT IN (SELECT ZoneID FROM [#TempCleanup])
	DELETE FROM ToDConsideration WHERE ZoneID IS NOT NULL AND ZoneID NOT IN (SELECT ZoneID FROM [#TempCleanup])
	DELETE FROM Tariff WHERE ZoneID IS NOT NULL AND ZoneID NOT IN (SELECT ZoneID FROM [#TempCleanup])
	-- DELETE FROM Route WHERE OurZoneID > 0 AND OurZoneID NOT IN (SELECT ZoneID FROM Zone WHERE SupplierID = 'SYS')
	-- DELETE FROM RouteOption WHERE SupplierZoneID > 0 AND SupplierZoneID NOT IN (SELECT ZoneID FROM Zone WHERE SupplierID <> 'SYS')
END