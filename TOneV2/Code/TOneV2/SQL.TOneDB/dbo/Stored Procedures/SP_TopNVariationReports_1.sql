-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SP_TopNVariationReports]
	(
	 @FromDate DATETIME = NULL,
	 @ToDate DATETIME = NULL,
	 @StringOfDates NVARCHAR(MAX),
	 @StringOfZoneID NVARCHAR(MAX)
	)
AS
BEGIN
	DECLARE @SQLStringT nvarchar(max)
	DECLARE @SQLStringO nvarchar(max)
	SET NOCOUNT ON;
	WITH dates AS
	(
		SELECT CAST(@FromDate AS DATE) 'date'
		UNION ALL
		SELECT DATEADD(day, 1, D.date)
		FROM dates D
		WHERE DATEADD(dd, 1, D.date) <= @ToDate
	)
	SELECT * into #tempDate FROM dates -- table of DATES 
	SET @SQLStringT= '
	;WITH 
	 x AS (
		Select Z.ZoneID,d.[date]  Calldate
		from Zone Z,#tempDate d 
		WHERE Z.IsEffective=''Y'' AND Z.SupplierID=''SYS''
		AND Z.ZoneID IN ('+@StringOfZoneID+')
	)
	,
	Traffic AS 
	(
	SELECT calldate,ourzoneid,sum(Ts.DurationsInSeconds/60.) Duration
	FROM TrafficStatsDaily TS 
	WHERE calldate>='''+CONVERT(CHAR,@FromDate)+'''AND calldate<='''+CONVERT(CHAR,@ToDate)+'''
	GROUP BY Ts.Calldate,TS.OurZoneID
	)
	,
	Myresult AS (
	SELECT x.*,ISNULL(Duration,0) AS Duration FROM x
	LEFT JOIN Traffic  TS ON x.zoneid=Ts.OurZoneID
	AND x.calldate=Ts.Calldate
	)
	SELECT * INTO #TEMPTermination FROM Myresult
	as P
    pivot
    (
        min(P.Duration)
        for P.Calldate in ('+@StringOfDates+')
    ) as PIV
    SELECT T.*,''T'' AS FLAG INTO #TEMP1 from #tempTermination T
    ;WITH 
     Y AS (
		SELECT Z.ZoneID,d.[date]  Calldate
		FROM Zone Z,#tempDate d 
		WHERE Z.IsEffective=''Y'' AND Z.SupplierID=''SYS''
		AND Z.ZoneID IN ('+@StringOfZoneID+')
	)
	,
	Traffic AS 
	(
	SELECT calldate,OriginatingZoneID,sum(Ts.DurationsInSeconds/60.) Duration
	FROM TrafficStatsDaily TS 
	WHERE calldate>='''+CONVERT(CHAR,@FromDate)+''' and calldate<='''+CONVERT(CHAR,@ToDate)+'''
	GROUP BY Ts.Calldate,TS.OriginatingZoneID
	)
	,
	Myresult AS (
	SELECT Y.*,ISNULL(Duration,0) AS Duration FROM Y
	LEFT JOIN Traffic  TS on Y.zoneid=Ts.OriginatingZoneID
	AND Y.calldate=Ts.Calldate
	)
	SELECT * INTO #TEMPOrigination FROM Myresult
	as P
    pivot
    (
        min(P.Duration)
        for P.Calldate in ('+@StringOfDates+')
    ) as PIV
    SELECT O.*, ''O'' AS FLAG INTO #TEMP2 FROM #TEMPOrigination O
     
   SELECT * FROM #TEMP1 UNION ALL SELECT * FROM #TEMP2
   ORDER BY ZONEID ASC
    '
     EXEC sp_executesql @SQLStringT

END