CREATE PROCEDURE [common].[sp_Currency_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT	[ID],[Name],[Symbol],[SourceID], [CreatedTime], [CreatedBy], [LastModifiedBy], [LastModifiedTime]
    from	[common].Currency WITH(NOLOCK) 
	ORDER BY [Name]
END