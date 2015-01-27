
CREATE FUNCTION [dbo].[GetHoursOfDates]
(
	@FromDate DATETIME,
	@ToDate DATETIME
)
RETURNS 
@Result TABLE 
(
    [Hour] int,
    Date DATETIME
)
AS
BEGIN

SET @FromDate = '2010-01-01'
SET @ToDate = '2010-01-05'

DECLARE @Days INT 

SET @Days = DATEDIFF(dd,@FromDate,@ToDate) + 1

DECLARE @Hour INT 
DECLARE @Date DATETIME 
SET @Date = @FromDate

WHILE @Date <= @ToDate
BEGIN
	SET @Hour = 0
		WHILE @Hour < 24
		BEGIN 
			INSERT INTO @Result VALUES ( @Hour,@Date)
			SET @Hour = @Hour + 1
		END
SET @Date = DATEADD(dd,1,@Date)
END

	RETURN
END