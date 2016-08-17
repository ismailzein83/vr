create PROCEDURE [Retail].[sp_StatusChargingSet_Update]
	@ID int,
	@Name NVARCHAR(255),
	@Settings nvarchar(MAX)
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM Retail_BE.StatusChargingSet WHERE ID != @ID and Name = @Name )
	BEGIN
		update Retail_BE.StatusChargingSet 
		set  Name = @Name ,Settings= @Settings
		where  ID = @ID
	END
END