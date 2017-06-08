﻿CREATE PROCEDURE [VR_AccountBalance].[sp_BillingTransaction_SetBalanceUpdated]
	@BillingTransactionIDs varchar(max)
AS
BEGIN

		DECLARE @BillingTransactionIDsTable TABLE (BillingTransactionID bigint)
		INSERT INTO @BillingTransactionIDsTable (BillingTransactionID)
		select Convert(bigint, ParsedString) from [VR_AccountBalance].[ParseStringList](@BillingTransactionIDs)
		Update [VR_AccountBalance].BillingTransaction Set IsBalanceUpdated = 1 where ID in (Select BillingTransactionID from @BillingTransactionIDsTable)
END