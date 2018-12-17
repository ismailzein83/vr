CREATE PROCEDURE  [bp].[sp_BPDefinitionArgumentState_GetAll]
AS
BEGIN
	SELECT	BPDefinitionID,InputArgument
	FROM	[bp].[BPDefinitionArgumentState] WITH(NOLOCK)
END