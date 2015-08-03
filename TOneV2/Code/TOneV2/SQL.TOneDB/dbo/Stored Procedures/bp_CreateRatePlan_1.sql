
-- ========================================================================
-- Author:		Fadi Chamieh
-- Create date: 01/10/2007
-- Description:	Create a Rate Plan for a customer or a policy from the 
--              effective rates this customer or policy has. 
-- ========================================================================
CREATE PROCEDURE [dbo].[bp_CreateRatePlan](@CustomerID varchar(10) = NULL, @Currency VARCHAR(3) = NULL, @RatePlanID int output)
AS
BEGIN
	SET NOCOUNT ON;

	-- Check if a rate plan already exists for customer or policy
	SELECT @RatePlanID = RatePlanID	FROM RatePlan WHERE CustomerID = @CustomerID
	
	DECLARE @CurrencyID varchar(3) 
	
	IF(@Currency IS NULL )
	SELECT @CurrencyID = CurrencyID 
			FROM CarrierProfile   
			WHERE CarrierProfile.ProfileID IN 
							(SELECT ProfileID FROM CarrierAccount ca WHERE ca.CarrierAccountID=@CustomerID)
	ELSE
	SET @CurrencyID = @Currency
		
	-- Empty Planning Rates if any
	IF @RatePlanID IS NOT NULL
		RETURN
	ELSE
	BEGIN
		INSERT INTO RatePlan ( CustomerID, CurrencyID, BeginEffectiveDate )
		VALUES ( @CustomerID, @CurrencyID, getdate() ) 
		SELECT @RatePlanID = @@IDENTITY
	End		
		
	-- Current Rates
	INSERT INTO PlaningRate
		(
		RatePlanID,
		ZoneID,
		ServicesFlag,
		Rate,
		OffPeakRate,
		WeekendRate,
		BeginEffectiveDate,
		EndEffectiveDate
		)	
	SELECT 
		@RatePlanID,
		R.ZoneID,
		MAX(ServicesFlag),
		0, -- R.Rate / C.LastRate,
		0, -- R.OffPeakRate / C.LastRate,
		0, -- R.WeekendRate / C.LastRate,
		MAX(R.BeginEffectiveDate),
		MIN(R.EndEffectiveDate)
	 FROM Rate R, PriceList P 
		WHERE 
			P.CustomerID = @CustomerID
			AND P.PriceListID=R.PriceListID
			AND	R.IsEffective = 'Y'
	GROUP BY R.ZoneID 
	
	-- Pending Rates?
	INSERT INTO PlaningRate
		(
		RatePlanID,
		ZoneID,
		ServicesFlag,
		Rate,
		OffPeakRate,
		WeekendRate,
		BeginEffectiveDate,
		EndEffectiveDate
		)	
	SELECT 
		@RatePlanID,
		R.ZoneID,
		MAX(ServicesFlag),
		0, -- R.Rate / C.LastRate,
		0, -- R.OffPeakRate / C.LastRate,
		0, -- R.WeekendRate / C.LastRate,
		MAX(R.BeginEffectiveDate),
		MIN(R.EndEffectiveDate)
	 FROM Rate R, PriceList P 
		WHERE 
			R.ZoneID NOT IN(SELECT Prv.ZoneID FROM PlaningRate Prv WHERE Prv.RatePlanID = @RatePlanID)
			AND P.CustomerID = @CustomerID
			AND P.PriceListID = R.PriceListID
			AND	R.IsEffective = 'N'
			AND R.EndEffectiveDate IS NULL
	GROUP BY R.ZoneID 

END