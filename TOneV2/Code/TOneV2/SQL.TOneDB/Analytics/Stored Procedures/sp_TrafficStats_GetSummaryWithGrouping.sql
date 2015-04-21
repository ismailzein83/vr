-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Analytics].[sp_TrafficStats_GetSummaryWithGrouping]
	@TempTableName varchar(100),
	@FromDate datetime,
	@ToDate datetime,	
	@FromRow int,
	@ToRow int,
	@GroupingColumns varchar(200),
	@OrderByColumn varchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @sql varchar(max)
	
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	BEGIN
		SET @sql = 
		'WITH OurZones AS (SELECT ZoneID, Name FROM Zone z WITH (NOLOCK) WHERE SupplierID = ''SYS''),
		AllResult AS
		(
			SELECT
					 ts.OurZoneID AS OurZoneID
					 ,z.Name as ZoneName
				   , Sum(ts.Attempts) AS Attempts
				   , Sum(ts.DeliveredAttempts) AS DeliveredAttempts
				   , SUM(ts.DeliveredNumberOfCalls) AS DeliveredNumberOfCalls
				   , Sum(ts.SuccessfulAttempts) AS SuccessfulAttempts
				   , Sum(ts.DurationsInSeconds) AS DurationsInSeconds
				   , AVG(ts.PDDInSeconds) AS PDDInSeconds
				   , AVG(ts.PGAD) AS PGAD
				   , Max(ts.MaxDurationInSeconds) AS MaxDurationInSeconds
				   , Sum(ts.NumberOfCalls) AS NumberOfCalls
				   , Max(ts.LastCDRAttempt) AS LastCDRAttempt

			FROM TrafficStats ts WITH(NOLOCK ,INDEX(IX_TrafficStats_DateTimeFirst))
			JOIN OurZones z ON ts.OurZoneID = z.ZoneID
			WHERE
			FirstCDRAttempt BETWEEN ''' + CONVERT(VARCHAR, @FromDate) + ''' AND ''' + CONVERT(VARCHAR, @ToDate) + '''
			GROUP BY ts.OurZoneID, z.Name
		)
		SELECT * INTO ' + @TempTableName + ' FROM AllResult'
		execute (@sql)
			   
             
	END
	
	SET @sql = 'WITH OrderedResult AS (SELECT *, ROW_NUMBER()  OVER ( ORDER BY ' + @OrderByColumn + ' DESC) AS rowNumber FROM ' + @TempTableName + ')
	SELECT * FROM OrderedResult WHERE rowNumber between ' + CONVERT(VARCHAR, @FromRow) + ' AND ' + CONVERT(VARCHAR, @ToRow)
    
	EXECUTE(@sql)
END