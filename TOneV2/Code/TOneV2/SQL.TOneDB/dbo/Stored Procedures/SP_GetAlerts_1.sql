
-- =============================================
-- Author:		Siraj Mansour
-- Create date: 10/2/2013
-- Description:	Returns Alerts
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetAlerts]
	@TopN int,
	@ShowHiddenAlerts char(1), 
	@AlertLevel smallint = NULL,
	@TAG varchar(255) = NULL,
	@Source varchar(255) = NULL,
	@UserID int = NULL
AS
BEGIN

	SET NOCOUNT ON;
	
	WITH Criteria AS (SELECT ('PersistedAlertCriteria:'+ cast (AC.ID as char)) AS ID  FROM AlertCriteria AC WITH(NOLOCK) WHERE (@UserID IS NULL OR AC.UserID = @UserID))
	
	,Alerts AS (SELECT TOP (@TopN)
					   [ID]
					  ,[Created]
                      ,[Source]
                      ,[Level]
					  ,[Progress]
					  ,[Tag]
                      ,[Description]
                      ,[IsVisible] FROM Alert A WITH(NOLOCK,index = IX_Alert_Date) 
						WHERE 1=1
						AND
						(@ShowHiddenAlerts IS NULL OR A.IsVisible= 'Y')
						AND
						(@AlertLevel IS NULL OR A.[Level] = @AlertLevel)
						AND
						(@TAG IS NULL OR A.Tag = @TAG)
						AND 
						(A.[Source] LIKE CASE WHEN(@Source IS NULL) THEN 'PersistedAlertCriteria:%' ELSE '%' + @Source + '%' END)--@Source IS NULL OR A.[Source] LIKE '%' + @Source + '%'
						AND
						(@UserID IS NULL OR A.[Source] IN (SELECT ID FROM Criteria))
				)
				
SELECT * FROM Alerts ORDER BY [Created] DESC
	
END