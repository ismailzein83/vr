-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_User_UpdateLastLogin]
	@UserID INT,
	@LastModifiedBy INT
AS
BEGIN
	UPDATE sec.[User]
	SET LastLogin = GETDATE(), LastModifiedBy = @LastModifiedBy, LastModifiedTime = GETDATE()
	WHERE ID = @UserID
END