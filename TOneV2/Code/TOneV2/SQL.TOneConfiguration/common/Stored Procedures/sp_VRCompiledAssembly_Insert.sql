CREATE PROCEDURE [common].[sp_VRCompiledAssembly_Insert]
	@ID uniqueidentifier,
	@Name varchar(900),
	@DevProjectID uniqueidentifier,
	@AssemblyContent varbinary(max),
	@CompiledTime datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO [common].[VRCompiledAssembly]
           (
			   [ID]
			   ,[Name]
			   ,[DevProjectID]
			   ,[AssemblyContent]
			   ,[CompiledTime]
			   ,[CreatedTime]
		   )
     VALUES
           (
			   @ID,
			   @Name,
			   @DevProjectID,
			   @AssemblyContent,
			   @CompiledTime,
			   GETDATE()
		   )

END