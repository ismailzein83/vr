	CREATE Procedure [bp].[sp_VRWorkflow_Insert]
	@ID uniqueidentifier,
	@Name nvarchar(255),
	@Title nvarchar(255),
	@DevProjectId uniqueidentifier,
	@Settings nvarchar(MAX),
	@CreatedBy int

AS
BEGIN
IF NOT EXISTS(select 1 from [bp].[VRWorkflow] where Name = @Name)
	BEGIN
		insert into [bp].[VRWorkflow] ( [ID], [Name], [Title],[DevProjectID], [Settings], [CreatedBy], [LastModifiedBy])
		values( @ID, @Name, @Title,@DevProjectId, @Settings, @CreatedBy, @CreatedBy)
	END
END