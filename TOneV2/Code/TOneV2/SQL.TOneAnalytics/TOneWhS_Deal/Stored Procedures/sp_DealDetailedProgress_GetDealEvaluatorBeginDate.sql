Create PROCEDURE [TOneWhS_Deal].[sp_DealDetailedProgress_GetDealEvaluatorBeginDate]
	@LastTimeStamp timestamp
AS
BEGIN
	Select min(FromTime) as BegintDate
	From [TOneV2_Dev_Analytics].[TOneWhS_Deal].[DealDetailedProgress]
	Where @LastTimeStamp is null or [timestamp] > @LastTimeStamp
END