
CREATE PROCEDURE [dbo].[bp_RT_Full_FixUnsoldZones]
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

	
	CREATE TABLE #UnsoldZones (CustomerID nVARCHAR(10), OurZoneID INT, CodeGroup nVARCHAR(10), Rate REAL, ServicesFlag smallint,ProfileId INT, PRIMARY KEY(CustomerID, OurZoneID));

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
					From ZoneRates zr  WITH(NOLOCK) Inner Join Zone z  WITH(NOLOCK) on  zr.ZoneID=z.ZoneID
					Where  (Z.CodeGroup = '1')
												)
	)
	INSERT INTO #UnsoldZones With(Tablock)
	(CustomerID, OurZoneID, CodeGroup, Rate, ServicesFlag,ProfileId)
	Select z.CustomerID, z.OurZoneID, z.CodeGroup, z.Rate, z.ServicesFlag,z.ProfileId 
		From CusZOne z
		left Join ZoneRates zr on  z.OurZoneID =zr.ZoneID  And z.CustomerID = zr.CustomerID 
		WHERE zr.ZoneID is null and z.CodeGroup<>'1' ;



	With CusZOne As (

			SELECT DISTINCT ca.CarrierAccountID CustomerID, z.ZoneID OurZoneID, z.CodeGroup CodeGroup, -1 Rate, ca.ServicesFlag ServicesFlag,ca.ProfileId ProfileId
			  FROM Zone z WITH(NOLOCK) , CarrierAccount ca  WITH(NOLOCK) 
				Where z.SupplierID = 'SYS'
				And z.IsEffective = 'Y' AND ca.CarrierAccountID <> 'SYS' 
				And ( Z.CodeGroup Like '7%' )
				And	ca.ActivationStatus <> @Account_Inactive
				AND ca.RoutingStatus <> @Account_Blocked
				AND ca.RoutingStatus <> @Account_BlockedInbound
				And 	ca.CarrierAccountID in (
					Select zr.CustomerID	
					From ZoneRates zr Inner Join Zone z on  zr.ZoneID=z.ZoneID
					Where  (Z.CodeGroup = '7')
												)
	)
	INSERT INTO #UnsoldZones With(Tablock) 
	(CustomerID, OurZoneID, CodeGroup, Rate, ServicesFlag,ProfileId)
	Select z.CustomerID, z.OurZoneID, z.CodeGroup, z.Rate, z.ServicesFlag,z.ProfileId
		From CusZOne z
		left Join ZoneRates zr on  z.OurZoneID =zr.ZoneID  And z.CustomerID = zr.CustomerID
		
		WHERE zr.ZoneID is null and z.CodeGroup<>'7'


------------------- End Added By Sari ------------------------
	
	INSERT INTO ZoneRates With(Tablock)
	(
		ZoneID,
		SupplierID,
		CustomerID,
		ServicesFlag,
		ProfileId,
		ActiveRate,
		IsTOD, 
		IsBlock,
		CodeGroup
	)
	SELECT 
		uz.OurZoneID, 
		'SYS', 
		uz.CustomerID,  
		uz.ServicesFlag,
		ProfileId,
		-1,0,0,uz.CodeGroup
	FROM #UnsoldZones uz WITH(NOLOCK) 


	DROP TABLE #UnsoldZones

END