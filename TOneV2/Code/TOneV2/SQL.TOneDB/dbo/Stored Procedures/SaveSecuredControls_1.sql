
create PROCEDURE [dbo].[SaveSecuredControls]
(
	@PagePath varchar(100),
	@table _SecuredControl readonly,
	@RemovePrevious bit = 0
)
AS

if(@RemovePrevious = 1)
BEGIN
	Delete From PageSecuredControls where PagePath = @PagePath
	
	INSERT INTO PageSecuredControls
			([ControlId], [ControlType], [ParentControlId], [ParentControlType],
			[CommandName], [ColumnIndex], [PermissionId], [PropertyAction], [PagePath], [Details])
	SELECT	[ControlId], [ControlType], [ParentControlId], [ParentControlType],
			[CommandName], [ColumnIndex], [PermissionId], [PropertyAction], [PagePath], [Details]
	FROM @table
END
ELSE
BEGIN
	Update PageSecuredControls
	SET PermissionId = t.PermissionId,
		PropertyAction = t.PropertyAction
	FROM @Table t
	Where PageSecuredControls.Id = t.ID
	
	INSERT INTO PageSecuredControls
			([ControlId], [ControlType], [ParentControlId], [ParentControlType],
			[CommandName], [ColumnIndex], [PermissionId], [PropertyAction], [PagePath], [Details])
	SELECT	[ControlId], [ControlType], [ParentControlId], [ParentControlType],
			[CommandName], [ColumnIndex], [PermissionId], [PropertyAction], [PagePath], [Details]
	FROM @table
	Where [ID] = 0

END