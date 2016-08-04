CREATE PROCEDURE [bp].[sp_BPInstance_GetStatusByID]	
	@ID bigint
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @ID_Local bigint
	SELECT @ID_Local = @ID
	
    SELECT [ExecutionStatus]
	FROM bp.[BPInstance] WITH(NOLOCK)
	WHERE ID = @ID_Local
END