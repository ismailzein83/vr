-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE common.sp_VRExclusiveSession_Release
	@SessionTypeId uniqueidentifier,
	@TargetId nvarchar(400),
	@UserId int
AS
BEGIN

	UPDATE [common].[VRExclusiveSession]
    SET TakenByUserId = null,
		LastTakenUpdateTime = null
	WHERE SessionTypeId = @SessionTypeId AND TargetId = @TargetId
			AND TakenByUserId = @UserId

END