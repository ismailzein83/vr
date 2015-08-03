CREATE PROCEDURE [dbo].[SaveNewPluginPermission]
(
	@Id varchar(255),
	@Name nvarchar(255),
	@Description ntext = null
)
AS
if ((SELECT Count(*) From [PluginPermissions] Where Id = @Id) = 0)
	INSERT INTO [PluginPermissions](Id, Name, [Description])
	Values(@Id, @Name, @Description)