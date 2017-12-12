-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountManager].[sp_AccountManager_Update] 
	@ID INT,
	@UserId INT,
	@accountManagerDefinitionId uniqueidentifier
AS
BEGIN
	
	Update AccountManager
	Set UserID = @UserId,
		AccountManagerDefinitionID = @accountManagerDefinitionId
	Where ID = @ID
	
END