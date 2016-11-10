CREATE PROCEDURE [genericdata].[sp_DataTransformationDefinition_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT	[ID],[Name],[Title],Details
    FROM	[genericdata].DataTransformationDefinition WITH(NOLOCK) 
	ORDER BY [Name]
END