
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleCode_GetCodeIds]
	@CodeIds varchar(max)
AS
BEGIN
SET NOCOUNT ON;
DECLARE @CodeIDsTable TABLE (CodeID bigint)
	INSERT INTO @CodeIDsTable (CodeID)
	SELECT CONVERT(bigint, ParsedString) FROM [TOneWhS_BE].[ParseStringList](@CodeIds)
	
SELECT  [ID],[Code],[ZoneID],[BED],[EED],[CodeGroupID],[SourceID]
FROM	[TOneWhS_BE].[SaleCode] sc WITH(NOLOCK) 
join @CodeIDsTable c on c.CodeID = sc.ID
End