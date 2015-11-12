-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [StatisticManagement].[sp_StatisticBatch_UpdateBatchInfo]
	@TypeID int,
	@BatchStart datetime,
	@BatchInfo nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
		
	UPDATE StatisticManagement.StatisticBatch
    SET		BatchInfo = @BatchInfo
	WHERE	TypeID = @TypeID 
			AND BatchStart = @BatchStart
END