

CREATE PROCEDURE  [TOneWhS_SPL].[sp_SupplierZoneRate_Preview_GetNumberOfZonesWithChangeType]
	@ProcessInstanceID_IN INT,
	@ZoneChangeType_IN INT
AS
BEGIN

	SELECT	COUNT(ZoneChangeType)
	FROM	[TOneWhS_SPL].[SupplierZoneRate_Preview] WITH(NOLOCK)
	WHERE	ProcessInstanceID = @ProcessInstanceID_IN and ZoneChangeType=@ZoneChangeType_IN
	
END