CREATE PROCEDURE [Retail_CDR].[sp_CDRCost_GetMaxId]
AS
BEGIN
  SELECT MAX(ID)
  FROM [Retail_CDR].[CDRCost] with(nolock)
END