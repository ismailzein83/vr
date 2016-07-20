Create PROCEDURE [TOneWhS_BE].[sp_Deal_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT sc.ID,
		sc.Settings
	FROM TOneWhS_BE.Deal sc
	SET NOCOUNT OFF
END