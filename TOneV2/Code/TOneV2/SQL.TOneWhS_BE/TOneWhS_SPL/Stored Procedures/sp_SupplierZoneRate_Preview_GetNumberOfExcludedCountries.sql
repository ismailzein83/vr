
CREATE PROCEDURE  [TOneWhS_SPL].[sp_SupplierZoneRate_Preview_GetNumberOfExcludedCountries]
	@ProcessInstanceID_IN INT
AS
BEGIN

	SELECT count(distinct countryID)
  FROM [TOneWhS_SPL].[SupplierZoneRate_Preview]
  where ProcessInstanceID = @ProcessInstanceID_IN and IsExcluded=1
	
END