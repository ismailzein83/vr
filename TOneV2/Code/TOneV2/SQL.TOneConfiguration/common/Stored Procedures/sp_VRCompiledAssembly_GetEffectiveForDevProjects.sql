CREATE PROCEDURE [common].[sp_VRCompiledAssembly_GetEffectiveForDevProjects]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT 
		projAssembly.ID, 
		projAssembly.[Name], 
		projAssembly.[DevProjectID], 
		projAssembly.[AssemblyContent], 
		projAssembly.[CompiledTime], 
		projAssembly.[CreatedTime]
	FROM [common].VRCompiledAssembly projAssembly WITH (NOLOCK)
	JOIN [common].VRDevProject proj WITH (NOLOCK) ON projAssembly.ID = proj.AssemblyID

END