

CREATE PROCEDURE [TOneWhS_BE].[sp_SalePriceListChange_GetNotSentRateChanges]
@CustomerIds nvarchar(max)
AS
BEGIN
DECLARE @CustomerIDsTable TABLE (CustomerID int)
INSERT INTO @CustomerIDsTable (CustomerID)
select Convert(int, ParsedString) from [TOneWhS_BE].[ParseStringList](@CustomerIds)

;with customerPricelist as 
(
	select id from  TOneWhS_BE.SalePriceList sp
	where (@CustomerIds  is null or ( sp.OwnerID in (select CustomerID from @CustomerIDsTable) and sp.OwnerType=1))
	and sp.IsSent=0

)
SELECT  scc.PricelistID
		,scc.CountryID
		,scc.CustomerID
		,spr.RecentRate
		,spr.Rate
		,spr.Change
		,spr.ZoneName
		,spr.BED
		,spr.EED

FROM TOneWhS_BE.SalePricelistRateChange spr
JOIN customerPricelist sp ON sp.id = spr.PricelistID
JOIN TOneWhS_BE.SalePricelistCustomerChange scc ON scc.PricelistId= sp.ID 

order by id desc
END