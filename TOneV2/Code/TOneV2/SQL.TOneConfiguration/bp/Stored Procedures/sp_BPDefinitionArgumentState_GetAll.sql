CREATE PROCEDURE  [bp].[sp_BPDefinitionArgumentState_GetAll]
AS
BEGIN
	SELECT	BPDefinitionID,InputArgument
	FROM	[bp].[BPDefintionArgumentState] WITH(NOLOCK)
END