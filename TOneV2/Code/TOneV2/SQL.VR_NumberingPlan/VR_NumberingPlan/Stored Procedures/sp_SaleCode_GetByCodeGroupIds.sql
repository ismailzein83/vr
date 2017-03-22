-- =============================================
-- Author:		Ali Ballouk
-- Create date: 06-06-2016
-- Description:	SP to get all SaleCodes by Code Group IDs

-- =============================================
CREATE PROCEDURE [VR_NumberingPlan].[sp_SaleCode_GetByCodeGroupIds]
	@CodeGroupIds varchar(max)
AS
BEGIN
SET NOCOUNT ON;
DECLARE @CodeGroupIDsTable TABLE (CodeGroupID INT)
	INSERT INTO @CodeGroupIDsTable (CodeGroupID)
	SELECT CONVERT(INT, ParsedString) FROM [VR_NumberingPlan].[ParseStringList](@CodeGroupIds)
	
SELECT  [ID],[Code],[ZoneID],[BED],[EED],[CodeGroupID],[SourceID]
FROM	[VR_NumberingPlan].[SaleCode] sc WITH(NOLOCK) 
WHERE	[CodeGroupID] in (SELECT CodeGroupID FROM @CodeGroupIDsTable)
End