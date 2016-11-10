-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_DataRecordStorage_GetALL]
AS
BEGIN
	SELECT	ID, Name, DataRecordTypeID, DataStoreID, Settings, [State]
	FROM	[genericdata].DataRecordStorage WITH(NOLOCK) 
	ORDER BY [Name]
END