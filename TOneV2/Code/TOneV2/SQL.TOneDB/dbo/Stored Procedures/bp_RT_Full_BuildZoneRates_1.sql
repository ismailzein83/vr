CREATE PROCEDURE [dbo].[bp_RT_Full_BuildZoneRates]
	@ZoneRatesIncludeBlockedZones CHAR(1) = 'Y'
	,@CheckTOD CHAR(1) = 'Y'
	
	
WITH RECOMPILE
AS
BEGIN
	
	SET NOCOUNT ON

	DECLARE @when datetime
	SET @when = getdate()
	DECLARE @Account_Inactive tinyint
	DECLARE @Account_Blocked tinyint
	DECLARE @Account_BlockedInbound tinyint
	DECLARE @Account_BlockedOutbound tinyint
	
	-- Set Enumerations
	SELECT @Account_Inactive = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.ActivationStatus' AND Name = 'Inactive'
	SELECT @Account_Blocked = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'Blocked'
	SELECT @Account_BlockedInbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedInbound'	
	SELECT @Account_BlockedOutbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedOutbound'	
	
	TRUNCATE TABLE ZoneRates
	DROP INDEX [IX_ZoneRates_ServicesFlag] ON [dbo].[ZoneRates] WITH ( ONLINE = OFF );
	DROP INDEX [IX_ZoneRates_SupplierIsBlock] ON [dbo].[ZoneRates] WITH ( ONLINE = OFF );
	

	
	;With BlocksCTE As(
		SELECT * FROM [fn_RT_Full_GetBlockRules] ( 3 ,-2 ,'')
	)

	,PriceListCTE As( Select * from PriceList WITH(NOLOCK))

	,CurrencyCTE As( Select * from Currency WITH(NOLOCK))
, CusCarriers AS 
 
 (
 SELECT S.*,Pr.Tax2 FROM CarrierAccount S WITH(NOLOCK) Left Join CarrierProfile Pr 
 On S.ProfileID=Pr.ProfileID
 WHERE 	S.ActivationStatus <> @Account_Inactive
		And S.RoutingStatus <> @Account_BlockedInbound 
	    AND S.RoutingStatus <> @Account_Blocked 
	    AND S.isdeleted='N'
 ) 
, SupCarriers AS 
 
 (
 SELECT S.*,Pr.Tax2 FROM CarrierAccount S WITH(NOLOCK) Left Join CarrierProfile Pr 
 On S.ProfileID=Pr.ProfileID
 WHERE 	S.ActivationStatus <> @Account_Inactive 
	    AND S.RoutingStatus <> @Account_Blocked 
		AND S.RoutingStatus <> @Account_BlockedOutbound
		 AND S.isdeleted='N'
 ) 
 
    -- Find the Effective TODs		
	,theTODs(ZoneID,SupplierID,CustomerID,RateType) AS 
	(
	  SELECT t.ZoneID AS ZoneID,  
			 t.SupplierID AS SupplierID,
			 t.CustomerID AS CustomerID,
			 t.RateType AS RateType
			
	  FROM ToDConsideration t WITH(NOLOCK)
	  WHERE
	      t.IsEffective ='Y'
	  AND t.IsActive = 'Y'
	)
,CommissionCTE As( 	SELECT * FROM Commission Co WITH(NOLOCK)
		WHERE Co.BeginEffectiveDate <= @when AND (Co.EndEffectiveDate IS NULL OR Co.EndEffectiveDate > @when)
)

		,RateCTE As( 

		SELECT			
			Ra.ZoneID,
			Ra.PriceListID,
			Ra.ServicesFlag,
			Rate,
			(Ra.OffPeakRate/ C.LastRate) +
			Case When (SC.Tax2 is null or isnull(Ra.OffPeakRate,0) =0) Then 0 Else CASE WHEN P.SupplierID <> 'SYS' THEN 1 ELSE -1 END * (Ra.OffPeakRate/ C.LastRate) * SC.Tax2 / 100.0 End + 
			Case When (Co.SupplierID is null or isnull(Ra.OffPeakRate,0) =0)Then 0 Else isnull(CASE WHEN P.SupplierID <> 'SYS' THEN 1 ELSE -1 END * (CASE WHEN isnull(Co.Amount,0)  <> 0 THEN Amount / C.LastRate ELSE (Ra.OffPeakRate/ C.LastRate) * Percentage / 100.0  END),0) End  
			OffPeakRate,

			(Ra.WeekendRate/ C.LastRate) +
			Case When (SC.Tax2 is null or isnull(Ra.WeekendRate,0) =0) Then 0 Else CASE WHEN P.SupplierID <> 'SYS' THEN 1 ELSE -1 END * (Ra.WeekendRate/ C.LastRate) * SC.Tax2 / 100.0 End + 
			Case When (Co.SupplierID is null or isnull(Ra.WeekendRate,0) =0) Then 0 Else isnull(CASE WHEN P.SupplierID <> 'SYS' THEN 1 ELSE -1 END * (CASE WHEN isnull(Co.Amount,0)  <> 0 THEN Amount / C.LastRate ELSE (Ra.WeekendRate/ C.LastRate) * Percentage / 100.0  END),0) End  
			WeekendRate,
			Ra.BeginEffectiveDate,
			Ra.EndEffectiveDate,
				CASE 
				WHEN @CheckTOD = 'Y' AND t.RateType = 1 THEN 1 -- OffPeak
				WHEN @CheckTOD = 'Y' AND t.RateType = 2 THEN 1 -- Weekend 
				WHEN @CheckTOD = 'Y' AND t.RateType = 4 THEN 1-- Holiday
				ELSE 0 END IsTOD,
			CASE 
				WHEN @CheckTOD = 'Y' AND t.RateType = 1 THEN ISNULL(OffPeakRate, Ra.Rate) -- OffPeak
				WHEN @CheckTOD = 'Y' AND t.RateType = 2 THEN ISNULL(WeekendRate,Ra.Rate) -- Weekend 
				WHEN @CheckTOD = 'Y' AND t.RateType = 4 THEN ISNULL(WeekendRate,Ra.Rate) -- Holiday
				ELSE 
					(Ra.Rate/ C.LastRate) +
			Case When SC.Tax2 is null Then 0 Else CASE WHEN P.SupplierID <> 'SYS' THEN 1 ELSE -1 END * (Ra.Rate/ C.LastRate) * SC.Tax2 / 100.0 End + 
			Case When Co.SupplierID is null Then 0 Else isnull(CASE WHEN P.SupplierID <> 'SYS' THEN 1 ELSE -1 END * (CASE WHEN isnull(Co.Amount,0)  <> 0 THEN Amount / C.LastRate ELSE (Ra.Rate/ C.LastRate) * Percentage / 100.0  END),0) End  
			END ActiveRate
		FROM Rate Ra WITH(NOLOCK)
			INNER JOIN PriceListCTE  P WITH(NOLOCK) ON Ra.PriceListID = P.PriceListID
			INNER JOIN SupCarriers SC WITH(NOLOCK) ON P.SupplierID = SC.CarrierAccountID
			Left JOIN CurrencyCTE C WITH(NOLOCK) ON P.CurrencyID = C.CurrencyID
			left JOIN CommissionCTE Co WITH(NOLOCK) ON P.SupplierID = Co.SupplierID	AND P.CustomerID = Co.CustomerID AND Ra.ZoneID = Co.ZoneID and Rate between co.FromRate and co.ToRate
			LEFT JOIN theTODs t WITH(NOLOCK) ON Ra.ZoneID = t.ZoneID AND P.CustomerID = t.CustomerID AND P.SupplierID = t.SupplierID



				WHERE dateadd(MI,isnull(SC.GMTTime,0),Ra.BeginEffectiveDate) <= @when AND (Ra.EndEffectiveDate IS NULL OR dateadd(MI,isnull(SC.GMTTime,0),Ra.EndEffectiveDate) > @when)			
		)
 
,ResultCTE As( 
		SELECT			
			Ra.ZoneID,
			P.SupplierID,
			P.CustomerID,			
			Ra.ServicesFlag,
			Case When P.SupplierID ='SYS' Then CS.ProfileID Else SC.ProfileID End ProfileID,
			ActiveRate,
			Ra.IsTOD,
			CASE WHEN @ZoneRatesIncludeBlockedZones = 'Y' AND rb.CustomerID = 'SYS' and P.CustomerID = 'Sys' AND Ra.ZoneID = Rb.ZoneID AND P.SupplierID = Rb.SupplierID THEN 1
				 WHEN @ZoneRatesIncludeBlockedZones = 'Y' AND Rb.CustomerID IS NOT null and P.CustomerID = Rb.CustomerID AND ra.ZoneID = rb.zoneid AND p.supplierid = 'sys' THEN 1
				 ELSE 0
			END 
			 IsBlocked,
			z.CodeGroup CodeGroup
			
		FROM RateCTE Ra WITH(NOLOCK)
				INNER JOIN PriceListCTE  P WITH(NOLOCK) ON Ra.PriceListID = P.PriceListID
				INNER JOIN CusCarriers CS WITH(NOLOCK) ON P.CustomerID = CS.CarrierAccountID
				INNER JOIN SupCarriers SC WITH(NOLOCK) ON P.SupplierID = SC.CarrierAccountID
				left JOIN BlocksCTE RB WITH(NOLOCK) on  Ra.ZoneID = RB.ZoneID AND P.SupplierID = RB.SupplierID AND P.CustomerID = RB.CustomerID
				LEFT JOIN Zone z ON z.ZoneID = Ra.ZoneID --AND z.SupplierID = P.SupplierID
			WHERE  (
					@ZoneRatesIncludeBlockedZones = 'Y'
					OR
					RB.ZoneID is  null 
			)
			AND z.IsEffective = 'Y'
)					
insert into ZoneRates with (tablock)
SELECT *
 from ResultCTE ra

CREATE NONCLUSTERED INDEX [IX_ZoneRates_ServicesFlag] ON [dbo].[ZoneRates]([ServicesFlag] ASC)
CREATE NONCLUSTERED INDEX [IX_ZoneRates_SupplierIsBlock] ON [dbo].[ZoneRates](	[SupplierID] ASC,	[IsBlock] ASC, [ZoneID] ASC)
			INCLUDE ( [CustomerID],[ServicesFlag],[ProfileId],[ActiveRate],[IsTOD]) 
WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
END