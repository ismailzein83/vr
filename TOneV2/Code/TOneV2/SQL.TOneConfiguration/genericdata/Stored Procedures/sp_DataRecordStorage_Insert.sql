-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_DataRecordStorage_Insert]
	@Name NVARCHAR(1000),
	@DataRecordTypeId INT,
	@DataStoreId INT,
	@Settings NVARCHAR(MAX),
	@CreatedTime DATETIME = NULL,
	@Id INT OUT
AS
BEGIN
	IF NOT EXISTS (SELECT NULL FROM genericdata.DataRecordStorage WHERE Name = @Name)
	BEGIN
		INSERT INTO genericdata.DataRecordStorage (Name, DataRecordTypeID, DataStoreID, Settings, CreatedTime)
		VALUES (@Name, @DataRecordTypeId, @DataStoreId, @Settings, @CreatedTime)
		SET @Id = SCOPE_IDENTITY()
	END
END