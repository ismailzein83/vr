
Create PROCEDURE [TOneWhS_Deal].[sp_DaysToReprocess_GetAll]

AS
BEGIN
	SET NOCOUNT ON;
	SELECT DTR.[ID], DTR.[Date], DTR.[IsSale], DTR.[CarrierAccountId]  FROM [TOneWhS_Deal].[DaysToReprocess] DTR WITH(NOLOCK)
	SET NOCOUNT OFF
END