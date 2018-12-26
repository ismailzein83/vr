
CREATE PROCEDURE  [bp].[sp_BPTaskType_GetAll]
AS
BEGIN
	SELECT	[ID],Name,Settings, LastModifiedTime
	FROM	[bp].[BPTaskType] WITH(NOLOCK)
END