-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [Retail_BE].[sp_AccountStatusHistory_Delete]
	@AccountStatusHistoryIdsToDelete nvarchar(MAX)
AS
BEGIN
	BEGIN
		DECLARE @AccountStatusHistoryIdsToDeleteTable TABLE (AccountStatusHistoryId varchar(50))
		INSERT INTO @AccountStatusHistoryIdsToDeleteTable (AccountStatusHistoryId)
		select ParsedString from [Retail_BE].[ParseStringList](@AccountStatusHistoryIdsToDelete)

		UPDATE [Retail_BE].AccountStatusHistory SET IsDeleted = 1 WHERE ID IN (SELECT AccountStatusHistoryId FROM @AccountStatusHistoryIdsToDeleteTable)
	END
END