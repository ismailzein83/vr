CREATE FUNCTION dbo.GetSaleCodeGaps(@CustomerID varchar(10), @when datetime = NULL)
RETURNS @results TABLE 
	(
		CustomerID varchar(10), 
		ParentCode varchar(15),
		ParentCodeID bigint,
		ParentZone nvarchar(255), 
		ParentZoneID int,
		ParentRate numeric(13,5),
		ParentRateID bigint,
		Code varchar(15),
		CodeID bigint,
		Zone nvarchar(255),
		ZoneID int
	)
AS
BEGIN
	
	INSERT INTO @results
	SELECT 
			pp.CustomerID, 
			pc.Code AS ParentCode, 
			pc.ID AS ParentCodeID,
			pz.Name AS ParentZone,
			pz.ZoneID AS ParentZoneID,
			pr.Rate AS ParentRate, 
			pr.RateID AS ParentRateID,
			c.Code as Code,
			c.ID AS CodeID, 
			z.Name AS Zone,
			z.ZoneID AS ZoneID
	  FROM PriceList pp, Rate pr, Zone pz, Code pc, Code c, Zone z
	WHERE 
			pp.CustomerID = @CustomerID
		AND pp.SupplierID = 'SYS'
		AND pp.PriceListID = pr.PriceListID
		AND pr.BeginEffectiveDate <= @when AND (pr.EndEffectiveDate IS NULL OR pr.EndEffectiveDate > @when)
		AND pr.ZoneID = pz.ZoneID
		AND	pc.ZoneID = pz.ZoneID
		AND pz.BeginEffectiveDate <= @when AND (pz.EndEffectiveDate IS NULL OR pz.EndEffectiveDate > @when)
		AND pc.BeginEffectiveDate <= @when AND (pc.EndEffectiveDate IS NULL OR pc.EndEffectiveDate > @when)
		AND pz.CodeGroup = z.CodeGroup
		AND c.BeginEffectiveDate <= @when AND (c.EndEffectiveDate IS NULL OR c.EndEffectiveDate > @when)
		AND c.Code LIKE (pc.Code + '%')
		AND c.Code > pc.Code
		AND z.ZoneID = c.ZoneID
		AND z.SupplierID = 'SYS'
		AND z.BeginEffectiveDate <= @when AND (z.EndEffectiveDate IS NULL OR z.EndEffectiveDate > @when)
		AND 
			NOT EXISTS(
					SELECT * FROM Rate r, PriceList pl 
						WHERE pl.CustomerID = @CustomerID
							AND pl.SupplierID = 'SYS' 
							AND r.PriceListID = pl.PriceListID
							AND r.BeginEffectiveDate <= @when AND (r.EndEffectiveDate IS NULL OR r.EndEffectiveDate > @when)
							AND r.ZoneID = z.ZoneID
			)
	RETURN
END