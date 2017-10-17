

CREATE PROCEDURE [TOneWhS_BE].[sp_SalePriceListChange_GetNotSentCodeChanges]
@CustomerIds nvarchar(max)
AS
BEGIN
DECLARE @CustomerIDsTable TABLE (CustomerID int)
INSERT INTO @CustomerIDsTable (CustomerID)
select Convert(int, ParsedString) from [TOneWhS_BE].[ParseStringList](@CustomerIds)
;with customerPriceLists as 
(
	select id 
	from TOneWhS_BE.SalePriceList sp WITH(NOLOCK)
	where (@CustomerIds  is null or ( sp.OwnerID in (select CustomerID from @CustomerIDsTable) and sp.OwnerType=1))
	and sp.IsSent = 0
)
SELECT  scc.PricelistID
		,scc.CountryID
		,scc.CustomerID
		,codec.Code
		,codec.RecentZoneName
		,codec.ZoneName,
		codec.Change,
		codec.BED,
		codec.EED
FROM TOneWhS_BE.SalePricelistCustomerChange scc WITH(NOLOCK)
JOIN TOneWhS_BE.SalePricelistCodeChange codec WITH(NOLOCK) ON codec.batchid = scc.BatchID
where scc.PricelistID in ( select id from customerPriceLists)
order by scc.PricelistID desc

END