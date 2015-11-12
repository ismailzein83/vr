-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [StatisticManagement].[sp_StatisticBatch_UnLock]
	@TypeID int,
	@BatchStart datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
		
	UPDATE StatisticManagement.StatisticBatch
    SET		LockedByProcessID = Null
	WHERE	TypeID = @TypeID 
			AND BatchStart = @BatchStart
END