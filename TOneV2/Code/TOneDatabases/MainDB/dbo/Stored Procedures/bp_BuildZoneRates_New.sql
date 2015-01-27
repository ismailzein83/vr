
CREATE  PROCEDURE [dbo].[bp_BuildZoneRates_New]
	@CustomerID varchar(10) = NULL,
	@SupplierID varchar(10) = NULL,
	@IncludeBlockedZones CHAR(1) = 'N'
WITH RECOMPILE
AS
BEGIN
	
	SET NOCOUNT ON

	DECLARE @when datetime
	SET @when = dbo.DateOf(getdate())
	
	DECLARE @Account_Inactive tinyint
	DECLARE @Account_Blocked tinyint
	DECLARE @Account_BlockedInbound tinyint
	DECLARE @Account_BlockedOutbound tinyint
	
	-- Set Enumerations
	SELECT @Account_Inactive = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.ActivationStatus' AND Name = 'Inactive'
	SELECT @Account_Blocked = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'Blocked'
	SELECT @Account_BlockedInbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedInbound'	
	SELECT @Account_BlockedOutbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedOutbound'	
	


	TRUNCATE TABLE ZoneRate
	DROP INDEX [IX_ZoneRate_Customer] ON [dbo].[ZoneRate] WITH ( ONLINE = OFF )
	DROP INDEX [IX_ZoneRate_ServicesFlag] ON [dbo].[ZoneRate] WITH ( ONLINE = OFF )
	DROP INDEX IX_ZoneRate_Supplier ON [dbo].[ZoneRate] WITH ( ONLINE = OFF )
	DROP INDEX IX_ZoneRate_Zone ON [dbo].[ZoneRate] WITH ( ONLINE = OFF );

	With BlocksCTE As(
   SELECT RB.CustomerID, RB.ZoneID, RB.SupplierID  
   FROM RouteBlock RB WITH(NOLOCK)
   WHERE RB.IsEffective = 'Y' AND RB.ZoneID IS NOT NULL
	)
	
	,PriceListCTE As( Select * from PriceList)

	,CurrencyCTE As( Select * from Currency)

	,RateCTE As( 

		SELECT			
			Ra.ZoneID,
			Ra.PriceListID,
			Ra.ServicesFlag,
			Rate,
			OffPeakRate,
			WeekendRate,
			Ra.BeginEffectiveDate,
			Ra.EndEffectiveDate
		FROM Rate Ra WITH(NOLOCK)
			WHERE Ra.BeginEffectiveDate <= @when AND (Ra.EndEffectiveDate IS NULL OR Ra.EndEffectiveDate > @when)
)
	,CommissionCTE As( 	SELECT * FROM Commission Co WITH(NOLOCK)
		WHERE Co.BeginEffectiveDate <= @when AND (Co.EndEffectiveDate IS NULL OR Co.EndEffectiveDate > @when)

)
, Carriers AS 
 
 (
 SELECT S.*,Pr.Tax2 FROM CarrierAccount S WITH(NOLOCK) Left Join CarrierProfile Pr 
 On S.ProfileID=Pr.ProfileID
 WHERE 	S.ActivationStatus <> @Account_Inactive 
	    AND S.RoutingStatus <> @Account_Blocked 
		AND S.RoutingStatus <> @Account_BlockedOutbound
 ) 
 
,ResultCTE As( 
		SELECT			
			Ra.ZoneID,
			P.SupplierID,
			P.CustomerID,
			(Ra.Rate/ C.LastRate) +
			Case When CC.Tax2 is null Then 0 Else CASE WHEN P.SupplierID <> 'SYS' THEN 1 ELSE -1 END * (Ra.Rate/ C.LastRate) * CC.Tax2 / 100.0 End + 
			Case When Co.SupplierID is null Then 0 Else isnull(CASE WHEN P.SupplierID <> 'SYS' THEN 1 ELSE -1 END * (CASE WHEN isnull(Co.Amount,0)  <> 0 THEN Amount / C.LastRate ELSE (Ra.Rate/ C.LastRate) * Percentage / 100.0  END),0) End  
			Rate,

			(Ra.OffPeakRate/ C.LastRate) +
			Case When (CC.Tax2 is null or isnull(Ra.OffPeakRate,0) =0) Then 0 Else CASE WHEN P.SupplierID <> 'SYS' THEN 1 ELSE -1 END * (Ra.OffPeakRate/ C.LastRate) * CC.Tax2 / 100.0 End + 
			Case When (Co.SupplierID is null or isnull(Ra.OffPeakRate,0) =0)Then 0 Else isnull(CASE WHEN P.SupplierID <> 'SYS' THEN 1 ELSE -1 END * (CASE WHEN isnull(Co.Amount,0)  <> 0 THEN Amount / C.LastRate ELSE (Ra.OffPeakRate/ C.LastRate) * Percentage / 100.0  END),0) End  
			OffPeakRate,

			(Ra.WeekendRate/ C.LastRate) +
			Case When (CC.Tax2 is null or isnull(Ra.WeekendRate,0) =0) Then 0 Else CASE WHEN P.SupplierID <> 'SYS' THEN 1 ELSE -1 END * (Ra.WeekendRate/ C.LastRate) * CC.Tax2 / 100.0 End + 
			Case When (Co.SupplierID is null or isnull(Ra.WeekendRate,0) =0) Then 0 Else isnull(CASE WHEN P.SupplierID <> 'SYS' THEN 1 ELSE -1 END * (CASE WHEN isnull(Co.Amount,0)  <> 0 THEN Amount / C.LastRate ELSE (Ra.WeekendRate/ C.LastRate) * Percentage / 100.0  END),0) End  
			WeekendRate,
			
			Ra.ServicesFlag,
			Case When P.SupplierID='SYS' Then CC.ProfileID Else CS.ProfileID End ProfileID
		FROM RateCTE Ra WITH(NOLOCK)
				INNER JOIN PriceListCTE  P WITH(NOLOCK) ON Ra.PriceListID = P.PriceListID
				left JOIN CurrencyCTE C WITH(NOLOCK) ON P.CurrencyID = C.CurrencyID
				INNER JOIN Carriers CS WITH(NOLOCK) ON P.SupplierID = CS.CarrierAccountID
				INNER JOIN Carriers CC WITH(NOLOCK) ON P.CustomerID = CC.CarrierAccountID
				left JOIN CommissionCTE Co WITH(NOLOCK) 
				ON P.SupplierID = Co.SupplierID	AND P.CustomerID = Co.CustomerID AND Ra.ZoneID = Co.ZoneID and Rate between co.FromRate and co.ToRate
				left JOIN BlocksCTE RB WITH(NOLOCK) on RB.SupplierID = p.SupplierID and Ra.ZoneID =RB.ZoneID

			WHERE  (
					@IncludeBlockedZones = 'Y'
					OR
					RB.ZoneID is  null 
					)
)					
insert into zonerate with (tablock)
Select * from ResultCTE

CREATE NONCLUSTERED INDEX [IX_ZoneRate_Customer] ON [dbo].[ZoneRate] (	[CustomerID] ASC)
CREATE NONCLUSTERED INDEX [IX_ZoneRate_ServicesFlag] ON [dbo].[ZoneRate] (	[ServicesFlag] ASC)
CREATE NONCLUSTERED INDEX [IX_ZoneRate_Supplier] ON [dbo].[ZoneRate] (	[SupplierID] ASC)
CREATE NONCLUSTERED INDEX [IX_ZoneRate_Zone] ON [dbo].[ZoneRate] (	[ZoneID] ASC)

End