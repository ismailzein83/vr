-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_VRExclusiveSession_TryTake]
	@SessionTypeId uniqueidentifier,
	@TargetId nvarchar(400),
	@UserId int,
	@TimeoutInSeconds int
AS
BEGIN
	UPDATE [common].[VRExclusiveSession]
    SET TakenByUserId = @UserId,
		LastTakenUpdateTime = GETDATE()
	WHERE SessionTypeId = @SessionTypeId AND TargetId = @TargetId
	AND (ISNULL(TakenByUserId, @UserId) = @UserId OR DATEDIFF(ss, LastTakenUpdateTime, GETDATE()) > @TimeoutInSeconds)
	
	SELECT TakenByUserId FROM [common].[VRExclusiveSession] with (nolock) WHERE SessionTypeId = @SessionTypeId AND TargetId = @TargetId
END