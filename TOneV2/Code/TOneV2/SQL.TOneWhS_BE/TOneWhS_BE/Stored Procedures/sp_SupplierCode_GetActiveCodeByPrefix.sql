-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierCode_GetActiveCodeByPrefix]
	-- Add the parameters for the stored procedure here
	@CodePrefix varchar(20),
	@EffectiveOn DateTime,
	@GetChildCodes bit,
	@GetParentCodes bit,
	@ActiveSuppliersInfo TOneWhS_BE.RoutingSupplierInfo READONLY
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT  sc.[ID]
		  ,sc.Code
		  ,sc.ZoneID
		  ,sc.BED
		  ,sc.EED
	  FROM [TOneWhS_BE].SupplierCode sc 
	  LEFT JOIN [TOneWhS_BE].SupplierZone sz ON sc.ZoneID=sz.ID 
	  JOIN @ActiveSuppliersInfo s on s.SupplierId = sz.SupplierId
	  Where ((sc.[Code] like @CodePrefix + '%' And @GetChildCodes = 1) OR (@CodePrefix like sc.Code + '%'  And @GetParentCodes = 1))
	  and (sc.BED <= @EffectiveOn and (sc.EED is null or sc.EED > @EffectiveOn))
END