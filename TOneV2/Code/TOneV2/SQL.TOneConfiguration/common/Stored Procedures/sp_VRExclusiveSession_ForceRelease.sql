-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_VRExclusiveSession_ForceRelease]
	@VRExclusiveSessionId int
AS
BEGIN

	UPDATE [common].[VRExclusiveSession]
    SET TakenByUserId = null, 
		LastTakenUpdateTime = null
	WHERE ID = @VRExclusiveSessionId

END