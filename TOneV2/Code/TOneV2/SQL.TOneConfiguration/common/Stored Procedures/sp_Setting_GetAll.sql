CREATE PROCEDURE [common].[sp_Setting_GetAll]
AS
BEGIN

	SELECT  ID, Name ,[Type],[Category], Settings, [Data]
	FROM	[common].Setting  WITH(NOLOCK) 
END