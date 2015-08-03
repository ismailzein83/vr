-- ====================================================================================================================
-- Author: Fadi Chamieh
-- Create date: 2008-03-26
-- Modification Date: 2009-08-20 (Changed return type to decimal from Money, reason: money is 4 decimal precision 
-- Description:	Gets the previous rate value for a given rate
-- ====================================================================================================================
CREATE FUNCTION [dbo].[GetPreviousRateValue] 
(
	-- Add the parameters for the function here
	@RateID bigint
)
RETURNS DECIMAL(9,5)
AS
BEGIN
	DECLARE @RateValue DECIMAL(9,5)
	SET @RateValue = (SELECT TOP 1 O.Rate FROM Rate R WITH(NOLOCK), Rate O WITH(NOLOCK), PriceList P WITH(NOLOCK), PriceList OP WITH(NOLOCK)
						WHERE 
							R.RateID = @RateID
							AND O.ZoneID = R.ZoneID
							AND P.PriceListID = R.PriceListID
							AND OP.PriceListID = O.PriceListID
							AND P.SupplierID = OP.SupplierID
							AND P.CustomerID = OP.CustomerID
						    AND (O.BeginEffectiveDate = R.BeginEffectiveDate OR (O.BeginEffectiveDate < R.BeginEffectiveDate AND O.BeginEffectiveDate < O.EndEffectiveDate)) 
						    AND O.RateID <> R.RateID
						ORDER BY O.BeginEffectiveDate DESC, O.RateID DESC)

	-- Declare the return variable here
	RETURN @RateValue 
END