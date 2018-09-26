

CREATE PROCEDURE  [TOneWhS_SPL].[sp_SupplierZoneRate_Preview_GetNumberOfImportedCodes]
	@ProcessInstanceID_IN INT
AS
BEGIN

	SELECT	COUNT(*)
	FROM	[TOneWhS_SPL].[SupplierCode_Preview] WITH(NOLOCK)
	WHERE	ProcessInstanceID = @ProcessInstanceID_IN 
	AND eed is null
END