﻿CREATE PROCEDURE [VR_AccountBalance].[sp_LiveBalance_UpdateFromBillingTransaction]
	@AccountTypeId uniqueidentifier,
	@AccountId bigint,
	@Amount decimal(20,6)
AS
BEGIN
	Update [VR_AccountBalance].LiveBalance Set CurrentBalance +=  @Amount 
	where AccountTypeID = @AccountTypeId and AccountID = @AccountId
END