CREATE PROCEDURE [Retail_CDR].[sp_CDRCost_GetDistinctDatesAfterId]
	@CDRCostId Bigint
AS
BEGIN
  SELECT distinct CONVERT(DATE, AttemptDateTime) as AttemptDateTime
  FROM [Retail_CDR].[CDRCost] with(nolock)
  where (@CDRCostId is null or ID > @CDRCostId) and ISNULL(IsDeleted, 0) <> 1
END