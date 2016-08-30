-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierCode_GetBySupplier]
	-- Add the parameters for the stored procedure here
	@SupplierId INT,
	@When DateTime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

SELECT  sc.[ID],sc.Code,sc.ZoneID,sc.BED,sc.EED
FROM	[TOneWhS_BE].SupplierCode sc WITH(NOLOCK) 
		LEFT JOIN [TOneWhS_BE].SupplierZone sz WITH(NOLOCK) ON sc.ZoneID=sz.ID 
Where	(sc.BED<=@When and (sc.EED is null or sc.EED > @when))
		and sz.SupplierID=@SupplierId
END