CREATE PROCEDURE [common].[sp_EmailTemplate_GetAll]
AS
BEGIN

	SELECT  ID, Name ,BodyTemplate, SubjectTemplate, [Type]
	FROM	[common].EmailTemplate as drf WITH(NOLOCK) 
END