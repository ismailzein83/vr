-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_Connection_Insert]
@ConnectionId uniqueidentifier,
@Name nvarchar(255),
@Settings nvarchar(MAX)
AS
BEGIN
IF NOT EXISTS(Select Name from [common].Connection WITH(NOLOCK) where Name = @Name)
	BEGIN
		Insert into [common].Connection (Id, Name, Settings)
		SELECT @ConnectionId, @Name, @Settings WHERE NOT EXISTS (Select Name from [common].Connection where Name = @Name)
	END
END