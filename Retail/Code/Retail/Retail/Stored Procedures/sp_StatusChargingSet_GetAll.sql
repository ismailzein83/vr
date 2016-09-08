CREATE PROCEDURE [Retail].[sp_StatusChargingSet_GetAll]
AS
BEGIN
	SELECT ID,Name,Settings
	FROM Retail_BE.StatusChargingSet WITH(NOLOCK) 
END