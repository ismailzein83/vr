-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [PSTN_BE].[sp_SwitchTrunk_Update]
	@ID INT,
	@Name NVARCHAR(255),
	@Symbol NVARCHAR(50),
	@SwitchID INT,
	@Type INT,
	@Direction INT
AS
BEGIN
	IF NOT EXISTS (SELECT 1 FROM PSTN_BE.SwitchTrunk WHERE Name = @Name AND ID != @ID)
	BEGIN
		UPDATE PSTN_BE.SwitchTrunk
		SET Name = @Name,
			Symbol = @Symbol,
			SwitchID = @SwitchID,
			Type = @Type,
			Direction = @Direction
		WHERE ID = @ID
	END
END