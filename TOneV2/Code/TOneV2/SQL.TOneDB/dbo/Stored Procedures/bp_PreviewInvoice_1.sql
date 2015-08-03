CREATE PROCEDURE [dbo].[bp_PreviewInvoice]
(
	@CustomerID varchar(5),
	@FromDate Datetime,
	@ToDate Datetime
)
AS
SET NOCOUNT ON 


SET @FromDate=     CAST(
(
STR( YEAR( @FromDate ) ) + '-' +
STR( MONTH( @FromDate ) ) + '-' +
STR( DAY( @FromDate ) ) 
)
AS DATETIME
)

SET @ToDate=     CAST(
(
STR( YEAR( @ToDate ) ) + '-' +
STR( MONTH(@ToDate ) ) + '-' +
STR( DAY( @ToDate ) ) + ' 23:59:59.99'
)
AS DATETIME
)

DECLARE @Pivot SMALLDATETIME
SET @Pivot = @FromDate

CREATE TABLE #temp
(
	
	[NAME] VARCHAR(255),
	MinAttempt DATETIME,
    MaxAttempt DATETIME,
    DurationInMinutes  numeric(13, 4) NULL,
    Rate FLOAT,
    Ratetype [tinyint] NOT NULL  DEFAULT ((0)),
    Amount FLOAT,
	Currency [varchar](3) NOT NULL,
    NumberOfCalls  INT 
)


WHILE @Pivot <= @ToDate
BEGIN
INSERT INTO #Temp
SELECT 
       Z.Name Destination,
       MIN(BS.CallDate) MinAttempt,
       MAX(BS.CallDate) MaxAttempt,
       SUM(BS.SaleDuration)/60.0 DurationInMinutes,
       Round(BS.Sale_Rate,5) Rate,
       BS.Sale_RateType Ratetype,
       ISNULL(SUM(BS.Sale_Nets ),0) Amount,
       BS.Sale_Currency Currency,
       SUM(BS.NumberOfCalls) AS NumberOfCalls
FROM   Billing_Stats BS WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer)) 
LEFT JOIN Zone z WITH(NOLOCK) ON z.ZoneID = bs.SaleZoneID AND z.SupplierID ='SYS'
WHERE  BS.CallDate = @Pivot
  AND  BS.CustomerID = @CustomerID
  AND  BS.Sale_Currency IS NOT NULL
GROUP BY
       Z.[Name],
       Round(BS.Sale_Rate,5),
       BS.Sale_RateType,
       BS.Sale_Currency 
ORDER BY z.[Name] 
SET @Pivot = DATEADD(dd, 1, @Pivot)
END 

SELECT 
       Destination,
       MinAttempt,
       MaxAttempt,
       DurationInMinutes,
       Rate,
       Ratetype,
       Amount,
       Currency,
       NumberOfCalls 
FROM #Temp 
GROUP BY
       Destination,
       Rate,
       RateType,
       Currency