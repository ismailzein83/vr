CREATE PROCEDURE [VRLocalization].[sp_Language_GetAll]
AS
BEGIN
	SELECT	ID, Name, ParentLanguageID,Settings 
	FROM	[VRLocalization].[Language] WITH(NOLOCK)
END