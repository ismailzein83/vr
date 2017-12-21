-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_VRExclusiveSession_TryKeep]
	@SessionTypeId uniqueidentifier,
	@TargetId nvarchar(400),
	@UserId int,
	@TimeoutInSeconds int
AS
BEGIN
	UPDATE [common].[VRExclusiveSession]
    SET TakenByUserId = @UserId,
		LastTakenUpdateTime = GETDATE()
	WHERE SessionTypeId = @SessionTypeId AND TargetId = @TargetId And TakenByUserId = @UserId
	
	SELECT TakenByUserId FROM [common].[VRExclusiveSession] with (nolock) WHERE SessionTypeId = @SessionTypeId AND TargetId = @TargetId
END