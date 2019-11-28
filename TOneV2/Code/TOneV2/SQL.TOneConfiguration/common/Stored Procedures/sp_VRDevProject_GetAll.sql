
CREATE PROCEDURE [common].[sp_VRDevProject_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT	proj.ID, proj.[Name], proj.AssemblyID, proj.ProjectDependencies,proj.CreatedTime, proj.LastModifiedTime,
			projAssembly.[Name] AssemblyName, projAssembly.CompiledTime AssemblyCompiledTime
	FROM	[common].[VRDevProject] proj WITH(NOLOCK) 
	LEFT JOIN [common].[VRCompiledAssembly] projAssembly WITH(NOLOCK) ON proj.AssemblyID = projAssembly.ID
	ORDER BY proj.[Name]

END