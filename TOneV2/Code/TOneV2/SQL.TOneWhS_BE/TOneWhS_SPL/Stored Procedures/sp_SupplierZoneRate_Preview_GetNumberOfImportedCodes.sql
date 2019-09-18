

CREATE PROCEDURE  [TOneWhS_SPL].[sp_SupplierZoneRate_Preview_GetNumberOfImportedCodes]
	@ProcessInstanceID_IN INT
AS
BEGIN

	--SELECT	COUNT(*)
	--FROM	[TOneWhS_SPL].[SupplierCode_Preview] WITH(NOLOCK)
	--WHERE	ProcessInstanceID = @ProcessInstanceID_IN 
	--AND eed is null


			SELECT	COUNT(*)
	FROM	[TOneWhS_SPL].[SupplierCode_Preview] cp WITH(NOLOCK)
	WHERE	cp.ProcessInstanceID = @ProcessInstanceID_IN and cp.EED is null 
	and cp.ZoneName in ( select zonename from [TOneWhS_SPL].SupplierZoneRate_Preview where ProcessInstanceID=@ProcessInstanceID_IN and ImportedRate is not null)
END