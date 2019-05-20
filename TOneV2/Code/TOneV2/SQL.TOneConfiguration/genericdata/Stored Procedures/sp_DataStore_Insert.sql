-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_DataStore_Insert]
	@Id uniqueidentifier ,
	@Name NVARCHAR(900),
    @DevProjectId uniqueidentifier,
	@Settings NVARCHAR(MAX)

AS
BEGIN
	IF NOT EXISTS(SELECT NULL FROM genericdata.DataStore WHERE Name = @Name)
	BEGIN
		INSERT INTO genericdata.DataStore (ID,Name,DevProjectId,Settings, CreatedTime)
		VALUES (@Id,@Name,@DevProjectId, @Settings, GETDATE())

	END
END