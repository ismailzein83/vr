-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail].[sp_ChargingPolicy_Insert]
	@Name NVARCHAR(255),
	@ServiceTypeId INT,
	@Settings NVARCHAR(MAX),
	@ID INT OUT
AS
BEGIN
	IF NOT EXISTS (SELECT 1 FROM Retail.ChargingPolicy WHERE Name = @Name)
	BEGIN
		INSERT INTO Retail.ChargingPolicy (Name, ServiceTypeId, Settings)
		VALUES (@Name, @ServiceTypeId, @Settings)
		SET @ID = SCOPE_IDENTITY()
	END
END