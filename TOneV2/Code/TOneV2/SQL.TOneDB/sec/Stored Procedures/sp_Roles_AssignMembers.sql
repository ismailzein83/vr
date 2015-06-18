-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_Roles_AssignMembers] 
	@RoleId int,
	@UserIds sec.[IntIDType] READONLY
AS
BEGIN	

    --Insert new members
	Insert into sec.UserRole
	([UserId], [RoleId])
	SELECT ID, @RoleId 
	FROM @UserIds
	WHERE ID NOT IN (SELECT UserID FROM sec.UserRole WHERE RoleID = @RoleId)
	
	--delete removed members
	DELETE sec.UserRole
	WHERE RoleID = @RoleId 
			AND 
			UserID NOT IN (SELECT ID FROM @UserIds)
	
END