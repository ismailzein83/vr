-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [Retail].[sp_AccountService_UpdateExecutedActions]
	@ID BIGINT,
	@ExecutedActions nvarchar(MAX)
AS
BEGIN
		UPDATE Retail.AccountService
		SET ExecutedActionsData = @ExecutedActions
		WHERE ID = @ID
END