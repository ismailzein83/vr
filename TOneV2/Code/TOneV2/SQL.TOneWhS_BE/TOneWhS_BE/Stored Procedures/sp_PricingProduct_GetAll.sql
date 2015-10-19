-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_PricingProduct_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT  pp.[ID]
      ,pp.[Name]
      ,pp.DefaultRoutingProductID
      ,pp.[SaleZonePackageID]
      ,pp.[Settings]
      ,szp.Name as SaleZonePackageName
      , rp.Name as DefaultRoutingProductName 
      from TOneWhS_BE.PricingProduct pp
      JOIN TOneWhS_BE.SaleZonePackage szp ON pp.SaleZonePackageID=szp.ID
      LEFT JOIN TOneWhS_BE.RoutingProduct rp ON (pp.DefaultRoutingProductID=rp.ID)
END