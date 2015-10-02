

CREATE PROCEDURE [Analytics].[sp_Rates_GetRates]
	@CarrierType VARCHAR(10),
	@EffectiveOn DATETIME,
	@CodeGroup NVARCHAR(MAX) =NULL,
	@Code VARCHAR(20) = NULL,
	@Zone NVARCHAR(255) = NULL,
	@Carrier VARCHAR(10),
	@From INT = 1,
	@To INT = 10
AS
BEGIN
	SET NOCOUNT ON;
	
	IF(@CarrierType = 'Supplier')
	BEGIN
	
		   ;WITH 
                     MyZone AS 
                     (
	                    SELECT  z.Zoneid,z.Name,z.CodeGroup 
	                    FROM Zone z with(nolock)
	                    WHERE z.SupplierID=@Carrier
                    )
                        ,Mycode As(
                            SELECT c.*,mz.Name,mz.CodeGroup
                            FROM code c with(nolock) inner join MyZone mz on c.zoneID=mz.zoneID
                            WHERE c.BeginEffectiveDate <= @EffectiveON  AND 
                            (c.EndEffectiveDate IS NULL OR c.EndEffectiveDate > c.BeginEffectiveDate )
                        )
                     ,MyPricelist AS 
                     (
	                    SELECT p.SupplierID,p.PriceListId,p.BeginEffectiveDate,p.CurrencyID 
	                    FROM PriceList p with(nolock)
	                    WHERE p.CustomerID='SYS' and p.SupplierID=@Carrier
                    ) 
                    ,MyRate AS 
                    (
	                    SELECT r.*,p.BeginEffectiveDate AS PriceListBeginEffectiveDate ,p.CurrencyID
                        FROM rate r with(nolock)
                        inner join MyPricelist p on p.PriceListID=r.PriceListID 
	                    WHERE r.BeginEffectiveDate <= @EffectiveON  AND 
	                    (r.EndEffectiveDate IS NULL OR r.EndEffectiveDate >@EffectiveON)
                     )
                SELECT
                c.ZoneID,c.CodeGroup,c.Name as ZoneName,c.Code,r.RateID,r.ServicesFlag,r.Rate,r.CurrencyID,r.OffPeakRate,r.WeekendRate,r.Change,
                r.BeginEffectiveDate AS RateBeginEffectiveDate
                ,r.EndEffectiveDate AS RateEndEffectiveDate
                ,c.BeginEffectiveDate AS CodeBeginEffectiveDate
                ,c.EndEffectiveDate AS CodeEndEffectiveDate
                ,r.PriceListID,cu.LastRate,cu.IsMainCurrency,r.PriceListBeginEffectiveDate,u.ID as userID,u.Name as UserName ,ROW_NUMBER() OVER (PARTITION BY r.zoneID order by r.zoneID, c.Code) AS RN
			    INTO #result1
                FROM Mycode c 
                    INNER JOIN MyRate r with(nolock) on c.ZoneID=r.ZoneID
                    INNER JOIN Currency cu with(nolock) on cu.CurrencyID=r.CurrencyID
                    INNER JOIN [User] u with(nolock) on r.UserID= u.ID
                    WHERE 
                        c.BeginEffectiveDate <= @EffectiveON AND 
                     (c.EndEffectiveDate IS NULL OR c.EndEffectiveDate >@EffectiveON)
   
               SELECT r.*,ROW_NUMBER() OVER ( ORDER BY ZoneNAme ) AS RowNumber INTO #result2  FROM #result1 r WHERE 1=1    
                AND (@CodeGroup IS NULL OR CodeGroup collate SQL_Latin1_General_CP1256_CI_AS IN (SELECT ParsedString FROM dbo.ParseStringList(@CodeGroup))) 
                AND (@Code IS NULL OR Code LIKE ''+@Code+'%')
                AND (@Zone IS NULL OR ZoneName LIKE '%'+@Zone+'%')
                AND  RN=1  
               --SELECT COUNT(1) FROM #result2 WHERE 1=1   
               SELECT * from #result2 WHERE RowNumber between @FROM and @TO ORDER BY RowNumber
	
	END
	
	
	
	IF(@CarrierType = 'Customer')
	BEGIN
	
		;WITH 
                     MyZone AS 
                     (
	                    SELECT  z.Zoneid,z.Name,z.CodeGroup 
	                    FROM Zone z with(nolock)
	                    WHERE z.SupplierID='SYS'
                    )
                        ,Mycode As(
                            SELECT c.*,mz.Name,mz.CodeGroup
                            FROM code c with(nolock) inner join MyZone mz on c.zoneID=mz.zoneID
                            WHERE c.BeginEffectiveDate <= @EffectiveON  AND 
                            (c.EndEffectiveDate IS NULL OR c.EndEffectiveDate > c.BeginEffectiveDate )
                        )
                     ,MyPricelist AS 
                     (
	                    SELECT p.SupplierID,p.PriceListId,p.BeginEffectiveDate,p.CurrencyID 
	                    FROM PriceList p with(nolock)
	                    WHERE p.CustomerID= @Carrier and p.SupplierID='SYS'
                    ) 
                    ,MyRate AS 
                    (
	                    SELECT r.*,p.BeginEffectiveDate AS PriceListBeginEffectiveDate ,p.CurrencyID
                        FROM rate r with(nolock)
                        inner join MyPricelist p on p.PriceListID=r.PriceListID 
	                    WHERE r.BeginEffectiveDate <= @EffectiveON  AND 
	                    (r.EndEffectiveDate IS NULL OR r.EndEffectiveDate >@EffectiveON)
                     )
                SELECT
                c.ZoneID,c.CodeGroup,c.Name as ZoneName,c.Code,r.RateID,r.ServicesFlag,r.Rate,r.CurrencyID,r.OffPeakRate,r.WeekendRate,r.Change,
                r.BeginEffectiveDate AS RateBeginEffectiveDate
                ,r.EndEffectiveDate AS RateEndEffectiveDate
                ,c.BeginEffectiveDate AS CodeBeginEffectiveDate
                ,c.EndEffectiveDate AS CodeEndEffectiveDate
                ,r.PriceListID,cu.LastRate,cu.IsMainCurrency,r.PriceListBeginEffectiveDate,u.ID as userID,u.Name as UserName ,ROW_NUMBER() OVER (PARTITION BY r.zoneID order by r.zoneID, c.Code) AS RN
			    INTO #result11
                FROM Mycode c 
                    INNER JOIN MyRate r with(nolock) on c.ZoneID=r.ZoneID
                    INNER JOIN Currency cu with(nolock) on cu.CurrencyID=r.CurrencyID
                    INNER JOIN [User] u with(nolock) on r.UserID= u.ID
                    WHERE 
                        c.BeginEffectiveDate <= @EffectiveON AND 
                     (c.EndEffectiveDate IS NULL OR c.EndEffectiveDate >@EffectiveON)
   
               SELECT r.*,ROW_NUMBER() OVER ( ORDER BY ZoneNAme ) AS RowNumber INTO #result21  FROM #result11 r WHERE 1=1    
                 AND (@CodeGroup IS NULL OR CodeGroup collate SQL_Latin1_General_CP1256_CI_AS IN (SELECT ParsedString FROM dbo.ParseStringList(@CodeGroup)))  
                 AND (@Code IS NULL OR Code LIKE ''+@Code+'%')
                 AND (@Zone IS NULL OR ZoneName LIKE '%'+@Zone+'%')
                 AND  RN=1  
               --SELECT COUNT(1) FROM #result21 WHERE 1=1   
               
               SELECT	t.*
               from		#result21 t
               WHERE RowNumber between @FROM and @TO ORDER BY RowNumber
               
	END
END