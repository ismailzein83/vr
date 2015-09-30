-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [PSTN_BE].[sp_Switch_Insert]
	@Name NVARCHAR(255),
	@TypeID INT,
	@AreaCode VARCHAR(10),
	@TimeOffset VARCHAR(50),
	@DataSourceID INT,
	@InsertedID INT OUT
AS
BEGIN
	IF NOT EXISTS (SELECT 1 FROM PSTN_BE.Switch WHERE Name = @Name)
	BEGIN
		INSERT INTO PSTN_BE.Switch (Name, TypeID, AreaCode, TimeOffset, DataSourceID)
		VALUES (@Name, @TypeID, @AreaCode, @TimeOffset, @DataSourceID)
		
		SET @InsertedID = @@IDENTITY
	END
END