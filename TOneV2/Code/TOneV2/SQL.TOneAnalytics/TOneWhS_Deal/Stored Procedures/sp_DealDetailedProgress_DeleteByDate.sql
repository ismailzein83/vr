CREATE PROCEDURE [TOneWhS_Deal].[sp_DealDetailedProgress_DeleteByDate]
	@IsSale bit,
	@BeginDate datetime,
	@EndDate datetime
AS
BEGIN
	delete from [TOneWhS_Deal].[DealDetailedProgress]
	where IsSale = @IsSale 
	and (@EndDate is null or [FromTime] < @EndDate)
	and (@BeginDate is null or [ToTime] > @BeginDate)
END