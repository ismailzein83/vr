-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [PSTN_BE].[sp_Switch_Update]
	@ID INT,
	@Name NVARCHAR(255),
	@TypeID INT,
	@AreaCode VARCHAR(10),
	@TimeOffset VARCHAR(50),
	@DataSourceID INT
AS
BEGIN
	IF NOT EXISTS (SELECT 1 FROM PSTN_BE.Switch WHERE Name = @Name AND ID != @ID)
	BEGIN
		UPDATE PSTN_BE.Switch
		SET Name = @Name,
			TypeID = @TypeID,
			AreaCode = @AreaCode,
			TimeOffset = @TimeOffset,
			DataSourceID = @DataSourceID
		WHERE ID = @ID
	END
END