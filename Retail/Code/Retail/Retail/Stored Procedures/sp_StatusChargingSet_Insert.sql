﻿create PROCEDURE [Retail].[sp_StatusChargingSet_Insert]
	@Name NVARCHAR(255),
    @Settings nvarchar(MAX)
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM Retail_BE.StatusDefinition WHERE Name = @Name )
	BEGIN
	INSERT INTO Retail_BE.StatusChargingSet (Name,Settings,CreatedTime)
	VALUES (@Name,@Settings,GETDATE())
	END
END