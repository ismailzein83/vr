
CREATE PROCEDURE [TOneWhS_Deal].[sp_DaysToReprocess_GetAll]

AS
BEGIN
	SET NOCOUNT ON;
	SELECT [ID], [Date], [IsSale], [CarrierAccountId]  
	FROM [TOneWhS_Deal].[DaysToReprocess] WITH(NOLOCK)	 
	SET NOCOUNT OFF
END