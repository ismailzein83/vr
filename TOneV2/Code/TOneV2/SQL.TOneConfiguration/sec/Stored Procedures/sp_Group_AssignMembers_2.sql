-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_Group_AssignMembers] 
	@GroupId int,
	@UserIds sec.[IntIDType] READONLY
AS
BEGIN	

    --Insert new members
	Insert into sec.UserGroup
	([UserId], [GroupId])
	SELECT ID, @GroupId 
	FROM @UserIds
	WHERE ID NOT IN (SELECT UserId FROM sec.UserGroup WHERE GroupId = @GroupId)
	
	--delete removed members
	DELETE sec.UserGroup
	WHERE GroupId = @GroupId
			AND 
			UserId NOT IN (SELECT ID FROM @UserIds)
	
END