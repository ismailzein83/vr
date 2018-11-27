CREATE PROCEDURE [common].[sp_VRDynamicAPIModule_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT	[ID],[Name], [CreatedTime], [CreatedBy], [LastModifiedBy], [LastModifiedTime]
    from	[common].[VRDynamicAPIModule] WITH(NOLOCK) 
	ORDER BY [Name]
END