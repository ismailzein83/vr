CREATE PROCEDURE [ICX_Data].[sp_UserSessionData_UpdateAndGetUserSessionData]
	@UserSessionDataDataTable [ICX_Data].[UserSessionDataType] READONLY
AS
BEGIN
	SELECT dt.UserSession, 
		CASE WHEN (us.StartDate IS NULL or dt.StartDate < us.StartDate) THEN dt.StartDate ELSE us.Startdate END AS StartDate,
		CASE WHEN us.UserSession IS NULL THEN 1 ELSE 0 END AS NeedInsert, 
		CASE WHEN (us.UserSession IS NOT NULL AND dt.StartDate < us.StartDate) THEN 1 ELSE 0 END AS NeedUpdate
	INTO #UserSessionDataTempTable
	FROM @UserSessionDataDataTable dt
	LEFT JOIN [ICX_Data].[UserSessionData] us ON us.UserSession = dt.UserSession
	
	SELECT * FROM #UserSessionDataTempTable
	
	IF EXISTS(SELECT 1 FROM #UserSessionDataTempTable WHERE NeedUpdate = 1)
	BEGIN
		WITH DataToUpdate AS 
		(
			SELECT UserSession, StartDate
			FROM #UserSessionDataTempTable 
			WHERE NeedUpdate = 1	
		)	
		
		UPDATE us
		SET us.Startdate = dtu.StartDate
		FROM [ICX_Data].[UserSessionData]  us
		JOIN DataToUpdate dtu ON us.UserSession = dtu.UserSession
	END
	
	INSERT INTO [ICX_Data].[UserSessionData] (UserSession, Startdate)
	SELECT UserSession, StartDate 
	FROM #UserSessionDataTempTable
	WHERE NeedInsert = 1
END