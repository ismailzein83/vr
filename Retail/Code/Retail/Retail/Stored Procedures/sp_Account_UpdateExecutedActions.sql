-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail].[sp_Account_UpdateExecutedActions]
	@ID BIGINT,
	@ExecutedActions nvarchar(MAX)
AS
BEGIN
		UPDATE Retail.Account
		SET ExecutedActionsData = @ExecutedActions, LastModifiedTime = GETDATE()
		WHERE ID = @ID
END