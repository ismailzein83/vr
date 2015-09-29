-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_Rate_GetCostBySupplier]
	@Supplier varchar(10),
	@when datetime
AS
BEGIN
	Select	r.RateID,
			p.SupplierID,
			p.CustomerID,
			r.Zoneid,
			r.rate,
			r.OffPeakRate,
			r.WeekendRate,
			r.pricelistid,
			r.ServicesFlag,
			r.BeginEffectiveDate as RateBeginEffectiveDate,
			r.EndEffectiveDate as RateEndEffectiveDate,
			p.CurrencyID,
			C.lastrate as CurrencyLastRate
	from	Rate r   With(nolock) 
			inner join zone z With(nolock)  on r.zoneid=z.zoneid
            inner join pricelist P With(nolock) on P.PriceListId=r.PriceListId
            inner join Currency C With(nolock) on P.Currencyid=C.Currencyid
    Where	P.PriceListId=r.PriceListId  
          and ((r.BeginEffectiveDate < @when ) and (r.EndEffectiveDate is null or r.EndEffectiveDate < @when))
          and ((z.BeginEffectiveDate < @when ) and (z.EndEffectiveDate is null or z.EndEffectiveDate < @when))
          and (P.SupplierID = @Supplier) order by z.Name
 END