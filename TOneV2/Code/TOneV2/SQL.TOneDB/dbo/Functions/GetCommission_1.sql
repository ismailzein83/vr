-- ============================================================================================================
-- Author:		Fadi Chamieh
-- Create date: 11/07/2007
-- Description:	Get the commission (or Extra Charge) Sale/Purchase Rate for a Customer/Supplier on a given Zone
-- ============================================================================================================
CREATE FUNCTION [dbo].[GetCommission]
(
	@SupplierID varchar(10),
	@CustomerID varchar(10),
	@RateCurrency CHAR(3),
	@EffectiveDate smalldatetime,
	@ZoneID int,
	@Rate float,
	@IsExtraCharge char(1)
)
RETURNS float
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Commission float
	DECLARE @Operator float
	DECLARE @ProfileCurrency CHAR(3)
	
	
	SET @Commission = 0
	
	SET @Operator = CASE WHEN @SupplierID <> 'SYS' THEN 1 ELSE -1 END
	
	IF (@SupplierID = 'SYS')
    SELECT @ProfileCurrency = cp.CurrencyID
	  FROM CarrierProfile cp WITH(NOLOCK) WHERE cp.ProfileID IN 
	(
	  SELECT ca.ProfileID
	  FROM CarrierAccount ca WITH(NOLOCK) WHERE ca.CarrierAccountID = @CustomerID
	)
	
	IF (@CustomerID = 'SYS')
	SELECT @ProfileCurrency = cp.CurrencyID
	  FROM CarrierProfile cp WITH(NOLOCK) WHERE cp.ProfileID IN 
	(
	  SELECT ca.ProfileID
	  FROM CarrierAccount ca WITH(NOLOCK) WHERE ca.CarrierAccountID = @SupplierID
	)
	
	DECLARE @Factor FLOAT 
	
	IF(@ProfileCurrency <> @RateCurrency)
    	SET @Factor = dbo.GetExchangeRate(@ProfileCurrency,GETDATE()) / dbo.GetExchangeRate(@RateCurrency,GETDATE())
	ELSE
		SET @Factor = 1
	

	-- Compute Commission Or Extra Charge
	SELECT @Commission = @Operator * (CASE WHEN Amount  IS NOT NULL THEN Amount / @Factor ELSE @Rate * Percentage / 100.0  END)
		FROM [Commission] WITH(NOLOCK)
		WHERE 
		    @EffectiveDate BETWEEN BeginEffectiveDate AND ISNULL(EndEffectiveDate,getdate())
			AND SupplierID = @SupplierID
			AND CustomerID = @CustomerID
			
			AND ZoneID = @ZoneID
			AND (FromRate IS NULL OR (@Rate * @Factor BETWEEN FromRate AND ToRate))
			AND IsExtraCharge = @IsExtraCharge

	-- Return the result of the function
	RETURN @Commission 

END