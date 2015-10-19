-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_RoutingProduct_GetBySaleZonePackage]
	@SaleZonePackageId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT [ID]
      ,[Name]
      ,[SaleZonePackageID]
      from TOneWhS_BE.RoutingProduct
      where (@SaleZonePackageId is NULL or [SaleZonePackageID] = @SaleZonePackageId )
END