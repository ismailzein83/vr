Create PROCEDURE [TOneWhS_Deal].[sp_Deal_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT	d.ID,d.Name,d.Settings
	FROM	[TOneWhS_Deal].Deal d WITH(NOLOCK) 
	SET NOCOUNT OFF
END