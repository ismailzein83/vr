
CREATE PROCEDURE [dbo].[SavePluginSecuredControls]
(
	@PagePath varchar(100),
	@table _SecuredControl readonly,
	@RemovePrevious bit = 0
)
AS

if(@RemovePrevious = 1)
BEGIN
	Delete From PluginPageSecuredControls where PagePath = @PagePath
	
	INSERT INTO PluginPageSecuredControls
			([ControlId], [ControlType], [ParentControlId], [ParentControlType],
			[CommandName], [ColumnIndex], [PermissionId], [PropertyAction], [PagePath], [Details])
	SELECT	[ControlId], [ControlType], [ParentControlId], [ParentControlType],
			[CommandName], [ColumnIndex], [PermissionId], [PropertyAction], [PagePath], [Details]
	FROM @table
END
ELSE
BEGIN
	Update PluginPageSecuredControls
	SET PermissionId = t.PermissionId,
		PropertyAction = t.PropertyAction
	FROM @Table t
	Where PluginPageSecuredControls.Id = t.ID
	
	INSERT INTO PluginPageSecuredControls
			([ControlId], [ControlType], [ParentControlId], [ParentControlType],
			[CommandName], [ColumnIndex], [PermissionId], [PropertyAction], [PagePath], [Details])
	SELECT	[ControlId], [ControlType], [ParentControlId], [ParentControlType],
			[CommandName], [ColumnIndex], [PermissionId], [PropertyAction], [PagePath], [Details]
	FROM @table
	Where [ID] = 0

END