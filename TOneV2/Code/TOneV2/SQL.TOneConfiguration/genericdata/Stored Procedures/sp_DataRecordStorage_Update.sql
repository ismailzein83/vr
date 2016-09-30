-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_DataRecordStorage_Update]
	@Id INT,
	@Name NVARCHAR(1000),
	@DataRecordTypeId uniqueidentifier,
	@DataStoreId INT,
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