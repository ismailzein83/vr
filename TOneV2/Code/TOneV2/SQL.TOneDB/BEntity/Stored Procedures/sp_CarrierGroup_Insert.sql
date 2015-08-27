-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_CarrierGroup_Insert] 
(
@Name nvarchar(255),
@ParentID int,
@CarrierGroupId smallint OUTPUT
)
AS
BEGIN
	INSERT INTO BEntity.CarrierGroup
		 (	Name ,
			ParentID)
			VALUES
			( @Name,
			  @ParentID
			)
			set @CarrierGroupId = scope_Identity()
END