CREATE PROCEDURE [TOneWhS_Deal].[sp_DealDetailedProgress_GetByDealId]
	@DealIds nvarchar(max),
	@FromDate datetime,
	@ToDate datetime
AS
BEGIN
		
	Declare @DealIdsLocal nvarchar(max)
	Declare @FromDateLocal Datetime
	Declare @ToDateLocale Datetime

	set @DealIdsLocal=@DealIds
	set @FromDateLocal=	@FromDate
	set @ToDateLocale =@ToDate

	DECLARE @DealIDsTable TABLE (DealID int)
	INSERT INTO @DealIDsTable (DealID)
	select Convert(int, ParsedString) from [TOneWhS_Deal].[ParseStringList](@DealIdsLocal)

	SELECT ddp.[ID],ddp.[DealID],ddp.[ZoneGroupNb],ddp.[IsSale],ddp.[TierNb],ddp.[RateTierNb],ddp.[FromTime],ddp.[ToTime],ddp.[ReachedDurationInSec],ddp.[CreatedTime]
	FROM [TOneWhS_Deal].[DealDetailedProgress] ddp  WITH(NOLOCK) 
	WHERE (@DealIds  is null or ddp.DealID in (select DealID from @DealIDsTable))
	and (@FromDateLocal is null or ddp.[ToTime] >= @FromDateLocal)
	and (@ToDateLocale is null or ddp.FromTime<@ToDateLocale)

	and tierNb is not null

END