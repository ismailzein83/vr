-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_DataStore_Insert]
	@Name NVARCHAR(900),
	@Settings NVARCHAR(MAX),
	@Id INT OUT
AS
BEGIN
	IF NOT EXISTS(SELECT NULL FROM genericdata.DataStore WHERE Name = @Name)
	BEGIN
		INSERT INTO genericdata.DataStore (Name, Settings, CreatedTime)
		VALUES (@Name, @Settings, GETDATE())
		SET @Id = SCOPE_IDENTITY()
	END
END