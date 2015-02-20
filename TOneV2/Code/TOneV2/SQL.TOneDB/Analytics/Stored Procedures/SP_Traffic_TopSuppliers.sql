
CREATE PROCEDURE [Analytics].[SP_Traffic_TopSuppliers]
	@FromDateTime DATETIME,
	@ToDateTime   DATETIME,
	@TopCount INT

WITH recompile 
AS
BEGIN

	DECLARE @Results TABLE(
			CarrierAccountID VARCHAR(5),
			NameSuffix VARCHAR(100),
			ProfileName VARCHAR(100),
			Attempts INT, 
			DurationsInMinutes NUMERIC(19,6)         

	)	
	
	SET rowcount @TopCount

		INSERT INTO @Results
		SELECT	TS.SupplierID As CarrierAccountID,
				CA.NameSuffix AS NameSuffix,
				CP.Name AS ProfileName,
			Sum(TS.NumberOfCalls) as Attempts,
			Sum(TS.DurationsInSeconds/60.) as DurationsInMinutes     
		FROM TrafficStats TS WITH(NOLOCK)
		JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
		JOIN CarrierProfile AS CP ON CA.ProfileID = CP.ProfileID
     WHERE 
				SupplierID IS NOT NULL AND
				FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
			Group By SupplierID, CA.NameSuffix, CP.Name
			ORDER BY Sum(DurationsInSeconds/60.) DESC
			

	SELECT * FROM @Results


END