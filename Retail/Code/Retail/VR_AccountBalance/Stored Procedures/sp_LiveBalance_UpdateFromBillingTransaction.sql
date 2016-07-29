CREATE PROCEDURE [VR_AccountBalance].[sp_LiveBalance_UpdateFromBillingTransaction]
	@AccountId bigint,
	@Amount decimal(20,6)
AS
BEGIN
	Update [VR_AccountBalance].LiveBalance Set CurrentBalance +=  @Amount where AccountID = @AccountId
END