CREATE PROCEDURE [dbo].[bp_BuildBillingStatsPeriodDefined]
(
	@CustomerID VARCHAR(5) = null,
	@FromDate DateTime,
	@ToDate Datetime
)
AS
SET NOCOUNT ON

DECLARE @n int
DECLARE @Date datetime


SELECT @Date = @FromDate
SET @n = datediff(dd,@FromDate,@ToDate)
--SELECT @n

WHILE (@n >= 0)
BEGIN
    --SELECT @Date
    PRINT 'Working on ' + cast(@Date AS varchar(20))
    EXEC bp_BuildBillingStats
         @CustomerID = @CustomerID,
         @Day = @Date
         SET @Date = dateadd(d,1,@Date)
         SET @n = @n-1
END  
	
     
RETURN