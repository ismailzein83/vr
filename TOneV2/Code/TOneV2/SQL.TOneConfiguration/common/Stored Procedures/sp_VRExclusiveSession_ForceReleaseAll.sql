-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_VRExclusiveSession_ForceReleaseAll]
AS
BEGIN

	UPDATE [common].[VRExclusiveSession]
    SET TakenByUserId = null, LastTakenUpdateTime = null

END