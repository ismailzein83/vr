CREATE proc [dbo].[SP_GetPendingRatesAfterSuchDate]
(
@ZebraSuppliers nvarchar(max),
@When datetime,
@from int=1,
@to int =50000
)
as
begin
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[cap]') AND type in (N'U'))
DROP TABLE [dbo].[cap]


SELECT ca.CarrierAccountID,cp.name INTO cap
FROM CarrierAccount ca JOIN CarrierProfile cp ON ca.ProfileID=cp.ProfileID



SELECT 
				--P.PriceListID,
    --               P.SupplierID,
    --               P.CurrencyID,
    --               Z.CodeGroup,
    --               Z.ZoneID,
    --               Z.Name,
    --               R.Rate AS peakRate ,
    --               R.BeginEffectiveDate AS RBED,
    --               R.EndEffectiveDate as REED,
    --               C.Code
    p.pricelistid,
p.SupplierID AS supplierID,
p.CustomerID AS customerID,
p.CurrencyID ,
z.ZoneID AS zoneID ,
r.RateID,
p.BeginEffectiveDate,
cps.name AS supplierName ,
cpc.name AS customerName,
z.codegroup ,
z.ServicesFlag,
z.name AS zoneName ,
z.BeginEffectiveDate as ZBED,
z.EndEffectiveDate as ZEED,
r.Rate,
r.Change,
r.BeginEffectiveDate as RBED,
r.EndEffectiveDate as REED,
STATUS =CASE 
WHEN r.change>0  THEN 'Increase'
WHEN r.change=-1 THEN 'Decrease'
WHEN r.Change=2 THEN 'New'
ELSE 'No Change' end into #TempData
                FROM PriceList P (NOLOCK) JOIN cap cps ON cps.carrieraccountid=p.SupplierID
						LEFT  JOIN cap cpc ON cpc.carrieraccountid=p.CustomerID
						, CarrierAccount S (NOLOCK), Rate R (NOLOCK), Zone Z (NOLOCK)
                WHERE 
                    R.PriceListID = P.PriceListID 
                    AND P.CustomerID = 'SYS' 
                    and P.SupplierID IN  (select * from dbo.ParseArray(@ZebraSuppliers,','))
                    AND S.CarrierAccountID = P.SupplierID
                    AND S.ActivationStatus <> 0
                  AND (R.BeginEffectiveDate > @When)AND (R.EndEffectiveDate is NULL OR r.EndEffectiveDate>@When)
                    And R.Rate>0
                    AND R.ZoneID = Z.ZoneID
                ORDER BY R.BeginEffectiveDate

                
select Count(*) C from #TempData
;with data as 
(
	SELECT *,ROW_NUMBER() OVER (ORDER BY (select 1)) AS RN FROM #TempData
)
	select * from data where RN between @from and @to

end