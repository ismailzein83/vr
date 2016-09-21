CREATE procedure [common].[SP_ExtensionConfiguration_GetByType]
@Type nvarchar(255)
AS
BEGIN
	SELECT	ec.ID,ec.Title,ec.Settings,ec.Name
	FROM	[Common].ExtensionConfiguration ec  WITH(NOLOCK) 
	WHERE	ec.[ConfigType]=@Type
END