

CREATE PROCEDURE [TOneWhS_BE].[sp_SalePriceListChange_GetNotSentRateChanges]
@CustomerIds nvarchar(max)
AS
BEGIN
DECLARE @CustomerIDsTable TABLE (CustomerID int)
INSERT INTO @CustomerIDsTable (CustomerID)
select Convert(int, ParsedString) from [TOneWhS_BE].[ParseStringList](@CustomerIds)

;with customerPricelist as 
(
	select id from  TOneWhS_BE.SalePriceList sp WITH(NOLOCK)
	where (@CustomerIds  is null or ( sp.OwnerID in (select CustomerID from @CustomerIDsTable) and sp.OwnerType=1))
	and sp.IsSent=0

)
SELECT  scc.PricelistID
		,scc.CountryID
		,scc.CustomerID
		,spr.RecentRate
		,spr.Rate
		,[RateTypeId]
		,spr.Change
		,spr.ZoneName
		,spr.BED
		,spr.EED
		,spr.RoutingProductID
FROM TOneWhS_BE.SalePricelistRateChange spr WITH(NOLOCK)
JOIN customerPricelist sp ON sp.id = spr.PricelistID
JOIN TOneWhS_BE.SalePricelistCustomerChange scc WITH(NOLOCK) ON scc.PricelistId= sp.ID 

order by id desc
END