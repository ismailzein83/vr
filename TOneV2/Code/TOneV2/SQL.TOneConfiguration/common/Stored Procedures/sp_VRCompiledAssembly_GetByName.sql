CREATE PROCEDURE [common].[sp_VRCompiledAssembly_GetByName]
	@Name varchar(900)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT ID, [Name], [DevProjectID], [AssemblyContent], [CompiledTime], [CreatedTime]
	FROM [common].VRCompiledAssembly WITH (NOLOCK)
	WHERE [Name] = @Name

END