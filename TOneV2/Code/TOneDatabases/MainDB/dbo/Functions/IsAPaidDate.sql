CREATE FUNCTION [dbo].[IsAPaidDate](@Date SMALLDATETIME, @CustomerID VARCHAR(10), @SupplierID VARCHAR(10), @CustomerProfileID SMALLINT, @SupplierProfileID SMALLINT) 
RETURNS CHAR(1)
AS
BEGIN
	DECLARE @Result CHAR(1)
	
	IF @SupplierID IS NOT NULL
	BEGIN
		SELECT @Result = 
			CASE WHEN EXISTS(SELECT * FROM Billing_Invoice bi 
			                 WHERE bi.SupplierID = @SupplierID 
								AND @Date BETWEEN bi.BeginDate AND bi.EndDate 
								AND bi.IsPaid = 'Y'
			)
				THEN 'Y'
				ELSE 'N'
			END			
	END
	ELSE		
	IF @CustomerID IS NOT NULL
	BEGIN
		SELECT @Result = 
			CASE WHEN EXISTS(SELECT * FROM Billing_Invoice bi 
			                 WHERE bi.CustomerID = @CustomerID
								AND @Date BETWEEN bi.BeginDate AND bi.EndDate 
								AND bi.IsPaid = 'Y'
			)
				THEN 'Y'
				ELSE 'N'
			END			
	END
	ELSE
	IF @SupplierProfileID IS NOT NULL
	BEGIN
		SELECT @Result = 
			CASE WHEN EXISTS(SELECT * FROM Billing_Invoice bi 
			                 WHERE bi.SupplierID IN (SELECT ca.CarrierAccountID FROM CarrierAccount ca WHERE ca.ProfileID = @SupplierProfileID) 
								AND @Date BETWEEN bi.BeginDate AND bi.EndDate 
								AND bi.IsPaid = 'Y'
			)
				THEN 'Y'
				ELSE 'N'
			END			
	END
	ELSE		
	IF @CustomerProfileID IS NOT NULL
	BEGIN
		SELECT @Result = 
			CASE WHEN EXISTS(SELECT * FROM Billing_Invoice bi 
			                 WHERE bi.CustomerID IN (SELECT ca.CarrierAccountID FROM CarrierAccount ca WHERE ca.ProfileID = @CustomerProfileID)
								AND @Date BETWEEN bi.BeginDate AND bi.EndDate 
								AND bi.IsPaid = 'Y'
			)
				THEN 'Y'
				ELSE 'N'
			END			
	END
		
	RETURN @Result

END