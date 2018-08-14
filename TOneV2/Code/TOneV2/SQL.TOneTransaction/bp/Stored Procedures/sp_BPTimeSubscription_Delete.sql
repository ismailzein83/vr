-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bp].[sp_BPTimeSubscription_Delete]
	@BPTimeSubscriptionId bigint
	
AS
BEGIN
	DELETE FROM [bp].[BPTimeSubscription] where id = @BPTimeSubscriptionId
END