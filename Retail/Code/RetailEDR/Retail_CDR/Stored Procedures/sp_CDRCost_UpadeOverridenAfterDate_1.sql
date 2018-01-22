CREATE PROCEDURE [Retail_CDR].[sp_CDRCost_UpadeOverridenAfterDate]
@FromTime datetime
AS
BEGIN

WITH CDRCostTemp AS 
(
SELECT
ID, SourceID
FROM [Retail_CDR].[CDRCost] with(nolock,index=[IX_Retail_CDR_CDRCost_AttempteDateTime])
where(@FromTime is null or AttemptDateTime >= @FromTime)
)

UPDATE cdrCost1
SET IsDeleted = 1
FROM [Retail_CDR].[CDRCost] cdrCost1 with(nolock,index=[IX_Retail_CDR_CDRCost_AttempteDateTime],index=[IX_CDRCost_ID])
JOIN CDRCostTemp cdrCost2 ON cdrCost1.SourceID = cdrCost2.SourceID AND cdrCost2.ID > cdrCost1.ID
where (@FromTime is null or cdrCost1.AttemptDateTime >= @FromTime)

END