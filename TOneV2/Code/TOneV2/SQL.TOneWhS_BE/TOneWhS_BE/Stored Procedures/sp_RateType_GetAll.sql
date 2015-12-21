CREATE PROCEDURE [TOneWhS_BE].[sp_RateType_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT	rt.ID,
			rt.Name
	FROM	[TOneWhS_BE].RateType  as rt WITH(NOLOCK) 
END