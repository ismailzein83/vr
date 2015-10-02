Create PROCEDURE [BEntity].[sp_SaleMarketPrice_GetAll]
AS
BEGIN
SELECT mp.[SaleZoneMarketPriceID]
      ,mp.[SaleZoneID]
      ,mp.[ServicesFlag]
      ,mp.[FromRate]
      ,mp.[ToRate]
FROM  [SaleZoneMarketPrice] mp
END