
CREATE PROCEDURE [TOneWhS_Deal].[sp_DaysToReprocess_DeleteByDate]
@Date Date
AS
BEGIN
	DELETE FROM [TOneWhS_Deal].[DaysToReprocess] WHERE [TOneWhS_Deal].[DaysToReprocess].[Date] = @Date
END