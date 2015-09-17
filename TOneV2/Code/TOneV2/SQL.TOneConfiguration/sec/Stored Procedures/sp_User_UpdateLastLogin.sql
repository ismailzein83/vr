-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_User_UpdateLastLogin]
	@UserID INT
AS
BEGIN
	UPDATE sec.[User]
	SET LastLogin = GETDATE()
	WHERE ID = @UserID
END