-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_LiveBalance_CheckIfHasTransactions]
	-- Add the parameters for the stored procedure here
	@AccountTypeId uniqueidentifier,
	@AccountID varchar(50),
	@HasTransactions BIT out
AS
BEGIN

	DECLARE @Count INT;

	SELECT  @Count = Count(*) 
	FROM	[VR_AccountBalance].LiveBalance lb  with(nolock)
	WHERE	AccountTypeID = @AccountTypeId and AccountID = @AccountID
        
	IF(@Count IS NULL OR @Count = 0)
	BEGIN
		SELECT  @Count = Count(*) 
		FROM	[VR_AccountBalance].BillingTransaction bt  with(nolock)
		WHERE	AccountTypeID = @AccountTypeId and AccountID = @AccountID AND bt.IsDeleted IS NULL
	END
	SET @HasTransactions = CASE WHEN @Count > 0 THEN 1 ELSE 0 END;
END