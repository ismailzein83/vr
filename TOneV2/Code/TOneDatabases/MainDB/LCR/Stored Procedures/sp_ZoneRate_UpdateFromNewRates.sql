CREATE PROCEDURE [LCR].[sp_ZoneRate_UpdateFromNewRates]
	@ratesUpdatedAfter timestamp
AS
BEGIN
	
    INSERT INTO [ZoneRate]
           ([RateID]
           ,[ZoneID]
           ,[SupplierID]
           ,[CustomerID]
           ,[NormalRate]
           ,[ServicesFlag]
           ,[BeginEffectiveDate]
           ,[EndEffectiveDate]
           ,[PricelistID]
           ,[ZoneName])
	SELECT r.[RateID]
      ,r.[ZoneID]
      ,p.SupplierID
      ,p.CustomerID
      ,r.[Rate]
      ,r.[ServicesFlag]
      ,r.[BeginEffectiveDate]
      ,r.[EndEffectiveDate]
      ,r.[PricelistID]
      ,z.[Name]
  FROM [Rate] r WITH (NOLOCK)
  JOIN PriceList p WITH (NOLOCK) ON r.PriceListID = p.PriceListID
  JOIN Zone Z WITH (NOLOCK) ON r.ZoneID = Z.ZoneID
  WHERE
	r.timestamp > @ratesUpdatedAfter
   AND NOT EXISTS (SELECT 1 FROM ZoneRate zr WITH (NOLOCK) WHERE zr.RateID = r.RateID)
END