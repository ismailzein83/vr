
CREATE PROCEDURE [sec].[sp_RegisteredApplication_GetAll] 
AS
BEGIN
	SELECT	[ID], [Name], [URL]
	FROM	[sec].[RegisteredApplication] WITH(NOLOCK)
END