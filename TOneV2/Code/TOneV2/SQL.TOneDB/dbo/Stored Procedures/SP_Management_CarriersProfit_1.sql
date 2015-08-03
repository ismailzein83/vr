
CREATE PROCEDURE [dbo].[SP_Management_CarriersProfit]
(
	@FromDate DATETIME,
	@ToDate DATETIME,
	@Top INT = NULL,
	@GroupByDay CHAR(1) = 'N',
	@TableName NVARCHAR(255),
	@From INT = 1,
	@To INT = 10
)
WITH RECOMPILE 
AS

BEGIN

	DECLARE @TempTableName NVARCHAR(255)
	DECLARE @Sql NVARCHAR(4000)
	DECLARE @Exists BIT
	
	SET @Sql = ''
	SET @TempTableName = 'tempdb.dbo.[' + @TableName + ']'
	SET @Exists = dbo.CheckGlobalTableExists(@TableName)
	
	IF(@Exists = 1 AND @From = 1)
	BEGIN
		DECLARE @DropTable VARCHAR(100)
		SET @DropTable = 'DROP TABLE ' + @TempTableName
		EXEC(@DropTable)
	END
	
	IF(@From = 1)
	BEGIN
	
		DECLARE @ExchangeRates TABLE(
				Currency VARCHAR(3),
				Date SMALLDATETIME,
				Rate FLOAT
				PRIMARY KEY(Currency, Date)
		)
		
		INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)
		
		DECLARE @TopCustomers TABLE ( CarrierAccountID VARCHAR(5), Profit NUMERIC(19,7))
		
		IF(@Top IS NOT NULL)
		BEGIN
			SET ROWCOUNT @Top
			INSERT INTO @TopCustomers
			SELECT	BS.CustomerID AS CarrierAccountID,
					(SUM(BS.Sale_Nets / isnull(ERS.Rate,1)) - SUM(bs.Cost_Nets / ISNULL(ERC.Rate, 1))) AS Profit
			FROM	Billing_Stats BS 
					LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
					LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
			WHERE	BS.CallDate BETWEEN @FromDate AND @ToDate
			GROUP BY BS.CustomerID
			ORDER BY (SUM(BS.Sale_Nets / isnull(ERS.Rate,1)) - SUM(bs.Cost_Nets / ISNULL(ERC.Rate, 1))) DESC
		END
		
		SET ROWCOUNT 0
		
		IF(@GroupByDay = 'Y')
		BEGIN
			SELECT	DATEPART(dd, BS.CallDate) AS [Day],
					DATEPART(mm, BS.CallDate) AS [Month],
					DATEPART(yyyy, BS.CallDate) AS [Year],
					BS.CustomerID AS CustomerID,
					SUM(BS.SaleDuration)/60.0 AS Duration,
					SUM(BS.Sale_Nets / ISNULL(ERS.Rate, 1)) AS SaleAmount,
					SUM(BS.Sale_Nets / ISNULL(ERS.Rate,1)) - SUM(bs.Cost_Nets / ISNULL(ERC.Rate, 1)) AS Profit
			INTO #RESULT1
			FROM	Billing_Stats BS
					LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.[Date] = bs.CallDate
					LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.[Date] = bs.CallDate
			WHERE	@Top IS NULL OR BS.CustomerID IN (SELECT CarrierAccountID FROM @TopCustomers)
					AND BS.CallDate BETWEEN @FromDate AND @ToDate
			GROUP BY	DATEPART(dd, BS.CallDate),
						DATEPART(mm, BS.CallDate),
						DATEPART(yyyy, BS.CallDate),
						BS.CustomerID
			ORDER BY	DATEPART(dd, BS.CallDate),
						DATEPART(mm, BS.CallDate),
						DATEPART(yyyy, BS.CallDate),
						BS.CustomerID
						
			SET @Sql = '
								SELECT * INTO #TEMP FROM #RESULT1
							 '
					
		END
		ELSE
		BEGIN
			SELECT	DATEPART(mm, BS.CallDate) AS [Month],
					DATEPART(yyyy, BS.CallDate) AS [Year],
					BS.CustomerID AS CustomerID,
					SUM(BS.SaleDuration)/60.0 AS Duration,
					SUM(BS.Sale_Nets / ISNULL(ERS.Rate, 1)) AS SaleAmount,
					SUM(BS.Sale_Nets / ISNULL(ERS.Rate,1)) - SUM(bs.Cost_Nets / ISNULL(ERC.Rate, 1)) AS Profit
			INTO	#RESULT2
			FROM	Billing_Stats BS
					LEFT JOIN @ExchangeRates ERC ON ERC.Currency = BS.Cost_Currency AND ERC.Date = BS.CallDate
					LEFT JOIN @ExchangeRates ERS ON ERS.Currency = BS.Sale_Currency AND ERS.Date = BS.CallDate
			WHERE	@Top IS NULL OR BS.CustomerID IN (SELECT CarrierAccountID FROM @TopCustomers)
					AND BS.CallDate BETWEEN @FromDate AND @ToDate
			GROUP BY	DATEPART(mm, BS.CallDate),
						DATEPART(yyyy, BS.CallDate),
						BS.CustomerID
			ORDER BY	DATEPART(mm, BS.CallDate),
						DATEPART(yyyy, BS.CallDate),
						BS.CustomerID
						
			SET @Sql = '
								SELECT * INTO #TEMP FROM #RESULT2
							 '
		END
		
		SET @Sql = @Sql + '
					 DECLARE @ShowNameSuffix nvarchar(1)
					 SET @ShowNameSuffix= (select SP.BooleanValue from SystemParameter SP where Name like ''ShowNameSuffix'')
					 ;WITH
					 Carriers AS
					 (
						 SELECT
							(CASE WHEN @ShowNameSuffix = ''Y'' THEN (CASE WHEN A.NameSuffix != '''' THEN P.Name+''(''+A.NameSuffix+'')'' ELSE P.Name END) ELSE (P.Name) END) AS CarrierName
							,A.CarrierAccountID AS CarrierAccountID
						 FROM CarrierAccount A INNER JOIN CarrierProfile P ON P.ProfileID = A.ProfileID
					 )
					 SELECT C.CarrierName, T.*
					 INTO ' + @TempTableName + '
					 FROM #TEMP T
					 LEFT JOIN Carriers C ON T.CustomerID = C.CarrierAccountID
					 ORDER BY
					 '
		IF(@GroupByDay = 'Y')
		BEGIN
			SET @Sql = @Sql + 'T.[Day],'
		END
		SET @Sql = @Sql + '
					T.Month, T.Year, C.CarrierName'
					 
		
	END
		SET @Sql = @Sql + '
					 SELECT COUNT(1) FROM ' + @tempTableName + '
					 ;WITH FINAL AS 
					 (
						 SELECT *, ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS rowNumber
						 FROM ' + @tempTableName + '
					 )
					 SELECT * FROM FINAL
					 WHERE rowNumber BETWEEN ' + CAST(@From AS VARCHAR) + ' AND ' + CAST(@To AS VARCHAR) 
						
	
	EXECUTE SP_EXECUTESQL @Sql
END