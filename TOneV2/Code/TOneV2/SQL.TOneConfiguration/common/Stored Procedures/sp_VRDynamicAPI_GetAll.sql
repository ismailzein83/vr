Create PROCEDURE [common].[sp_VRDynamicAPI_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT	[ID],[Name],[ModuleId],[Settings], [CreatedTime], [CreatedBy], [LastModifiedBy], [LastModifiedTime]
    from	[common].[VRDynamicAPI] WITH(NOLOCK) 
	ORDER BY [Name]
END