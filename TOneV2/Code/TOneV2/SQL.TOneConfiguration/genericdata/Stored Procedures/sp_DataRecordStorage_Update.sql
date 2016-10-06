-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_DataRecordStorage_Update]
	@Id uniqueidentifier,
	@Name NVARCHAR(1000),
	@DataRecordTypeId uniqueidentifier,
	@DataStoreId uniqueidentifier,
	@Settings NVARCHAR(MAX)
AS
BEGIN
	IF NOT EXISTS (SELECT NULL FROM genericdata.DataRecordStorage WHERE ID != @Id AND Name = @Name)
	BEGIN
		UPDATE genericdata.DataRecordStorage
		SET Name = @Name, DataRecordTypeID = @DataRecordTypeId, DataStoreID = @DataStoreId, Settings = @Settings
		WHERE ID = @Id
	END
END