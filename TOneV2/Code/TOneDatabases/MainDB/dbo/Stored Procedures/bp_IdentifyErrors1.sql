CREATE PROCEDURE [dbo].[bp_IdentifyErrors1] 
	@CodeZoneErrors char(1) = 'Y',
	@RateZoneErrors char(1) = 'Y',
	@CodeCodeErrors char(1) = 'Y',
	@RateRateErrors char(1) = 'Y'
AS
BEGIN
SET NOCOUNT ON;

Declare @MaxDate datetime
Set @MaxDate = '2020-01-01'

-- Code / Zone Errors
IF @CodeZoneErrors = 'Y'
      SELECT   
         Z.CodeGroup, Z.Name, CarrierProfile.Name AS Supplier, Z.BeginEffectiveDate AS BeginEffectiveDate#Z,
         Z.EndEffectiveDate AS EndEffectiveDate#Z,Z.IsEffective AS IsEffective#Z, C.Code, 
         C.BeginEffectiveDate AS BeginEffectiveDate#C, C.EndEffectiveDate AS EndEffectiveDate#C, C.IsEffective AS IsEffective#C
      FROM        
         Zone AS Z INNER JOIN
         Code AS C ON Z.ZoneID = C.ZoneID INNER JOIN
         CarrierAccount ON Z.SupplierID = CarrierAccount.CarrierAccountID INNER JOIN
         CarrierProfile ON CarrierAccount.ProfileID = CarrierProfile.ProfileID
      WHERE     
         (NOT (C.BeginEffectiveDate BETWEEN Z.BeginEffectiveDate AND ISNULL(Z.EndEffectiveDate, @MaxDate)))
          OR
         (NOT (ISNULL(C.EndEffectiveDate, @MaxDate) BETWEEN Z.BeginEffectiveDate AND ISNULL(Z.EndEffectiveDate, @MaxDate)))

-- Rate / Zone Errors
IF @RateZoneErrors = 'Y'
      SELECT     
          R.PriceListID, R.Rate, R.OffPeakRate, R.WeekendRate, R.Change, 
          R.BeginEffectiveDate AS BeginEffectiveDate#R, 
          R.EndEffectiveDate AS EndEffectiveDate#R, 
          R.IsEffective AS IsEffective#R, Z.CodeGroup, Z.Name, CarrierProfile.Name AS Supplier,
          Z.IsMobile, Z.IsProper, Z.IsSold, Z.BeginEffectiveDate AS BeginEffectiveDate#Z, 
          Z.EndEffectiveDate AS EndEffectiveDate#Z, Z.IsEffective AS IsEffective#Z
       FROM         
          Rate AS R INNER JOIN
          Zone AS Z ON R.ZoneID = Z.ZoneID INNER JOIN
          CarrierAccount ON Z.SupplierID = CarrierAccount.CarrierAccountID INNER JOIN
          CarrierProfile ON CarrierAccount.ProfileID = CarrierProfile.ProfileID
	    WHERE
		-- Test Begin 
		   NOT (R.BeginEffectiveDate BETWEEN Z.BeginEffectiveDate AND ISNULL(Z.EndEffectiveDate, @MaxDate))
		   OR
		-- Test End
		   NOT (ISNULL(R.EndEffectiveDate,@MaxDate) BETWEEN Z.BeginEffectiveDate AND ISNULL(Z.EndEffectiveDate, @MaxDate))

-- Code / Code Errors
IF @CodeCodeErrors = 'Y'
     SELECT     
       C1.Code, CarrierProfile.Name AS Supplier, C1.ID, Z1.Name, 
       C1.BeginEffectiveDate, C1.EndEffectiveDate, C2.ID, Z2.Name , 
       C2.BeginEffectiveDate , C2.EndEffectiveDate 
     FROM        
        Code AS C1 INNER JOIN
        Zone AS Z1 ON C1.ZoneID = Z1.ZoneID INNER JOIN
        Zone AS Z2 ON Z1.SupplierID = Z2.SupplierID INNER JOIN
        Code AS C2 ON Z2.ZoneID = C2.ZoneID AND C1.Code = C2.Code INNER JOIN
        CarrierAccount ON Z1.SupplierID = CarrierAccount.CarrierAccountID INNER JOIN
        CarrierProfile ON CarrierAccount.ProfileID = CarrierProfile.ProfileID
	WHERE 
			C1.ID <> C2.ID
		AND C2.BeginEffectiveDate >= C1.BeginEffectiveDate 
		AND C2.BeginEffectiveDate < ISNULL(C1.EndEffectiveDate, @MaxDate)
	ORDER BY Z1.SupplierID, C1.Code, C1.BeginEffectiveDate


-- Rate / Rate Errors
IF @RateRateErrors = 'Y'
          SELECT     
              CarrierProfile_1.Name AS Supplier, CarrierProfile.Name AS Customer, R1.ZoneID, Z.Name,
              R1.RateID, R1.Rate, R1.BeginEffectiveDate,R1.EndEffectiveDate, R2.RateID ,
              R2.Rate , R2.BeginEffectiveDate , R2.EndEffectiveDate 
           FROM         
              Rate AS R1 INNER JOIN
              PriceList AS P1 ON P1.PriceListID = R1.PriceListID INNER JOIN
              PriceList AS P2 ON P2.SupplierID = P1.SupplierID AND P2.CustomerID = P1.CustomerID AND P1.PriceListID <> P2.PriceListID INNER JOIN
              Rate AS R2 ON R1.ZoneID = R2.ZoneID AND R2.PriceListID = P2.PriceListID INNER JOIN
              Zone AS Z ON R1.ZoneID = Z.ZoneID INNER JOIN
              CarrierAccount AS CarrierAccount_1 ON P1.SupplierID = CarrierAccount_1.CarrierAccountID INNER JOIN
              CarrierAccount ON P1.CustomerID = CarrierAccount.CarrierAccountID INNER JOIN
              CarrierProfile AS CarrierProfile_1 ON CarrierAccount_1.ProfileID = CarrierProfile_1.ProfileID INNER JOIN
              CarrierProfile ON CarrierAccount.ProfileID = CarrierProfile.ProfileID
	       WHERE 1=1
		      AND R2.BeginEffectiveDate >= R1.BeginEffectiveDate 
		      AND R2.BeginEffectiveDate < ISNULL(R1.EndEffectiveDate, @MaxDate)
		      AND R2.BeginEffectiveDate <> R2.EndEffectiveDate
		      AND R1.Rate <> R2.Rate
	       ORDER BY P1.SupplierID, P1.CustomerID, Z.Name, R1.BeginEffectiveDate


END