CREATE proc [dbo].[SP_GetImportedPriceListData]
(
@LastImportedPriceListID int,
@ZebraSuppliers nvarchar(max),
@From int=0,
@To int=100
)
as
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[cap]') AND type in (N'U'))
DROP TABLE [dbo].[cap]

IF EXISTS (SELECT 'x'
          FROM tempdb..sysobjects
          WHERE type = 'U' and NAME = '#tempdata')
          begin
  Drop table #tempdata
end

SELECT ca.CarrierAccountID,cp.name INTO cap
FROM CarrierAccount ca JOIN CarrierProfile cp ON ca.ProfileID=cp.ProfileID


SELECT 
pl.pricelistid,
pl.SupplierID AS supplierID,
pl.CustomerID AS customerID,
pl.CurrencyID ,
z.ZoneID AS zoneID ,
r.RateID,
pl.BeginEffectiveDate,
cps.name AS supplierName ,
cpc.name AS customerName,
z.codegroup ,
z.ServicesFlag,
z.name AS zoneName ,
z.BeginEffectiveDate as ZBED,
z.EndEffectiveDate as ZEED,
r.Rate,
--r.OffPeakRate,
--r.WeekendRate,
r.Change,
--r.ServicesFlag,
r.BeginEffectiveDate as RBED,
r.EndEffectiveDate as REED,
STATUS =CASE 
WHEN r.change>0  THEN 'Increase'
WHEN r.change=-1 THEN 'Decrease'
WHEN r.Change=2 THEN 'New'
ELSE 'No Change'
END  into #tempdata
FROM Pricelist pl WITH(NOLOCK) JOIN cap cps ON cps.carrieraccountid=pl.SupplierID
LEFT  JOIN cap cpc ON cpc.carrieraccountid=pl.CustomerID
LEFT  JOIN rate r WITH(NOLOCK,INDEX=IX_Rate_Pricelist,INDEX=IX_Rate_Zone) ON r.PriceListID=pl.PriceListID
LEFT  JOIN Zone z WITH(NOLOCK) ON z.ZoneID=r.ZoneID
where pl.pricelistid >@LastImportedPriceListID --AND r.Change !=0   
and Pl.SupplierID IN  (select * from dbo.ParseArray(@ZebraSuppliers,',')) 


    select Count(*) C from #tempdata;
    with dataimported as (SELECT *,ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS RN FROM #tempdata)
    select * from dataimported where RN between @from and @to