create PROCEDURE [VR_AccountBalance].sp_AccountUsage_GetByAccountIds
	@AccountTypeID uniqueidentifier,
	@AccountsIds varchar(max) = NULL,
	@TransactionTypeIds varchar(max)
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	    BEGIN
	    DECLARE @AccountsIdsTable TABLE (AccountID varchar(50))
		INSERT INTO @AccountsIdsTable (AccountID)
		select ParsedString from [VR_AccountBalance].[ParseStringList](@AccountsIds)

		DECLARE @TransactionTypeIdsTable TABLE (TransactionTypeID uniqueidentifier)
		INSERT INTO @TransactionTypeIdsTable (TransactionTypeID)
		select Convert(uniqueidentifier, ParsedString) from [VR_AccountBalance].[ParseStringList](@TransactionTypeIds)

		SELECT	au.ID, au.AccountTypeID,TransactionTypeID, au.AccountID,au.CurrencyId,PeriodStart,PeriodEnd,UsageBalance, IsOverridden, OverriddenAmount, CorrectionProcessID
			FROM VR_AccountBalance.AccountUsage au with(nolock) 
		
        WHERE	(@AccountsIds  IS NULL or au.AccountID IN (select AccountID from @AccountsIdsTable))
			    AND (@TransactionTypeIds  IS NULL OR au.TransactionTypeID IN (select TransactionTypeID from @TransactionTypeIdsTable))
				and isnull(IsOverridden, 0) = 0
				AND au.AccountTypeID = @AccountTypeID

		
		END
	SET NOCOUNT OFF
END