
CREATE PROCEDURE [VR_AccountBalance].[sp_BillingTransaction_HasData]
	@AccountTypeID uniqueidentifier
AS
BEGIN
	IF EXISTS(Select TOP 1 ID from [VR_AccountBalance].BillingTransaction WITH(NOLOCK) 
	where AccountTypeID = @AccountTypeID 
	AND 
	((ISNULL(IsDeleted, 0) = 0 AND ISNULL(IsBalanceUpdated, 0) = 0)
	OR 
	(ISNULL(IsDeleted, 0) = 1 AND ISNULL(IsBalanceUpdated, 0) = 1 and ISNULL(IsSubtractedFromBalance, 0) = 0)))
	BEGIN
		SELECT 1
	END

	ELSE
	BEGIN
		SELECT 0
	END
END