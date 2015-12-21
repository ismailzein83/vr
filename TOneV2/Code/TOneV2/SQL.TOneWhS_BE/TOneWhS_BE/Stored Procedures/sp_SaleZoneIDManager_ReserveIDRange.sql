create PROCEDURE [TOneWhS_BE].[sp_SaleZoneIDManager_ReserveIDRange]
	@NumberOfIDs int
AS
BEGIN

	INSERT INTO TOneWhS_BE.SaleZoneIDManager WITH (TABLOCK) ([LastTakenID]) 
	SELECT 0 WHERE NOT EXISTS (SELECT TOP 1 NULL FROM TOneWhS_BE.SaleZoneIDManager)

	DECLARE @StartingID bigint
	UPDATE TOneWhS_BE.SaleZoneIDManager 
	SET @StartingID = LastTakenID + 1,
		LastTakenID =  LastTakenID + @NumberOfIDs	
	SELECT @StartingID
	
END