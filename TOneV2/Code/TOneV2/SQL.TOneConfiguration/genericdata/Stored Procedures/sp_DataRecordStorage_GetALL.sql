-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_DataRecordStorage_GetALL]
AS
BEGIN
	SELECT	rs.ID, CASE WHEN rec.DevProjectID IS NOT NULL THEN rec.DevProjectID ELSE ds.DevProjectID END DevProjectID, 
		rs.Name, rs.DataRecordTypeID, rs.DataStoreID, rs.Settings, rs.[State]
	FROM	[genericdata].DataRecordStorage rs WITH(NOLOCK) 
	JOIN [genericdata].DataStore ds ON rs.DataStoreID = ds.ID
	JOIN [genericdata].DataRecordType rec ON rs.DataRecordTypeID = rec.ID
	ORDER BY rs.[Name]
END