CREATE PROCEDURE [BEntity].[sp_Rate_GetRates]
	@zoneId int,
	@customerId varchar(10),
	@when datetime
AS
BEGIN
Select 
r.RateID,
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
from Rate r   With(nolock) inner join zone z With(nolock)  on r.zoneid=z.zoneid
                                    inner join pricelist P With(nolock) on P.PriceListId=r.PriceListId
                                    inner join Currency C With(nolock) on P.Currencyid=C.Currencyid
                                    Where P.PriceListId=r.PriceListId  
                                    and (r.EndEffectiveDate is null or (r.EndEffectiveDate > @when and r.BeginEffectiveDate<r.EndEffectiveDate))
                                    and (z.EndEffectiveDate is null or (z.EndEffectiveDate > @when and z.BeginEffectiveDate<z.EndEffectiveDate))
                                    and (P.CustomerID = @customerId)
                                    and (z.ZoneID=@zoneId OR @zoneId IS NULL)
                                    order by z.Name
--SELECT  t.TariffID as TariffID,
--		t.[ZoneID] as ZoneID,
--		z.name ,
--		z.supplierid,
--		z.codegroup,
--		z.BeginEffectiveDate,
--		z.EndEffectiveDate,
--		t.[CustomerID] as SupplierID,
--		t.[SupplierID] as CustomerID,
--		t.[CallFee] as CallFee,
--		t.[FirstPeriodRate] as FirstPeriodRate,
--		t.[FirstPeriod] as FirstPeriod,
--		t.[RepeatFirstPeriod] as RepeatFirstPeriod,
--		t.[FractionUnit] as FractionUnit,
--		t.[BeginEffectiveDate],
--		t.[EndEffectiveDate],
--		t.[IsEffective],
--		t.[UserID]
--FROM  [Tariff] t  with (nolock) inner join Zone z  with (nolock) on t.ZoneID=z.ZoneID 
--Where  (t.EndEffectiveDate is null or (t.EndEffectiveDate > @when and t.BeginEffectiveDate<t.EndEffectiveDate))
--		and (z.EndEffectiveDate is null or (z.EndEffectiveDate > @when and z.BeginEffectiveDate<z.EndEffectiveDate))
--		and (t.CustomerID = @customerId)
--		and (z.ZoneID=@zoneId)
--order by z.Name

END