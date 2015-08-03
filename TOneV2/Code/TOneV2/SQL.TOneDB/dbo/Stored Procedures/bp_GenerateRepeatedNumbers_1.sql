CREATE PROCEDURE [dbo].[bp_GenerateRepeatedNumbers](@DialedCount int = 100)
AS
	DROP TABLE ##CDPN
	CREATE TABLE ##CDPN(TrafficDate smalldatetime, Customer varchar(5), CDPN varchar(30), Dialed int, Minutes numeric(13,5)) 
	INSERT INTO ##CDPN
		SELECT 
			CAST(FLOOR(CAST(AttemptDatetime AS float)) AS datetime),
			IN_CARRIER, 
			CDPN, 
			Count(*), 
			Sum(DurationInSeconds) / 60.0
		FROM
			CDR WITH(NOLOCK)
			GROUP BY CAST(FLOOR(CAST(AttemptDatetime AS float)) AS datetime), IN_CARRIER, CDPN
			HAVING Count(*) > @DialedCount

	

	SELECT * FROM ##CDPN ORDER BY Dialed DESC, TrafficDate