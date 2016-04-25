create procedure [Common].[SP_ExtensionConfiguration_GetByType]
@Type nvarchar(255)
AS
BEGIN
	SELECT	ec.ID,
			ec.Title,
			ec.Settings

	FROM	Common.ExtensionConfiguration ec 
	WHERE ec.[ConfigType]=@Type
END