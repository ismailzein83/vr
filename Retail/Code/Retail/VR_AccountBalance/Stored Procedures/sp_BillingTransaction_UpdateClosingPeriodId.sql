-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
--- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_BillingTransaction_UpdateClosingPeriodId] 
 @ClosingPeriodID bigint,
 @AccountTypeID uniqueidentifier
AS
BEGIN
		UPDATE [VR_AccountBalance].[BillingTransaction]
		SET ClosingPeriodId = @ClosingPeriodID
		WHERE @ClosingPeriodID IS NULL AND ISNULL(IsBalanceUpdated, 0) = 1 and AccountTypeID = @AccountTypeID 

END