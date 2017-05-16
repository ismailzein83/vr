﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_LiveBalance_TryAddAndGet]
	@AccountID varchar(50),
	@AccountTypeID uniqueidentifier,
	@InitialBalance decimal(20,6),
	@CurrencyId int,
	@CurrentBalance decimal(20,6)
	
AS
BEGIN

	Declare @ID bigint;

	Select @ID = ID from [VR_AccountBalance].LiveBalance WHERE AccountID = @AccountID AND AccountTypeID = @AccountTypeID
	IF(@ID IS NULL)
	BEGIN
		INSERT INTO [VR_AccountBalance].LiveBalance (AccountID , AccountTypeID, InitialBalance, CurrencyID, CurrentBalance)
		VALUES (@AccountID, @AccountTypeID, @InitialBalance, @CurrencyId, @CurrentBalance)	
		Set @ID = SCOPE_IDENTITY()
	END
	SELECT @ID as ID, @CurrencyId as CurrencyId ,@AccountID as AccountID
END