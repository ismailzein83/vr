CREATE PROCEDURE [dbo].[bp_BuildCodeSupply]
	@SupplierID varchar(10) = NULL
AS
BEGIN
	SET NOCOUNT ON

	-- If no supplier or customer given then Empty Code Supply
	IF @SupplierID IS NULL
	  BEGIN
		TRUNCATE TABLE CodeSupply
	
		-- DROP INDEXES
		DROP INDEX IX_CodeSupply_Code ON CodeSupply
		DROP INDEX IX_CodeSupply_Zone ON CodeSupply
		DROP INDEX IX_CodeSupply_Supplier ON CodeSupply
		--DROP INDEX IX_CodeSupply_ServicesFlag ON CodeSupply	

	  END
	ELSE
	  BEGIN
		DELETE FROM CodeSupply WHERE SupplierID = @SupplierID
	  END

	-- Build Code Supply
	INSERT INTO CodeSupply With(Tablock)
	(
		Code,
		SupplierID,
		SupplierZoneID,
		SupplierNormalRate,
		SupplierOffPeakRate,
		SupplierWeekendRate,
		SupplierServicesFlag,
		Profileid
	)
	SELECT
		CM.Code,
		CM.SupplierID,
		CM.SupplierZoneID,
		ZR.NormalRate,
		ZR.OffPeakRate,
		ZR.WeekendRate,		
		ZR.ServicesFlag,
		ZR.ProfileID
	FROM
		CodeMatch CM INNER JOIN ZoneRate ZR ON CM.SupplierZoneID = ZR.ZoneID 
			AND ZR.SupplierID <> 'SYS'
			AND (@SupplierID IS NULL OR ZR.SupplierID = @SupplierID)

	-- If no supplier or customer given then Empty Code Supply
	--IF @SupplierID IS NULL 
	--  BEGIN	
	--	-- Recreate INDEXES
		CREATE NONCLUSTERED INDEX IX_CodeSupply_Code ON CodeSupply(Code ASC)
		CREATE NONCLUSTERED INDEX IX_CodeSupply_Supplier ON CodeSupply(SupplierID ASC)
		CREATE NONCLUSTERED INDEX IX_CodeSupply_Zone ON CodeSupply(SupplierZoneID ASC)
	--	CREATE NONCLUSTERED INDEX IX_CodeSupply_ServicesFlag ON CodeSupply(SupplierServicesFlag ASC)
	--  END

END