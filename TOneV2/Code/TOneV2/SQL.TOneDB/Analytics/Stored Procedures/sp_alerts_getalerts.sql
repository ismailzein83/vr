
CREATE PROCEDURE [Analytics].[sp_alerts_getalerts]
	@TopN int = null,
	@ShowHiddenAlerts char(1), 
	@AlertLevel smallint = NULL,
	@TAG varchar(255) = NULL,
	@Source varchar(255) = NULL,
	@UserID int = NULL,
	@From INT = 1,
	@To INT = 10
AS
BEGIN

	SET NOCOUNT ON;
	IF(@TopN IS NULL)
	BEGIN
		WITH Criteria AS (
			SELECT ('PersistedAlertCriteria:'+ cast (AC.ID as char)) AS ID  
			FROM AlertCriteria AC WITH(NOLOCK) 
			WHERE (@UserID IS NULL OR AC.UserID = @UserID))
		
		,Alerts AS (
			SELECT 
					   [ID]
					  ,[Created]
					  ,[Source]
					  ,[Level]
					  ,[Progress]
					  ,[Tag]
					  ,[Description]
					  ,[IsVisible]
					  ,(Row_Number() OVER (ORDER BY [Created] DESC)) AS RowNumber
			 FROM Alert A WITH(NOLOCK,index = IX_Alert_Date) 
			 WHERE 1=1
			 AND ( A.IsVisible = case when @ShowHiddenAlerts= 'Y' then 'N' else 'Y' end)
			 AND (@AlertLevel IS NULL OR A.[Level] = @AlertLevel)
			 AND (@TAG IS NULL OR A.Tag = @TAG)
			 AND (A.[Source] LIKE CASE WHEN(@Source IS NULL) THEN '%:%' ELSE '%' + @Source + '%' END)
			 AND (@UserID IS NULL OR A.[Source] IN (SELECT ID FROM Criteria))
		)
					
		SELECT * FROM Alerts 
		WHERE RowNumber BETWEEN @From AND @To
		ORDER BY [Created] DESC
	END
	ELSE
	BEGIN
		WITH Criteria AS (
			SELECT ('PersistedAlertCriteria:'+ cast (AC.ID as char)) AS ID  
			FROM AlertCriteria AC WITH(NOLOCK) 
			WHERE (@UserID IS NULL OR AC.UserID = @UserID))
		
		,Alerts AS (
			SELECT TOP (@TopN)
					   [ID]
					  ,[Created]
					  ,[Source]
					  ,[Level]
					  ,[Progress]
					  ,[Tag]
					  ,[Description]
					  ,[IsVisible]
					  ,(Row_Number() OVER (ORDER BY [Created] DESC)) AS RowNumber
			 FROM Alert A WITH(NOLOCK,index = IX_Alert_Date) 
			 WHERE 1=1
			 AND ( A.IsVisible = case when @ShowHiddenAlerts= 'Y' then 'N' else 'Y' end)
			 AND (@AlertLevel IS NULL OR A.[Level] = @AlertLevel)
			 AND (@TAG IS NULL OR A.Tag = @TAG)
			 AND (A.[Source] LIKE CASE WHEN(@Source IS NULL) THEN '%' ELSE '%' + @Source + '%' END)
			 AND (@UserID IS NULL OR A.[Source] IN (SELECT ID FROM Criteria))
		)
					
		SELECT * FROM Alerts 
		WHERE RowNumber BETWEEN @From AND @To
		ORDER BY [Created] DESC
	END
	
END