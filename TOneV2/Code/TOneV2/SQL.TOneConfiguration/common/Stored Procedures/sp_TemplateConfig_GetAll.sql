
CREATE PROCEDURE [common].[sp_TemplateConfig_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT [ID],[Name],[ConfigType],Editor,[BehaviorFQTN],Settings
      from [common].TemplateConfig WITH(NOLOCK) 
END