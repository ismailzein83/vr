CREATE proc [dbo].[SP_GetCurrentRates]
(
@ZebraSuppliers nvarchar(max),
@When datetime
)
as
begin
SELECT 
                 -------------------
                 --  R.OffPeakRate AS Offpeackrate,
                 --  R.WeekendRate AS WeekendRate,
                 --  R.Change,
                 --  R.ServicesFlag AS RSF,
                 --   Z.ServicesFlag AS ZSF,
                 --  R.RateID,
                 --   P.BeginEffectiveDate AS PBED,
                 --  P.SourceFileName,
                 --   P.PriceListID
                ------------------
                   P.SupplierID,
                   P.CurrencyID,
                   Z.CodeGroup,
                   Z.ZoneID,
                   Z.BeginEffectiveDate AS ZBED,
                   Z.EndEffectiveDate AS ZEED,
                   Z.Name,
                   R.Rate AS peakRate ,
                   R.BeginEffectiveDate AS RBED,
                   R.EndEffectiveDate as REED,
                   P.CustomerID--,--,
                  -- zm.supplierzoneid
                   --C.ID,
                   --C.Code,
                   --C.ZoneID,
                   --C.BeginEffectiveDate,
                   --C.EndEffectiveDate
                FROM PriceList P (NOLOCK), CarrierAccount S (NOLOCK), Rate R (NOLOCK), Zone Z --,Zonematch ZM --,Code C (NOLOCK),
                WHERE 
                    R.PriceListID = P.PriceListID 
                    AND P.CustomerID = 'SYS' 
                    and P.SupplierID IN  (select * from dbo.ParseArray(@ZebraSuppliers,','))
                    AND S.CarrierAccountID = P.SupplierID
                    --AND Zm.ourzoneid=z.zoneid 
                    --AND C.ZoneID=Z.ZoneID--added new
                    --AND S.RoutingStatus IN  (2)
                    AND S.ActivationStatus <> 0
                                      -- AND (R.BeginEffectiveDate > @When)AND (R.EndEffectiveDate is NULL OR r.EndEffectiveDate>@When)
                    AND (R.EndEffectiveDate is null or (R.EndEffectiveDate > @When And R.BeginEffectiveDate<>R.EndEffectiveDate))
                    --AND (Z.EndEffectiveDate is null or (Z.EndEffectiveDate > @When ))
                    And R.Rate>0
                    AND R.ZoneID = Z.ZoneID
                ORDER BY R.BeginEffectiveDate,r.ZoneID
                end