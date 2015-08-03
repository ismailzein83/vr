CREATE PROCEDURE [dbo].[bp_FixUnsoldZonesForRouteBuild]
AS
BEGIN

	DECLARE @Account_Inactive tinyint
	DECLARE @Account_Blocked tinyint
	DECLARE @Account_BlockedInbound tinyint
	DECLARE @Account_BlockedOutbound tinyint
	
	-- Set Enumerations
	SELECT @Account_Inactive = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.ActivationStatus' AND Name = 'Inactive'
	SELECT @Account_Blocked = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'Blocked'
	SELECT @Account_BlockedInbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedInbound'	
	SELECT @Account_BlockedOutbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedOutbound'	

	
	CREATE TABLE #UnsoldZones (CustomerID nVARCHAR(10), OurZoneID INT, CodeGroup nVARCHAR(10), Rate REAL, ServicesFlag smallint,ProfileId int, PRIMARY KEY(CustomerID, OurZoneID));

------------------- Begin Added By Sari ------------------------
	With CusZOne As (

			SELECT DISTINCT ca.CarrierAccountID CustomerID, z.ZoneID OurZoneID, z.CodeGroup CodeGroup, -1 Rate, ca.ServicesFlag ServicesFlag,ca.ProfileId ProfileId
			  FROM Zone z, CarrierAccount ca 
				Where z.SupplierID = 'SYS'
				And z.IsEffective = 'Y' AND ca.CarrierAccountID <> 'SYS' 
				And ( Z.CodeGroup Like '1%' )
				And	ca.ActivationStatus <> @Account_Inactive
				AND ca.RoutingStatus <> @Account_Blocked
				AND ca.RoutingStatus <> @Account_BlockedInbound
				And 	ca.CarrierAccountID in (
					Select zr.CustomerID	
					From Zonerate zr Inner Join Zone z on  zr.ZoneID=z.ZoneID
					Where  (Z.CodeGroup = '1')
												)
	)
	INSERT INTO #UnsoldZones (CustomerID, OurZoneID, CodeGroup, Rate, ServicesFlag,ProfileId)
	Select z.CustomerID, z.OurZoneID, z.CodeGroup, z.Rate, z.ServicesFlag,z.ProfileId 
		From CusZOne z
		left Join Zonerate zr on  z.OurZoneID =zr.ZoneID  And z.CustomerID = zr.CustomerID 
		WHERE zr.ZoneID is null and z.CodeGroup<>'1' ;



	With CusZOne As (

			SELECT DISTINCT ca.CarrierAccountID CustomerID, z.ZoneID OurZoneID, z.CodeGroup CodeGroup, -1 Rate, ca.ServicesFlag ServicesFlag,ca.ProfileId ProfileId
			  FROM Zone z, CarrierAccount ca 
				Where z.SupplierID = 'SYS'
				And z.IsEffective = 'Y' AND ca.CarrierAccountID <> 'SYS' 
				And ( Z.CodeGroup Like '7%' )
				And	ca.ActivationStatus <> @Account_Inactive
				AND ca.RoutingStatus <> @Account_Blocked
				AND ca.RoutingStatus <> @Account_BlockedInbound
				And 	ca.CarrierAccountID in (
					Select zr.CustomerID	
					From Zonerate zr Inner Join Zone z on  zr.ZoneID=z.ZoneID
					Where  (Z.CodeGroup = '7')
												)
	)
	INSERT INTO #UnsoldZones (CustomerID, OurZoneID, CodeGroup, Rate, ServicesFlag,ProfileId)
	Select z.CustomerID, z.OurZoneID, z.CodeGroup, z.Rate, z.ServicesFlag,z.ProfileId 
		From CusZOne z
		left Join Zonerate zr on  z.OurZoneID =zr.ZoneID  And z.CustomerID = zr.CustomerID 
		WHERE zr.ZoneID is null and z.CodeGroup<>'7'


------------------- End Added By Sari ------------------------


------------------- Begin Stopped By Sari ------------------------
--	INSERT INTO #UnsoldZones (CustomerID, OurZoneID, CodeGroup, Rate, ServicesFlag)
--		SELECT DISTINCT ca.CarrierAccountID, z.ZoneID, z.CodeGroup, -1, ca.ServicesFlag
--		  FROM Zone Z, CarrierAccount ca WHERE
--			z.SupplierID = 'SYS'
--			AND z.IsEffective = 'Y' 
--			AND NOT EXISTS (SELECT * FROM ZoneRate zr WHERE zr.ZoneID = z.ZoneID AND ca.CarrierAccountID = zr.CustomerID AND zr.SupplierID = 'SYS')
--			AND EXISTS (SELECT * FROM ZoneRate zr, Zone oz WHERE zr.ZoneID = oz.ZoneID AND oz.CodeGroup = z.CodeGroup AND ca.CarrierAccountID = zr.CustomerID AND zr.SupplierID = 'SYS')
------------------- End Stopped By Sari ------------------------
		
	/*
	UPDATE #UnsoldZones SET 
		MinRate = -1, 
		MaxServicesFlag = 
		(
			SELECT MAX(zr.ServicesFlag) 
				FROM ZoneRate zr, Zone z 
				WHERE 
						z.SupplierID = 'SYS' 
					AND z.CodeGroup = #UnsoldZones.CodeGroup COLLATE SQL_Latin1_General_CP1256_CI_AS 
					AND z.ZoneID = zr.ZoneID
					AND zr.CustomerID = #UnsoldZones.CustomerID COLLATE SQL_Latin1_General_CP1256_CI_AS
		)				
	*/
	
	INSERT INTO ZoneRate
	(
		ZoneID,
		SupplierID,
		CustomerID,
		NormalRate,
		OffPeakRate,
		WeekendRate,
		ServicesFlag,
		ProfileId
	)
	SELECT 
		uz.OurZoneID, 
		'SYS', 
		uz.CustomerID, 
		uz.Rate, 
		uz.Rate, 
		uz.Rate, 
		uz.ServicesFlag,
		ProfileId
	FROM #UnsoldZones uz

	--SELECT * FROM #UnsoldZones

	DROP TABLE #UnsoldZones

END