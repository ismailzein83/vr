-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [Retail].[sp_Account_UpdateExecutedActions]
	@ID BIGINT,
	@ExecutedActions nvarchar(MAX)
AS
BEGIN
		UPDATE Retail.Account
		SET ExecutedActionsData = @ExecutedActions
		WHERE ID = @ID
END