-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [PSTN_BE].[sp_Switch_Update]
	@ID INT,
	@Name NVARCHAR(255),
	@TypeID INT = NULL,
	@AreaCode VARCHAR(10) = NULL,
	@TimeOffset VARCHAR(50) = NULL
AS
BEGIN
	IF NOT EXISTS (SELECT 1 FROM PSTN_BE.Switch WHERE Name = @Name AND ID != @ID)
	BEGIN
		UPDATE PSTN_BE.Switch
		SET Name = @Name,
			TypeID = @TypeID,
			AreaCode = @AreaCode,
			TimeOffset = @TimeOffset
		WHERE ID = @ID
	END
END