CREATE PROCEDURE [dbo].[bp_IdentifyErrors] 
	@CodeZoneErrors char(1) = 'Y',
	@RateZoneErrors char(1) = 'Y',
	@CodeCodeErrors char(1) = 'Y',
	@RateRateErrors char(1) = 'Y',
	@CodeCodeGroupErrors char(1) = 'Y',
	@FromDate DATETIME = '2011-01-01'
AS
BEGIN
SET NOCOUNT ON;

-- Code / Zone Errors
IF @CodeZoneErrors = 'Y'
BEGIN
	WITH 
	tempCode AS(
	SELECT   
		 C.ZoneID AS ZoneID,
		 C.Code AS Code, 
		 C.BeginEffectiveDate AS BeginEffectiveDate,
		 C.EndEffectiveDate AS EndEffectiveDate, 
		 C.IsEffective AS IsEffective
	FROM Code AS C WITH(NOLOCK)
	WHERE (BeginEffectiveDate != EndEffectiveDate AND EndEffectiveDate >=@FromDate)
	   OR EndEffectiveDate IS NULL
	),
	tempZone AS(
	SELECT  
		 Z.ZoneID AS ZoneID,
		 Z.CodeGroup AS CodeGroup,
		 Z.Name AS Name,
		 Z.SupplierID AS SupplierID,
		 Z.BeginEffectiveDate AS BeginEffectiveDate,
		 Z.EndEffectiveDate AS EndEffectiveDate,
		 Z.IsEffective AS IsEffective
	FROM dbo.Zone Z WITH(NOLOCK)
	WHERE --(BeginEffectiveDate != EndEffectiveDate AND EndEffectiveDate >= @FromDate)
	      EndEffectiveDate >= @FromDate
       OR EndEffectiveDate IS NULL
	)
	SELECT  tz.CodeGroup,
			tz.Name,
			tz.SupplierID AS SupplierID,
			'' AS Supplier,
			tz.BeginEffectiveDate AS BeginEffectiveDate#Z,
			tz.EndEffectiveDate AS EndEffectiveDate#Z,
			tz.IsEffective AS Eff_#Z,
			tc.Code, 
			tc.BeginEffectiveDate AS BeginEffectiveDate#C,
			tc.EndEffectiveDate AS EndEffectiveDate#C,
			tc.IsEffective AS Eff_#C
	FROM tempCode tc INNER JOIN tempZone tz ON tc.ZoneID = tz.ZoneID
	WHERE     
	   (tc.BeginEffectiveDate < tz.BeginEffectiveDate 
			 OR
			 (  tz.EndEffectiveDate IS NOT NULL 
			  AND 
				(
				  tc.EndEffectiveDate IS NULL
				  OR
				  tc.EndEffectiveDate > tz.EndEffectiveDate
				  OR 
				  tc.BeginEffectiveDate > tz.EndEffectiveDate			  
				)
			 )
		 ) 
END
    
-- Rate / Zone Errors
IF @RateZoneErrors = 'Y'
BEGIN
    WITH 
    tempRate AS(
	SELECT  PriceListID,
			ZoneID,	
			Rate,
			OffPeakRate , 
			WeekendRate ,
			BeginEffectiveDate AS BeginEffectiveDate,
			EndEffectiveDate AS EndEffectiveDate,
			IsEffective AS IsEffective
	FROM Rate WITH(NOLOCK)
	WHERE (BeginEffectiveDate != EndEffectiveDate AND EndEffectiveDate >=@FromDate)
	   OR EndEffectiveDate IS NULL
	),
	tempZone AS(
	SELECT  ZoneID,
			CodeGroup, 
			Name, 
			SupplierID,
			BeginEffectiveDate, 
			EndEffectiveDate, 
			IsEffective
	FROM Zone WITH(NOLOCK)
	WHERE --(BeginEffectiveDate!=EndEffectiveDate AND EndEffectiveDate >=@FromDate)
	       EndEffectiveDate >=@FromDate
	    OR EndEffectiveDate IS NULL
	)
	SELECT     
		R.PriceListID, 
		R.Rate, 
		R.OffPeakRate AS O_P_Rate, 
		R.WeekendRate AS W_E_Rate, 
		R.BeginEffectiveDate AS BeginEffectiveDate#R, 
		R.EndEffectiveDate AS EndEffectiveDate#R, 
		R.IsEffective AS Eff_#R, 
		Z.CodeGroup, 
		Z.Name, 
		Z.SupplierID AS SupplierID,
		'' AS Supplier,
		Z.BeginEffectiveDate AS BeginEffectiveDate#Z, 
		Z.EndEffectiveDate AS EndEffectiveDate#Z, 
		Z.IsEffective AS Eff_#Z
	FROM         
		tempRate  R INNER JOIN tempZone Z ON Z.ZoneID = R.ZoneID 
	WHERE
		(R.BeginEffectiveDate < Z.BeginEffectiveDate 
		OR
		(  Z.EndEffectiveDate IS NOT NULL 
		AND 
		(
		  R.EndEffectiveDate IS NULL
		  OR
		  R.BeginEffectiveDate > Z.EndEffectiveDate
		  OR 
		  R.EndEffectiveDate > Z.EndEffectiveDate
		)
		)
		)
END

-- Code / Code Errors
IF @CodeCodeErrors = 'Y'
BEGIN
	WITH 
	tempCode AS(
	SELECT ID,
		   Code, 
		   ZoneID,	
		   BeginEffectiveDate, 
		   EndEffectiveDate
	FROM   Code WITH(NOLOCK)
	WHERE  (BeginEffectiveDate != EndEffectiveDate AND EndEffectiveDate >=@FromDate)
		OR EndEffectiveDate IS NULL
	),
	tempZone AS(
	SELECT ZoneID,
		   Name,
		   SupplierID,
		   BeginEffectiveDate , 
		   EndEffectiveDate
	FROM   Zone WITH(NOLOCK)
	WHERE  (BeginEffectiveDate != EndEffectiveDate AND EndEffectiveDate >=@FromDate)
	    OR EndEffectiveDate IS NULL
	),
	tempZoneCode AS(
	SELECT c.Code, 
		   c.ID, 
		   z.SupplierID,
   		   z.Name,
		   c.BeginEffectiveDate, 
		   c.EndEffectiveDate
	FROM   tempCode c INNER JOIN tempZone z ON z.ZoneID = c.ZoneID
	),
	tempForResult AS(
	SELECT 	zc1.Code,
			zc1.SupplierID,
			zc1.ID AS ID1,
			zc1.Name AS Name1,
			zc1.BeginEffectiveDate AS BED1,
			zc1.EndEffectiveDate AS EED1,
			zc2.ID AS ID2, 
			zc2.Name AS Name2,
			zc2.BeginEffectiveDate AS BED2, 
			zc2.EndEffectiveDate AS EED2
	FROM tempZoneCode zc1 INNER JOIN tempZoneCode zc2 ON zc1.SupplierID = zc2.SupplierID 
	WHERE zc1.code = zc2.code AND zc1.ID!=zc2.ID
	)
	SELECT t.Code,
		   t.SupplierID,
		   '' Supplier, 
		   t.ID1 ID1, 
		   t.Name1 ZoneName1,
		   t.BED1 BeginEffectiveDate1, 
		   t.EED1 EndEffectiveDate1, 
		   t.ID2 ID2, 
		   t.Name2 ZoneName2,
		   t.BED2 BeginEffectiveDate2, 
		   t.EED2 EndEffectiveDate2
	FROM  tempForResult t
	WHERE (t.EED1 IS NULL AND ( t.EED2 IS NULL OR t.EED2 > t.BED1))
	   OR (t.BED1 < t.EED2 AND t.EED1 > t.BED2)
END

-- Rate / Rate Errors
IF @RateRateErrors = 'Y'
BEGIN 
	WITH 
	tempZone AS(
	SELECT ZoneID,
		   Name
	FROM Zone WITH(NOLOCK)
	WHERE BeginEffectiveDate != EndEffectiveDate
	OR EndEffectiveDate IS NULL
	),
	tempRate AS(
	SELECT  r.ZoneID,
			z.Name,
			r.PriceListID, 
			r.RateID, 
			r.Rate,
			r.BeginEffectiveDate,
			r.EndEffectiveDate
	FROM  Rate r WITH(NOLOCK) INNER JOIN tempZone z ON r.ZoneID = z.ZoneID
	WHERE (BeginEffectiveDate != EndEffectiveDate AND EndEffectiveDate >= @Fromdate)
	   OR EndEffectiveDate IS NULL
	)
	,tempPriceList AS(
	SELECT  PriceListID,
			SupplierID, 
			CustomerID
	FROM PriceList WITH(NOLOCK)
	WHERE (BeginEffectiveDate != EndEffectiveDate AND EndEffectiveDate >= @FromDate)
	OR EndEffectiveDate IS NULL
	),
	tempRatePriceList AS(
	SELECT r.ZoneID,
		   r.Name,
		   r.PriceListID, 
		   r.RateID, 
		   r.Rate, 
		   r.BeginEffectiveDate,
		   r.EndEffectiveDate,
		   p.SupplierID, 
		   p.CustomerID
	FROM tempRate r INNER JOIN tempPriceList p ON r.PriceListID = p.PriceListID
	)
	SELECT t1.SupplierID, 
		   '' Supplier, 
		   t1.CustomerID,
		   '' Customer, 
		   t1.ZoneID, 
		   t1.Name ZoneName,
		   t1.RateID RateID1, 
		   t1.Rate Rate1, 
		   t1.BeginEffectiveDate BeginEffectiveDate1,
		   t1.EndEffectiveDate EndEffectiveDate1, 
		   t2.RateID RateID2, 
		   t2.Rate Rate2, 
		   t2.BeginEffectiveDate BeginEffectiveDate2,
		   t2.EndEffectiveDate EndEffectiveDate2 
	FROM tempRatePriceList t1 INNER JOIN tempRatePriceList t2 ON (t1.ZoneID = t2.ZoneID
															AND t1.SupplierID = t2.SupplierID
															AND t1.CustomerID = t2.CustomerID
															AND t1.RateID <> t2.RateID)
	WHERE (t1.EndEffectiveDate IS NULL AND ( t2.EndEffectiveDate IS NULL OR t2.EndEffectiveDate > t1.BeginEffectiveDate))
	     OR (t1.BeginEffectiveDate < t2.EndEffectiveDate AND t1.EndEffectiveDate > t2.BeginEffectiveDate)
END

--Code / CodeGroup Errors
IF @CodeCodeGroupErrors = 'Y'
BEGIN
    
	SELECT Code,c.ZoneID,CodeGroup
	FROM   Code c WITH(NOLOCK) INNER JOIN Zone z WITH(NOLOCK) ON c.ZoneID = z.ZoneID
	WHERE  CodeGroup IS NULL 
	   OR  DATALENGTH(Code)<DATALENGTH(CodeGroup)
	   OR  CodeGroup!= SUBSTRING(Code,1,DATALENGTH(CodeGroup))
	
	
END
END