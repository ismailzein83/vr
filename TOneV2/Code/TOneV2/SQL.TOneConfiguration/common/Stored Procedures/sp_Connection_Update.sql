
CREATE PROCEDURE  [common].[sp_Connection_Update]
@ConnectionId uniqueidentifier,
@Name nvarchar(255),
@Settings nvarchar(MAX)
AS
BEGIN
  	IF NOT EXISTS(Select Name from [common].Connection WITH(NOLOCK) where Name = @Name And ID != @ConnectionId)
	BEGIN
		Update [common].Connection set Name = @Name,
		Settings = @Settings
		where ID = @ConnectionId
	END
END