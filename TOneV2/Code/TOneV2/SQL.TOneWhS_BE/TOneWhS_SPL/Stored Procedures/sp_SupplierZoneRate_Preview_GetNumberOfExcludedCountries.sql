
CREATE PROCEDURE  [TOneWhS_SPL].[sp_SupplierZoneRate_Preview_GetNumberOfExcludedCountries]
AS
BEGIN

	SELECT count(distinct countryID)
  FROM [TOneV2_Dev].[TOneWhS_SPL].[SupplierZoneRate_Preview]
  where IsExcluded=1
	
END