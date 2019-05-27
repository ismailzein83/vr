CREATE PROCEDURE [VRLocalization].[sp_TextResourceTranslation_GetAll]
AS
BEGIN
SELECT	ID, TextResourceID, LanguageID,Settings 
FROM	[VRLocalization].[TextResourceTranslation] WITH(NOLOCK)
END