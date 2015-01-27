CREATE  FUNCTION [dbo].[GetCommissionedRate]
(
	@SupplierID varchar(10) = NULL,
	@CustomerID varchar(10) = NULL,
	@RateCurrency CHAR(3),
	@EffectiveDate smalldatetime = getdate,
	@ZoneID int,
	@Rate float
)
RETURNS float
AS
BEGIN
	-- Declare the return variable here
	DECLARE @CommissionedRate float

	SELECT @CommissionedRate = 
		@Rate 
			-- Add Commission (if any)
			+ dbo.GetCommission(@SupplierID, @CustomerID,@RateCurrency, @EffectiveDate, @ZoneID, @Rate, 'N')
			-- Add Extra Charges (if any)
			+ dbo.GetCommission(@SupplierID, @CustomerID,@RateCurrency, @EffectiveDate, @ZoneID, @Rate, 'Y')

	-- Return the result of the function
	RETURN @CommissionedRate

END