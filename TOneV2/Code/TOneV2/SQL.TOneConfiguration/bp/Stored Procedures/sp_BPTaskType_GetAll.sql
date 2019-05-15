
CREATE PROCEDURE  [bp].[sp_BPTaskType_GetAll]
AS
BEGIN
	SELECT	[ID],[DevProjectID],Name,Settings, LastModifiedTime
	FROM	[bp].[BPTaskType] WITH(NOLOCK)
END