CREATE PROCEDURE [TOneWhS_Deal].[sp_DealDetailedProgress_GetDealEvaluatorBeginDate]
	@LastTimeStamp timestamp
AS
BEGIN
	Select min(FromTime) as BegintDate
	From [TOneWhS_Deal].[DealDetailedProgress]
	Where @LastTimeStamp is null or [timestamp] > @LastTimeStamp
END