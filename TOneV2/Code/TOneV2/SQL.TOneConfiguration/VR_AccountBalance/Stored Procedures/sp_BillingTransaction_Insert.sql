﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_BillingTransaction_Insert]
	@AccountID varchar(50),
	@AccountTypeID uniqueidentifier,
	@Amount decimal(20,6),
	@CurrencyId int,
	@TransactionTypeId uniqueidentifier,
	@TransactionTime datetime,
	@Notes nvarchar(1000),
	@Reference nvarchar(255),
	@SourceId nvarchar(255),
	@Settings nvarchar(max),
	@CreatedByInvoiceId bigint = null,
	@PaymentToInvoiceID bigint,
	@ID INT OUT
AS
BEGIN
	INSERT INTO VR_AccountBalance.BillingTransaction (AccountID, AccountTypeID, Amount, CurrencyId, TransactionTypeId, TransactionTime, Notes, Reference, SourceID, Settings, CreatedByInvoiceID, PaymentToInvoiceID)
	VALUES (@AccountID, @AccountTypeID, @Amount, @CurrencyId, @TransactionTypeId, @TransactionTime, @Notes, @Reference, @SourceID, @Settings, @CreatedByInvoiceId, @PaymentToInvoiceID)
	SET @ID = SCOPE_IDENTITY()
END