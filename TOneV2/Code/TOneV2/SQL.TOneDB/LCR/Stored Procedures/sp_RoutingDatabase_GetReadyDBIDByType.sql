-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,
-- Description:	<Description,,
-- =============================================
CREATE PROCEDURE [LCR].[sp_RoutingDatabase_GetReadyDBIDByType] 
   @Type int,
   @EffectiveBefore datetime
AS
BEGIN
	SELECT TOP 1 ID FROM LCR.RoutingDatabase
	WHERE Type = @Type AND EffectiveTime <= @EffectiveBefore AND ISNULL(IsReady, 0) = 1 AND ISNULL(IsDeleted, 0) = 0
	ORDER BY EffectiveTime DESC
END