
CREATE PROCEDURE  [TOneWhS_SPL].[sp_SupplierZoneRate_Preview_GetNumberOfZonesWithChangedSe]
	@ProcessInstanceID_IN INT
AS
BEGIN

	SELECT	COUNT(ZoneServiceChangeType)
	FROM	[TOneWhS_SPL].[SupplierZoneRate_Preview] WITH(NOLOCK)
	WHERE	ProcessInstanceID = @ProcessInstanceID_IN and ZoneServiceChangeType <>0
	
END