-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_DataStore_Update]
	@Id uniqueidentifier,
	@Name VARCHAR(255),
	@Setting NVARCHAR(MAX)
AS
BEGIN
	IF NOT EXISTS(SELECT NULL FROM genericdata.DataStore WHERE ID != @Id AND Name = @Name)
	BEGIN
		UPDATE genericdata.DataStore
		SET Name = @Name, Settings = @Setting
		WHERE ID = @Id
	END
END