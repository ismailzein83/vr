-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail].[sp_Switch_Update]
	@ID INT,
	@Name NVARCHAR(255),
	@Settings NVARCHAR(MAX)
AS
BEGIN
	IF NOT EXISTS
	(
		SELECT 1 FROM Retail_BE.Switch
		WHERE ID != @ID
			AND Name = @Name
	)
	BEGIN
		UPDATE Retail_BE.Switch
		SET Name = @Name, Settings = @Settings
		WHERE ID = @ID
	END
END