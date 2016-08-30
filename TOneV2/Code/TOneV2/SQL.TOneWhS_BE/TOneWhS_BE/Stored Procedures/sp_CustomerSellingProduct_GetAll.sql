CREATE PROCEDURE [TOneWhS_BE].[sp_CustomerSellingProduct_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
SET NOCOUNT ON;
SELECT cpp.[ID],cpp.CustomerID,cpp.SellingProductID,cpp.AllDestinations,cpp.BED,cpp.EED
FROM TOneWhS_BE.CustomerSellingProduct cpp WITH(NOLOCK)                     

SET NOCOUNT OFF
END