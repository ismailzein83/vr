Create PROCEDURE [sec].[sp_UserFailedLogin_DeleteByUserId]
	@UserID int
AS
BEGIN
delete
	FROM		[sec].[UserFailedLogin] 
	WHERE		UserId = @UserID
END