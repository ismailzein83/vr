-- =============================================
-- Author:		Ali Ballouk
-- Create date: 06-06-2016
-- Description:	SP to get all SaleCodes by Code Group IDs

-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleCode_GetByCodeGroupIds]
	@CodeGroupIds varchar(max)
AS
BEGIN
SET NOCOUNT ON;
DECLARE @CodeGroupIDsTable TABLE (CodeGroupID INT)
	INSERT INTO @CodeGroupIDsTable (CodeGroupID)
	SELECT CONVERT(INT, ParsedString) FROM [TOneWhS_BE].[ParseStringList](@CodeGroupIds)
	
SELECT  [ID],[Code],[ZoneID],[BED],[EED]
FROM	[TOneWhS_BE].[SaleCode] sc WITH(NOLOCK) 
WHERE	[CodeGroupID] in (SELECT CodeGroupID FROM @CodeGroupIDsTable)
End