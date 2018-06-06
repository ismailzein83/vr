drop table #ZonesWithValidCountries
go
with zoneAllCountries as (SELECT 
	zone.ID ZoneID, zone.CountryID ExistingCountryID, codeGroup.CountryID ValidCountryID
	, ROW_NUMBER() OVER(PARTITION BY zone.ID ORDER BY codeGroup.Code desc)  countryPriority FROM
[TOneWhS_BE].[SupplierZone] zone
JOIN [TOneWhS_BE].[SupplierCode] code ON zone.ID = code.ZoneID
JOIN [TOneWhS_BE].[CodeGroup] codeGroup ON code.CodeGroupID = codeGroup.ID
)

select * 
INTO #ZonesWithValidCountries
from zoneAllCountries
where countryPriority = 1 AND ExistingCountryID <> ValidCountryID
order by zoneID, ValidCountryID desc

select * from #ZonesWithValidCountries

UPDATE zone
SET CountryID = ZonesWithValidCountries.ValidCountryID
FROM [TOneWhS_BE].[SupplierZone] zone
JOIN #ZonesWithValidCountries ZonesWithValidCountries ON zone.ID = ZonesWithValidCountries.ZoneID