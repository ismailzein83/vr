CREATE PROCEDURE [common].[sp_Setting_GetAll]
AS
BEGIN

	SELECT  ID, Name ,[Type],[Category], Settings, [Data], [IsTechnical]
	FROM	[common].Setting  WITH(NOLOCK) 
	ORDER BY [Name]
END