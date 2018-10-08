CREATE PROCEDURE  [bp].[sp_ProcessSynchronisation_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT	[ID],[Name],[IsEnabled],Settings,CreatedBy,CreatedTime,LastModifiedBy,LastModifiedTime
	FROM	[bp].[ProcessSynchronisation] WITH(NOLOCK)
END