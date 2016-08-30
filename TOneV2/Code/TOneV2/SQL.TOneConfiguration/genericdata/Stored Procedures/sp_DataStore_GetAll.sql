-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_DataStore_GetAll]
AS
BEGIN
	SELECT	ID, Name, Settings
	FROM	[genericdata].DataStore WITH(NOLOCK) 
END