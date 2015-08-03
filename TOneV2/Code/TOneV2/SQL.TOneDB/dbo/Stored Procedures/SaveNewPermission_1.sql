CREATE PROCEDURE [dbo].[SaveNewPermission]
(
	@Id varchar(255),
	@Name nvarchar(255),
	@Description ntext = null
)
AS
if ((SELECT Count(*) From [Permission] Where Id = @Id) = 0)
	INSERT INTO [Permission](Id, Name, [Description])
	Values(@Id, @Name, @Description)