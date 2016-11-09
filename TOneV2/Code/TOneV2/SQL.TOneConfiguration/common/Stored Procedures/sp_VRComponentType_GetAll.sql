Create PROCEDURE [common].[sp_VRComponentType_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
SELECT ct.[ID]
      ,ct.[Name]
      ,ct.[ConfigID]
      ,ct.[Settings]
  FROM [common].[VRComponentType] ct WITH(NOLOCK) 
END