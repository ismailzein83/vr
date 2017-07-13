-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_LiveBalance_TryAddAndGet]
	@AccountID varchar(50),
	@AccountTypeID uniqueidentifier,
	@InitialBalance decimal(20,6),
	@CurrencyId int,
	@CurrentBalance decimal(20,6),
	@BED datetime ,
	@EED datetime,
	@Status int,
	@IsDeleted bit
AS
BEGIN

	Declare @ID bigint, @CurrencyIdToReturn int;

	Select @ID = ID, @CurrencyIdToReturn = CurrencyID from [VR_AccountBalance].LiveBalance WHERE AccountID = @AccountID AND AccountTypeID = @AccountTypeID AND ISNULL(IsDeleted,0) = 0
	IF(@ID IS NULL)
	BEGIN
		INSERT INTO [VR_AccountBalance].LiveBalance (AccountID , AccountTypeID, InitialBalance, CurrencyID, CurrentBalance,BED,EED,[Status],IsDeleted)
		VALUES (@AccountID, @AccountTypeID, @InitialBalance, @CurrencyId, @CurrentBalance,@BED,@EED,@Status,@IsDeleted)	
		Set @ID = SCOPE_IDENTITY()

		SET @CurrencyIdToReturn = @CurrencyId
	END
	SELECT @ID as ID, @CurrencyIdToReturn as CurrencyId ,@AccountID as AccountID , @BED as BED, @EED as EED,@Status as [Status]
END