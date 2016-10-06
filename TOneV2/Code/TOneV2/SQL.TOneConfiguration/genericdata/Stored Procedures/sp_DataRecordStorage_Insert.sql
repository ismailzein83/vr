-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_DataRecordStorage_Insert]
	@Id uniqueidentifier,
	@Name NVARCHAR(1000),
	@DataRecordTypeId uniqueidentifier,
	@DataStoreId uniqueidentifier,
	@Settings NVARCHAR(MAX),
	@CreatedTime DATETIME = NULL
	
AS
BEGIN
	IF NOT EXISTS (SELECT NULL FROM genericdata.DataRecordStorage WHERE Name = @Name)
	BEGIN
		INSERT INTO genericdata.DataRecordStorage (Id,Name, DataRecordTypeID, DataStoreID, Settings, CreatedTime)
		VALUES (@Id,@Name, @DataRecordTypeId, @DataStoreId, @Settings, @CreatedTime)
	END
END