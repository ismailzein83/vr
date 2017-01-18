-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [VR_AccountBalance].[sp_BalanceUsageQueue_GetByQueueType]
	@AccountTypeID uniqueidentifier,
	@QueueType int
AS
BEGIN
	SELECT ID, AccountTypeID , UsageDetails
	FROM VR_AccountBalance.BalanceUsageQueue with(nolock)
	where AccountTypeID = @AccountTypeID AND QueueType = @QueueType
END