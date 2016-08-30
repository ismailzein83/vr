CREATE PROCEDURE [TOneWhS_BE].[sp_DealProgress_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;	
	SELECT	dp.[ID],dp.[Date],dp.[IsSelling],dp.[EstimatedDuration],dp.[ReachedDuration],dp.[EstimatedAmount],dp.[ReachedAmount]
	FROM	[dbo].[DealProgress] dp WITH(NOLOCK) 
	SET NOCOUNT OFF
END