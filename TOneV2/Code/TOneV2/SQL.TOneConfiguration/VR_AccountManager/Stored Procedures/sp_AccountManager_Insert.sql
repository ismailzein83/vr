-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountManager].[sp_AccountManager_Insert]
	@AccountManagerDefinitionId uniqueidentifier,
	@userId INT,
	@ID INT OUT
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM AccountManager WHERE AccountManagerDefinitionId = @AccountManagerDefinitionId and UserID = @userId)
BEGIN
	INSERT INTO AccountManager(AccountManagerDefinitionId,UserID)
	VALUES (@AccountManagerDefinitionId,@userId)
	SET  @ID = scope_identity()
	END
END