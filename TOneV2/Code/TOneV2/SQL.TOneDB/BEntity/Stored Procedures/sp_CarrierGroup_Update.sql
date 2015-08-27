-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_CarrierGroup_Update] 
(
@ID smallint,
@Name nvarchar(255),
@ParentID int
)
AS
BEGIN
	UPDATE BEntity.CarrierGroup set 
		 	Name = @Name,
			ParentID = @ParentID
	where ID = @ID
	
	delete  from BEntity.CarrierGroupMember where CarrierGroupID = @ID
END