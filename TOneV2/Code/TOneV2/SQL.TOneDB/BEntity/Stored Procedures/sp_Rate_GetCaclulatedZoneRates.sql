

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_Rate_GetCaclulatedZoneRates]
	@ZoneIds LCR.IntIDType READONLY,
	@EffectiveTime datetime,
	@IsFuture bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @IsZoneFiltered BIT
	Select @IsZoneFiltered = CAST(COUNT(*) AS BIT) From @ZoneIds 
    DECLARE @Account_Inactive tinyint
    DECLARE @Account_Blocked tinyint
    DECLARE @Account_BlockedInbound tinyint
    DECLARE @Account_BlockedOutbound tinyint

    -- Set Enumerations
    SELECT @Account_Inactive = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.ActivationStatus' AND Name = 'Inactive'
    SELECT @Account_Blocked = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'Blocked'
    SELECT @Account_BlockedInbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedInbound'	
    SELECT @Account_BlockedOutbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedOutbound'	;


    WITH CusCarriers AS  
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

    SELECT
             r.RateID, 
             p.PriceListID, 
             r.ZoneID, 
             p.SupplierID, 
             p.CustomerID,
			(R.Rate/ C.LastRate) +
			Case When Co.SupplierID is null Then 0 Else isnull(CASE WHEN P.SupplierID <> 'SYS' THEN 1 ELSE -1 END * (CASE WHEN isnull(Co.Amount,0)  <> 0 THEN Amount / C.LastRate ELSE (R.Rate/ C.LastRate) * Percentage / 100.0  END),0) End  
			NormalRate,
			r.ServicesFlag
    FROM Rate r WITH (NOLOCK)
    LEFT JOIN @ZoneIds zf on zf.ID = r.ZoneID
    JOIN PriceList p WITH (NOLOCK) ON r.PriceListID = p.PriceListID
    LEFT JOIN Currency C WITH(NOLOCK) ON P.CurrencyID = C.CurrencyID
    JOIN CusCarriers CS WITH(NOLOCK) ON P.CustomerID = CS.CarrierAccountID
    JOIN SupCarriers CC WITH(NOLOCK) ON P.SupplierID = CC.CarrierAccountID
    LEFT JOIN Commission Co WITH(NOLOCK) ON P.SupplierID = Co.SupplierID	
                                            AND P.CustomerID = Co.CustomerID AND r.ZoneID = Co.ZoneID and r.Rate between co.FromRate and co.ToRate	
                                            AND
                                            (
												(@IsFuture = 0 AND co.BeginEffectiveDate <= @EffectiveTime AND (co.EndEffectiveDate > @EffectiveTime OR co.EndEffectiveDate IS NULL))
												OR
												(@IsFuture = 1 AND (co.BeginEffectiveDate > GETDATE() OR co.EndEffectiveDate IS NULL))
											)
    WHERE  (zf.ID IS NOT NULL OR @IsZoneFiltered = 0)
			AND (
			(@IsFuture = 0 AND r.BeginEffectiveDate <= @EffectiveTime AND (r.EndEffectiveDate > @EffectiveTime OR r.EndEffectiveDate IS NULL))
			OR
			(@IsFuture = 1 AND (r.BeginEffectiveDate > GETDATE() OR r.EndEffectiveDate IS NULL))
		  )
END