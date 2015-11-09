
Create PROCEDURE TOneWhS_Routing.[sp_RoutingDatabase_GetReadyDBIDByType] 
   @Type int,
   @EffectiveBefore datetime =  NULL
AS
BEGIN
	SELECT TOP 1 ID FROM TOneWhS_Routing.RoutingDatabase
	WHERE Type = @Type AND (EffectiveTime <=@EffectiveBefore or @EffectiveBefore is null) AND ISNULL(IsReady, 0) = 1 AND ISNULL(IsDeleted, 0) = 0
	ORDER BY EffectiveTime DESC, ID desc
END

SET ANSI_NULLS ON