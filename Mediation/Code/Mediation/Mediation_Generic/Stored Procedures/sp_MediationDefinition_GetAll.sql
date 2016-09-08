CREATE PROCEDURE [Mediation_Generic].[sp_MediationDefinition_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT def.[ID]
      ,def.[Name]
      ,def.Details
      ,def.CreatedTime
      FROM [Mediation_Generic].MediationDefinition def WITH(NOLOCK) 
END