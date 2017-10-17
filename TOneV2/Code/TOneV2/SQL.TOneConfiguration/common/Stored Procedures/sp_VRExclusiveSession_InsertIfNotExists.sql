
CREATE PROCEDURE [common].[sp_VRExclusiveSession_InsertIfNotExists]
	@SessionTypeId uniqueidentifier,
	@TargetId nvarchar(400)
AS
BEGIN
	IF NOT EXISTS (SELECT TOP 1 NULL FROM [common].[VRExclusiveSession] with (nolock) WHERE SessionTypeId = @SessionTypeId AND TargetId = @TargetId)
	BEGIN
		BEGIN TRY
			INSERT INTO [common].[VRExclusiveSession]
			(SessionTypeId, TargetId)
			SELECT @SessionTypeId, @TargetId
			WHERE NOT EXISTS (SELECT TOP 1 NULL FROM [common].[VRExclusiveSession] WHERE SessionTypeId = @SessionTypeId AND TargetId = @TargetId)
		END TRY
		BEGIN CATCH
			 
		END CATCH		
	END
END