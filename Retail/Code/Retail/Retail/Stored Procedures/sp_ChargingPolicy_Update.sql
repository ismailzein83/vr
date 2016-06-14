-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail].[sp_ChargingPolicy_Update]
	@ID INT,
	@Name NVARCHAR(255),
	@Settings NVARCHAR(MAX)
AS
BEGIN
	IF NOT EXISTS (SELECT 1 FROM Retail.ChargingPolicy WHERE Name = @Name AND ID != @ID)
	BEGIN
		UPDATE Retail.ChargingPolicy
		SET Name = @Name, Settings = @Settings
		WHERE ID = @ID
	END
END