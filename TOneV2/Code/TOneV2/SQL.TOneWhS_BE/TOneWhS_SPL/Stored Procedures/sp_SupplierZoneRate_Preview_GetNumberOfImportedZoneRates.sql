

CREATE PROCEDURE  [TOneWhS_SPL].[sp_SupplierZoneRate_Preview_GetNumberOfImportedZoneRates]
	@ProcessInstanceID_IN INT
AS
BEGIN

	SELECT	COUNT(*)
	FROM	[TOneWhS_SPL].SupplierZoneRate_Preview WITH(NOLOCK)
	WHERE	ProcessInstanceID = @ProcessInstanceID_IN 
	and zoneeed is null
END