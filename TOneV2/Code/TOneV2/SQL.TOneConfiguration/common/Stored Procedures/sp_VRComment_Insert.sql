CREATE PROCEDURE [common].[sp_VRComment_Insert]
	@DefinitionId uniqueidentifier,
	@ObjectId nvarchar(255),
	@Content nvarchar(max),
	@CreatedBy bigint,
	@LastModifiedBy bigint,	
	@ID bigint out
AS

	BEGIN
		INSERT INTO [common].[VRComment] (DefinitionId,ObjectId,Content,CreatedBy,LastModifiedBy)
		VALUES(@DefinitionId,@ObjectId,@Content,@CreatedBy,@LastModifiedBy)
		SET @ID = SCOPE_IDENTITY();
	END