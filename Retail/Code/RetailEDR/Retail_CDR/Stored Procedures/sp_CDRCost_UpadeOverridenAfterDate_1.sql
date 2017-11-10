CREATE PROCEDURE [Retail_CDR].[sp_CDRCost_UpadeOverridenAfterDate]
@FromTime datetime
AS
BEGIN
WITH CDRCostTemp AS 
(
SELECT ID, SourceID
FROM [Retail_CDR].[CDRCost]
where (@FromTime is null or AttemptDateTime >= @FromTime)
)
UPDATE cdrCost1
SET IsDeleted = 1
FROM [Retail_CDR].[CDRCost] cdrCost1
JOIN CDRCostTemp cdrCost2 ON cdrCost1.SourceID = cdrCost2.SourceID AND cdrCost2.ID > cdrCost1.ID
where (@FromTime is null or cdrCost1.AttemptDateTime >= @FromTime)
END