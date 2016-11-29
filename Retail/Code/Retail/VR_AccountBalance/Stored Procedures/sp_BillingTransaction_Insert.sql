-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_BillingTransaction_Insert]
	@AccountID INT,
	@AccountTypeID uniqueidentifier,
	@Amount decimal(20,6),
	@CurrencyId int,
	@TransactionTypeId uniqueidentifier,
	@TransactionTime datetime,
	@Notes nvarchar(1000),
	@Reference nvarchar(255),
	@ID INT OUT
AS
BEGIN
	INSERT INTO VR_AccountBalance.BillingTransaction (AccountID, AccountTypeID, Amount, CurrencyId, TransactionTypeId,TransactionTime,Notes,Reference)
	VALUES (@AccountID,@AccountTypeID, @Amount, @CurrencyId, @TransactionTypeId,@TransactionTime,@Notes,@Reference)
	SET @ID = SCOPE_IDENTITY()
END