CREATE PROCEDURE [Retail_CDR].[sp_CDRCost_UpadeOverridenAfterDate]
	@FromTime datetime
AS
BEGIN
	WITH CDRCostTemp AS (
	SELECT *
	FROM [Retail_CDR].[CDRCost]
	where (@FromTime is null or AttemptDateTime >= @FromTime)
	)

	UPDATE cdrCost2
	SET cdrCost2.IsDeleted = 1
	FROM CDRCostTemp cdrCost1
	JOIN CDRCostTemp cdrCost2 ON cdrCost1.SourceID = cdrCost2.SourceID AND cdrCost1.ID > cdrCost2.ID
END