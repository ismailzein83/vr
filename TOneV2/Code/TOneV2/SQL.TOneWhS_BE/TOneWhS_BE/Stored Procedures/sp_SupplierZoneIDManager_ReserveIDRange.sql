CREATE PROCEDURE TOneWhS_BE.sp_SupplierZoneIDManager_ReserveIDRange
	@NumberOfIDs int
AS
BEGIN

	INSERT INTO TOneWhS_BE.SupplierZoneIDManager WITH (TABLOCK) ([LastTakenID]) 
	SELECT 0 WHERE NOT EXISTS (SELECT TOP 1 NULL FROM TOneWhS_BE.SupplierZoneIDManager)

	DECLARE @StartingID bigint
	UPDATE TOneWhS_BE.SupplierZoneIDManager 
	SET @StartingID = LastTakenID + 1,
		LastTakenID =  LastTakenID + @NumberOfIDs	
	SELECT @StartingID
	
END