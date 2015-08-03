CREATE  PROCEDURE [dbo].[bp_BuildZoneRates]
	@CustomerID varchar(10) = NULL,
	@SupplierID varchar(10) = NULL,
	@IncludeBlockedZones CHAR(1) = 'N'
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
	


	TRUNCATE TABLE ZoneRate
	DROP INDEX [IX_ZoneRate_Customer] ON [dbo].[ZoneRate] WITH ( ONLINE = OFF )
	DROP INDEX [IX_ZoneRate_ServicesFlag] ON [dbo].[ZoneRate] WITH ( ONLINE = OFF )
	DROP INDEX IX_ZoneRate_Supplier ON [dbo].[ZoneRate] WITH ( ONLINE = OFF )
	DROP INDEX IX_ZoneRate_Zone ON [dbo].[ZoneRate] WITH ( ONLINE = OFF );

	With BlocksCTE As(
   SELECT RB.CustomerID, RB.ZoneID, RB.SupplierID  
   FROM RouteBlock RB WITH(NOLOCK)
   WHERE RB.IsEffective = 'Y' AND RB.ZoneID IS NOT NULL And RB.CustomerID is null 
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
	    AND S.IsDeleted = 'N'-- Needed for bug ID 2822
 ) 
, SupCarriers AS 
 
 (
 SELECT S.*,Pr.Tax2 FROM CarrierAccount S WITH(NOLOCK) Left Join CarrierProfile Pr 
 On S.ProfileID=Pr.ProfileID
 WHERE 	S.ActivationStatus <> @Account_Inactive 
	    AND S.RoutingStatus <> @Account_Blocked 
		AND S.RoutingStatus <> @Account_BlockedOutbound
		AND S.IsDeleted = 'N'-- Needed for bug ID 2822
 ) 
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
			INNER JOIN PriceListCTE  P WITH(NOLOCK) ON Ra.PriceListID = P.PriceListID
			INNER JOIN SupCarriers CS WITH(NOLOCK) ON P.SupplierID = CS.CarrierAccountID
				WHERE dateadd(MI,isnull(cs.GMTTime,0),Ra.BeginEffectiveDate) <= @when AND (Ra.EndEffectiveDate IS NULL OR dateadd(MI,isnull(cs.GMTTime,0),Ra.EndEffectiveDate) > @when)
)
	,CommissionCTE As( 	SELECT * FROM Commission Co WITH(NOLOCK)
		WHERE Co.BeginEffectiveDate <= @when AND (Co.EndEffectiveDate IS NULL OR Co.EndEffectiveDate > @when)

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
			Case When P.SupplierID='SYS' Then CS.ProfileID Else CC.ProfileID End ProfileID,
			Case When @IncludeBlockedZones= 'Y' and RB.ZoneID is not null Then 1 Else 0 End Blocked
		FROM RateCTE Ra WITH(NOLOCK)
				INNER JOIN PriceListCTE  P WITH(NOLOCK) ON Ra.PriceListID = P.PriceListID
				left JOIN CurrencyCTE C WITH(NOLOCK) ON P.CurrencyID = C.CurrencyID
				INNER JOIN CusCarriers CS WITH(NOLOCK) ON P.CustomerID = CS.CarrierAccountID
				INNER JOIN SupCarriers CC WITH(NOLOCK) ON P.SupplierID = CC.CarrierAccountID
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
Select [ZoneID]
      ,[SupplierID]
      ,[CustomerID]
      ,cast(Rate as numeric(13,5))
      ,[OffPeakRate]
      ,[WeekendRate]
      ,[ServicesFlag]
      ,[Profileid]
      ,[Blocked] from ResultCTE

CREATE NONCLUSTERED INDEX [IX_ZoneRate_Customer] ON [dbo].[ZoneRate] (	[CustomerID] ASC)
CREATE NONCLUSTERED INDEX [IX_ZoneRate_ServicesFlag] ON [dbo].[ZoneRate] (	[ServicesFlag] ASC)
CREATE NONCLUSTERED INDEX [IX_ZoneRate_Supplier] ON [dbo].[ZoneRate] (	[SupplierID] ASC)
CREATE NONCLUSTERED INDEX [IX_ZoneRate_Zone] ON [dbo].[ZoneRate] (	[ZoneID] ASC)

End